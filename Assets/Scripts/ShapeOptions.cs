using UnityEngine;
using UnityEngine.UI;
public class ShapeOptions : PersitentSetting
{
    [SerializeField]
    private Shape _shape;
    [SerializeField]
    private Image _exampleShape, _exampleShapeOutline;
    [SerializeField]
    private GameObject _colorSelection;
    [SerializeField]
    private Toggle _isOn;
    [SerializeField]
    private Slider _redSlider, _blueSlider, _greenSlider;
    [SerializeField]
    private float _inactiveAlpha = 0.5f, _fullAlpha = 1f, _tintFactor = 0.66f;
    [SerializeField]
    private ShapeSelectionOptions _selectionOptions;

    public Shape OptionShape { get { return _shape; } }
    public Color ShapeColor { get { return _exampleShape.color; } }
    public Color OutlineColor { get { return _exampleShapeOutline.color; } }
    public bool IsActivatedShape { get { return _isOn.isOn; } }

    public void OnActivationToggled(GameObject errorMenu)
    {
        if (!_isOn.isOn && !_selectionOptions.ShapeCanBeDeactivated())
        {
            _isOn.isOn = true;
            errorMenu.SetActive(true);
        }
        OnColorValueChanged();
    }

    public void OnColorValueChanged()
    {
        _colorSelection.SetActive(_isOn.isOn);
        var alphaValue = _isOn.isOn ? _fullAlpha : _inactiveAlpha;
        var redValue = _redSlider.value;
        var blueValue = _blueSlider.value;
        var greenValue = _greenSlider.value;
        _exampleShape.color = new Color(redValue, greenValue, blueValue, alphaValue);
        _exampleShapeOutline.color = GetColorTint(_exampleShape.color, _tintFactor);
    }

    public override void SaveSetting()
    {
        var redValue = _redSlider.value;
        var blueValue = _blueSlider.value;
        var greenValue = _greenSlider.value;

        var newShapeSettings = new ShapeSettings(_isOn.isOn, redValue, greenValue, blueValue);
        var serialziedShapeSettings = JsonUtility.ToJson(newShapeSettings);
        PlayerPrefs.SetString(_shape.ToString(), serialziedShapeSettings);
        PlayerPrefs.Save();
    }

    private void Start()
    {
        var serializedShapeSettings = PlayerPrefs.GetString(_shape.ToString());
        var shapeSetting = JsonUtility.FromJson<ShapeSettings>(serializedShapeSettings) ?? new ShapeSettings();
        _isOn.isOn = shapeSetting.IsActive;
        _redSlider.value = shapeSetting.RedSetting;
        _blueSlider.value = shapeSetting.BlueSetting;
        _greenSlider.value = shapeSetting.GreenSetting;
        OnColorValueChanged();
    }

    private Color GetColorTint(Color color, float tintScale)
    {
        return new Color(GetValueColorTint(color.r, tintScale), GetValueColorTint(color.g, tintScale), GetValueColorTint(color.b, tintScale), color.a);
    }

    private float GetValueColorTint(float colorValue, float tintScale)
    {
        return colorValue + ((1 - colorValue) * tintScale);
    }
}
