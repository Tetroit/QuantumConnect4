using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EInvalidMove
{
    NONE,
    NO_LINKS,
}
public interface IBoardEvents
{
    //link any audio, particles, etc to these
    public event Action<EInvalidMove> InvalidMoveAction;
    public event Action<EPlayer> OnPlayerTurnStart;
    public event Action<EPlayer> OnLinkUsed;
    public event Action<ETurnState, ETurnState> OnTurnStateChanged;
    public event Action OnSplit;
    public event Action OnJoin;
    public event Action<GamePiece> OnPiecePlaced;
    public event Action<GamePiece> OnQuantumResolved;
    public event Action<EPlayer> OnWin;
    public event Action OnTie;
    public event Action OnGameStart;
    public event Action OnGameEnd;
    public void SubscribeToLinksChanged(EPlayer player, Action<int> action);
    public void UnsubscribeFromLinksChanged(EPlayer player, Action<int> action);
}
public class BoardEvents : IBoardEvents
{
    readonly QuantumLinker _quantumLinker;
    public event Action<EInvalidMove> InvalidMoveAction;
    public event Action<EPlayer> OnPlayerTurnStart;
    public event Action<ETurnState, ETurnState> OnTurnStateChanged;
    public event Action<EPlayer> OnLinkUsed;
    public event Action OnSplit;
    public event Action OnJoin;
    public event Action<GamePiece> OnPiecePlaced;
    public event Action<GamePiece> OnQuantumResolved;
    public event Action<EPlayer> OnWin;
    public event Action OnTie;
    public event Action OnGameStart;
    public event Action OnGameEnd;

    public BoardEvents(QuantumLinker quantumLinker)
    {
        _quantumLinker = quantumLinker;
    }
    public void RaiseInvalidMoveAction(EInvalidMove move)
    {
        InvalidMoveAction?.Invoke(move);
    }
    public void RaisePlayerTurnStartAction(EPlayer player)
    {
        OnPlayerTurnStart?.Invoke(player);
    }
    public void RaiseTurnStateChangedAction(ETurnState from, ETurnState to) 
    {
        OnTurnStateChanged?.Invoke(from, to);
    }
    public void RaiseLinkUsedAction(EPlayer player)
    {
        OnLinkUsed?.Invoke(player);
    }
    public void RaiseSplitAction() 
    {
        OnSplit?.Invoke();
    }
    public void RaiseJoinAction() 
    {
        OnSplit?.Invoke();
    }
    public void RaisePiecePlacedAction(GamePiece piece) 
    {
        OnPiecePlaced?.Invoke(piece);
    }
    public void RaiseQuantumResolved(GamePiece piece)
    {
        OnQuantumResolved?.Invoke(piece);
    }
    public void RaiseWinAction(EPlayer player) 
    {
        OnWin?.Invoke(player);
    }
    public void RaiseTieAction() 
    {
        OnTie?.Invoke();
    }
    public void RaiseGameStartAction() 
    {
        OnGameStart?.Invoke();
    }
    public void RaiseGameEndAction() 
    {
        OnGameEnd?.Invoke();
    }
    public void SubscribeToLinksChanged(EPlayer player, Action<int> action)
    {
        _quantumLinker.GetProfile(player).LinksChangedEvent += action;
    }
    public void UnsubscribeFromLinksChanged(EPlayer player, Action<int> action)
    {
        _quantumLinker.GetProfile(player).LinksChangedEvent -= action;
    }
}
