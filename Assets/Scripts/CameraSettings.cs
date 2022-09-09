using System;

[Serializable]
public class CameraSettings
{
    public float RedSetting;
    public float GreenSetting;
    public float BlueSetting;

    public CameraSettings(float redSetting, float greenSetting, float blueSetting)
    {
        RedSetting = redSetting;
        GreenSetting = greenSetting;
        BlueSetting = blueSetting;
    }

    public CameraSettings()
    {
        RedSetting = 0;
        GreenSetting = 0;
        BlueSetting = 0;
    }
}
