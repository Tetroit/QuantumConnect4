using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class MainMenu : MonoBehaviour
{
    [SerializeField]
    GameObject _rules;
    CanvasGroup _canvasGroup;

    Tween _fadeTween = null;
    Tween _rulesFadeTween = null;
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    public void StartGame()
    {
        GameStateMachine.instance.SetState("Match");
    }
    public void ShowRules()
    {
        if (_rules == null) 
        {
            Debug.Log("No rules window attached");
            return;
        }
        if (_rulesFadeTween != null && _rulesFadeTween.active)
            _rulesFadeTween.Kill();
        _rules.SetActive(true);
        _rulesFadeTween = _rules.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f);
        _canvasGroup.interactable = false;
    }
    public void HideRules()
    {
        if (_rules == null)
        {
            Debug.Log("No rules window attached");
            return;
        }
        if (_rulesFadeTween != null && _rulesFadeTween.active)
            _rulesFadeTween.Kill();
        _rulesFadeTween = _rules.GetComponent<CanvasGroup>().DOFade(0.0f, 0.5f);
        _rulesFadeTween.OnComplete(() => { 
            _rules.SetActive(false);
            _canvasGroup.interactable = true;
        });
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
        _fadeTween = _canvasGroup.DOFade(1.0f, 0.3f).OnComplete(() =>
        {
            _canvasGroup.interactable = true;
        });
    }

    public void Quit()
    {
        Application.Quit();
    }
}
