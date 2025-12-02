using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "States/GameState")]
public class GameState : ScriptableObject
{
    [SerializeField] string _stateName;
    [SerializeField] protected bool _isGameInteractable = false;
    [SerializeField] protected bool _restartGame = false;
    [SerializeField] protected bool _endGame = false;

    public string stateName => _stateName;
    public bool isGameInteractable => _isGameInteractable;
    public bool restartGame => _restartGame;
    public virtual void OnEnter()
    {
    }
    public virtual void OnExit()
    {
    }
    public virtual void Update()
    {
    }
}
