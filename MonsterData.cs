using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterData
{
    public int id;
    public GameObject monsterGameObject;//id�� �´� ���͸� �ֱ�
    public int spawnCount; //���Ͱ� �ڸ�ƾ�� �� ��� �ݺ� �ϴ��� - ��� �����ϴ���
    public float spawnDelay;//�ڸ�ƾ���� ����������� �ð�
    public int dangerLevel;
    public float probability;//Ȯ��
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
        Normal,         // �Ϲ� ���� (�̸�)
        Suicide,        // ������ ����
        MultiAttack,    // ���� ���� ����
        Boss,           // ����
        Ranged,         // ���Ÿ� ����
        Support,        // ���� / ����
        Guard,          // �����, ��� �����ִ� ����
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
