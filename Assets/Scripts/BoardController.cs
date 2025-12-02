using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(Board), typeof(Collider))]
public class BoardController : MonoBehaviour
{
    Board _board;
    Collider _collider;
    [SerializeField] bool _allowInteraction;
    private void Awake()
    {
        _board = GetComponent<Board>();
        _collider = GetComponent<Collider>();
    }
    void Start()
    {
    }
    private void OnEnable()
    {
         _board.OnTurnStateChanged += OnTurnStateChange;
        GameStateMachine.instance.OnStateChange.AddListener(OnStateChange);
    }

    private void OnDisable()
    {
        _board.OnTurnStateChanged -= OnTurnStateChange;
        GameStateMachine.instance.OnStateChange.RemoveListener(OnStateChange);
    }

    // Update is called once per frame
    void Update()
    {
        if (_allowInteraction)
        {
            ProcessCursor();
        }
    }

    void ProcessCursor()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
        if (_collider.Raycast(mouseRay, out RaycastHit result, 100.0f))
        {
            Vector2Int cell = _board.LocalToCell(transform.InverseTransformPoint(result.point));
            if (cell.x >= 0 && cell.y >= 0 && cell.x < _board.nCols && cell.y < _board.nRows){
                _board.SetCursor(cell);
            }
            else
            {
                _board.SetCursor(null);
            }
        }
        else
        {
            _board.SetCursor(null);
        }
    }

    void OnStateChange(GameState from, GameState to)
    {
    }
    void OnTurnStateChange(Board.TurnState from, Board.TurnState to)
    {
        if (from == Board.TurnState.NOT_IN_GAME)
        {
            _allowInteraction = true;
        }
        if (to == Board.TurnState.NOT_IN_GAME)
        {
            _allowInteraction = false;
            _board.SetCursor(null);
        }
    }
}
