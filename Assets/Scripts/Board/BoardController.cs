using System;
using UnityEngine;
using UnityEngine.InputSystem;


public interface IBoardController
{
    public event Action OnClickRequest;
    public event Action OnSplitRequest;
    public event Action<Vector2Int?> OnCursorPlaceRequest;
    public bool allowInteraction { get; set; }
    public IGrid grid { set;}
    public BoardSpaceTransforms transforms { set; }
}

public abstract class BoardControllerBase : MonoBehaviour, IBoardController
{
    public abstract bool allowInteraction { get; set; }
    public abstract IGrid grid { set; }
    public abstract BoardSpaceTransforms transforms { set; }
    public abstract event Action OnClickRequest;
    public abstract event Action OnSplitRequest;
    public abstract event Action<Vector2Int?> OnCursorPlaceRequest;
}

[RequireComponent (typeof(BoardModel), typeof(Collider))]
public class BoardController : BoardControllerBase
{
    IGrid _grid;
    BoardSpaceTransforms _transforms;
    Collider _collider;
    [SerializeField] bool _allowInteraction;
    [SerializeField] InputActionReference _confirmPlacementAction;
    [SerializeField] InputActionReference _splitPieceAction;

    public override event Action OnClickRequest;
    public override event Action OnSplitRequest;
    public override event Action<Vector2Int?> OnCursorPlaceRequest;
    public override bool allowInteraction { get => _allowInteraction; set => _allowInteraction = value; }
    public override IGrid grid { set => _grid = value; }
    public override BoardSpaceTransforms transforms { set => _transforms = value; }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }
    private void OnEnable()
    {
        _confirmPlacementAction.action.performed += RequestClick;
        _splitPieceAction.action.performed += RequestSplit;
    }
    private void OnDisable()
    {
        _confirmPlacementAction.action.performed -= RequestClick;
        _splitPieceAction.action.performed -= RequestSplit;
    }
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
            Vector2Int cell = _transforms.LocalToCell(transform.InverseTransformPoint(result.point));
            if (cell.x >= 0 && cell.y >= 0 && cell.x < _grid.nCols && cell.y < _grid.nRows) {
                OnCursorPlaceRequest?.Invoke(cell);
            }
            else
            {
                OnCursorPlaceRequest?.Invoke(null);
            }
        }
        else
        {
            OnCursorPlaceRequest?.Invoke(null);
        }
    }
    void RequestSplit(InputAction.CallbackContext context)
    {
        OnSplitRequest?.Invoke();
    }
    void RequestClick(InputAction.CallbackContext context)
    {
        OnClickRequest?.Invoke();
    }
}
