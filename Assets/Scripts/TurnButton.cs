using UnityEngine;

public delegate void TurnTaken(PlayerMove move);

public class TurnButton : MonoBehaviour
{
    [SerializeField]
    private ShapeObject _currentTurnsShapePrefab;
    [SerializeField]
    private uint _posX, _posY;

    private TurnTaken _onTurnTaken;
    [SerializeField]
    private bool _buttonPressed = false;
    private ShapeObject _demoShape;

    public uint PosY { get { return _posY; } }
    public uint PosX { get { return _posX; } }
    public bool HasBeenPressed { get { return _buttonPressed; } }

    public void SubscribeToTurnTaken(TurnTaken listener)
    {
        _onTurnTaken += listener;
    }

    public void UnsubscribeFromOnTurnTaken(TurnTaken listener)
    {
        _onTurnTaken -= listener;
    }

    public void SetPlayerTurn(ShapeObject shapeObject)
    {
        _currentTurnsShapePrefab = shapeObject;
    }

    public void ToggleHasBeenPressed(bool value)
    {
        _buttonPressed = value;
    }

    public void UpdatePosition(uint posY, uint posX)
    {
        _posY = posY;
        _posX = posX;
        name = $"Button_position_{_posY}_{_posX}";
    }
    private void OnMouseEnter()
    {
        if (!_buttonPressed && _demoShape == null)
        {
            _demoShape = Instantiate(_currentTurnsShapePrefab, this.transform);
            var spriteRender = _demoShape.GetComponent<SpriteRenderer>();
            spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, spriteRender.color.a / 2);
        }
    }

    private void OnMouseExit()
    {
        if (!_buttonPressed && _demoShape != null)
        {
            Destroy(_demoShape.gameObject);
            _demoShape = null;
        }
    }

    private void OnMouseDown()
    {
        if (!_buttonPressed)
        {
            if (_demoShape != null)
            {
                Destroy(_demoShape.gameObject);
                _demoShape = null;
            }

            _buttonPressed = true;
            var playerMove = new PlayerMove(_currentTurnsShapePrefab.ShapeType, _posY, _posX);

            if (_onTurnTaken != null)
                _onTurnTaken(playerMove);
        }
    }
}
