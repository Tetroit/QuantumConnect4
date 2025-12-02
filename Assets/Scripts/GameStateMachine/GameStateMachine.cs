using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class GameStateMachine : MonoBehaviour
{
    static GameStateMachine _instance;
    public static GameStateMachine instance => _instance;

    GameState _currentState = null;
    public GameState currentState => _currentState;

    //I would use a dictionary, but it is not serializable :(
    [SerializeField] List<GameState> states;
    [SerializeField] int _startStateID = 0;
    [SerializeField] Linker _linker;

    public UnityEvent<GameState, GameState> OnStateChange;
    GameState FindState(string key)
    {
        GameState res = states.Find((GameState state) => { return state.stateName == key; });
        if (res == null)
            Debug.Log($"No state found with {key} key");
        return res;
    }
    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }
    private void OnEnable()
    {
        if (_linker == null)
            _linker = GameObject.FindGameObjectWithTag("Linker").GetComponent<Linker>();
        if (_linker == null)
            Debug.Log("Failed to find linker");
    }
    private void Start()
    {
        if (_startStateID >= 0 && _startStateID < states.Count)
        {
            SetState(states[_startStateID]);
        }
    }
    void SetState(GameState gameState)
    {
        if (gameState == null || _currentState == gameState)
            return;

        gameState.OnEnter();
        if (_currentState != null)
            _currentState.OnExit();
        OnStateChange?.Invoke(_currentState, gameState);
        _currentState = gameState;
    }
    public void SetState(string gameStateName)
    {
        if (_currentState != null && gameStateName == _currentState.name){
            return;
        }
        GameState gameState = FindState(gameStateName);
        SetState(gameState);
    }
    private void Update()
    {
        _currentState?.Update();
    }
}
