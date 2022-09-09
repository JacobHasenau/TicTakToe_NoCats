using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WinningMenu : MonoBehaviour
{
    [SerializeField]
    private List<UIShapeObject> _shapeObjects;
    [SerializeField]
    private Image _victoryShape, _victoryShapeOutline;
    [SerializeField]
    private Text _winningText, _lossingText;
    [SerializeField]
    private BoardTranslator _oldBoard;
    [SerializeField]
    private MainMenu _menu;

    public void ActivateVictory(object sender, WinningInformation info)
    {
        gameObject.SetActive(true);
        SetVictoryShape(info.WinningShape);
        _winningText.text = string.Format("{0} {1}", info.WinningShape, GetVictoryPostFix());

        var lossingShapes = info.PlayingShapes.Where(x => x != info.WinningShape);
        var lossingShapesToString = "";

        if (lossingShapes.Count() <= 1)
            lossingShapesToString = lossingShapes.First().ToString();
        else if (lossingShapes.Count() == 2)
            lossingShapesToString = string.Join(" and ", lossingShapes.Select(x => x.ToString()));
        else
        {
            lossingShapesToString = string.Join(", ", lossingShapes.Select(x => x.ToString()));
            lossingShapesToString = lossingShapesToString.Insert(lossingShapesToString.LastIndexOf(',') + 1, " and");
        }
        
        _lossingText.text = string.Format("{0}, {1}", lossingShapesToString, GetLossingPostFix());
    }

    public void OnNewGame(bool startActive)
    {
        if(startActive)
            _oldBoard.RestartGame();
        else
            _oldBoard.gameObject.SetActive(startActive);
    }

    private void SetVictoryShape(Shape shape)
    {
        var jsonOption = PlayerPrefs.GetString(shape.ToString());
        var shapeOption = JsonUtility.FromJson<ShapeSettings>(jsonOption);
        var uiShape = _shapeObjects.SingleOrDefault(x => x.ShapeType == shape);
        var mainColor = shapeOption.GetShapeColorWithFullAlpha();
        var outlineColor = shapeOption.GetShapeColorTintWithFullAlpha(mainColor, 0.66f); //TODO: Remove magic number into preference
        uiShape.SetColor(mainColor, outlineColor);

        _victoryShape.sprite = uiShape.MainShape;
        _victoryShape.color = uiShape.MainShapeColor;

        _victoryShapeOutline.sprite = uiShape.OutlineShape;
        _victoryShapeOutline.color = uiShape.OutlineColor;
    }

    private string GetVictoryPostFix()
    {
        return "has achived victory!";
    }

    private string GetLossingPostFix()
    {
        return "you've lost. better luck next time.";
    }

    private void Start()
    {
        if (_victoryShape == null)
            _victoryShape = gameObject.GetComponent<Image>();
        if (_victoryShapeOutline == null)
            _victoryShapeOutline = gameObject.GetComponentInChildren<Image>();
    }
}
