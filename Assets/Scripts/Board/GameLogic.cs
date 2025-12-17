using System.Collections.Generic;
using UnityEngine;

public class GameLogic
{
    IGrid _grid;
    BoardSpaceTransforms _transforms;
    IRule<(GamePiece, EPlayer)> _canResolveQuantum;
    GamePiece[,] _gamePieces;

    public GameLogic( IGrid grid, BoardSpaceTransforms transforms, IRule<(GamePiece, EPlayer)> resolveRule)
    {
        _grid = grid;
        _gamePieces = new GamePiece[grid.nCols, grid.nRows];
        _canResolveQuantum = resolveRule;
        _transforms = transforms;

    }
    public void Reset()
    {
        foreach (var piece in _gamePieces) 
        {
            if (piece != null)
            {
                GameObject.Destroy(piece.gameObject);
            }
        }
    }
    public GamePiece GetPieceAt(Vector2Int pos)
    {
        return _gamePieces[pos.x, pos.y];
    }
    public void SetPieceAt(Vector2Int pos, GamePiece piece)
    {
        _gamePieces[pos.x, pos.y] = piece;
        if (piece != null)
        {
            piece.PlaceIn(pos);
        }
    }
    public Vector2Int? GetStackNext(int col)
    {
        for (int i = 0; i < _grid.nRows; i++)
        {
            if (_gamePieces[col, i] == null)
                return new Vector2Int(col, i);
        }
        return null;
    }
    public bool IsQuantum(Vector2Int piece)
    {
        return _gamePieces[piece.x, piece.y] is QuantumGamePiece;
    }
    public EPlayer WhosePiece(Vector2Int piece)
    {
        return _gamePieces[piece.x, piece.y].player;
    }
    bool IsEmpty(Vector2Int pos)
    {
        return _gamePieces[pos.x, pos.y] == null;
    }
    bool SequenceBreaks(Vector2Int pos, GamePiece piece)
    {
        return
            IsEmpty(pos) ||
            (WhosePiece(pos) != piece.player) ||
            IsQuantum(pos);
    }
    public bool InBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < _grid.nCols && pos.y < _grid.nRows;
    }
    bool CheckDirection(GamePiece piece, Vector2Int delta)
    {
        int nAheadForward = 0;
        int nAheadBackward = 0;
        for (Vector2Int pos = piece.pos + delta; InBounds(pos); pos += delta)
        {
            if (SequenceBreaks(pos, piece))
                break;
            nAheadForward++;
        }
        for (Vector2Int pos = piece.pos - delta; InBounds(pos); pos -= delta)
        {
            if (SequenceBreaks(pos, piece))
                break;
            nAheadBackward++;
        }
        if (nAheadForward + nAheadBackward >= 3)
            return true;
        return false;
    }
    public EPlayer? CheckWinner(GamePiece piece)
    {
        if (piece == null || piece is QuantumGamePiece)
            return null;
        if (CheckDirection(piece, new Vector2Int(1, 0)) ||
            CheckDirection(piece, new Vector2Int(1, 1)) ||
            CheckDirection(piece, new Vector2Int(0, 1)) ||
            CheckDirection(piece, new Vector2Int(-1, 1)))
        {
            return piece.player;
        }
        return null;
    }
    public bool CheckTie(EPlayer player)
    {
        int quantumCount = 0;
        int totalCount = 0;
        foreach (GamePiece piece in _gamePieces)
        {
            if (piece != null)
            {
                totalCount++;
                if (_canResolveQuantum.IsAllowed((piece, player)))
                    quantumCount++;
            }
        }
        if (totalCount == _grid.nRows * _grid.nCols && quantumCount == 0)
        {
            return true;
        }
        return false;
    }
    public List<GamePiece> RecalculateColumn(int col)
    {
        int drop = 0;
        List<GamePiece> moved = new();
        for (int i = 0; i < _grid.nRows; i++)
        {
            Vector2Int oldPos = new Vector2Int(col, i);
            var piece = GetPieceAt(new Vector2Int(col, i));
            if (piece == null)
            {
                drop++;
            }
            else if (drop > 0)
            {
                moved.Add(piece);
                Vector2Int newPos = new Vector2Int(col, i - drop);
                SetPieceAt(oldPos, null);
                SetPieceAt(newPos, piece);
                piece.MoveTo(newPos, _transforms.CellToLocal(newPos));
            }
        }
        return moved;
    }
}
