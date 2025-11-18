using UnityEngine;

public class GameState : ScriptableObject
{
    [SerializeField]
    string _stateName;
    public string stateName => _stateName;
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
