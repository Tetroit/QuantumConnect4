using UnityEngine;

public class QuantumGamePiece : GamePiece
{
    int _linkId;
    public int linkId => _linkId;
    public void AssignId(int id) => _linkId = id;
    public void AssignColor(Color color)
    {
        _material.SetColor("_Glow_Color", color);
    }
}
