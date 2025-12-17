using TMPro;
using UnityEngine;

[RequireComponent (typeof(TextMeshProUGUI))]
public class LinksDisplay : MonoBehaviour
{
    [SerializeField] BoardModel _board;
    [SerializeField] string _name;
    [SerializeField] EPlayer _player;
    TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        SubscribeDelayed();
    }
    private void OnDisable()
    {
        _board.events.UnsubscribeFromLinksChanged(_player, SetLinks);
    }
    void SubscribeDelayed()
    {
        if (_board.didAwake)
            Subscribe();
        else
            _board.OnInitialised += Subscribe;
    }
    void Subscribe()
    {
        _board.events.SubscribeToLinksChanged(_player, SetLinks);
    }
    void SetLinks(int links)
    {
        _text.text = _name + ": " + links;
    }
}
