using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public partial class Board : MonoBehaviour
{
    public enum Player
    {
        WHITE,
        BLACK
    }
    public enum TurnState
    {
        NOT_IN_GAME,
        ANIMATION,
        NORMAL_1,
        QUANTUM_2,
        QUANTUM_1
    }

    GamePiece[,] _gamePieces;
    Collider _collider;
    QuantumLinker _quantumLinker = new();
    Player _currentPlayer = Player.WHITE;
    TurnState _turnState = TurnState.NOT_IN_GAME;
    Vector2Int? _cursor;

    GamePiece _normalPreview;
    QuantumGamePiece _quantumPreview1;
    QuantumGamePiece _quantumPreview2;

    [SerializeField] Vector3 _normalPreviewPosition;
    [SerializeField] Vector3 _quantumPreviewPosition1;
    [SerializeField] Vector3 _quantumPreviewPosition2;

    [SerializeField] private int _nRows = 6;
    [SerializeField] private int _nCols = 7;
    [SerializeField] private float _cellWidth = 1;
    [SerializeField] private float _cellHeight = 1;

    [SerializeField] private GameObject _highlighter;
    [SerializeField] private GameObject _quantumPrefab;
    [SerializeField] private GameObject _normalPrefab;

    [SerializeField] InputActionReference _confirmPlacementAction;
    [SerializeField] InputActionReference _splitPieceAction;

    public float width => _cellWidth * _nCols;
    public float height => _cellHeight * _nRows;
    public float cellWidth => _cellWidth;
    public float cellHeight => _cellHeight;
    public float nCols => _nCols;
    public float nRows => _nRows;
    public Player currentPlayer => _currentPlayer;
    public TurnState turnState => _turnState;
    public Vector2Int? cursor => _cursor;
    public Vector2Int LocalToCell(Vector3 point)
    {
        point.x += width/2;
        point.y += height/2;
        Vector2Int pos = new ((int)(point.x /  _cellWidth), (int)(point.y / _cellHeight));
        return pos;
    }
    public Vector3 CellToLocal(Vector2Int cell)
    {
        Vector3 pos = new(
            -width / 2 + (cell.x + 0.5f) * _cellWidth, 
            -height / 2 + (cell.y + 0.5f) * _cellHeight, 
            0);
        return pos;

    }
    public Vector3 SnapToCell(Vector3 point)
    {
        return CellToLocal(LocalToCell(point));
    }
    public Vector3 OverflowPosition(int col)
    {
        return CellToLocal(new Vector2Int(col, _nRows));
    }

    private void OnEnable()
    {
        Assert.IsNotNull(_normalPrefab.GetComponent<GamePiece>(), "Game piece prefab has no GamePiece component");
        Assert.IsNotNull(_quantumPrefab.GetComponent<QuantumGamePiece>(), "Quantum game piece prefab has no GamePiece component");
        _collider = GetComponent<Collider>();
        _confirmPlacementAction.action.performed += MakeMove;
        _splitPieceAction.action.performed += Split;

        GameStateMachine.instance.OnStateChange.AddListener(OnStateChange);
    }
    private void OnDisable()
    {
        _confirmPlacementAction.action.performed -= MakeMove;
        _splitPieceAction.action.performed -= Split;

        GameStateMachine.instance.OnStateChange.RemoveListener(OnStateChange);
    }
    void FillPieceData(GamePiece piece, Player player)
    {
        piece.AssignPlayer(player);
        piece.cellToLocal = CellToLocal;
        Material mat = piece.GetComponent<MeshRenderer>().material;
        mat.color = GetPlayerColor(_currentPlayer);
    }
    GamePiece CreateNormal(Vector3 pos, Player player)
    {
        GamePiece piece = Instantiate(_normalPrefab, pos, Quaternion.identity, transform).GetComponent<GamePiece>();
        FillPieceData(piece, player);
        return piece;
    }
    GamePiece CreateNormal(Vector2Int pos, Player player)
    {
        GamePiece piece = CreateNormal(CellToLocal(pos), player);
        if (InBounds(pos)) SetPieceAt(pos, piece);
        return piece;
    }
    QuantumGamePiece CreateQuantum(Vector3 pos, Player player)
    {
        QuantumGamePiece piece = Instantiate(_quantumPrefab, pos, Quaternion.identity, transform).GetComponent<QuantumGamePiece>();
        FillPieceData(piece, player);
        return piece;
    }
    QuantumGamePiece CreateQuantum(Vector2Int pos, Player player)
    {
        QuantumGamePiece piece = CreateQuantum(CellToLocal(pos), player);
        if (InBounds(pos)) SetPieceAt(pos, piece);
        return piece;
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
            piece.SetBoardPos(pos);
        }
    }
    public void SetCursor(Vector2Int? cell)
    {
        if (cell == null)
        {
            _cursor = null;
            _highlighter.SetActive(false);
            return;
        }
        Vector2Int? cellToHighlight = cell;

        GamePiece piece = GetPieceAt(cell.Value);
        if (!CanResolveQuantum(piece))
            cellToHighlight = GetStackNext(cell.Value.x);

        if (cellToHighlight.HasValue)
        {
            _highlighter.SetActive(true);
            _cursor = cellToHighlight;
            _highlighter.transform.localPosition = CellToLocal(cellToHighlight.Value);
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
    Color GetPlayerColor(Player player) => (player == Player.WHITE) ? Color.gray8 : Color.black;
    
    bool CanResolveQuantum(GamePiece piece)
    {
        return
            piece != null &&
            piece.player == _currentPlayer &&
            piece is QuantumGamePiece &&
            _quantumLinker.IsResolvable(piece as QuantumGamePiece);
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
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_normalPreviewPosition, 0.2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_quantumPreviewPosition1, 0.2f);
        Gizmos.DrawWireSphere(_quantumPreviewPosition2, 0.2f);
    }
#endif
}
