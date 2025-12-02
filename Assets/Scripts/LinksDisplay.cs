using NUnit.Framework;
using TMPro;
using UnityEngine;

[RequireComponent (typeof(TextMeshProUGUI))]
public class LinksDisplay : MonoBehaviour
{
    Board _board;
    [SerializeField] string _name;
    [SerializeField] Board.Player _player;
    TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        _board = GameObject.FindGameObjectWithTag("Linker").GetComponent<Linker>().board;
        _board.SubscribeToLinksChange(_player, SetLinks);
    }
    private void OnDisable()
    {
        _board.SubscribeToLinksChange(_player, SetLinks);
    }

    void SetLinks(int links)
    {
        _text.text = _name + ": " + links;
    }
}
