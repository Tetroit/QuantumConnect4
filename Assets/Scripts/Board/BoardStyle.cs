using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "Board Style", menuName = "Skinning")]
public class BoardStyle : ScriptableObject {
    public List<Color> linkColors;
    public Color whitePlayerColor;
    public Color blackPlayerColor;
}
