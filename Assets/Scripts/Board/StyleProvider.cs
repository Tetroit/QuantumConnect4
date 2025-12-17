using UnityEngine;

[System.Serializable]
public class StyleProvider
{
    [SerializeField]
    BoardStyle _style;
    public Color GetPlayerColor(EPlayer player)
    {
        if (_style == null)
            return Color.purple;

        switch (player)
        {
            case (EPlayer.BLACK):
                return _style.blackPlayerColor;
            case (EPlayer.WHITE):
                return _style.whitePlayerColor;
        }
        return Color.purple;
    }
    public Color GetLinkColor(int id)
    {
        if (_style == null)
            return Color.purple;
        if (id < 0 || id >= _style.linkColors.Count)
            return Color.purple;

        return _style.linkColors[id];
    }
}
