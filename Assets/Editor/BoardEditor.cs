using UnityEditor;

[CustomEditor(typeof(BoardModel))]
public class BoardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var board = (BoardModel)target;
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("Cursor at: " + board.cursor);
        EditorGUILayout.LabelField("Current state: " + board.boardState.turnState);
        EditorGUILayout.LabelField("Current player: " + board.boardState.currentPlayer);
    }
}
