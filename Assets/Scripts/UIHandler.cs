using UnityEngine;

public class UIHandler : MonoBehaviour
{
    private void OnEnable()
    {
        GameStateMachine.instance.OnStateChange.AddListener(OnStateChange);
    }
    private void OnDisable()
    {
        GameStateMachine.instance.OnStateChange.RemoveListener(OnStateChange);
    }
    public void OnStateChange(GameState from, GameState to)
    {
        if (from && from.stateName == "Menu")
        {
            var menu = Linker.instance.menu;
            menu.Hide();
        }
        else if (to.stateName == "Menu")
        {
            var menu = Linker.instance.menu;
            menu.Show();
        }
        if (from && from.stateName == "End")
        {
            var endScreen = Linker.instance.endScreen;
            endScreen.Hide();
        }
        else if (to.stateName == "End")
        {
            var endScreen = Linker.instance.endScreen;
            endScreen.Show();
        }
    }
}
