using UnityEngine;

public class ShapeObject : MonoBehaviour
{
    [SerializeField]
    private Shape _shape;
    [SerializeField]
    private SpriteRenderer _mainShapeRender, _outlineShapeRender;

    public Shape ShapeType { get { return _shape; } }
    public void SetColor(Color mainColor, Color outLineColor)
    {
        _mainShapeRender.color = mainColor;
        _outlineShapeRender.color = outLineColor;
    }
}
