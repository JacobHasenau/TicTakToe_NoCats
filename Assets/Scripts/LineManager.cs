using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    [SerializeField]
    private float _yOffset = 2.5f, _xOffset = 2.5f;
    [SerializeField]
    private Vector3 _initialScale = new Vector3(0.1f, 1.25f, 1);
    [SerializeField]
    private GameObject _verticalLinePrefab, _horizontalLinePrefab;

    private List<GameObject> _verticalLines = new List<GameObject>(), _horizontalLines = new List<GameObject>();

    public void ResetState()
    {
        for (int i = 0; i < _verticalLines.Count; i++)
            Destroy(_verticalLines[i].gameObject);
        for (int i = 0; i < _horizontalLines.Count; i++)
            Destroy(_horizontalLines[i].gameObject);

        _verticalLines = new List<GameObject>();
        _horizontalLines = new List<GameObject>();
    }

    public void UpdateLines(uint ySize, uint xSize)
    {
        var verticalScale = new Vector3(_initialScale.x, _initialScale.y * xSize, _initialScale.z);
        var horizontalScale = new Vector3(_initialScale.x, _initialScale.y * ySize, _initialScale.z);

        CalculateNeededLinePositions(out List<Vector3> verticalLinePositions, out List<Vector3> horizontalLinePositions, ySize, xSize);
        UpdateLineObjects(verticalScale, horizontalScale, verticalLinePositions, horizontalLinePositions);
    }

    private void CalculateNeededLinePositions(out List<Vector3> verticalLinePositions, out List<Vector3> horizontalLinePositions, uint ySize, uint xSize)
    {
        verticalLinePositions = new List<Vector3>();
        horizontalLinePositions = new List<Vector3>();
        for (int i = 0; i < (ySize - 1) / 2; i++)
        {
            var xPosition = i * 5;
            verticalLinePositions.Add(new Vector3((float)(-xPosition - _xOffset), 0, 0));
            verticalLinePositions.Add(new Vector3((float)(xPosition + _xOffset), 0, 0));
        }

        for (int i = 0; i < (xSize - 1) / 2; i++)
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
                var newLine = Instantiate(_horizontalLinePrefab, position, _horizontalLinePrefab.transform.rotation, this.transform);
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
                var newLine = Instantiate(_verticalLinePrefab, position, _verticalLinePrefab.transform.rotation, this.transform);
                newLine.name = $"VerticalLine_{position.y}_{position.x}";
                newLine.transform.localScale = verticalScale;
                _verticalLines.Add(newLine);
            }
            else
                currentLine.transform.localScale = verticalScale;
        }
    }
}
