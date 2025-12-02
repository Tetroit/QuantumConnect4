using UnityEditor;

[CustomEditor(typeof(Board))]
public class BoardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var board = (Board)target;
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("Cursor at: " + board.cursor);
        EditorGUILayout.LabelField("Current state: " + board.turnState);
    }
}
