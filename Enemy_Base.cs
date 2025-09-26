using MasterStylizedProjectile;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NavMeshAgent))]
[DefaultExecutionOrder(1)]
public class Enemy_Base : PoolChild
{
    Enemy_Base enemy_Base;

    public enum EnemyType { Basic, Magic, Bose, Range }
    public EnemyType enemyType;

    public string property;

    public int monsterId;
    public NavMeshAgent agent;
    public GameObject[] playerSerch;
    public GameObject[] coreSerch;//���� ����� �ھ ã��
    public GameObject[] barrierSerch;
    public GameObject nearCore;
    public GameObject nearBarrier;
    public GameObject baseFind;
    public GameObject attackPos;
    public GameObject rangeBullet;
    public Material attackPathMaterial;
    Vector3 originalScale;
    public float offsetY;
    public float offsetX;
    public float offsetZ;

    public float saveSpeed;

    [Header("����")]
    public float playerDiscoveryRadius;//���Ͱ� �÷��̾  ���� �� �� �ִ� �Ÿ� �� ��� �Դٸ�
    public float playerAttackArearRadius;//���Ͱ� �÷��̾ ���� �� �� �ִ� �Ÿ� �� ��� �Դٸ�
    public LayerMask target;
    public LayerMask hitLayerMask;
    public Vector3 circleTargetPosition;
    [Header("ü��")]
    public float maxHp;
    public float currentHp;
    public EnemyHPBar enemyHPBar;
    [Header("����")]
    public Transform attackTransform;
    public AudioSource enemyAudioSource;
    public AudioSource enemyHitAudioSource;
    public AudioClip attackAudioClip;
    public AudioClip[] hitAudioClip;
    public bool isAttacking;
    public int attackKind;
    public string[] attackName;
    public float attackCurrentCoolTime;
    public float attackCoolTime;
    public float maxDistence;
    [Header("�߼Ҹ� ����Ʈ")]
    public AudioClip[] footstepClips;
    [Header("����")]
    public BoxCollider hitBoxCollider;//�׾����� ��Ȱ��ȭ 
    public bool hitAnimationPresence;//�´� �ִϸ��̼� ���� ����(���°�� �˹�)
    public ParticleSystem bloodParticle;
    public Rigidbody rig;
    public float slow;//�÷��̾�� ���ο� ��ų�� ����
    public GameObject sloawPaticle;
    [Header("���� �ð�")]
    public bool isInvincible;

    public float stopDistence;
    [Header("���� Ÿ��")]
    public GameObject currentTarget;
    float agentangleSpeedStartSave;
    [Header("���Ÿ� ����")]
    public Coroutine attackCoroutine;
    public bool IsUltimateActive;//�ʻ�� ����
    public GameObject ultimateActivePrefab;
    public Transform ultimateActivePrefabPos;
    Animator animator;
    [Header("���� - ����")]
    public bool boom;
    public AudioClip boomSound;
    public Explosion explosion;
    [Header("���� - ��� ������")]
    public bool Die;
    GameObject dropItem;
    Coroutine dieCoroutin;
    public int dropItemCount;
    public ExpFollow expFollow;
    public AudioClip dieAudioClip;
    [SerializeField]
    private float trackStopTime;
    public bool tracking;
    public MonsterData monsterData;
    public MonsterStat monsterStat;
    Renderer renderer;//����
    Renderer renderer2;//�ٸ�
    MaterialPropertyBlock block;
    MaterialPropertyBlock block2;

    DissolvingControllerShader controllerShader;


    [Header("����ȭ ������ ����")]
    private float nextActionTime = 0.0f;
    public float period = 0.05f; // ���ϴ� �ð� ���� (0.05��)

    
    private void Awake()
    {
        originalScale = transform.localScale;
    }
    IEnumerator Start()
    {
        monsterData = monsterStat.GetById(monsterId);
        StartInitialization();
        yield return null; // �� ������ ��ٸ���
    }
    private void OnDisable()
    {
        dieCoroutin = null;
        if (dieCoroutin == null)
        {
            StopCoroutine(DieAnimation("Die"));
        }
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1 && Manager.Instance.turnState == Manager.TurnState.Battle)
        {
            Animation();

            if (Time.time > nextActionTime)
            {
                LookAtRotation(300);
                nextActionTime += period;
                DieCheck();

                if (nearBarrier == null)
                {
                    if (nearCore == null || !nearCore.activeSelf)
                    {
                        nearCore = null;
                        currentTarget = CanChasePlayer()
                            ? Player.Instance.gameObject
                            : baseFind;
                    }
                    else
                    {
                        currentTarget = CanChasePlayer()
                            ? Player.Instance.gameObject
                            : nearCore;
                    }
                }
                else if (nearBarrier != null)
                {
                    currentTarget = CanChasePlayer() ? Player.Instance.gameObject : nearBarrier;

                }
                if (Die && gameObject.activeSelf)
                {
                    if (dieCoroutin == null)
                    {
                        dieCoroutin = StartCoroutine(DieAnimation("Die"));
                    }
                }
                if (nearCore == null)
                {
                    CoreSerch();

                }
                else if (nearBarrier == null)
                {
                    DefenseBase();
                }

                if (agent != null && agent.isOnNavMesh && attackCoroutine == null)
                {

                    if (currentTarget == null) return;
                    AIManager.Instance.MakeAgentsCircleTarget();
                }
            }
        }
        else
        {
            Release();
        }
    }
    public void MoveTo(Vector3 Position)
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.SetDestination(Position);
        }
    }

    bool CanChasePlayer()
    {
        if (Player.Instance.hideDefenceSkill) return false;
        // �÷��̾� Ž�� ���� ���� �ش� ���̾�(target) �ݶ��̴��� �ִ��� �˻�
        Collider[] detectedTargets = Physics.OverlapSphere(transform.position + new Vector3(0, offsetY, 0), playerDiscoveryRadius, target);
        // Ž���� ����� �ϳ��� ������ true ��ȯ
        return detectedTargets.Length > 0;
    }
    bool AttackArear()
    {
        Collider[] hitsPlayer = Physics.OverlapSphere(attackPos.transform.position + new Vector3(offsetX, offsetY, offsetZ), playerAttackArearRadius, hitLayerMask);

        bool hasPlayer = false;
        foreach (Collider hit in hitsPlayer)
        {
            //Debug.Log("���ݹ����� ����");
            hasPlayer = true;

            if (hit.transform.CompareTag("Player") || hit.transform.CompareTag("Shild"))
            {
                float Distence = Vector3.Distance(transform.position, Player.Instance.transform.position);
                //Debug.Log($"�̸� :{gameObject.name}" + " ����");
                if (Distence < maxDistence)
                {
                    if (isAttacking && !Player.Instance.shild)
                    {
                        // Debug.Log("���� ����");

                        Player.Instance.Hit(monsterData.attackDamage, enemy_Base);
                        isAttacking = false;
                        break;
                    }
                    else if (isAttacking && Player.Instance.shild)
                    {
                        Player.Instance.durability -= monsterData.attackDamage / 100f;
                        isAttacking = false;
                    }
                }

            }
        }

        return hasPlayer;
    }

    void Animation()
    {
        if (Die) return;
        if (currentTarget == null) return;
        bool Run = Vector3.Distance(transform.position, circleTargetPosition) > stopDistence * ApplyEvent.instance.giantShadowCurseValue;
        animator.SetFloat("Slow", slow * ApplyEvent.instance.berserkCurseValue);//ApplyEvent.instance.berserkCurseValue : ���� �ӵ� ����

        agent.speed = monsterData.moveSpeed * slow;
        if (attackCoroutine == null)
        {
            animator.SetBool("Run", Run);
        }


        if (AttackArear())
        {
            AttackType();
        }

    }
    void AttackType()
    {
        //0:�⺻ ����
        //1:�߰� ����
        //2:���� ����
        int rand = Random.Range(0, attackKind + 1);
        if (attackCoroutine == null)
        {
            //Debug.Log("�ִϸ��̼� Ÿ�� �Լ��� ����");
            if (!gameObject.activeSelf) return;
            if (Vector3.Distance(transform.position, circleTargetPosition) <= (stopDistence + 0.3f) * ApplyEvent.instance.giantShadowCurseValue)
            {
                if (monsterData.enemyType == MonsterData.EnemyType.MultiAttack)
                {
                    switch (rand)
                    {
                        case 0:
                            {
                                attackCoroutine = StartCoroutine(Attack(attackName[0]));
                                break;
                            }
                        case 1:
                            {
                                attackCoroutine = StartCoroutine(Attack(attackName[1]));
                                break;
                            }
                    }
                }
                else if (enemyType == EnemyType.Range)
                {
                    attackCoroutine = StartCoroutine(RangeAttack());
                }
                else
                {
                    //Debug.Log("�ִϸ��̼� ���õ�");
                    attackCoroutine = StartCoroutine(Attack(attackName[0]));
                }
            }
            else
            {
                if (!IsUltimateActive)
                {
                    return;
                }
                attackCoroutine = StartCoroutine(UltimateActive("Attack"));
            }
        }
    }
    IEnumerator Attack(string attackName)
    {
        try
        {
            //Debug.Log("���� �̸� : " + attackName);
            if (agent.isOnNavMesh)
            {
                animator.SetBool("Run", false);
                agent.isStopped = monsterData.enemyType == MonsterData.EnemyType.Guard ? false : true;
                boom = false;
                while (!Frontcheck())
                {
                    agent.angularSpeed = 900;
                    yield return null;
                }
                agent.angularSpeed = agentangleSpeedStartSave;
                animator.SetTrigger("Attack");
                // Debug.Log("���� ����");
                // �ִϸ��̼� ���� ���
                float timer = 0f;
                while (!IsAnimationClipFinishedLayer0(attackName))
                {
                    timer += Time.deltaTime;
                    if (timer >= 3f)
                    {
                        break;
                    }
                    yield return null;
                }
                if (monsterData.enemyType == MonsterData.EnemyType.Guard)
                {
                    currentHp = 0;
                }
                //Debug.Log("���� ����");
                if (agent.enabled)
                {
                    agent.isStopped = false;
                }
                yield return new WaitForSeconds(0.5f);
                if (nearCore == null)
                {
                    CoreSerch();

                }
                attackCoroutine = null;
            }
            else
            {
                if (agent.enabled)
                {
                    agent.isStopped = false;
                }
                attackCoroutine = null;

                yield break;
            }
        }
        finally
        {
            if (agent.enabled)
            {
                animator.SetBool("Run", false);
                attackCoroutine = null;
                agent.isStopped = false;
            }
        }
        yield return new WaitForSeconds(monsterData.attackDelay);
    }
    IEnumerator RangeAttack()
    {
        try
        {
            if (agent.isOnNavMesh)
            {
                animator.SetBool("Run", false);
                agent.isStopped = false;
                while (!Frontcheck())
                {
                    agent.angularSpeed = 1200;
                    yield return null;
                }
                agent.angularSpeed = agentangleSpeedStartSave;
                animator.SetTrigger("Attack");
                yield return new WaitForSeconds(0.5f);

                yield return new WaitForSeconds(2f);
                // attackPathMaterial.SetFloat("_Fill", 1);
                animator.SetTrigger("Shoot");
                //Debug.Log("���� ����");
                if (agent.enabled)
                {
                    agent.isStopped = false;
                }
                yield return new WaitForSeconds(0.5f);

                if (nearCore == null)
                {
                    CoreSerch();

                }
                attackCoroutine = null;
            }
            else
            {
                if (agent.enabled)
                {
                    agent.isStopped = false;
                }
                attackCoroutine = null;

                yield break;
            }
        }
        finally
        {
            if (agent.enabled)
            {
                animator.SetBool("Run", false);
                attackCoroutine = null;
                agent.isStopped = false;
            }
        }
    }
    // ���� �׾��� �� ȣ��� �̺�Ʈ
    public static event System.Action<Enemy_Base> OnEnemyDied;
    public IEnumerator DieAnimation(string dieName)
    {
        Die = true;
        Manager.Instance.killedMonsterCount++;
        Manager.Instance.monsterSpaw.spawCount--;
        hitBoxCollider.enabled = false;
        if (monsterData.enemyType == MonsterData.EnemyType.Guard)
        {
            explosion.enabled = true;
            int randCount1 = Random.Range(1, Player.Instance.dropItemCount + 1 + EventManager.Instance.bonusCoreDropCount + ApplyEvent.instance.ironMonsterRewardValue);
            for (int i = 0; i < randCount1; i++)
            {
                GameObject item = PoolManager.Instance.GetFromPool<PoolChild>(dropItem, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 1, Random.Range(-0.5f, 0.5f)), Quaternion.identity).gameObject;
                yield return null;
            }
            Release();
            yield break;

        }
        else
        {
            controllerShader.enabled = true;
        }

        animator.SetTrigger(dieName);
        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
        if (monsterData.enemyType != MonsterData.EnemyType.Guard)
        {

            StartCoroutine(controllerShader.DissolveCo(expFollow));

        }

        DieSound();
        float timer = 0f;
        while (!IsAnimationClipFinishedLayer0(dieName))
        {
            agent.enabled = false;
            timer += Time.deltaTime;
            if (timer >= 3f)
            {
                break;
            }
            yield return null;
        }

        int randCount2 = Random.Range(1, Player.Instance.dropItemCount + 1 + EventManager.Instance.bonusCoreDropCount + ApplyEvent.instance.ironMonsterRewardValue);
        for (int i = 0; i < randCount2; i++)
        {
            GameObject drop = PoolManager.Instance.GetFromPool<PoolChild>(dropItem, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 1, Random.Range(-0.5f, 0.5f)), Quaternion.identity).gameObject;
            yield return new WaitForSeconds(0.5f);
        }
        if (monsterData.enemyType != MonsterData.EnemyType.Guard)
        {
            yield return new WaitUntil(() => controllerShader.dissolveAmount >= 1);
        } 
        Release();
    }

    // ������ ���� �Լ� : �ߺ� ���� + Ǯ ���
    IEnumerator SpawnDropItems(GameObject prefab, Vector3 position, int count)
    {
        for (int i = 0; i < count; i++)
        {
            PoolManager.Instance.GetFromPool<PoolChild>(prefab, position, Quaternion.identity);
            yield return null; // �� ������ ����
        }
    }
    public void Hit(float damage)
    {
        if (Die)
        {
            enemyHPBar.UpdateHPBar(0, maxHp);

            return;
        }
        if (currentHp > 0)
        {
            if (isInvincible) return;
            enemyHPBar.UpdateHPBar(currentHp, maxHp);
            enemyHPBar.Initialize(this);
            currentHp -= damage;
            isInvincible = true;
            StartCoroutine(HitCooldown());
        }
    }
    public void TurrentBulletHit(float damage)
    {
        if (Die)
        {
            enemyHPBar.UpdateHPBar(0, maxHp);
            return;
        }
        if (currentHp > 0)
        {
            isInvincible = false;
            enemyHPBar.UpdateHPBar(currentHp, maxHp);
            enemyHPBar.Initialize(this);
            currentHp -= damage;
        }
    }
    IEnumerator HitCooldown()
    {
        enemyHPBar.Initialize(this);
        yield return new WaitForSeconds(0.5f);

        isInvincible = false;
    }
    void LookAtRotation(float speed)
    {
        if (Die) return;
        if (currentTarget == null) return;
        Quaternion targetRot = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, speed * Time.deltaTime);
    }
    void DieCheck()
    {
        Die = currentHp <= 0;
        if (Player.Instance.hp < 0 || SceneManager.GetActiveScene().buildIndex == 0)
        {
            Debug.Log("DieCheck");
            Die = true;
        }
    }
    bool Frontcheck()
    {
        Vector3 toPlayer = (currentTarget.transform.position - transform.position).normalized;
        Vector3 foward = transform.forward;

        float dot = Vector3.Dot(toPlayer, foward);
        float thresholdDot = Mathf.Cos(10 * Mathf.Deg2Rad);
        if (dot >= thresholdDot)
        {
            //Debug.Log("�÷��̾ ���� ��5�� �̳��� ����");
            return true;
        }
        else
        {
            // Debug.Log("�÷��̾ ���鿡 ����");
            return false;

        }
    }
    public void AttackInitialization()
    {
        if (agent.enabled)
        {
            agent.isStopped = false;
        }

        isAttacking = false;
        attackCoroutine = null;
    }
    IEnumerator UltimateActive(string attackName)
    {
        Debug.Log("���� �̸� : " + attackName);
        int count = 0;
        isAttacking = true;
        agent.isStopped = true;
        while (count < 5)
        {
            LookAtRotation(30);
            animator.SetTrigger(attackName);
            yield return new WaitUntil(() => IsAnimationClipFinishedLayer0(attackName));
            count++;
        }
        isAttacking = false;
        attackCoroutine = null;
    }
    public void UltimateActiveFire()
    {
        Vector3 dir = currentTarget.transform.position - transform.position;
        dropItemCount = 0;
        dir.y = 0; // ���� ���⸸ ���
        dir = dir.normalized;
        GameObject fire = Instantiate(ultimateActivePrefab, ultimateActivePrefabPos.position, Quaternion.identity);
        var bullet = fire.gameObject.AddComponent<Bullet>();
        bullet.enemyBullet = true;
        bullet.Speed = 20;
        bullet.target = currentTarget.transform;
        bullet.isArrow = true;
        bullet.bulletDir = dir;
    }
    public void AttackCoolTime()
    {
        if (attackCoroutine == null) return;
        attackCurrentCoolTime += Time.deltaTime;
    }

    private bool IsAnimationClipFinishedLayer0(string clipName)
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0); // �⺻ ���̾� 0
        if (stateInfo.IsName(clipName))
        {
            // �ִϸ��̼� Ŭ���� ���� ���°� �������� Ȯ�� (normalizedTime >= 1)
            return stateInfo.normalizedTime >= 1f;
        }
        return false;
    }


    public void AttackAudioEvent()
    {
        enemyAudioSource.clip = attackAudioClip;
        enemyAudioSource.volume = Sound.Instance.backSound;
        enemyAudioSource.Play();
    }
    public void AttackEvent(int tf)
    {
        isAttacking = tf == 1 ? true : false;
        if (tf == 1)
        {
            enemyAudioSource.clip = attackAudioClip;
            enemyAudioSource.Play();
        }
    }
    public void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;

        var clip = footstepClips[Random.Range(0, footstepClips.Length)];
        enemyAudioSource.pitch = Random.Range(0.95f, 1.05f);
        enemyAudioSource.PlayOneShot(clip, Sound.Instance.playerSound);
    }
    public void DieSound()
    {
        if (footstepClips.Length == 0) return;

        enemyAudioSource.pitch = Random.Range(1.1f, 1.5f);
        enemyAudioSource.PlayOneShot(dieAudioClip, Sound.Instance.playerSound);
    }
    // Ǯ���� ���� ������ �ʱ�ȭ ����
    public override void OnGetFromPool()
    {

        monsterData = monsterStat.GetById(monsterId); // �ν����Ϳ��� ������ ID ���� �ʱ�ȭ
        if (monsterData.enemyType == MonsterData.EnemyType.Normal)
        {
            if (monsterData.maxHP > 50 && monsterData.id == 0)
            {
                maxHp = 30 * ApplyEvent.instance.ironMonsterCurseValue;
            }
            else
            {
                float t = Mathf.Clamp01(Manager.Instance.currentTrun / 100);

                maxHp = Mathf.Lerp(monsterData.maxHP, monsterData.maxHP * 3f, t) * ApplyEvent.instance.ironMonsterCurseValue;
            }
        }
        else
        {
            maxHp = monsterData.maxHP * ApplyEvent.instance.ironMonsterCurseValue;
        }

        float EnhanceMAxHp = Manager.Instance.currentTrun >= 5 ? Mathf.FloorToInt(Manager.Instance.currentTrun * 0.5f) : 0;
        currentHp = maxHp + EnhanceMAxHp;
        StartInitialization();
    }
    public void StartInitialization()
    {
        dieCoroutin = null;
        attackCoroutine = null;
        isInvincible = false;
        baseFind = GameObject.FindGameObjectWithTag("Base");
        enemy_Base = this;
        if (!AIManager.Instance.enemy.Contains(this))
            AIManager.Instance.enemy.Add(this);

        if (monsterData.enemyType == MonsterData.EnemyType.Guard)
        {
            StartCoroutine(DieTime());
            explosion.enabled = false;
            Transform child3 = transform.GetChild(3);
            Transform child4 = transform.GetChild(4);
            renderer2 = child3.GetComponentInChildren<Renderer>();
            renderer = child4.GetComponentInChildren<Renderer>();
            block = new MaterialPropertyBlock();
            block2 = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            renderer2.GetPropertyBlock(block2);
            block.SetFloat("_GlowIntensity", 0);
            renderer.SetPropertyBlock(block);
            renderer2.SetPropertyBlock(block2);
        }
        controllerShader = GetComponent<DissolvingControllerShader>();
        controllerShader.enabled = false;
        controllerShader.dissolveAmount = 0f;

        for (int i = 0; i < controllerShader.monsterMaterials.Length; i++)
        {
            controllerShader.monsterMaterials[i].SetFloat("_DissolveAmount", 0);
        }

        Die = false;
        hitBoxCollider.enabled = true;
        expFollow.enabled = false;
        dropItem = monsterStat.GetRandomDrop(monsterData);
        slow = 1;
        agent.enabled = false;
       


        transform.localScale = originalScale * ApplyEvent.instance.giantShadowCurseValue;

        saveSpeed = monsterData.moveSpeed; 
        
        agent.speed = monsterData.moveSpeed * (ApplyEvent.instance.smallShadowCurseValue * ApplyEvent.instance.giantShadowRewardMonsterValue);
      
        enemyHPBar.hpText.text = maxHp.ToString();
        animator = GetComponent<Animator>();
        animator.Rebind();
        animator.Update(0f);

        StartCoroutine(StartCoroutineInitialization());
    }
    void UpdateTarget()
    {
        if (CanChasePlayer())
            currentTarget = Player.Instance.gameObject;
        else if (nearBarrier != null)
            currentTarget = nearBarrier;
        else if (nearCore != null)
            currentTarget = nearCore;
        else
            currentTarget = baseFind;
    }
    IEnumerator DieTime()
    {
        yield return new WaitForSeconds(15f);
        Manager.Instance.killedMonsterCount++;
        StartCoroutine(DieAnimation("Die"));
    }
    public IEnumerator StartCoroutineInitialization()
    {
        yield return new WaitForSeconds(0.25f); // �ּ� �����̷� �� ������ ��ٸ�

        CoreSerch();
        DefenseBase();

        // ������ �˻��� ��� ������� Ÿ�� ����
        UpdateTarget();

        agent.enabled = true;
        agent.isStopped = false;
    }

    public void ChanceDropSkill()
    {
        GameObject item = PoolManager.Instance.GetFromPool<PoolChild>(dropItem, transform.position + Vector3.up, Quaternion.identity).gameObject;
    }

    void CoreSerch()
    {
        coreSerch = GameObject.FindGameObjectsWithTag("Core");
        float minDistance = float.MaxValue;
        nearCore = null;

        foreach (var core in coreSerch)
        {
            if (!core.activeInHierarchy) continue; //��Ȱ��ȭ�� �ھ�� ����

            float distance = Vector3.Distance(transform.position, core.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearCore = core;
            }
        }
    }

    void DefenseBase()
    {
        barrierSerch = GameObject.FindGameObjectsWithTag("Barrier");
        float minDistance = float.MaxValue;
        nearBarrier = null;

        foreach (var barrier in barrierSerch)
        {
            float distance = Vector3.Distance(transform.position, barrier.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearBarrier = barrier;
            }
        }
    }

    /*    void OnDrawGizmosSelected()
        {
            Vector3 center = transform.position + new Vector3(0, offsetY, 0);
            Vector3 Atcenter = attackPos.transform.position + new Vector3(offsetX, offsetY, offsetZ);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(center, playerDiscoveryRadius);

    #if UNITY_EDITOR
            UnityEditor.Handles.Label(Atcenter + Vector3.up * 0.5f, "�÷��̾� ���� ����");
    #endif

            // ���� ���� ��� �ð�ȭ
            // Collider[] hits = Physics.OverlapSphere(center, playerAttackArearRadius, target); 
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(Atcenter, playerAttackArearRadius);

            *//*       foreach (var col in hits)
                   {
                       Gizmos.DrawSphere(col.transform.position, 0.2f);
                   }*//*
        }
    */

}
