using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShapeSelectionOptions : MonoBehaviour
{
    [SerializeField]
    private ushort _minPlayers = 2;
    [SerializeField]
    private List<ShapeOptions> _shapeOptions;

    public bool ShapeCanBeDeactivated(bool hasAlreadyBeenDeactivated = true)
    {
        var activatedShapes = _shapeOptions
            .Where(x => x.IsActivatedShape)
            .Count() + (hasAlreadyBeenDeactivated ? 1 : 0);

        return activatedShapes > _minPlayers;
    }

    private void Start()
    {
        _shapeOptions = gameObject.GetComponentsInChildren<ShapeOptions>().ToList();
    }
}
