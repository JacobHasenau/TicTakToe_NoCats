using UnityEngine;

public class ShapeObject : MonoBehaviour 
{
    [SerializeField]
    private Shape _shape;

    public Shape ShapeType { get { return _shape; } }
}
