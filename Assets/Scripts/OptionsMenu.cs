using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField]
    private Button _saveButton;
    [SerializeField]
    private List<PersitentSetting> _settingsToSave;

    private PersitenseManager _persitenseManager;

    public void OnValueChange()
    {
        _saveButton.interactable = true;
    }

    public void SaveSettings()
    {
        _persitenseManager.SaveSettings(_settingsToSave);
        _saveButton.interactable = false;
    }

    public void LoadNewScreen(GameObject screen)
    {
        screen.SetActive(true);
        gameObject.SetActive(false);
    }

    private void Start()
    {
        _persitenseManager = FindObjectOfType<PersitenseManager>();
        _settingsToSave = GetComponentsInChildren<PersitentSetting>().ToList();
    }
}
