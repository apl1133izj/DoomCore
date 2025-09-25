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
    //가중치 랜덤 구현
    public MonsterData GetRandomByProbability(bool onlyMagic)
    {
        // 조건 필터링
        var filteredList = monsters.Where(data =>
            onlyMagic ? (data.id >= 8 && data.id <= 10)
                      : (data.id >= 0 && data.id <= 5)
        ).ToList();

        if (filteredList.Count == 0)
            return null; // 리스트가 비어있으면 null 반환 (또는 예외 처리)

        // 가중치 총합
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
