using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;
    // Start is called before the first frame update
    void Start()
    {
        _camera = this.GetComponent<Camera>();
        var serializedShapeSettings = PlayerPrefs.GetString(nameof(CameraSettings));
        var cameraSetting = JsonUtility.FromJson<CameraSettings>(serializedShapeSettings) ?? new CameraSettings();
        _camera.backgroundColor = new Color(cameraSetting.RedSetting, cameraSetting.GreenSetting, cameraSetting.BlueSetting);
    }
}
