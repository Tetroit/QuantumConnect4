using UnityEngine;

[CreateAssetMenu(fileName = "MenuGameState", menuName = "GameStates/MenuGameState")]
public class MenuGameState : GameState
{
    MainMenu _menu;
    public override void OnEnter()
    {
        if (_menu == null)
            _menu = GameObject.FindGameObjectWithTag("Linker").GetComponent<Linker>().menu;
        if (_menu == null )
        {
            Debug.Log("No menu object found");
            return;
        }
        _menu.Show();
    }
    public override void OnExit()
    {
        if (_menu == null)
            _menu = GameObject.FindGameObjectWithTag("Linker").GetComponent<Linker>().menu;
        if (_menu == null)
        {
            Debug.Log("No menu object found");
            return;
        }
        _menu.Hide();
    }
}
