using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "Item", menuName = "Item/Data", order = 0)]
public class Item : ScriptableObject
{
    public ItemData itemData;
    string FilePath;
    private void OnEnable()
    {
        FilePath = Path.Combine(Application.persistentDataPath, "Item.json");
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(itemData, true);
        File.WriteAllText(FilePath, json);
        Debug.Log("���� �Ϸ�");
    }

    public void Load()
    {
        if(File.Exists(FilePath))
        { 
         string json = File.ReadAllText(FilePath);
            JsonUtility.FromJsonOverwrite(json, itemData);
            Debug.Log("�ҷ�����");
        }
    }

    public ItemData GetItemData()
    {
        return itemData;
    }
}
