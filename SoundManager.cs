using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "Sound", menuName = "SoundData")]
public class SoundManager : ScriptableObject
{
    public SoundData SoundData;
    string FilePath;
    private void OnEnable()
    {
        FilePath = Path.Combine(Application.persistentDataPath, "Satting.json");
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(SoundData, true);
        File.WriteAllText(FilePath, json);
    }

    public void Load()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            JsonUtility.FromJsonOverwrite(json, SoundData);

        }
    }

    public SoundData GetSoundData()
    {
        return SoundData;
    }
}
