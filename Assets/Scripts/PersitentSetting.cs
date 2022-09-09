using UnityEngine;

public abstract class PersitentSetting : MonoBehaviour
{
    public abstract string Key { get; }
    public abstract string SerializedValue { get; }
    public abstract void SaveSetting();
}
