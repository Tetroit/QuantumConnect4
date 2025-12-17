using UnityEngine;

[System.Serializable]
public class BoardPreviewBar : MonoBehaviour
{
    GamePiece _normalPreview;
    QuantumGamePiece _quantumPreview1;
    QuantumGamePiece _quantumPreview2;
    [SerializeField] Vector3 _normalPreviewPosition;
    [SerializeField] Vector3 _quantumPreviewPosition1;
    [SerializeField] Vector3 _quantumPreviewPosition2;
    public BoardPieceFactory factory;

    public GamePiece normalPreview => _normalPreview;
    public QuantumGamePiece quantumPreview1 => _quantumPreview1;
    public QuantumGamePiece quantumPreview2 => _quantumPreview2;
    public GamePiece CreateNormal(EPlayer player)
    {
        _normalPreview = factory.CreateNormal(player, _normalPreviewPosition, transform);
        return _normalPreview;
    }
    public (QuantumGamePiece, QuantumGamePiece) CreateQuantum(EPlayer player)
    {
        _quantumPreview1 = factory.CreateQuantum(player, _quantumPreviewPosition1, transform);
        _quantumPreview2 = factory.CreateQuantum(player, _quantumPreviewPosition2, transform);
        return (_quantumPreview1, _quantumPreview2);
    }
    public GamePiece GetPiece(ETurnState state)
    {
        GamePiece piece = null;
        switch (state)
        {
            case ETurnState.NORMAL_1:
                piece = _normalPreview;
                _normalPreview = null;
                break;
            case ETurnState.QUANTUM_1:
                piece = _quantumPreview1;
                _quantumPreview1 = null;
                break;
            case ETurnState.QUANTUM_2:
                piece = _quantumPreview2;
                _quantumPreview2 = null;
                break;
        }
        return piece;
    }
    public void RemoveNormal()
    {
        _normalPreview = null;
    }
    public void RemoveQuantum1()
    {
        _quantumPreview1 = null;
    }
    public void RemoveQuantum2()
    {
        _quantumPreview2 = null;
    }
    public void DestroyNormal()
    {
        if (_normalPreview != null)
        {
            Destroy(_normalPreview.gameObject);
            _normalPreview = null;
        }
    }
    public void DestroyQuantum1()
    {
        if (_quantumPreview1 != null)
        {
            Destroy(_quantumPreview1.gameObject);
            _quantumPreview1 = null;
        }
    }
    public void DestroyQuantum2()
    {
        if (_quantumPreview2 != null)
        {
            Destroy(_quantumPreview2.gameObject);
            _quantumPreview2 = null;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_normalPreviewPosition, 0.2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_quantumPreviewPosition1, 0.2f);
        Gizmos.DrawWireSphere(_quantumPreviewPosition2, 0.2f);
    }
#endif
}
