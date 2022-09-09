using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundColorSelection : MonoBehaviour
{
    [SerializeField]
    private Image _exampleShape;
    [SerializeField]
    private Slider _redSlider, _blueSlider, _greenSlider;

    public void OnValueChanged()
    {
        var redValue = _redSlider.value;
        var blueValue = _blueSlider.value;
        var greenValue = _greenSlider.value;
        _exampleShape.color = new Color(redValue, greenValue, blueValue);
        var newCameraSettings = new CameraSettings(redValue, greenValue, blueValue);
        var serialziedShapeSettings = JsonUtility.ToJson(newCameraSettings);
        PlayerPrefs.SetString(nameof(CameraSettings), serialziedShapeSettings);
        PlayerPrefs.Save();
        Camera.main.backgroundColor = _exampleShape.color;
    }

    private void Start()
    {
        var serializedShapeSettings = PlayerPrefs.GetString(nameof(CameraSettings));
        var shapeSetting = JsonUtility.FromJson<CameraSettings>(serializedShapeSettings) ?? new CameraSettings();
        _redSlider.value = shapeSetting.RedSetting;
        _blueSlider.value = shapeSetting.BlueSetting;
        _greenSlider.value = shapeSetting.GreenSetting;
        OnValueChanged();
    }
}
