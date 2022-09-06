using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardTranslator : MonoBehaviour
{
    private TicTacToeBoard _board;
    private TurnManager _turnManager;

    [SerializeField]
    private float _yOffset = 2.5f, _xOffset = 2.5f;
    [SerializeField]
    private double _lineScale = 1.25, _spaceOffset = 5;
    [SerializeField]
    private uint _ySize = 3, _xSize = 3;
    [SerializeField]
    private GameObject _verticalLinePrefab, _horizontalLinePrefab;
    [SerializeField]
    private TurnButton _buttonPrefab;
    [SerializeField]
    private ShapeObject[] _shapePrefabs;
    [SerializeField]
    private Camera _mainCamera;

    private List<GameObject> _verticalLines, _horizontalLines;
    private List<ShapeObject> _shapeObjects;
    private List<TurnButton> _buttons;

    private GameObject _emptyForLines;
    private GameObject _emptyForShapes;
    private GameObject _emptyForButtons;

    public void UpdateBoardState(Shape? newPlayer = null)
    {
        var verticalScale = new Vector3(0.1f, (float)(_lineScale * _board.XSize), 1);
        var horizontalScale = new Vector3(0.1f, (float)(_lineScale * _board.YSize), 1);

        CalculateNeededLinePositions(out List<Vector3> verticalLinePositions, out List<Vector3> horizontalLinePositions);
        UpdateLineObjects(verticalScale, horizontalScale, verticalLinePositions, horizontalLinePositions);
        UpdateShapes();

        if (newPlayer == null)
            newPlayer = _turnManager.GetCurrentPlayersTurn();

        foreach (var button in _buttons.Where(x => !x.HasBeenPressed))
        {
            button.SetPlayerTurn(_shapePrefabs.Single(x => x.ShapeType == newPlayer));
        }
    }

    private void OnPlayerTurnUpdated(object sender, Shape newPlayer)
    {
        UpdateBoardState(newPlayer);
    }

    private void UpdateShapes()
    {
        for (uint yPos = 0; yPos < _board.YSize; yPos++)
        {
            for (uint xPos = 0; xPos < _board.XSize; xPos++)
            {
                var normalizedPosY = ((int)yPos - (_horizontalLines.Count / 2)) * _spaceOffset;
                var normalizedPosX = ((int)xPos - (_verticalLines.Count / 2)) * _spaceOffset;

                var postionToCheck = new Vector3((float)normalizedPosX, (float)normalizedPosY);

                var shape = _board.GetShapeAtPosition(yPos, xPos);
                var shapeAtPosition = _shapeObjects.SingleOrDefault(shapeObj => shapeObj.transform.position == postionToCheck);
                if ((shape == null && shapeAtPosition != null)
                    || (shape != null && shapeAtPosition == null)
                    || (shape != shapeAtPosition?.ShapeType))
                {
                    if (shapeAtPosition != null)
                    {
                        _shapeObjects.Remove(shapeAtPosition);
                        Destroy(shapeAtPosition.gameObject);
                    }

                    if (shape != null)
                    {
                        var shapeToMake = _shapePrefabs.Single(x => x.ShapeType == shape);
                        var newShape = Instantiate(shapeToMake, postionToCheck, shapeToMake.transform.rotation, _emptyForShapes.transform);
                        newShape.name = $"{shapeToMake.name}_position_{xPos}_{yPos}";
                        _shapeObjects.Add(newShape);
                    }
                }

                var button = _buttons.SingleOrDefault(bttn => bttn.PosY == yPos && bttn.PosX == xPos);
                if (button == null)
                {
                    var newButton = Instantiate(_buttonPrefab, postionToCheck, _buttonPrefab.transform.rotation, _emptyForButtons.transform);
                    newButton.UpdatePosition(yPos, xPos);
                    newButton.SubscribeToTurnTaken(_turnManager.PlayerMakesTurn);
                    _buttons.Add(newButton);
                }
                else
                {
                    button.ToggleHasBeenPressed(shape != null);
                    button.transform.position = postionToCheck;
                }    
            }
        }
    }

    private void CalculateNeededLinePositions(out List<Vector3> verticalLinePositions, out List<Vector3> horizontalLinePositions)
    {
        verticalLinePositions = new List<Vector3>();
        horizontalLinePositions = new List<Vector3>();
        for (int i = 0; i < (_board.YSize - 1) / 2; i++)
        {
            var xPosition = i * 5;
            verticalLinePositions.Add(new Vector3((float)(-xPosition - _xOffset), 0, 0));
            verticalLinePositions.Add(new Vector3((float)(xPosition + _xOffset), 0, 0));
        }

        for (int i = 0; i < (_board.XSize - 1) / 2; i++)
        {
            var yPosition = i * 5;
            horizontalLinePositions.Add(new Vector3(0, (float)(-yPosition - _yOffset), 0));
            horizontalLinePositions.Add(new Vector3(0, (float)(yPosition + _yOffset), 0));
        }
    }

    private void UpdateLineObjects(Vector3 verticalScale, Vector3 horizontalScale, List<Vector3> verticalLinePositions, List<Vector3> horizontalLinePositions)
    {
        foreach (var position in horizontalLinePositions)
        {
            var currentLine = _horizontalLines.SingleOrDefault(line => line.transform.position == position);
            if (currentLine is null)
            {
                var newLine = Instantiate(_horizontalLinePrefab, position, _horizontalLinePrefab.transform.rotation, _emptyForLines.transform);
                newLine.name = $"HorizontalLine_{position.y}_{position.x}";
                newLine.transform.localScale = horizontalScale;
                _horizontalLines.Add(newLine);
            }
            else
                currentLine.transform.localScale = horizontalScale;
        }

        foreach (var position in verticalLinePositions)
        {
            var currentLine = _verticalLines.SingleOrDefault(line => line.transform.position == position);
            if (currentLine is null)
            {
                var newLine = Instantiate(_verticalLinePrefab, position, _verticalLinePrefab.transform.rotation, _emptyForLines.transform);
                newLine.name = $"VerticalLine_{position.y}_{position.x}";
                newLine.transform.localScale = verticalScale;
                _verticalLines.Add(newLine);
            }
            else
                currentLine.transform.localScale = verticalScale;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (_board == null)
            _board = new TicTacToeBoard(_ySize, _xSize);
        
        if (_turnManager == null)
            _turnManager = new TurnManager(_board, (ushort)_board.XSize, _shapePrefabs.Select(x => x.ShapeType).ToList());
        _turnManager.SubscribeToOnPlayerTurnUpdate(OnPlayerTurnUpdated);

        _emptyForLines = new GameObject("LineParent");
        _emptyForLines.transform.position = Vector2.zero;
        _emptyForButtons = new GameObject("ButtonParent");
        _emptyForButtons.transform.position = Vector2.zero;
        _emptyForShapes = new GameObject("ShapeParent");
        _emptyForShapes.transform.position = Vector2.zero;

        _verticalLines = new List<GameObject>();
        _horizontalLines = new List<GameObject>();
        _shapeObjects = new List<ShapeObject>();
        _buttons = new List<TurnButton>();

        UpdateBoardState();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
