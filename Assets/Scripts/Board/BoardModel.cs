using System;
using UnityEngine;

public enum EPlayer
{
    WHITE,
    BLACK
}   
public enum ETurnState
{
    NONE,
    ANIMATION,
    NORMAL_1,
    QUANTUM_2,
    QUANTUM_1
}

public interface IGrid
{
    public float width { get; }
    public float height { get; }
    public float cellWidth { get; }
    public float cellHeight { get; }
    public int nCols { get; }
    public int nRows { get; }
}
public interface IBoardControllable
{
    public void RequestSplit();
    public void RequestClick();
    public void RequestCursorSet(Vector2Int? pos);
}

[RequireComponent(typeof(BoardPreviewBar))]
public class BoardModel : MonoBehaviour, IGrid, IBoardControllable
{
    BoardEvents _boardEvents;
    BoardSpaceTransforms _boardSpaceTransforms;
    BoardState _boardState = new();
    QuantumLinker _quantumLinker;
    BoardPreviewBar _previewBar;
    GameLogic _logic;
    IRule<(GamePiece, EPlayer)> _canResolveQuantum;
    [SerializeField] BoardPieceFactory _pieceFactory;
    [SerializeField] StyleProvider _styleProvider;
    [SerializeField] ControllerManager _controllerManager;

    public event Action OnInitialised;

    public IBoardEvents events => _boardEvents;
    public BoardSpaceTransforms transforms => _boardSpaceTransforms;

    //for editor scripts
#if UNITY_EDITOR
    public BoardState boardState => _boardState;
#endif

    Collider _collider;
    Vector2Int? _cursor;

    [SerializeField] int _nRows = 6;
    [SerializeField] int _nCols = 7;
    [SerializeField] float _cellWidth = 1;
    [SerializeField] float _cellHeight = 1;
    [SerializeField] GameObject _highlighter;

    public float width => _cellWidth * _nCols;
    public float height => _cellHeight * _nRows;
    public float cellWidth => _cellWidth;
    public float cellHeight => _cellHeight;
    public int nCols => _nCols;
    public int nRows => _nRows;
    public Vector2Int? cursor => _cursor;

    private void OnValidate()
    {
        _boardState = new BoardState();
        _previewBar = GetComponent<BoardPreviewBar>();
    }
    private void Awake()
    {
        _boardSpaceTransforms = new BoardSpaceTransforms(this);
        _quantumLinker = new QuantumLinker(_styleProvider);
        _boardEvents = new BoardEvents(_quantumLinker);
        _canResolveQuantum = new CanResolveQuantumRule(_quantumLinker, _boardState);
        _logic = new GameLogic(this, transforms, _canResolveQuantum);

        _boardState.OnPlayerTurnStart += _boardEvents.RaisePlayerTurnStartAction;
        _boardState.OnTurnStateChanged += _boardEvents.RaiseTurnStateChangedAction;

        _previewBar.factory = _pieceFactory;
        _pieceFactory.styleProvider = _styleProvider;

        _boardState.OnTurnStateChanged += OnTurnStateChange;
        _controllerManager.SetBoard(this);

        OnInitialised?.Invoke();
    }
    private void OnEnable()
    {
        _collider = GetComponent<Collider>();

        GameStateMachine.instance.OnStateChange.AddListener(OnGameStateChange);
    }
    private void OnDisable()
    {
        GameStateMachine.instance.OnStateChange.RemoveListener(OnGameStateChange);
    }
    void OnTurnStateChange(ETurnState from, ETurnState to)
    {
        if (from == ETurnState.NONE)
        {
            _controllerManager.EnableAll();
        }
        if (to == ETurnState.NONE)
        {
            _controllerManager.DisableAll();
            RequestCursorSet(null);
        }
    }

    void PlacePiece()
    {
        //decide what to do depending on piece placed
        GamePiece toPlace = null;
        Vector3 placePosLocal = _boardSpaceTransforms.CellToLocal(_cursor.Value);
        bool passTurn = false;
        toPlace = _previewBar.GetPiece(_boardState.turnState);
        switch (_boardState.turnState)
        {
            case ETurnState.NORMAL_1:
                passTurn = true;
                break;

            case ETurnState.QUANTUM_2:
                _boardState.SetTurnState(ETurnState.QUANTUM_1);
                break;

            case ETurnState.QUANTUM_1:
                passTurn = true;
                _quantumLinker.MarkResolvable(toPlace as QuantumGamePiece);
                break;
        }

        //place piece
        _logic.SetPieceAt(_cursor.Value, toPlace);

        EPlayer? winner = _logic.CheckWinner(toPlace);
        _boardEvents.RaisePiecePlacedAction(toPlace);
        toPlace.PlaceIn(_cursor.Value);
        toPlace.MoveIn(transforms.OverflowPosition(_cursor.Value.x), transforms.CellToLocal(_cursor.Value));
        if (winner != null)
        {
            EndGame(winner, showEndScreen: true);
            return;
        }
        //OnPiecePlaced can also be called on animation end like this:
        //toPlace.PlaceIn(_cursor.Value, new Vector2Int(_cursor.Value.x, _nRows), () => OnPiecePlaced(toPlace));
        if (passTurn)
        {
            _boardState.SetTurnState(ETurnState.ANIMATION);
            _boardState.SwitchPlayer();
        }
        if (_logic.CheckTie(_boardState.currentPlayer))
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
        _boardState.SetTurnState(ETurnState.NORMAL_1);
        _previewBar.CreateNormal(_boardState.currentPlayer);
    }
    void ResolveQuantum(QuantumGamePiece piece)
    {
        //determine which piece is real
        QuantumGamePiece piece2 = _quantumLinker.GetLinked(piece);
        var realPiece = _quantumLinker.GetProfile(_boardState.currentPlayer).ResolveLink((piece).linkId);
        var falsePiece = realPiece == piece ? piece2 : piece;

        //create normal piece
        GamePiece normal = _pieceFactory.CreateNormal(realPiece.player, _boardSpaceTransforms.CellToLocal(realPiece.pos), transform);
        normal.PlaceIn(realPiece.pos);
        _logic.SetPieceAt(realPiece.pos, normal);
        _logic.SetPieceAt(falsePiece.pos, null);
        var moved = _logic.RecalculateColumn(falsePiece.pos.x);

        //destroy quantum pieces
        Destroy(falsePiece.gameObject);
        Destroy(realPiece.gameObject);

        //check for winning patterns
        EPlayer? realPieceRes = _logic.CheckWinner(normal); //priority 1
        if (realPieceRes != null)
        {
            EndGame(realPieceRes, showEndScreen: true);
            return;
        }
        foreach (var movedPiece in moved) //priority 2
        {
            EPlayer? winner = _logic.CheckWinner(movedPiece);
            if (winner != null)
            {
                EndGame(winner, showEndScreen: true);
                return;
            }
        }
        _boardEvents.RaiseQuantumResolved(normal);
        //SwitchPlayer();
        //ArrangeNext();
    }
    void Split(ETurnState state, EPlayer player)
    {
        switch (state)
        {
            case ETurnState.ANIMATION or ETurnState.QUANTUM_1:
                return;
            case ETurnState.NORMAL_1:

                if (!_quantumLinker.CanCreateLink(player))
                    break;
                (QuantumGamePiece p1, QuantumGamePiece p2) pair = _previewBar.CreateQuantum(player);
                _quantumLinker.GetProfile(player).CreateLink(pair.p1, pair.p2);
                _previewBar.DestroyNormal();
                _boardState.SetTurnState(ETurnState.QUANTUM_2);
                _boardEvents.RaiseSplitAction();
                break;

            case ETurnState.QUANTUM_2:
                _quantumLinker.GetProfile(player).DestroyLink(_previewBar.quantumPreview1.linkId);
                _previewBar.DestroyQuantum1();
                _previewBar.DestroyQuantum2();
                _previewBar.CreateNormal(player);
                _boardState.SetTurnState(ETurnState.NORMAL_1);
                _boardEvents.RaiseJoinAction();
                break;
        }
    }
    public void RequestCursorSet(Vector2Int? cell)
    {
        if (cell == null)
        {
            _cursor = null;
            _highlighter.SetActive(false);
            return;
        }
        Vector2Int? cellToHighlight = cell;

        GamePiece piece = _logic.GetPieceAt(cell.Value);
        if (!_canResolveQuantum.IsAllowed((piece, _boardState.currentPlayer)))
            cellToHighlight = _logic.GetStackNext(cell.Value.x);

        if (cellToHighlight.HasValue)
        {
            _highlighter.SetActive(true);
            _cursor = cellToHighlight;
            _highlighter.transform.localPosition = _boardSpaceTransforms.CellToLocal(cellToHighlight.Value);
        }
        else
        {
            _cursor = null;
            _highlighter.SetActive(false);
        }
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
    public void EndGame(EPlayer? winner, bool isTie = false, bool showEndScreen = false)
    {
        if (showEndScreen)
        {
            GameStateMachine.instance.SetState(EGameState.END);
        }

        if (winner.HasValue)
        {
            Debug.Log(winner + " wins!");
            _boardEvents.RaiseWinAction(winner.Value);
        }
        else if (isTie)
        {
            Debug.Log("Tie!");
            _boardEvents.RaiseTieAction();
        }
        _boardEvents.RaiseGameEndAction();
        _boardState.SetTurnState(ETurnState.NONE);
        _previewBar.DestroyNormal();
        _previewBar.DestroyQuantum1();
        _previewBar.DestroyQuantum2();
        //_controllerManager.DisableAll();
    }
    public void StartGame()
    {
        _quantumLinker.Clear();
        _logic.Reset();
        _boardEvents.RaiseGameStartAction();
        _boardState.SetTurnState(ETurnState.NORMAL_1);
        //_controllerManager.EnableAll();
        //_currentPlayer = EPlayer.WHITE;
        ArrangeNext();
    }
    public void RestartGame()
    {
        EndGame(null);
        StartGame();
    }
    public void OnGameStateChange(GameState from, GameState to)
    {
        if (to.restartGame)
        {
            RestartGame();
        }
        if (!to.isGameInteractable)
        {
            RequestCursorSet(null);
        }
    }
    public void RequestClick()
    {
        if (_boardState.turnState == ETurnState.ANIMATION)
            return;
        if (!_cursor.HasValue)
            return;
        var piece = _logic.GetPieceAt(_cursor.Value);

        if (_canResolveQuantum.IsAllowed((piece, _boardState.currentPlayer)))
            ResolveQuantum(piece as QuantumGamePiece);
        else
            PlacePiece();
    }
    public void RequestSplit()
    {
        Split(_boardState.turnState, _boardState.currentPlayer);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.worldToLocalMatrix;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(width, height, 1));
        Gizmos.color = Color.red;
        for (float x = 0.5f * _cellWidth - width / 2; x < width / 2; x += _cellWidth) {
            for (float y = 0.5f * _cellHeight - height / 2; y < height / 2; y += _cellHeight)
            {
                Gizmos.DrawWireCube(
                    Vector3.zero + new Vector3(x, y, 0),
                    new Vector3(_cellWidth, _cellHeight, 1)
                );

            }
        }
    }
#endif
}
