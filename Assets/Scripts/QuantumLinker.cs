using System;
using System.Collections.Generic;
using UnityEngine;

public class QuantumLinker
{
    public static readonly List<Color> glowColors = new List<Color>()
    {
        new(0, 1, 0, 1),
        new(1, 0, 0, 1),
        new(0, 0, 1, 1),
        new(1, 1, 0, 1),
        new(1, 0, 1, 1),
        new(0, 1, 1, 1),
        new(1, 0.5f, 0, 1),
        new(1, 0, 0.5f, 1),
        new(0, 1, 0.5f, 1),
        new(0, 0.5f, 1, 1),
        new(0.5f, 0, 1, 1),
        new(0, 0.5f, 0, 1),
        new(0, 0, 0.5f, 1),
        new(0.5f, 0, 0, 1),
    };
    public class QuantumLink
    {
        public QuantumLink (QuantumGamePiece pieceA, QuantumGamePiece pieceB)
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
        Dictionary<int, QuantumLink> _links = new();
        Stack<int> _freeIds = new();
        //can be any in fact, just 3 seems reasonably balanced
        int _linksLeft = 3;

        public event Action<int> LinksChangedEvent;
        void SetLinks(int links)
        {
            _linksLeft = links;
            LinksChangedEvent?.Invoke(_linksLeft);
        }
        public bool CreateLink(QuantumGamePiece pieceA, QuantumGamePiece pieceB)
        {
            if (!HasLinks())
                return false;

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
            pieceA.AssignColor(glowColors[id]);
            pieceB.AssignColor(glowColors[id]);
            SetLinks(_linksLeft - 1);
            LinksChangedEvent?.Invoke(_linksLeft);
            return true;
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
    Dictionary<Board.Player, PlayerProfile> _profiles;
    public QuantumLinker()
    {
        _profiles = new Dictionary<Board.Player, PlayerProfile>()
        {
            { Board.Player.WHITE, new PlayerProfile() },
            { Board.Player.BLACK, new PlayerProfile() },
        };
    }
    public PlayerProfile GetProfile(Board.Player player) => _profiles[player];
    public bool CanCreateLink(Board.Player player) => GetProfile(player).HasLinks();
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
