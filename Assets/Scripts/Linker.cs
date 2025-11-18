using UnityEngine;

/// <summary>
/// Helper class to keep track of unique game objects in the scene
/// </summary>
public class Linker : MonoBehaviour
{
    [SerializeField]
    MainMenu _menu;
    public MainMenu menu => _menu;
}
