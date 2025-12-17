using System;
using System.Collections.Generic;

public class QuantumLink
{
    public QuantumLink(QuantumGamePiece pieceA, QuantumGamePiece pieceB)
    {
        this.pieceA = pieceA;
        this.pieceB = pieceB;
        isResolvable = false;
    }
    public readonly QuantumGamePiece pieceA;
    public readonly QuantumGamePiece pieceB;
    public bool isResolvable;
}
public class PlayerProfile
{
    StyleProvider _styleProvider;
    Dictionary<int, QuantumLink> _links = new();
    Stack<int> _freeIds = new();
    //can be any in fact, just 3 seems reasonably balanced
    int _linksLeft = 3;

    public event Action<int> LinksChangedEvent;
    public PlayerProfile(StyleProvider styleProvider)
    {
        _styleProvider = styleProvider;
    }
    void SetLinks(int links)
    {
        _linksLeft = links;
        LinksChangedEvent?.Invoke(_linksLeft);
    }
    public int CreateLink(QuantumGamePiece pieceA, QuantumGamePiece pieceB)
    {
        if (!HasLinks())
            return -1;

        int id = 0;
        if (_freeIds.Count > 0)
        {
            id = _freeIds.Pop();
        }
        else
        {
            id = _links.Count;
        }
        _links.Add(id, new QuantumLink(pieceA, pieceB));
        pieceA.AssignId(id);
        pieceB.AssignId(id);
        pieceA.AssignColor(_styleProvider.GetLinkColor(id));
        pieceB.AssignColor(_styleProvider.GetLinkColor(id));
        SetLinks(_linksLeft - 1);
        LinksChangedEvent?.Invoke(_linksLeft);
        return id;
    }
    public void DestroyLink(int id)
    {
        _links.Remove(id);
        _freeIds.Push(id);
        SetLinks(_linksLeft + 1);
    }
    public bool HasLinks() => _linksLeft > 0;
    public bool IsResolvable(QuantumGamePiece piece) => _links[piece.linkId].isResolvable;
    public void MarkResolvable(QuantumGamePiece piece) => _links[piece.linkId].isResolvable = true;
    public void Clear()
    {
        SetLinks(3);
        _links.Clear();
    }
    public QuantumGamePiece ResolveLink(int id)
    {
        QuantumGamePiece res = (UnityEngine.Random.value > 0.5f) ? _links[id].pieceA : _links[id].pieceB;
        DestroyLink(id);
        return res;
    }
    public QuantumGamePiece GetLinked(QuantumGamePiece piece)
    {
        QuantumLink link = _links[piece.linkId];
        return (link.pieceA == piece) ? link.pieceB : link.pieceA;
    }

}
public class QuantumLinker
{
    Dictionary<EPlayer, PlayerProfile> _profiles;
    public QuantumLinker(StyleProvider styleProvider)
    {
        _profiles = new Dictionary<EPlayer, PlayerProfile>()
        {
            { EPlayer.WHITE, new PlayerProfile(styleProvider) },
            { EPlayer.BLACK, new PlayerProfile(styleProvider) },
        };
    }
    public PlayerProfile GetProfile(EPlayer player) => _profiles[player];
    public bool CanCreateLink(EPlayer player) => GetProfile(player).HasLinks();
    public QuantumGamePiece GetLinked(QuantumGamePiece piece) => GetProfile(piece.player).GetLinked(piece);
    public bool IsResolvable(QuantumGamePiece piece) => GetProfile(piece.player).IsResolvable(piece);
    public void MarkResolvable(QuantumGamePiece piece) => GetProfile(piece.player).MarkResolvable(piece);
    public void Clear()
    {
        foreach (var profile in _profiles.Values)
        {
            profile.Clear();
        }
    }
}
