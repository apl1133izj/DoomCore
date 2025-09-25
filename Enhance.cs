using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Enhance", menuName = "EnhanceData")]
public class Enhance : ScriptableObject
{
    public List<EnhanceData> enhanceData;
    string FillPathBase;
    private void OnEnable()
    {
        FillPathBase = Path.Combine(Application.persistentDataPath, "enhanceData.json");
    }


    public void Save(int i)
    {
        string path = FillPathBase + i;
        string json = JsonUtility.ToJson(enhanceData[i], true);
        File.WriteAllText(path, json);
    }

    public void Load(int i)
    {
        string path = FillPathBase + i;
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            if (enhanceData[i] == null)
                enhanceData[i] = new EnhanceData(); // null이면 새로 생성
            JsonUtility.FromJsonOverwrite(json, enhanceData[i]);
        }
    }

    public EnhanceData GetEnhanceData(int id)
    {
        return enhanceData.FirstOrDefault(m => m.id == id);
    }
}
