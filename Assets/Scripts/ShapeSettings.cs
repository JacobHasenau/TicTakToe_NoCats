using System;
using UnityEngine;

[Serializable]
public class ShapeSettings
{
    public bool IsActive;
    public float RedSetting;
    public float GreenSetting;
    public float BlueSetting;

    public ShapeSettings(bool isActive, float redSetting, float greenSetting, float blueSetting)
    {
        IsActive = isActive;
        RedSetting = redSetting;
        GreenSetting = greenSetting;
        BlueSetting = blueSetting;
    }

    public ShapeSettings()
    {
        IsActive = true;
        RedSetting = 1;
        GreenSetting = 1;
        BlueSetting = 1;
    }

    public Color GetShapeColorWithFullAlpha()
    {
        return new Color(RedSetting, GreenSetting, BlueSetting);
    }
    public Color GetShapeColorTintWithFullAlpha(Color color, float tintScale)
    {
        return new Color(GetValueColorTint(color.r, tintScale), GetValueColorTint(color.g, tintScale), GetValueColorTint(color.b, tintScale), color.a);
    }

    private float GetValueColorTint(float colorValue, float tintScale)
    {
        return colorValue + ((1 - colorValue) * tintScale);
    }
}
