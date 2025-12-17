using UnityEngine;
using UnityEngine.Events;
using AYellowpaper.SerializedCollections;

public enum EGameState
{
    MENU,
    GAME,
    END
}

public class GameStateMachine : MonoBehaviour
{
    static GameStateMachine _instance;
    public static GameStateMachine instance => _instance;
    EGameState _currentState;
    [SerializedDictionary("Type, State")]
    [SerializeField] SerializedDictionary<EGameState, GameState> _states;
    [SerializeField] EGameState _startState;

    public GameState currentState
    {
        get
        {
            if (_states == null || !_states.ContainsKey(_currentState))
                return null;
            return _states[_currentState];
        }
    }
    public UnityEvent<GameState, GameState> OnStateChange;
    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }
    private void OnEnable()
    {
    }
    private void Start()
    {
        SetState(_startState);
    }
    private void Update()
    {
        currentState.Update();
    }
    public void SetState(EGameState state)
    {
        if (_currentState == state)
        {
            return;
        }
        GameState newGameState = _states[state];
        if (newGameState == null || currentState == newGameState)
        {
            return;
        }

        newGameState.OnEnter();
        if (currentState != null)
            currentState.OnExit();
        OnStateChange?.Invoke(currentState, newGameState);
        _currentState = state;
    }
}
