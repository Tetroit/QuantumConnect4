using System;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class Board
{
    public enum InvalidMoves
    {
        NONE,
        NO_LINKS,
    }

    //link any audio, particles, etc to these
    public event Action<InvalidMoves> InvalidMoveAction;
    public event Action<Player> OnPlayerTurnStart;
    public event Action<Player> OnPlayerTurnEnd;
    public event Action<TurnState, TurnState> OnTurnStateChanged;
    public event Action OnSplit;
    public event Action OnJoin;
    public event Action<GamePiece> OnPiecePlaced;
    public event Action<GamePiece> OnQuantumResolved;
    public event Action<Player> OnWin;
    public event Action OnTie;
    public event Action OnGameStart;
    public event Action OnGameEnd;

    public void SubscribeToLinksChange(Player player, Action<int> action)
    {
        _quantumLinker.GetProfile(player).LinksChangedEvent += action;
    }
    public void UnsubscribeFromLinksChange(Player player, Action<int> action)
    {
        _quantumLinker.GetProfile(player).LinksChangedEvent -= action;
    }
    void SetTurnState(TurnState state)
    {
        if (state == _turnState)
            return;
        OnTurnStateChanged?.Invoke(_turnState, state);
        _turnState = state;
    }
    public void EndGame(Player? winner, bool isTie = false, bool showEndScreen = false)
    {

        if (showEndScreen)
        {
            GameStateMachine.instance.SetState("End");
        }

        if (winner.HasValue)
        {
            Debug.Log(winner + " wins!");
            OnWin?.Invoke(winner.Value);
        }
        else if (isTie)
        {
            Debug.Log("Tie!");
            OnTie?.Invoke();
        }
        OnGameEnd?.Invoke();
        SetTurnState(TurnState.NOT_IN_GAME);
        if (_normalPreview != null) Destroy(_normalPreview.gameObject);
        if (_quantumPreview1 != null) Destroy(_quantumPreview1.gameObject);
        if (_quantumPreview2 != null) Destroy(_quantumPreview2.gameObject);
    }
    public void StartGame()
    {
        _quantumLinker.Clear();
        if (_gamePieces != null)
        {
            foreach (var piece in _gamePieces)
            {
                if (piece != null)
                {
                    Destroy(piece.gameObject);
                }
            }
        }
        OnGameStart?.Invoke();
        SetTurnState(TurnState.NORMAL_1);
        _currentPlayer = Player.WHITE;
        _gamePieces = new GamePiece[_nCols, _nRows];
        ArrangeNext();
    }
    public void Restart()
    {
        EndGame(null);
        StartGame();
    }
    public void OnStateChange(GameState from, GameState to)
    {
        if (to.restartGame)
        {
            Restart();
        }
        if (!to.isGameInteractable)
        {
            SetCursor(null);
        }
    }
    void PlacePiece()
    {
        //decide what to do depending on piece placed
        GamePiece toPlace = null;
        Vector3 placePosLocal = CellToLocal(_cursor.Value);
        bool passTurn = false;
        switch (_turnState)
        {
            case TurnState.NORMAL_1:
                toPlace = _normalPreview;
                passTurn = true;
                _normalPreview = null;
                break;

            case TurnState.QUANTUM_2:
                toPlace = _quantumPreview2;
                SetTurnState(TurnState.QUANTUM_1);
                _quantumPreview2 = null;
                break;

            case TurnState.QUANTUM_1:
                toPlace = _quantumPreview1;
                passTurn = true;
                _quantumLinker.MarkResolvable(_quantumPreview1);
                _quantumPreview1 = null;
                break;
        }

        //place piece
        SetPieceAt(_cursor.Value, toPlace);

        Player? winner = CheckWinner(toPlace);
        OnPiecePlaced?.Invoke(toPlace);
        toPlace.PlaceIn(_cursor.Value, new Vector2Int(_cursor.Value.x, _nRows));
        if (winner != null)
        {
            EndGame(winner, showEndScreen: true);
            return;
        }
        //OnPiecePlaced can also be called on animation end like this:
        //toPlace.PlaceIn(_cursor.Value, new Vector2Int(_cursor.Value.x, _nRows), () => OnPiecePlaced(toPlace));
        if (passTurn)
        {
            SetTurnState(TurnState.ANIMATION);
            SwitchPlayer();
        }
        if (CheckTie(_currentPlayer))
        {
            EndGame(null, isTie: true, showEndScreen: true);
            return;
        }
        //generate next piece after we check that it is not a tie condition
        if (passTurn)
        {
            toPlace.OnAnimationFinish += ArrangeNext;
        }
    }
    void ArrangeNext()
    {
        if (_normalPreview != null) Destroy(_normalPreview.gameObject);
        if (_quantumPreview1 != null) Destroy(_quantumPreview1.gameObject);
        if (_quantumPreview2 != null) Destroy(_quantumPreview2.gameObject);
        SetTurnState(TurnState.NORMAL_1);
        _normalPreview = CreateNormal(_normalPreviewPosition, _currentPlayer);
    }
    void ResolveQuantum(QuantumGamePiece piece)
    {
        //determine which piece is real
        QuantumGamePiece piece2 = _quantumLinker.GetLinked(piece);
        var realPiece = _quantumLinker.GetProfile(_currentPlayer).ResolveLink((piece).linkId);
        var falsePiece = realPiece == piece ? piece2 : piece;

        //create normal piece
        SetPieceAt(realPiece.pos, null);
        SetPieceAt(falsePiece.pos, null);
        GamePiece normal = CreateNormal(realPiece.pos, realPiece.player);
        normal.SetBoardPos(realPiece.pos);
        var moved = RecalculateColumn(falsePiece.pos.x);

        //destroy quantum pieces
        Destroy(falsePiece.gameObject);
        Destroy(realPiece.gameObject);

        //check for winning patterns
        Player? realPieceRes = CheckWinner(normal); //priority 1
        if (realPieceRes != null)
        {
            EndGame(realPieceRes, showEndScreen: true);
            return;
        }
        foreach (var movedPiece in moved) //priority 2
        {
            Player? winner = CheckWinner(movedPiece);
            if (winner != null)
            {
                EndGame(winner, showEndScreen: true);
                return;
            }
        }
        OnQuantumResolved?.Invoke(normal);
        //SwitchPlayer();
        //ArrangeNext();
    }
    void SwitchPlayer()
    {
        OnPlayerTurnEnd?.Invoke(_currentPlayer);
        _currentPlayer = _currentPlayer == Player.WHITE ? Player.BLACK : Player.WHITE;
        OnPlayerTurnStart?.Invoke(_currentPlayer);
    }
    private void MakeMove(InputAction.CallbackContext context)
    {
        if (_turnState == TurnState.ANIMATION)
            return;
        if (!_cursor.HasValue)
            return;
        var piece = GetPieceAt(_cursor.Value);

        if (CanResolveQuantum(piece))
            ResolveQuantum(piece as QuantumGamePiece);
        else
            PlacePiece();
    }
    private void Split(InputAction.CallbackContext context)
    {
        switch (_turnState)
        {
            case TurnState.ANIMATION or TurnState.QUANTUM_1:
                return;
            case TurnState.NORMAL_1:

                if (!_quantumLinker.CanCreateLink(_currentPlayer))
                    break;

                _quantumPreview1 = CreateQuantum(_quantumPreviewPosition1, _currentPlayer);
                _quantumPreview2 = CreateQuantum(_quantumPreviewPosition2, _currentPlayer);
                _quantumLinker.GetProfile(_currentPlayer).CreateLink(_quantumPreview1, _quantumPreview2);

                Destroy(_normalPreview.gameObject);
                SetTurnState(TurnState.QUANTUM_2);
                OnSplit?.Invoke();
                break;

            case TurnState.QUANTUM_2:
                _quantumLinker.GetProfile(_currentPlayer).DestroyLink(_quantumPreview1.linkId);
                Destroy(_quantumPreview1.gameObject);
                Destroy(_quantumPreview2.gameObject);

                _normalPreview = CreateNormal(_normalPreviewPosition, _currentPlayer);
                SetTurnState(TurnState.NORMAL_1);
                OnJoin?.Invoke();
                break;
        }
    }
}
