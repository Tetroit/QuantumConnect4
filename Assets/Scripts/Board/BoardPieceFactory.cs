using UnityEngine;

[System.Serializable]
public class BoardPieceFactory
{
    [SerializeField] QuantumGamePiece _quantumPrefab;
    [SerializeField] GamePiece _normalPrefab;
    [HideInInspector] public StyleProvider styleProvider;
    public GamePiece CreateNormal(EPlayer player, Vector3 pos, Transform parent)
    {
        GamePiece piece = GameObject.Instantiate(_normalPrefab, pos, Quaternion.identity, parent);
        piece.Init(styleProvider.GetPlayerColor(player), player);
        return piece;
    }
    public QuantumGamePiece CreateQuantum(EPlayer player, Vector3 pos, Transform parent)
    {
        QuantumGamePiece piece = GameObject.Instantiate(_quantumPrefab, pos, Quaternion.identity, parent);
        piece.Init(styleProvider.GetPlayerColor(player), player);
        return piece;
    }
}
