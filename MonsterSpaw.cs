using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MonsterSpaw : MonoBehaviour
{
    public MonsterStat monsterStat;
    public List<Transform> comparisonPoint;//���� ���� ����Ʈ ��
    public GameObject playerFind;
    public float maxDistance = 60f;
    public GameObject closePlayerPoint;//�÷��̾�� ����� ����Ʈ���� ���� ����
    private GameObject lastNearestPoint;//�Ѱ��� ���� �ӹ��� �ִ��� Ȯ��
    public GameObject miniboseMonster;
    public float samePointTimer = 0f;
    public float maxSameTime = 180; // 3��
    private int lastTrun = -1; // �߰�


    public GameObject monster;
    public int currentSpawCount = 0;
    //public MonsterData monsterData;
    public Transform baseTransform;
    public bool onlyTakesMagicDamage;//���� ���ݸ� ��ȿ���� - ���� ��� �϶�
    public Coroutine spaw;
    public Coroutine penaltyMonsterSpawCoroutine;
    [Header("idȮ��")]
    public int id;
    private float penaltyCheckTimer = 0f;
    private float maxPenaltyCheckTimer = 20f;
    float nextframe;
    void Start()
    {
        playerFind = GameObject.FindWithTag("Player");
    }
    void Update()
    {
        if (Manager.Instance.eventManagerActive.activeSelf) return;
        if (Manager.Instance.gameOverSceneActive || Manager.Instance.gameClearEvent)
        {
            return;
        }
        if (Time.time > nextframe)
        {
            nextframe += 0.05f;
            comparisonDistance();
        }

        int currentTurn = Manager.Instance.currentTrun;

        if (!ApplyEvent.instance.lurkingHarvest)
        {
            if (currentTurn != lastTrun)
            {
                lastTrun = currentTurn;

                // ���� �ٲ������ ���� �ڷ�ƾ ���� �� null ó��
                if (spaw != null || Manager.Instance.killedMonsterCount == Manager.Instance.GetTurnMonsterCount(Manager.Instance.currentTrun))
                {
                    StopCoroutine(spaw);
                    spaw = null;
                }
            }
        }
        if (Manager.Instance.turnState == Manager.TurnState.RepairTime)
        {
            penaltyCheckTimer = 0f;
            penaltyMonsterSpawCoroutine = null;
        }
        if (Manager.Instance.turnState == Manager.TurnState.Battle)
        {


            float t = Mathf.Clamp01(Manager.Instance.currentTrun / 100);
            maxPenaltyCheckTimer = Mathf.Lerp(20, 10, t);
            penaltyCheckTimer += Time.deltaTime;

            //��Ÿ���� ��������  + �ڷ�ƾ�� ���� �� ����
            if (penaltyCheckTimer >= maxPenaltyCheckTimer && penaltyMonsterSpawCoroutine == null)
            {
                penaltyMonsterSpawCoroutine = StartCoroutine(PenaltyMonsterSpawSequence());
            }

        }
        if (spaw != null || ApplyEvent.instance.lurkingHarvest) return;

        int turnMonsterCount = Manager.Instance.GetTurnMonsterCount(Manager.Instance.currentTrun);
        int spawned = Manager.Instance.spawMonsterCount;

        if (spawned >= turnMonsterCount) return;

        spaw = StartCoroutine(Spaw());
    }
    int maxSpaw = 15;
    public int spawCount;
    IEnumerator Spaw()
    {
        yield return new WaitUntil(() => Manager.Instance.turnState == Manager.TurnState.Battle);
        MonsterData monsterData = monsterStat.GetRandomByProbability(Manager.Instance.currentTrun % 5 == 0);
        id = monsterData.id;
        currentSpawCount = 0;
        maxSpaw = Manager.Instance.currentTrun % 5 == 0 ? 13 : 20;
        while (currentSpawCount < monsterData.spawnCount)
        {
            yield return new WaitForSeconds(monsterData.spawnDelay);
            yield return new WaitUntil(() => spawCount < maxSpaw);
            spawCount++;
            if (Manager.Instance.spawMonsterCount % 14 == 0 && Manager.Instance.spawMonsterCount != 0)
            {
                MonsterData forceMonster = monsterStat.GetById(Manager.Instance.currentTrun % 5 == 0 ? 15 : 6);
                var monster = PoolManager.Instance.GetFromPool<PoolChild>(
                    forceMonster.monsterGameObject,
                    closePlayerPoint.transform.position,
                    Quaternion.identity
                );
                Enemy_Base enemy_Base = monster.GetComponent<Enemy_Base>();
                monster.name = forceMonster.name;
                if (forceMonster.name == "onlyTakesPhysicsDamageBose" || forceMonster.name == "onlyTakesMagicDamageBose" || forceMonster.name == "MiniBose")
                {
                    yield return new WaitUntil(() => !monster.gameObject.activeSelf || !enemy_Base.Die);
                }

            }
            else
            {
                var monsters = PoolManager.Instance.GetFromPool<PoolChild>(monsterData.monsterGameObject,
                                                                                           closePlayerPoint.transform.position
                                                                                           , Quaternion.identity);
                Enemy_Base enemy_Base = monsters.GetComponent<Enemy_Base>();
                monsters.name = monsterData.name;
                if (monsters.name == "onlyTakesPhysicsDamageBose" || monsters.name == "onlyTakesMagicDamageBose" || monsters.name == "MiniBose")
                {
                    yield return new WaitUntil(() => !monsters.gameObject.activeSelf || !enemy_Base.Die);
                }
            }
            currentSpawCount++;

            Manager.Instance.spawMonsterCount++;
            float t = Mathf.Clamp01((float)Manager.Instance.currentTrun / 100);
            if (Manager.Instance.physicalAttackTrun)
            {
                float delayBetween = Mathf.Lerp(2, 0.5f, t);
                yield return new WaitForSeconds(delayBetween);
            }
            else
            {
                float delayBetween = Mathf.Lerp(5, 2f, t);
                yield return new WaitForSeconds(delayBetween);
            }


        }
        spaw = null;
    }


    IEnumerator PenaltyMonsterSpawSequence()
    {
        // 1�� ����
        yield return StartCoroutine(PenaltyMonsterSpaw(13));

        // 1�� ���Ϳ� ��ġ�� �ʰ� ��� ������
        //float delayBetween = 30f; // 1�� �� 20�� �� 2�� ���� ����
        float t = Mathf.Clamp01((float)Manager.Instance.currentTrun / 100);
        float delayBetween = Mathf.Lerp(30, 15, t);
        yield return new WaitForSeconds(delayBetween);

        // 2�� ����
        yield return StartCoroutine(PenaltyMonsterSpaw(14));

        yield return new WaitForSeconds(delayBetween);
        penaltyCheckTimer = 0f;
        penaltyMonsterSpawCoroutine = null;
    }

    IEnumerator PenaltyMonsterSpaw(int spawId)
    {

        yield return new WaitUntil(() => Manager.Instance.turnState == Manager.TurnState.Battle);

        MonsterData monsterData = monsterStat.GetById(spawId);
        id = monsterData.id;

        GameObject playerFind = GameObject.FindGameObjectWithTag("Player");

        Vector3 spawnPos = new Vector3(playerFind.transform.localPosition.x, 11.5f, playerFind.transform.localPosition.z);

        Instantiate(
             monsterData.monsterGameObject,
             spawnPos,
             Quaternion.identity
         );

        yield return new WaitForSeconds(monsterData.spawnDelay);

    }

    void comparisonDistance()
    {
        if (Manager.Instance.physicalAttackTrun)
        {
            float minDistance = float.MaxValue;
            float maxDistance = float.MinValue;

            GameObject nearestPoint = null;
            GameObject farthestPoint = null;

            // �Ÿ� ��
            for (int i = 0; i < comparisonPoint.Count; i++)
            {
                Transform nearTarget = Player.Instance.gameObject.transform;

                float dist = Vector3.Distance(comparisonPoint[i].position, nearTarget.position);

                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearestPoint = comparisonPoint[i].gameObject;
                }

                if (dist > maxDistance)
                {
                    maxDistance = dist;
                    farthestPoint = comparisonPoint[i].gameObject;
                }
            }

            // Ÿ�̸� ���� üũ
            if (nearestPoint == lastNearestPoint)
            {
                samePointTimer += Time.deltaTime;

                if (samePointTimer >= maxSameTime)
                {
                    // 3�� ���� ���� ����Ʈ������ ���� �� ����Ʈ�� ����
                    closePlayerPoint = farthestPoint;
                    return;
                }
            }
            else
            {
                // ����Ʈ�� �ٲ�� Ÿ�̸� ����
                samePointTimer = 0f;
                lastNearestPoint = nearestPoint;
            }

            // �Ϲ������δ� ���� ����� ����Ʈ�� ����
            closePlayerPoint = nearestPoint;
        }
        else
        {
            int rand = Random.Range(0, comparisonPoint.Count);
            closePlayerPoint = comparisonPoint[rand].gameObject;
        }
    }

}
