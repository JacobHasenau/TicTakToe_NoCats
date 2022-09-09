using System.Collections.Generic;
using UnityEngine;

public class PersitenseManager : MonoBehaviour
{
    [SerializeField]
    private List<PersitentSetting> _defaultSettings;

    public void SaveSettings(IReadOnlyCollection<PersitentSetting> settingsToSave)
    {
        foreach (var setting in settingsToSave)
            PlayerPrefs.SetString(setting.Key, setting.SerializedValue);

        PlayerPrefs.Save();
    }

    private void Start()
    {
        var setBefore = PlayerPrefs.GetInt("ShapesSet");
        if (setBefore == 0)
        {
            SaveSettings(_defaultSettings);
            PlayerPrefs.SetInt("ShapesSet", 1);
        }

    }
}
