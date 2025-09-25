using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "MonsterStat", menuName = "Monster/Stat", order = 0)]
public class MonsterStat : ScriptableObject
{
    public List<MonsterData> monsters = new List<MonsterData>();
    public MonsterData GetById(int id)
    {
        return monsters.FirstOrDefault(m => m.id == id);
    }
    //����ġ ���� ����
    public MonsterData GetRandomByProbability(bool onlyMagic)
    {
        // ���� ���͸�
        var filteredList = monsters.Where(data =>
            onlyMagic ? (data.id >= 8 && data.id <= 10)
                      : (data.id >= 0 && data.id <= 5)
        ).ToList();

        if (filteredList.Count == 0)
            return null; // ����Ʈ�� ��������� null ��ȯ (�Ǵ� ���� ó��)

        // ����ġ ����
        float totalWeight = filteredList.Sum(data => data.probability);
        float rand = Random.Range(0f, totalWeight);

        float current = 0f;
        foreach (var data in filteredList)
        {
            current += data.probability;
            if (rand <= current)
                return data;
        }

        // fallback
        return filteredList[0];
    }

    public GameObject GetRandomDrop(MonsterData monster)
    {
        if (monster.dropItems == null || monster.dropItems.Count == 0)
            return null;

        float total = monster.dropItems.Sum(x => x.dropProbability);

        float rand = Random.Range(0, total);
        float cumulative = 0f;

        foreach (var drop in monster.dropItems)
        {
            cumulative += drop.dropProbability;
            if (rand <= cumulative)
            {
                return drop.item;
            }
        }
        return null;
    }
    

}
