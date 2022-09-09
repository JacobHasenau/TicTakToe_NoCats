using UnityEngine;

public class UIShapeObject : MonoBehaviour
{
    [SerializeField]
    private Shape _shape;
    [SerializeField]
    private Sprite _mainShapeRender, _outlineShapeRender;

    private Color _mainShapeColor, _outlineColor;

    public Shape ShapeType { get { return _shape; } }
    public Sprite MainShape { get { return _mainShapeRender; } }
    public Sprite OutlineShape { get { return _outlineShapeRender; } }
    public Color MainShapeColor { get { return _mainShapeColor; } }
    public Color OutlineColor { get { return _outlineColor; } }

    public void SetColor(Color mainColor, Color outLineColor)
    {
        _mainShapeColor = mainColor;
        _outlineColor = outLineColor;
    }
}
