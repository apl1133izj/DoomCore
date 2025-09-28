using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AutoDefenseTurret : MonoBehaviour
{
    public AutoDefenseTurretEnhance autoDefenseTurretEnhance;

    [Header("기본 스텟")]
    public float damage = 10f;
    public float attackSpeed = 1f;
    public float rotationSpeed = 180f;
    public float range = 5f;
    public float duration = 3f;
    public float bulletSpeed = 10f;
    public float currentSaveExp;
    public float maxSaveExp;

    [Header("최대 기본 스텟")]
    public float Maxdamage = 100f;
    public float MaxattackSpeed = 0.05f;
    public float MaxrotationSpeed = 180f;
    public float Maxrange = 70f;
    public float Maxduration = 600f;
    public float MaxmaxSaveExp;
    public float MaxbulletSpeed = 10f;

    [Header("강화 스텟")]
    public float enhanceDamage;
    public float enhanceAttackSpeed;
    public float enhanceRotationSpeed;
    public float enhanceRange;
    public float enhanceDuration;
    public float enhanceMaxSaveExp;

    [Header("UI")]
    public Text autoDefenseTurretState;

    [Header("효과음")]
    public AudioSource audioSource;
    public AudioClip audioClips;

    [Header("기타")]
    Transform target;
    public GameObject bulletPrefab;
    public Transform[] shotPos;
    public LayerMask targetLayer;
    public Core core;

    private int shotPosIndex = 0;
    private Coroutine attackCoroutine;
    private GameObject coreDisable;

    // 최종 적용 값 계산
    public float TotalDamage => Mathf.Min(Maxdamage, damage + enhanceDamage);
    public float TotalAttackSpeed => Mathf.Max(MaxattackSpeed, attackSpeed - enhanceAttackSpeed);
    public float TotalRotationSpeed => Mathf.Min(MaxrotationSpeed, rotationSpeed + enhanceRotationSpeed);
    public float TotalRange => Mathf.Min(Maxrange, range + enhanceRange);
    public float TotalDuration => Mathf.Min(Maxduration, duration + enhanceDuration);
    public float TotalMaxSaveExp => Mathf.Min(MaxmaxSaveExp, maxSaveExp + enhanceMaxSaveExp);

    public float currentDuration = 0f;
    private string lastState = "";

    private void OnEnable()
    {
        attackCoroutine = StartCoroutine(AttackRoutine());

        if (core != null && core.coreItemGameObject != null)
        {
            coreDisable = core.coreItemGameObject;
            coreDisable.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        if (coreDisable != null)
            coreDisable.SetActive(true);
    }

    private void Update()
    {
        if (target != null)
        {
            LookTarget(target);
        }
    }
    //공격
    IEnumerator AttackRoutine()
    {
        autoDefenseTurretState.text = "작동중";
        lastState = "작동중";

        while (true)
        {
            if (Manager.Instance.turnState == Manager.TurnState.Battle)
            {
                currentDuration += Time.deltaTime;
                if (currentDuration >= TotalDuration)
                {
                    SetTurretState("수리 필요");
                    yield return new WaitUntil(() => currentDuration < TotalDuration);
                    SetTurretState("작동중");
                }
            }
            // 타겟 체크
            target = FindClosestTargetNonAlloc(TotalRange + EventManager.Instance.bonusTurretRange);


            if (target != null && FrontCheck())
            {
                // 발사 위치 순환
                if (shotPosIndex >= shotPos.Length) shotPosIndex = 0;

                var bulletInstance = PoolManager.Instance.GetFromPool<PoolChild>(
                   bulletPrefab,
                   shotPos[shotPosIndex].position,
                   Quaternion.LookRotation((target.position - shotPos[shotPosIndex].position).normalized)
               );
                AutoDefenseTurretBullet autoDefenseTurretBullet = bulletInstance.GetComponent<AutoDefenseTurretBullet>();

                autoDefenseTurretBullet.attackDamage = TotalDamage + EventManager.Instance.bonusTurretPower;
                autoDefenseTurretBullet.defenseTurret = this;
                autoDefenseTurretBullet.autoDefenseTurretEnhance = autoDefenseTurretEnhance;

                // 유도탄 타겟 세팅
                autoDefenseTurretBullet.SetTarget(target, 15f);

                shotPosIndex++;
                audioSource.clip = audioClips;
                audioSource.Play();

                yield return new WaitForSeconds(TotalAttackSpeed);
            }
            else
            {
                yield return new WaitForSeconds(0.1f); // Target 없으면 지연
            }
        }
    }


    //적 찾기
    private Collider[] overlapResults = new Collider[20];
    Transform FindClosestTargetNonAlloc(float distance)
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, distance, overlapResults, targetLayer);
        if (count == 0) return null;

        Transform closest = overlapResults[0].transform;
        float minDis = Vector3.Distance(transform.position, closest.position);

        for (int i = 1; i < count; i++)
        {
            Transform t = overlapResults[i].transform;

            // Tag 확인 (Enemy만)
            if (!t.CompareTag("Enemy"))
                continue;

            float d = Vector3.Distance(transform.position, t.position);
            if (d < minDis)
            {
                minDis = d;
                closest = t;
            }
        }
        return closest;
    }

    //정면 확인
    bool FrontCheck()
    {
        if (target == null) return false;

        Vector3 toTarget = (target.position - transform.position).normalized;
        float dot = Vector3.Dot(toTarget, transform.forward);
        float thresholdDot = Mathf.Cos(10 * Mathf.Deg2Rad);
        return dot >= thresholdDot;
    }

    //타겟된 적까지 회전
    void LookTarget(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0;
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float step = (TotalRotationSpeed + EventManager.Instance.bonusTurretRotationSpeed) * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
    }

    //현재 상태 안내
    void SetTurretState(string state)
    {
        if (lastState != state)
        {
            autoDefenseTurretState.text = state;
            lastState = state;
        }
    }
}
