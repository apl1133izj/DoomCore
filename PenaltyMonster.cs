using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PenaltyMonster : MonoBehaviour
{
    public bool test;
    public MonsterData monsterData;
    public MonsterStat monsterStat;
    private Animator animator;
    public int monsterId;
    public AudioSource enemyAudioSource;
    public AudioClip[] hitAudioClip;
    public AudioClip dieAudioClip;
    public AudioClip[] attack1Sound;//눈알꽃
    public AudioClip[] attack2Sound;//샌드윔

    public GameObject attackGauge;
    public GameObject attackBullet;
    Coroutine attackCoroutine;
    [Header("레이저 공격 = 샌드윔")]
    public MeshRenderer chargingMeshRenderer;
    public MeshRenderer trajectoryMeshRenderer;
    public Material chargingMaterial;
    public Material trajectoryMaterial;
    public GameObject laserBullet;
    public GameObject barrageBullet;
    public Transform laserBulletPos;
    public bool chargingCompletion;
    public bool laserType;
    public Material laserGaugeMaterial;
    GameObject findPlayer;
    Coroutine dieCoroutine;

    public bool rotationFireCoroutine;
    private void OnEnable()
    {
        monsterData = monsterStat.GetById(monsterId);
        StartInitialization();
        StartCoroutine(AttackLoop());
    }
    private void Update()
    {
        if (EventManager.Instance.eventActive.activeSelf) return;
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (Manager.Instance.turnState == Manager.TurnState.RepairTime)
            {
                if (dieCoroutine == null)
                {
                    dieCoroutine = StartCoroutine(DieAnimation());
                }

            }
            else
            {
                if (laserType)
                {
                    if (!rotationFireCoroutine)
                    {
                        animator.SetBool("BarrageAttack", false);
                        LookPlayer();
                    }
                    else
                    {
                        animator.SetBool("BarrageAttack", true);
                        transform.Rotate(0f, 90f * Time.deltaTime, 0f);
                    }
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(1f);
        if (test)
        {
            while (true)
            {
                if (laserType)
                {
                    int radAttack = Random.Range(0, 2);

                    switch (radAttack)
                    {
                        case 0:
                            yield return StartCoroutine(Attack());
                            yield return new WaitForSeconds(monsterData.attackDelay);
                            break;
                        case 1:
                            float t = Mathf.Clamp01((float)Manager.Instance.currentTrun / 100f);
                            float interval = Mathf.Lerp(0.5f, 0.1f, t);
                            yield return StartCoroutine(RotationFire(interval, 80f + Manager.Instance.currentTrun, 3));
                            break;

                    }
                }
                else
                {
                    yield return StartCoroutine(Attack());
                    yield return new WaitForSeconds(monsterData.attackDelay);

                }
            }
        }
        else
        {
            while (Manager.Instance.turnState == Manager.TurnState.Battle)
            {
                if (laserType)
                {
                    int radAttack = Random.Range(0, 2);

                    switch (radAttack)
                    {
                        case 0:
                            yield return StartCoroutine(Attack());
                            break;
                        case 1:
                            float t = Mathf.Clamp01((float)Manager.Instance.currentTrun / 100f);
                            float interval = Mathf.Lerp(0.5f, 0.1f, t);

                            yield return StartCoroutine(RotationFire(interval, 80f + Manager.Instance.currentTrun, 3));
                            break;

                    }
                }
                else
                {
                    yield return StartCoroutine(Attack());
                    yield return new WaitForSeconds(monsterData.attackDelay);
                }

            }
            attackCoroutine = null;
        }

    }

    public void AttackEvent()
    {
        if (attackCoroutine == null)
            attackCoroutine = StartCoroutine(Attack());
    }
    IEnumerator Attack()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!laserType)
            {
                GameObject Player = GameObject.FindGameObjectWithTag("Player");

                float randSize = Random.Range(0.7f, 2f);
                animator.SetTrigger("Attack");
                GameObject _gauge = Instantiate(attackGauge, new Vector3(Player.transform.position.x, 11.51f, Player.transform.position.z), Quaternion.identity);
                _gauge.transform.localScale = new Vector3(randSize, randSize, randSize);
                Gauge gauge = _gauge.GetComponent<Gauge>();

                enemyAudioSource.PlayOneShot(attack1Sound[0]);
                yield return new WaitUntil(() => gauge.Completion);

                GameObject attbulet = PoolManager.Instance.GetFromPool<PoolChild>(attackBullet, gauge.transform.position + new Vector3(0, 10, 0), Quaternion.identity).gameObject;
                // 프리팹 원래 크기 가져오기
                Vector3 baseScale = attbulet.transform.localScale;

                // 랜덤 배율 적용
                attbulet.transform.localScale = baseScale * randSize;
                attbulet.transform.rotation = Quaternion.Euler(90, 0, 0);
                float t = Mathf.Clamp01((float)Manager.Instance.currentTrun / 100);
                float attackDelay = Mathf.Lerp(monsterData.attackDelay, 4f, t);
                yield return new WaitForSeconds(attackDelay);
                attackCoroutine = null;
            }
            else
            {
                animator.SetTrigger("Attack");
                yield return new WaitForSeconds(1f);

                yield return new WaitUntil(() => chargingCompletion);
                enemyAudioSource.PlayOneShot(attack2Sound[1]);
                GameObject attbulet = Instantiate(laserBullet, laserBulletPos.position, Quaternion.identity);
                FowardAttackRangeBulletBullet fowardAttackRangeBulletBullet = attbulet.GetComponent<FowardAttackRangeBulletBullet>();
                fowardAttackRangeBulletBullet.rb.AddForce(transform.forward * fowardAttackRangeBulletBullet.power, ForceMode.Impulse);
                if (test)
                {
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    attbulet.transform.rotation = Quaternion.LookRotation(player.transform.position);
                }
                else
                {
                    attbulet.transform.rotation = Quaternion.LookRotation(Player.Instance.transform.position);
                }
                float t = Mathf.Clamp01((float)Manager.Instance.currentTrun / 100);
                float attackDelay = Mathf.Lerp(monsterData.attackDelay, 4f, t);
                yield return new WaitForSeconds(attackDelay);
                chargingCompletion = false;
                animator.SetFloat("Speed", 1f);
                attackCoroutine = null;
            }
        }
        else
        {

            attackCoroutine = null;
            yield break;
        }
    }
    public IEnumerator DieAnimation()
    {
        Debug.Log("DieAnimation실행");
        attackCoroutine = null;
        animator.SetTrigger("Die");
        DieSound();

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        Debug.Log("DieAnimation통과");

    }
    public void DieReleaseEvent()
    {
        Destroy(gameObject);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="interval : 생성 속도"></param>
    /// <param name="angel : 각도"></param>
    /// <param name="cnt : 생성횟수"></param>
    /// <returns></returns>
    IEnumerator RotationFire(float interval, float angle, int cnt)
    {
        rotationFireCoroutine = true;
        float duration = 10f;
        float time = 0;

        while (time < duration)
        {
            float gap = cnt > 1 ? angle / (cnt - 1) : 0;
            float startAngle = -angle / 2.0f;

            // 무조건 하나는 비워두는 구조
            int skipIndex = Random.Range(0, cnt);

            for (int i = 0; i < cnt; i++)
            {
                if (i == skipIndex) continue;

                float theta = startAngle + gap * i;
                Quaternion rot = Quaternion.AngleAxis(theta, Vector3.up);
                Vector3 dir = rot * transform.forward;

                GameObject go = PoolManager.Instance.GetFromPool<PoolChild>(barrageBullet, laserBulletPos.position - new Vector3(0, 1f, 0), Quaternion.identity).gameObject;
                go.GetComponent<BarrageBullet>().SetNormalizedDir(dir.normalized, 5);
            }


            time += interval;
            yield return new WaitForSeconds(interval);
        }
        rotationFireCoroutine = false;
        float t = Mathf.Clamp01((float)Manager.Instance.currentTrun / 100);
        float attackDelay = Mathf.Lerp(monsterData.attackDelay, 4f, t);//턴이 증가 할수록 감소 10~4
        yield return new WaitForSeconds(attackDelay);
    }


    public void StartInitialization()
    {
        dieCoroutine = null;
        attackCoroutine = null;
        monsterData = monsterStat.GetById(monsterId); // 인스펙터에서 설정된 ID 기준 초기화
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        rotationFireCoroutine = false;
        if (laserType)
        {
            laserGaugeMaterial.SetFloat("_Border", 1);
            trajectoryMaterial = trajectoryMeshRenderer.material;
            trajectoryMaterial.SetFloat("_Size", 0);
            trajectoryMaterial.SetFloat("_Border", 1);
        }
        animator.Rebind();
        animator.Update(0f);
    }


    public void Charging()
    {
        StartCoroutine(ChargingGauge());
    }
    public IEnumerator ChargingGauge()
    {

        chargingCompletion = false;
        chargingCompletion = false;

        float chargingTime = 1f; // Size가 0 → 1 되는데 걸리는 시간
        float size = 0f;
        float border = 0f;
        float borderRatio = 0.1f; // Border = Size * borderRatio, 10% 정도 두께

        while (size < 1f)
        {
            // Size 증가
            size += Time.deltaTime / chargingTime;
            size = Mathf.Clamp01(size);

            // Border는 항상 Size 대비 비율 유지
            border = Mathf.Max(size * borderRatio, 0.01f); // 최소값 보정

            // Shader 적용
            trajectoryMaterial.SetFloat("_Size", size);
            trajectoryMaterial.SetFloat("_Border", border);

            yield return null;
        }

        // Size가 최대일 때, Border 애니메이션 (예: 강조 효과)
        float borderTime = 0.5f;
        float borderAnim = border;
        while (borderAnim < size * 0.3f) // 최대 30% 두께까지 확대
        {
            borderAnim += Time.deltaTime / borderTime * (size * 0.3f);
            borderAnim = Mathf.Min(borderAnim, size * 0.3f);

            trajectoryMaterial.SetFloat("_Size", size);
            trajectoryMaterial.SetFloat("_Border", borderAnim);

            yield return null;
        }

        // Border 다시 Size 대비 기본 비율로 줄이기
        while (borderAnim > size * borderRatio)
        {
            borderAnim -= Time.deltaTime / borderTime * (size * 0.3f);
            borderAnim = Mathf.Max(borderAnim, size * borderRatio);

            trajectoryMaterial.SetFloat("_Size", size);
            trajectoryMaterial.SetFloat("_Border", borderAnim);

            yield return null;
        }
        enemyAudioSource.PlayOneShot(attack2Sound[0]);
        // Size 점차 0으로 줄이면서 Border도 비율 유지
        while (size > 0f)
        {
            size -= Time.deltaTime / chargingTime;
            size = Mathf.Clamp01(size);

            border = Mathf.Max(size * borderRatio, 0.01f);

            trajectoryMaterial.SetFloat("_Size", size);
            trajectoryMaterial.SetFloat("_Border", border);

            yield return null;
        }

        chargingCompletion = true;
    }
    public void Stop()
    {
        animator.SetFloat("Speed", 0f);
    }
    public void LookPlayer()
    {
        if (findPlayer == null)
        {
            findPlayer = GameObject.FindGameObjectWithTag("Player");
        }
        Vector3 dir = findPlayer.transform.position - transform.position;
        dir.y = 0; // y축 고정해서 수평 회전만
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }
    public void DieSound()
    {
        enemyAudioSource.pitch = Random.Range(1.1f, 1.5f);
        enemyAudioSource.PlayOneShot(dieAudioClip, Sound.Instance.playerSound);
    }

}