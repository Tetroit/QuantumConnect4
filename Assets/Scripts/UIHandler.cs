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
            var menu = GameObject.FindGameObjectWithTag("Linker").GetComponent<Linker>().menu;
            menu.Hide();
        }
        else if (to.stateName == "Menu")
        {
            var menu = GameObject.FindGameObjectWithTag("Linker").GetComponent<Linker>().menu;
            menu.Show();
        }
        if (from && from.stateName == "End")
        {
            var endScreen = GameObject.FindGameObjectWithTag("Linker").GetComponent<Linker>().endScreen;
            endScreen.Hide();
        }
        else if (to.stateName == "End")
        {
            var endScreen = GameObject.FindGameObjectWithTag("Linker").GetComponent<Linker>().endScreen;
            endScreen.Show();
        }
    }
}
