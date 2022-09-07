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
    private ShapeObject[] _objests;

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

    // Start is called before the first frame update
    private void Start()
    {
        if (_board == null)
            _board = new TicTacToeBoard(_ySize, _xSize);
        
        if (_turnManager == null)
            _turnManager = new TurnManager(_board, (ushort)_board.XSize, _objests.Select(x => x.ShapeType).ToList());
        _turnManager.SubscribeToOnPlayerTurnUpdate(OnPlayerTurnUpdated);

        if (_shapeManager == null)
        {
            var shapeManagerParent = new GameObject("ShapeManager");
            shapeManagerParent.transform.position = Vector2.zero;
            shapeManagerParent.transform.parent = this.gameObject.transform;

            _shapeManager = shapeManagerParent.AddComponent<ShapeManager>();
        }

        _shapeManager.Initialize(_board, _turnManager.PlayerMakesTurn);

        if (_lineManager == null)
        {
            var lineManagerParent = new GameObject("LineManager");
            lineManagerParent.transform.position = Vector2.zero;
            lineManagerParent.transform.parent = this.gameObject.transform;

            _lineManager = lineManagerParent.AddComponent<LineManager>();
        }

        UpdateBoardState();
    }
}
