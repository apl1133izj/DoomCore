using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AutoDefenseTurret : MonoBehaviour
{
    public AutoDefenseTurretEnhance autoDefenseTurretEnhance;

    [Header("�⺻ ����")]
    public float damage = 10f;
    public float attackSpeed = 1f;
    public float rotationSpeed = 180f;
    public float range = 5f;
    public float duration = 3f;
    public float bulletSpeed = 10f;
    public float currentSaveExp;
    public float maxSaveExp;

    [Header("�ִ� �⺻ ����")]
    public float Maxdamage = 100f;
    public float MaxattackSpeed = 0.05f;
    public float MaxrotationSpeed = 180f;
    public float Maxrange = 70f;
    public float Maxduration = 600f;
    public float MaxmaxSaveExp;
    public float MaxbulletSpeed = 10f;

    [Header("��ȭ ����")]
    public float enhanceDamage;
    public float enhanceAttackSpeed;
    public float enhanceRotationSpeed;
    public float enhanceRange;
    public float enhanceDuration;
    public float enhanceMaxSaveExp;

    [Header("UI")]
    public Text autoDefenseTurretState;

    [Header("ȿ����")]
    public AudioSource audioSource;
    public AudioClip audioClips;

    [Header("��Ÿ")]
    Transform target;
    public GameObject bulletPrefab;
    public Transform[] shotPos;
    public LayerMask targetLayer;
    public Core core;

    private int shotPosIndex = 0;
    private Coroutine attackCoroutine;
    private GameObject coreDisable;

    // ���� ���� �� ���
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
    //����
    IEnumerator AttackRoutine()
    {
        autoDefenseTurretState.text = "�۵���";
        lastState = "�۵���";

        while (true)
        {
            if (Manager.Instance.turnState == Manager.TurnState.Battle)
            {
                currentDuration += Time.deltaTime;
                if (currentDuration >= TotalDuration)
                {
                    SetTurretState("���� �ʿ�");
                    yield return new WaitUntil(() => currentDuration < TotalDuration);
                    SetTurretState("�۵���");
                }
            }
            // Ÿ�� üũ
            target = FindClosestTargetNonAlloc(TotalRange + EventManager.Instance.bonusTurretRange);


            if (target != null && FrontCheck())
            {
                // �߻� ��ġ ��ȯ
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

                // ����ź Ÿ�� ����
                autoDefenseTurretBullet.SetTarget(target, 15f);

                shotPosIndex++;
                audioSource.clip = audioClips;
                audioSource.Play();

                yield return new WaitForSeconds(TotalAttackSpeed);
            }
            else
            {
                yield return new WaitForSeconds(0.1f); // Target ������ ����
            }
        }
    }


    //�� ã��
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

            // Tag Ȯ�� (Enemy��)
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

    //���� Ȯ��
    bool FrontCheck()
    {
        if (target == null) return false;

        Vector3 toTarget = (target.position - transform.position).normalized;
        float dot = Vector3.Dot(toTarget, transform.forward);
        float thresholdDot = Mathf.Cos(10 * Mathf.Deg2Rad);
        return dot >= thresholdDot;
    }

    //Ÿ�ٵ� ������ ȸ��
    void LookTarget(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0;
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float step = (TotalRotationSpeed + EventManager.Instance.bonusTurretRotationSpeed) * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
    }

    //���� ���� �ȳ�
    void SetTurretState(string state)
    {
        if (lastState != state)
        {
            autoDefenseTurretState.text = state;
            lastState = state;
        }
    }
}
