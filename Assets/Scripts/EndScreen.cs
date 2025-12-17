using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class EndScreen : MonoBehaviour
{
    Tween _fadeTween;
    CanvasGroup _canvasGroup;
    IBoardEvents _board;
    [SerializeField] TextMeshProUGUI _textMeshPro;
    [SerializeField] Button _menuButton;

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    private void OnEnable()
    {
        _board = Linker.instance.board.events;

        Debug.Assert(_textMeshPro != null, "Message field (TMP) was null");
        Debug.Assert(_menuButton != null, "Menu button was null");
        _board.OnWin += ShowWinMessage;
        _board.OnTie += ShowTieMessage;
        _menuButton.onClick.AddListener(ReturnToMenu);
    }
    private void OnDisable()
    {
        _board.OnWin -= ShowWinMessage;
        _board.OnTie -= ShowTieMessage;
        _menuButton.onClick.RemoveListener(ReturnToMenu);
    }
    void ReturnToMenu()
    {
        GameStateMachine.instance.SetState(EGameState.MENU);
    }
    void ShowWinMessage(EPlayer player)
    {
        _textMeshPro.text = $"{player} wins!";
    }
    void ShowTieMessage()
    {
        _textMeshPro.text = $"Tie!";
    }
    public void Hide()
    {
        if (_fadeTween != null && _fadeTween.active)
            _fadeTween.Kill();
        _fadeTween = _canvasGroup.DOFade(0.0f, 0.3f);
        _canvasGroup.interactable = false;
        _fadeTween.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
    public void Show()
    {
        gameObject.SetActive(true);
        if (_fadeTween != null && _fadeTween.active)
            _fadeTween.Kill();
        _fadeTween = _canvasGroup.DOFade(1.0f, 0.3f);
        _fadeTween.OnComplete(() =>
        {
            _canvasGroup.interactable = true;
        });
    }
}
