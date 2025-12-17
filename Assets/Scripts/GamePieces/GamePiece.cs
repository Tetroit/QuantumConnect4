using DG.Tweening;
using System;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    protected Material _material;
    protected Vector2Int _pos;
    protected EPlayer _player;
    protected Tween _movingTween;
    [SerializeField] float animationSpeed = 0.2f;

    public Vector2Int pos => _pos;
    public EPlayer player => _player;
    public bool isInAnimation => _movingTween != null;
    public Action OnAnimationFinish;
    public void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
    }
    public void Init(Color color, EPlayer player)
    {
        _material.color = color;
        _player = player;
    }
    public void TerminateMovingAnimation()
    {
        _movingTween.Kill(false);
        OnAnimationFinish?.Invoke();
        _movingTween = null;
        OnAnimationFinish = null;

    }
    public void PlaceIn(Vector2Int boardPos)
    {
        _pos = boardPos;
       
    }
    public void MoveIn(Vector3 topPos, Vector3 boardPos, Action OnFinish = null)
    {
        if (_movingTween != null) TerminateMovingAnimation();
        OnAnimationFinish += OnFinish;
        _movingTween = transform.DOLocalMove(topPos, animationSpeed).SetEase(Ease.OutQuad).OnComplete(() =>
        _movingTween = transform.DOLocalMove(boardPos, animationSpeed).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            OnAnimationFinish?.Invoke();
            _movingTween = null;
            OnAnimationFinish = null;
        }));
    }
    public void MoveTo(Vector2Int boardPos, Vector3 localPos, Action OnFinish = null)
    {
        PlaceIn(boardPos);
        if (_movingTween != null) TerminateMovingAnimation();

        _pos = boardPos;
        OnAnimationFinish += OnFinish;
        _movingTween = transform.DOLocalMove(localPos, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            OnAnimationFinish?.Invoke();
            _movingTween = null;
            OnAnimationFinish = null;
        });
    }
}
