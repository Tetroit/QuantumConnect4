using UnityEditor;

[CustomEditor (typeof(GameStateMachine))]
public class GameStateMachineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameStateMachine gameStateMachine = (GameStateMachine)target;
        base.OnInspectorGUI();
        if (gameStateMachine.currentState != null)
            EditorGUILayout.LabelField("Current state:", gameStateMachine.currentState.stateName);
    }
}
