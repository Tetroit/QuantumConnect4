using System;

public class BoardState
{
    ETurnState _turnState = ETurnState.NONE;
    EPlayer _currentPlayer = EPlayer.WHITE;
    public event Action<EPlayer> OnPlayerTurnStart;
    public event Action<ETurnState, ETurnState> OnTurnStateChanged;

    public EPlayer currentPlayer => _currentPlayer;
    public ETurnState turnState => _turnState;
    public void SwitchPlayer()
    {
        _currentPlayer = _currentPlayer == EPlayer.WHITE ? EPlayer.BLACK : EPlayer.WHITE;
        OnPlayerTurnStart?.Invoke(_currentPlayer);
    }
    public void SetTurnState(ETurnState state)
    {
        if (state == _turnState)
            return;
        OnTurnStateChanged?.Invoke(_turnState, state);
        _turnState = state;
    }
}
