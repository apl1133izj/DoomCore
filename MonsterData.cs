using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterData
{
    public int id;
    public GameObject monsterGameObject;//id에 맞는 몬스터를 넣기
    public int spawnCount; //몬스터가 코르틴에 들어가 몇번 반복 하는지 - 몇번 생성하는지
    public float spawnDelay;//코르틴에서 재생성까지의 시간
    public int dangerLevel;
    public float probability;//확률
    public string name;
    public int maxHP;
    public float moveSpeed;
    public float attackDamage;
    public float attackRange;
    public float attackDelay;
    public float detectionRange;
    public float baseScale;
    public bool magic;
    public enum EnemyType
    {
        Normal,         // 일반 몬스터 (쫄몹)
        Suicide,        // 자폭형 몬스터
        MultiAttack,    // 여러 공격 패턴
        Boss,           // 보스
        Ranged,         // 원거리 공격
        Support,        // 힐러 / 버퍼
        Guard,          // 방어형, 대신 막아주는 몬스터
    }
    public EnemyType enemyType;

    [System.Serializable]
    public class DropItemInfo
    {
        public GameObject item;
        public float dropProbability;
    }
    [System.Serializable]
    public class DropModulesInfo
    {
        public GameObject ModulesItem;
        public float ModulesItemProbability;
    }

    public List<DropItemInfo> dropItems;
    public List<DropModulesInfo> dropModulesItems;
}
