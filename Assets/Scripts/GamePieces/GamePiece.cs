using DG.Tweening;
using System;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    Vector2Int _pos;
    Board.Player _player;
    Tween _movingTween;

    [SerializeField] float animationSpeed = 0.2f;

    public Vector2Int pos => _pos;
    public Board.Player player => _player;
    public bool isInAnimation => _movingTween != null;
    public Func<Vector2Int, Vector3> cellToLocal;
    public Action OnAnimationFinish;
    public void AssignPlayer(Board.Player player)
    {
        _player = player; 
    }
    public void TerminateMovingAnimation()
    {
        _movingTween.Kill(false);
        OnAnimationFinish?.Invoke();
        _movingTween = null;
        OnAnimationFinish = null;

    }
    public void PlaceIn(Vector2Int boardPos, Vector2Int topPos, Action OnFinish = null)
    {
        if (_movingTween != null) TerminateMovingAnimation();

        _pos = boardPos;
        Vector3 localTopPos = cellToLocal(topPos);
        Vector3 localPos = cellToLocal(boardPos);
        OnAnimationFinish += OnFinish;
        _movingTween = transform.DOLocalMove(localTopPos, animationSpeed).SetEase(Ease.OutQuad).OnComplete(() => 
        _movingTween = transform.DOLocalMove(localPos, animationSpeed).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            OnAnimationFinish?.Invoke();
            _movingTween = null;
            OnAnimationFinish = null;
        }));
    }
    public void MoveTo(Vector2Int boardPos, Action OnFinish = null)
    {
        if (_movingTween != null) TerminateMovingAnimation();

        _pos = boardPos;
        Vector3 localPos = cellToLocal(boardPos);
        OnAnimationFinish += OnFinish;
        _movingTween = transform.DOLocalMove(localPos, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            OnAnimationFinish?.Invoke();
            _movingTween = null;
            OnAnimationFinish = null;
        });
    }

    public void SetBoardPos(Vector2Int boardPos)
    {
        _pos = boardPos;
    }

}
