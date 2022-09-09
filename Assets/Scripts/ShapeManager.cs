using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShapeManager : MonoBehaviour
{
    [SerializeField]
    private double _spaceOffset = 5;
    [SerializeField]
    private TurnButton _buttonPrefab;
    [SerializeField]
    private ShapeObject[] _shapePrefabs;

    private TicTacToeBoard _board;
    private TurnTaken _updateAction;

    private List<ShapeObject> _shapeObjects = new List<ShapeObject>();
    private List<TurnButton> _buttons = new List<TurnButton>();

    private GameObject _emptyForShapes;
    private GameObject _emptyForButtons;

    public void Initialize(TicTacToeBoard board, TurnTaken action)
    {
        _updateAction = null;

        _board = board;
        _updateAction += action;

        for (int i = 0; i < _shapeObjects.Count; i++)
            Destroy(_shapeObjects[i].gameObject);

        for (int i = 0; i < _buttons.Count; i++)
            Destroy(_buttons[i].gameObject);

        _shapeObjects = new List<ShapeObject>();
        _buttons = new List<TurnButton>();

        CreateEmptyForButtons();
        CreateEmptyForShapes();
    }

    public void ToggleInteraction(bool interaction)
    {
        foreach (var button in _buttons)
            button.gameObject.SetActive(interaction);
    }

    public void UpdateShapesAndButtons(Shape currentPlayersMove)
    {
        for (uint yPos = 0; yPos < _board.YSize; yPos++)
        {
            for (uint xPos = 0; xPos < _board.XSize; xPos++)
            {
                var normalizedPosY = ((int)yPos - ((_board.YSize - 1) / 2)) * _spaceOffset;
                var normalizedPosX = ((int)xPos - ((_board.XSize - 1) / 2)) * _spaceOffset;

                var positionToCheck = new Vector3((float)normalizedPosX, (float)normalizedPosY);
                var shapeAtPosition = _shapeObjects.SingleOrDefault(shapeObj => shapeObj.transform.position == positionToCheck);
                var shape = _board.GetShapeAtPosition(yPos, xPos);

                UpdateShape(yPos, xPos, positionToCheck, shapeAtPosition, shape);
                UpdateButton(yPos, xPos, positionToCheck, shape);
            } 
        }

        foreach (var bttn in _buttons.Where(x => !x.HasBeenPressed))
        {
            bttn.SetPlayerTurn(_shapePrefabs.Single(x => x.ShapeType == currentPlayersMove));
        }
    }

    private void UpdateShape(uint yPos, uint xPos, Vector3 positionToCheck, ShapeObject shapeObjectAtPosition, Shape? shapeAtPosition)
    {
        if ((shapeAtPosition == null && shapeObjectAtPosition != null)
            || (shapeAtPosition != null && shapeObjectAtPosition == null)
            || (shapeAtPosition != shapeObjectAtPosition?.ShapeType))
        {
            if (shapeObjectAtPosition != null)
            {
                _shapeObjects.Remove(shapeObjectAtPosition);
                Destroy(shapeObjectAtPosition.gameObject);
            }

            if (shapeAtPosition != null)
            {
                var shapeToMake = _shapePrefabs.Single(x => x.ShapeType == shapeAtPosition);
                var newShape = Instantiate(shapeToMake, positionToCheck, shapeToMake.transform.rotation, _emptyForShapes.transform);
                newShape.name = $"{shapeToMake.name}_position_{yPos}_{xPos}";
                _shapeObjects.Add(newShape);
            }
        }
    }

    private void UpdateButton(uint ySize, uint xSize, Vector3 positionToCheck, Shape? shapeAtPosition)
    {
        var button = _buttons.SingleOrDefault(bttn => bttn.PosY == ySize && bttn.PosX == xSize);
        if (button == null)
        {
            var newButton = Instantiate(_buttonPrefab, positionToCheck, _buttonPrefab.transform.rotation, _emptyForButtons.transform);
            newButton.UpdatePosition(ySize, xSize);
            newButton.SubscribeToTurnTaken(_updateAction);
            newButton.ToggleHasBeenPressed(shapeAtPosition != null);
            _buttons.Add(newButton);
        }
        else
        {
            button.ToggleHasBeenPressed(shapeAtPosition != null);
            button.transform.position = positionToCheck;
        }
    }

    private void CreateEmptyForShapes()
    {
        if (_emptyForShapes == null)
        {
            _emptyForShapes = new GameObject("ShapeHolder");
            _emptyForShapes.transform.position = Vector2.zero;
            _emptyForShapes.transform.parent = this.gameObject.transform;
        }
    }

    private void CreateEmptyForButtons()
    {
        if (_emptyForButtons == null)
        {
            _emptyForButtons = new GameObject("ButtonHolder");
            _emptyForButtons.transform.position = Vector2.zero;
            _emptyForButtons.transform.parent = this.gameObject.transform;
        }
    }
}
