using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ControllerManager
{
    IBoardControllable _board;
    IGrid _grid;
    BoardSpaceTransforms _transforms;
    bool controllersEnabled = false;
    [SerializeField] List<BoardControllerBase> _controllers;

    public void SetBoard(BoardModel board) 
    {
        _board = board;
        _grid = board;
        _transforms = board.transforms;

        foreach (var controller in _controllers) 
        {
            controller.grid = _grid;
            controller.transforms = _transforms;
        }
    }
    public void EnableAll()
    {
        controllersEnabled = true;
        foreach (BoardControllerBase controller in _controllers)
        {
            EnableController(controller);
        }
    }
    public void DisableAll()
    {
        controllersEnabled = false;
        foreach (BoardControllerBase controller in _controllers)
        {
            DisableController(controller);
        }
    }
    public void RegisterController(BoardControllerBase controller)
    {
        _controllers.Add(controller);
        controller.grid = _grid;
        controller.transforms = _transforms;
        if (controllersEnabled)
        {
            EnableController(controller);
        }
    }
    public void UnregisterController(BoardControllerBase controller)
    {
        if (controllersEnabled)
        {
            DisableController(controller);
        }
        _controllers.Remove(controller);
    }
    public void EnableController(IBoardController controller)
    {
        controller.OnClickRequest += _board.RequestClick;
        controller.OnSplitRequest += _board.RequestSplit;
        controller.OnCursorPlaceRequest += _board.RequestCursorSet;
        controller.allowInteraction = true;
    }
    public void DisableController(IBoardController controller)
    {
        controller.allowInteraction = false;
        controller.OnClickRequest -= _board.RequestClick;
        controller.OnSplitRequest -= _board.RequestSplit;
        controller.OnCursorPlaceRequest -= _board.RequestCursorSet;
    }
}
