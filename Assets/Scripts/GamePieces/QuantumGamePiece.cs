using UnityEngine;

public class QuantumGamePiece : GamePiece
{
    MeshRenderer _renderer;
    Material _material;
    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _material = _renderer.material;
    }
    int _linkId;
    public int linkId => _linkId;
    public void AssignId(int id) => _linkId = id;
    public void AssignColor(Color color)
    {
        _material.SetColor("_Glow_Color", color);
    }
}
