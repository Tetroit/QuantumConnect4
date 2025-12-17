using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Helper class to keep track of unique game objects in the scene
/// </summary>
public class Linker : MonoBehaviour
{
    public static Linker instance;
    //can also have a dictionary with gameobjects for easy custom links
    [SerializeField] MainMenu _menu;
    [SerializeField] EndScreen _endScreen;
    [SerializeField] BoardModel _board;

    private void Awake()
    {
        instance = this;
        SceneManager.sceneUnloaded += OnSceneUnload;
    }
    public void OnSceneUnload(Scene scene)
    {
        instance = null;
    }
    public MainMenu menu => _menu;
    public EndScreen endScreen => _endScreen;
    public BoardModel board => _board;
}
