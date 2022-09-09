using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardTranslator : MonoBehaviour
{
    private TicTacToeBoard _board;
    [SerializeField]
    private TurnManager _turnManager;
    [SerializeField]
    private LineManager _lineManager;
    [SerializeField]
    private ShapeManager _shapeManager;

    [SerializeField]
    private uint _ySize = 3, _xSize = 3;
    [SerializeField]
    private Camera _mainCamera;
    [SerializeField]
    private ShapeObject[] _objectPool;
    [SerializeField]
    private WinningMenu _winningMenu;


    public void UpdateBoardStateOnWin(object sender, WinningInformation info)
    {
        _lineManager.UpdateLines(_board.YSize, _board.XSize);

        _shapeManager.UpdateShapesAndButtons(info.WinningShape);
        _shapeManager.ToggleInteraction(false);
        _mainCamera.orthographicSize = (Mathf.Max(_board.XSize, _board.YSize) * 3) - 1;
    }


    public void UpdateBoardState(Shape? newPlayer = null)
    {
        _lineManager.UpdateLines(_board.YSize, _board.XSize);

        if (newPlayer == null)
            newPlayer = _turnManager.GetCurrentPlayersTurn();

        _shapeManager.UpdateShapesAndButtons(newPlayer.Value);
        _mainCamera.orthographicSize = (Mathf.Max(_board.XSize, _board.YSize) * 3) - 1;
    }

    private void OnPlayerTurnUpdated(object sender, Shape newPlayer)
    {
        UpdateBoardState(newPlayer);
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        RestartGame();
    }

    private void OnEnable()
    {
        RestartGame();
    }

    public void RestartGame()
    {
        List<Shape> activeShapes = ModifyShapeObjects();

        _board = new TicTacToeBoard(_ySize, _xSize);

        CreateTurnManager(activeShapes);
        CreateShapeManager();
        CreateLineManager();
        UpdateBoardState();
    }

    private List<Shape> ModifyShapeObjects()
    {
        var activeShapes = new List<Shape>();

        foreach (var shape in _objectPool)
        {
            var jsonOption = PlayerPrefs.GetString(shape.ShapeType.ToString());
            var shapeOption = JsonUtility.FromJson<ShapeSettings>(jsonOption);
            if (shapeOption.IsActive)
            {
                activeShapes.Add(shape.ShapeType);
                var shapeObject = _objectPool.SingleOrDefault(x => x.ShapeType == shape.ShapeType);
                if (shapeObject != null)
                {
                    var mainColor = shapeOption.GetShapeColorWithFullAlpha();
                    var outlineColor = shapeOption.GetShapeColorTintWithFullAlpha(mainColor, 0.66f); //TODO: Remove magic number into preference
                    shapeObject.SetColor(mainColor, outlineColor);
                }
            }
        }

        return activeShapes;
    }

    private void CreateTurnManager(List<Shape> activeShapes)
    {
        if (_turnManager == null)
        {
            _turnManager = new TurnManager(_board, (ushort)_board.XSize, activeShapes);
            _turnManager.SubscribeToOnPlayerTurnUpdate(OnPlayerTurnUpdated);
            _turnManager.SubscribeToOnPlayerWin(_winningMenu.ActivateVictory);
            _turnManager.SubscribeToOnPlayerWin(UpdateBoardStateOnWin);
        }
        else
            _turnManager.ResetManager(_board, (ushort)_board.XSize, activeShapes);

    }

    private void CreateLineManager()
    {
        if (_lineManager == null)
        {
            var lineManagerParent = new GameObject("LineManager");
            lineManagerParent.transform.position = Vector2.zero;
            lineManagerParent.transform.parent = this.gameObject.transform;

            _lineManager = lineManagerParent.AddComponent<LineManager>();
        }
        else
            _lineManager.ResetState();
    }

    private void CreateShapeManager()
    {
        if (_shapeManager == null)
        {
            var shapeManagerParent = new GameObject("ShapeManager");
            shapeManagerParent.transform.position = Vector2.zero;
            shapeManagerParent.transform.parent = this.gameObject.transform;

            _shapeManager = shapeManagerParent.AddComponent<ShapeManager>();
        }

        _shapeManager.Initialize(_board, _turnManager.PlayerMakesTurn);
    }
}
