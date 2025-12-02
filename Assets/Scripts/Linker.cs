using UnityEngine;

/// <summary>
/// Helper class to keep track of unique game objects in the scene
/// </summary>
public class Linker : MonoBehaviour
{
    //can also have a dictionary with gameobjects for easy custom links
    [SerializeField] MainMenu _menu;
    [SerializeField] EndScreen _endScreen;
    [SerializeField] Board _board;
    public MainMenu menu => _menu;
    public EndScreen endScreen => _endScreen;
    public Board board => _board;
}
