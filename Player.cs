using MasterStylizedProjectile;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [Header("���� Ÿ��")]
    public bool physicsType = true; //true : ���� false : ������
    public SwordManager swordManager;
    public GameObject SwordGameObject;
    public Transform MagicGameObjectPos;
    public GameObject[] MagicGameObject;
    [Header("ü��")]
    public float hp;
    [Header("��ȭ ������ �������ͽ�")]
    public float maxhp;//��ȭ ������ �������ͽ�
    public Slider hpSlider;
    public bool damaged;//���� ����
    public AudioClip[] damagedAudioClip;
    public ShockwavePostEffect shockwavePostEffect;
    Coroutine screenHitEffect;
    [Header("��ų")]
    public Skill skills;
    public float hitSize;
    public ParticleSystem CriticalParticleSystem;
    public AudioClip CriticalParticleSystemAudioClip;
    public float skillCriticalDamage;
    public float skillCriticalProbability;
    public Manager manager;
    public float ItemHealBoost;
    public Shild shildS;
    public float jumpSpeedSkill = 1f;
    public float dashPowerBoostSkill = 1f;
    public GameObject dashPowerBoostSkillGameObject;
    public float attackSpeed;
    public GameObject barrageShield_Skill;
    public int dropItemCount;
    public int combo4SkillPluse;
    public bool attackLootSkill;//Ž���� Ÿ���� Ȱ��ȭ �������


    [Header("������")]
    public bool isWalk;
    public float speed;
    public float speedRatio;
    public float walkAndRunspeed;
    public float moveSpeed;
    public float runSpeed;


    public bool w;
    public bool s;
    public bool a;
    public bool d;
    private float nextSwitchTime = 0f;
    public float switchDelay = 0.2f; // 0.2�ʸ��� �Ӽ� ����
    [Header("���� ����")]
    public bool jump;
    public Transform groundCheck;
    public LayerMask isGroundLayerMask;

    public bool dontMove;
    public bool dontRun;
    public GameObject ray;
    public bool isGround;
    [SerializeField] private bool land;
    CameraController cameraController;
    GameObject cameraFollow;

    public bool tauntDuration;

    public bool knockCorutin;
    public float staminaMax;
    public float stamina;
    public Slider staminaSlider;
    public Rigidbody rb;
    Quaternion currentRotation = Quaternion.identity;
    [Header("UI")]
    public TextMeshProUGUI[] HpText;
    public TextMeshProUGUI[] StaminaText;
    public Image hitspriteRenderer;
    public Sprite[] hitImage;

    [SerializeField] private float rotationSpeed = 5f;
    public bool shild;
    public Coroutine shildUseTime;


    public bool enemyshilding;//�� ������
    public bool blocking;
    //���з� ���� �а� �ִ°�?
    public bool push;
    public float pushSpeed;
    public GameObject[] pushPaticle;
    //���и� ��� ������� shildMeshColliderxƮ���Ű� ��Ȱ��ȭ��
    public MeshCollider shildMeshCollider;
    public MeshCollider shildMeshCollider2;
    Blocking blockingS;
    Animator animator;
    public bool playerinvincibility;//�÷��̾� ��������
    [Header("����")]
    //��
    public float combo;
    public float comboDurationTime;
    public Sword sword;
    public Coroutine attack;
    public float power;//��
    public float attackPower;//�������� ����
    public bool swinging_timing;
    public bool hit;
    public BoxCollider deshBoxCollider;
    public BulletShooter shoot;

    //����
    public string[] magicProperty;
    public int magicCombo;

    public string currentMagicProperty;
    public int magicPropertyindex;
    public GameObject[] magicCardImage;
    public GameObject[] magicBulletPrefab;
    public GameObject[] MagicCase; //0 : ���ӿ�����Ʈ 1: UI
    public GameObject comboeffect;

    //��ȭ
    public int enhanceSwordDamage;// �� ������ ��ȭ
    public int enhanceMagicDamage;//���� ������ ��ȭ
    public float attackDelayEnhance; //������ ���ݽ� ������ ����
    public int maxMagicCombo;//���� ���ݽ� ��ȭ ��ų�� �ʿ��� �޺� ���� ����;
    public float swordStaminaReduction;
    public float MagicStaminaReduction;
    //���� ������
    [Range(0f, 1f)]
    public float durability = 1f;
    [Header("��ȭ ������ �������ͽ�")]
    public float shilDreduction;//��ȭ ������ �������ͽ�
    public bool durabilityBool;//������
    //��� ���� ��ȭ ��ų
    public MagicDefense magicDefense;
    public bool hideDefenceSkill;


    //�뽬
    [SerializeField] float dashPower = 500f;
    [SerializeField] float dashDuration = 0.2f;
    [Header("��ȭ ������ �������ͽ�")]
    public float dashCooldown = 2f;//��ȭ ������ �������ͽ�
    public float groundCheckDistance = 1.5f;
    float lastDashTime = -999f;
    Vector3 deshdir;
    [SerializeField] AnimationCurve dashCurve; // ���� �

    [Header("�Ҹ�")]
    public AudioSource playerSound;
    public AudioClip[] attackClip;
    public AudioClip[] attackVoiceClip;
    public AudioClip deshAudioClip;
    public AudioClip deshAttackClip;
    public int attackSoundCount;
    public AudioSource playerlowHP;
    public AudioClip[] magicBulletSound;
    [Header("�߼Ҹ� ����Ʈ")]
    public AudioClip[] footstepClips;
    public AudioClip landClip;
    [Header("������")]
    public Item item;
    public ItemData data;
    [Header("��ȭ ������ �������ͽ�")]
    public float itemAbsorptionDistence = 5;//��ȭ ������ �������ͽ�
    [Header("����ġ")]
    public int level;
    public int saveLevel;
    public float currentExp;
    public float[] maxExp;
    public Slider expSilder;
    public Material xpBarMaterial;


    [Header("����ȭ ������ ����")]
    private float nextActionTime = 0.0f;
    public float period = 0.055f; // ���ϴ� �ð� ���� (0.05��)
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        data = item.GetItemData();
    }
    void Start()
    {
        saveLevel = level;
        swordManager = GetComponent<SwordManager>();
        shoot = GetComponent<BulletShooter>();
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; // Ŀ���� ȭ�� �߾ӿ� ����
        Cursor.visible = false; // Ŀ�� ����
        animator = GetComponent<Animator>();
        blockingS = GetComponentInChildren<Blocking>();
        playerSound = GetComponent<AudioSource>();
        cameraFollow = GameObject.FindGameObjectWithTag("Follow");
        sword = GetComponentInChildren<Sword>();
        for (int i = 0; i < maxExp.Length; i++)
        {
            maxExp[i] = i * 12f;
        }
    }
    void Update()
    {
        if (Manager.Instance.eventManagerActive.activeSelf) return;
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            UI();
            if (Manager.Instance.uiBool) return;
            Shild();
            Blocking();
            PushPaticle();

            AttackState();

            SwordGameObject.SetActive(physicsType);
            MagicCase[0].SetActive(!physicsType);
            MagicCase[1].SetActive(!physicsType);
        }
    }
    private void FixedUpdate()
    {
        if (Manager.Instance.eventManagerActive.activeSelf) return;
        animator.SetBool("IsAir", !isGround);
        animator.SetFloat("AttackSpeed", (attackSpeed + (attackDelayEnhance * 2)) + swordManager.weponAttackSpeedApplication);
        Rotation();
        Move();
        Stamina();
        HP();
        WallCheck();
        Desh();
        Jump();
    }
    void HP()
    {
        if (hp <= maxhp * 0.3f)
        {
            if (!playerlowHP.isPlaying)
            {
                playerlowHP.Play();
                playerlowHP.loop = true;
            }
        }
        else
        {
            playerlowHP.Stop();
            playerlowHP.loop = false;
        }
        if (hp <= 0)
        {
            Manager.Instance.GameOver = true;
        }
        hpSlider.value = hp;
        hpSlider.maxValue = maxhp;
    }
    void Stamina()
    {
        staminaSlider.value = stamina;
        if (shild || Input.GetKey(KeyCode.LeftShift))
        {
            if (stamina >= 0)
            {
                stamina -= Time.deltaTime;
            }
            return;
        }
        if (enemyshilding) return;
        if (shild) return;//���и� ��� �ִ°�� ���¹̳� ȸ���� ����
        if (stamina <= staminaMax)
        {
            stamina += Time.deltaTime * 2.5f;
        }

    }
    void AttackState()
    {
        if (Manager.Instance.turnState != Manager.TurnState.Tutorials)
        {
            physicsType = Manager.Instance.currentTrun % 5 != 0;
        }
        if (!physicsType)
        {

            if (Manager.Instance.currentTrun % 5 == 0 || Manager.Instance.turnState == Manager.TurnState.Tutorials)
            {

                if (Input.GetKey(KeyCode.Q) && Time.time >= nextSwitchTime)
                {
                    nextSwitchTime = Time.time + switchDelay;

                    magicPropertyindex = (magicPropertyindex + 1) % magicProperty.Length;

                    for (int i = 0; i < magicCardImage.Length; i++)
                    {
                        magicCardImage[i].SetActive(i == magicPropertyindex);
                        MagicGameObject[i].SetActive(i == magicPropertyindex);
                    }

                    currentMagicProperty = magicProperty[magicPropertyindex];
                }

            }

        }
        if (Input.GetMouseButton(0) && stamina >= 5)
        {
            comboDurationTime = 0f;
            Debug.Log("����");
            if (attack != null) return;
            //���� �ڸ�ƾ�� �������� �ƴ�
            attack = StartCoroutine(Attack());
        }
        if (comboDurationTime >= 4f)
        {
            combo = 0f;
            return;
        }
        else
        {
            //�޺� ���ӽð�
            comboDurationTime += Time.deltaTime;
        }
    }
    IEnumerator Attack()
    {
        //���� �ڸ�ƾ ����
        attackSoundCount++;
        string attackType = physicsType == true ? "Attack" : "MagicAttack";
        animator.SetTrigger(attackType);
        shildMeshCollider.gameObject.layer = 17;
        if (physicsType)
        {
            animator.SetFloat("Combo", combo);


            deshBoxCollider.isTrigger = true;
            if (combo >= 3)
            {
                combo = 0f;
            }
            if (attackSoundCount > 3)
            {
                attackSoundCount = 0;
            }
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            comboDurationTime = 0;
            while (!stateInfo.IsName(attackType))
            {
                dontMove = true;
                yield return null;
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            }
            land = false;
            dontMove = false;
            float aniTime = stateInfo.length;
            shildMeshCollider.gameObject.layer = 6;
            yield return new WaitForSeconds(aniTime - attackDelayEnhance * 2f);
            combo += 1.0f;
            Invoke("InvokeSwordEffectInitialization", 0.15f);
            deshBoxCollider.isTrigger = false;
            yield return new WaitForSeconds(0.125f - attackDelayEnhance);
        }
        else
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(1);
            while (!stateInfo.IsName(attackType))
            {
                yield return null;
                stateInfo = animator.GetCurrentAnimatorStateInfo(1);
            }
            float aniTime = stateInfo.length;
            if (magicCombo == maxMagicCombo + 1)
            {
                magicCombo = 0;
                comboeffect.SetActive(false);
            }
            else if (magicCombo == maxMagicCombo)
            {
                comboeffect.SetActive(true);
                magicCombo += 1;
            }
            else
            {
                magicCombo += 1;
            }
            yield return new WaitForSeconds(aniTime);
        }
        attack = null;
        sword.hitCoroutine = null;
        //���� ����
    }
    public void AttackDesh()
    {
        StartCoroutine(AttackDeshCoroutine());
    }
    void Shild()
    {

        if (playerinvincibility) return;
        if (durability <= 0 || stamina <= 0 || shildUseTime != null)
        {
            shildUseTime = StartCoroutine(ShildCoolTime());
            return;
        }
        shild = Input.GetMouseButton(1);
        animator.SetBool("Shild", shild);
        shildMeshCollider.enabled = shild;
        shildMeshCollider2.enabled = shild;
        if (physicsType == true)
        {
            animator.SetLayerWeight(1, shild ? 1 : 0);
        }
        else
        {
            if (shild)
            {
                animator.SetLayerWeight(1, shild ? 1 : 0);
                magicDefense.UseDefenceSkill(magicPropertyindex);
                return;
            }
            animator.SetLayerWeight(1, attack != null ? 1 : 0);
        }
    }
    IEnumerator ShildCoolTime()//���¹̳� �������� ���и� ���� ���� ���и� ������ ������� �ٽ� ���и� �������� �ð�
    {

        shild = false;

        yield return new WaitForSeconds(3);

        shildUseTime = null;
    }
    void Blocking()
    {
        if (!shild || gameObject.layer == 14)
        {
            return;
        }
        blocking = Input.GetKey(KeyCode.E);
        animator.SetBool("blocking", blocking);
        animator.SetLayerWeight(1, blocking ? 1 : 0);
    }
    void Jump()
    {
        bool jump = Input.GetKey(KeyCode.Space);
        Ray rayS = new Ray(groundCheck.position, Vector3.down);
        isGround = Physics.Raycast(rayS, 0.6f, isGroundLayerMask);

        animator.SetFloat("Speed", speed);
        if (jump && isGround && !land && !ApplyEvent.instance.suppressedLeap)
        {
            land = true;
            animator.SetTrigger("Jump");
            Vector3 velocity = rb.velocity;
            velocity.y = 5.5f * jumpSpeedSkill; // ������ ���� �ӵ� ��
            rb.velocity = velocity;
        }
    }
    void Desh()
    {
        if (durability <= 0 || stamina <= 0 || shildUseTime != null)
        {
            shildUseTime = StartCoroutine(ShildCoolTime());
            return;
        }
        if (stamina >= 5)
        {
            if (Input.GetKey(KeyCode.E) && Time.time >= lastDashTime + dashCooldown * (ApplyEvent.instance.chainShackleCurseValue * ApplyEvent.instance.suppressedLeapRewardValue))
            {
                animator.SetTrigger("Desh");
                lastDashTime = Time.time;
                StartCoroutine(SlowTime(0.3f));
            }
        }
    }
    public void DeshClip()
    {
        StartCoroutine(DeshCoroutine());
    }
    IEnumerator SlowTime(float slowTime)
    {
        float realSlow = slowTime;
        yield return new WaitForSeconds(0.1f);
        while (realSlow < 1)
        {
            realSlow += Time.unscaledDeltaTime;
            Time.timeScale = realSlow;
            yield return null;
        }

        Time.timeScale = 1f;
    }
    IEnumerator DeshCoroutine()
    {
        float Reductionstamina = physicsType ? swordStaminaReduction : MagicStaminaReduction;
        playerSound.PlayOneShot(deshAudioClip, Sound.Instance.playerSound);
        gameObject.layer = 14;
        stamina -= 5 - Reductionstamina;
        float timer = 0f;
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        Vector3 dashDir = GetGroundAdjustedForward(); // ��� ����
        dashDir.y = 0; // ���� ���� ����
        dashDir.Normalize();
        deshBoxCollider.isTrigger = true;
        Vector3 dashVelocity = dashDir * (dashPower * dashPowerBoostSkill);
        float originalDrag = rb.drag; // ���� �巡�� �� ����
        rb.drag = 0; // ��� �� ���� ����

        while (timer < dashDuration)
        {

            rb.velocity = dashVelocity; // ������ �ӵ� ����
            timer += Time.unscaledDeltaTime; // ���ο� ��ǿ��� �����ϰ� �帣��
            yield return null;
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        rb.velocity = Vector3.zero; // ���߱� (optional)
        rb.drag = originalDrag; // ���� �巡�� ����
        deshBoxCollider.isTrigger = false;
        yield return new WaitForSeconds(1f);
        gameObject.layer = 10;
    }

    IEnumerator AttackDeshCoroutine()
    {

        Debug.Log("AttackDeshCoroutine");
        float deshEffectamplification = w || a || s || d ? 2 : 1;
        //PlayAttackVoice();
        if (deshEffectamplification > 1)
        {
            playerSound.PlayOneShot(deshAttackClip, Sound.Instance.playerSound);
        }
        stamina -= 2 * deshEffectamplification;

        //ũ��Ƽ�� Ȯ��
        float Critical = swordManager.weponCriticalApplication + skillCriticalProbability + EventManager.Instance.bonusCriticalChance;
        float CriticalDamage = 0;
        if (Random.value < Critical) //���� �ִ� ġ��Ÿ Ȯ�� 50%
        {
            CriticalDamage = swordManager.weponCriticalDamageApplication + skillCriticalDamage;
        }

        // �⺻ ���ݷ�
        float basePower = power;

        // ��ȭ �� ���ʽ� ���ݷ� �ջ�
        float enhancedPower = enhanceSwordDamage
                            + swordManager.aweponDamageApplication
                            + EventManager.Instance.bonusAttackPower;

        // �̺�Ʈ ��� ����
        float eventMultiplier = ApplyEvent.instance.smallShadowRewardValue
                              * ApplyEvent.instance.bindingRageRewardValue;

        // ���� ���ݷ� ���
        attackPower = (basePower + enhancedPower * eventMultiplier) * (1 + CriticalDamage);
        Debug.Log("attackPower :" + attackPower);
        if (physicsType)
        {
            sword.swordEffect.Play();
            sword.swordEffect2.Play();
            sword.swordEffect3.Play();
            sword.swordEffect.transform.localScale = Vector3.one * deshEffectamplification;
            sword.swordEffect2.transform.localScale = Vector3.one * deshEffectamplification;
            playerSound.clip = attackClip[attackSoundCount];
            playerSound.Play();
        }
        if (dashPowerBoostSkill > 1.1f && deshEffectamplification > 1)
            PoolManager.Instance.GetFromPool<PoolChild>(dashPowerBoostSkillGameObject, new Vector3(transform.position.x, 11.5f,
                                                                                                  transform.position.z), Quaternion.Euler(-90, 0, 0));
        float timer = 0f;
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        Vector3 dashDir = GetGroundAdjustedForward(); // ��� ����
        dashDir.y = 0; // ���� ���� ����
        dashDir.Normalize();
        swinging_timing = true;
        Vector3 dashVelocity = dashDir * (dashPower * dashPowerBoostSkill);
        float originalDrag = rb.drag; // ���� �巡�� �� ����
        rb.drag = 0; // ��� �� ���� ����

        while (timer < 0.3f)
        {
            rb.velocity = dashVelocity; // ������ �ӵ� ����
            timer += Time.unscaledDeltaTime; // ���ο� ��ǿ��� �����ϰ� �帣��
            yield return null;
        }
        attackPower = 0;
        Invoke("InvokeSwordEffectInitialization", 0.15f);
        land = false;
        swinging_timing = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        deshBoxCollider.isTrigger = false;
        rb.velocity = Vector3.zero; // ���߱� (optional)
        rb.drag = originalDrag; // ���� �巡�� ����
    }
    void InvokeSwordEffectInitialization()
    {
        sword.swordEffect.transform.localScale = Vector3.one;
        sword.swordEffect2.transform.localScale = Vector3.one;
    }
    Vector3 GetGroundAdjustedForward()
    {
        RaycastHit hits;
        Vector3 origin = transform.position + Vector3.up * 0.2f;

        if (Physics.Raycast(origin, Vector3.down, out hits, groundCheckDistance))
        {
            if (hits.distance > 0.5f) // ���� ����� ������������ ���� ����
                return deshdir;

            Vector3 adjusted = Vector3.ProjectOnPlane(deshdir, hits.normal).normalized;
            float slopeAngle = Vector3.Angle(Vector3.up, hits.normal);
            if (slopeAngle > 45f)
                return deshdir;

            return adjusted;
        }

        return transform.forward;
    }

    void Move()
    {
        if (push)
        {
            Vector3 pushDir = -transform.forward;
            rb.MovePosition(rb.position + pushDir * (blocking ? 1 : pushSpeed) * Time.deltaTime);
            return;
        }

        Vector3 input = Vector3.zero;
        isWalk = false;

        Quaternion cameraYRotation = Quaternion.Euler(0, cameraFollow.transform.eulerAngles.y, 0);
        w = Input.GetKey(KeyCode.W);
        s = Input.GetKey(KeyCode.S);
        a = Input.GetKey(KeyCode.A);
        d = Input.GetKey(KeyCode.D);

        if (w) { input += cameraYRotation * Vector3.forward; isWalk = true; }
        if (s) { input -= cameraYRotation * Vector3.forward; isWalk = true; }
        if (a) { input -= cameraYRotation * Vector3.right; isWalk = true; }
        if (d) { input += cameraYRotation * Vector3.right; isWalk = true; }

        input = input.normalized;
        deshdir = input;

        WallCheck(); // �̵� ���� �� üũ�ؼ� dontMove ����

        if (dontMove)
            return;
        if (Manager.Instance.turnTime > 0)
        {
            speed = isWalk ? moveSpeed : 0;
        }
        else
        {
            speed = isWalk ? 1 : 0;
            runSpeed = 1.5f;
        }

        walkAndRunspeed = Input.GetKey(KeyCode.LeftShift) && !dontRun ? runSpeed : 0;
        animator.SetFloat("WalkSpeed", Input.GetKey(KeyCode.LeftShift) == true ? 1.5f : 1f);
        Vector3 targetPosition = rb.position;

        // �⺻ �̵� �ӵ� ��
        float baseSpeed = speed + walkAndRunspeed + EventManager.Instance.bonusMoveSpeed;

        // ���� ��� (���ǵ� ���� + �̺�Ʈ ���� ��)
        float modifiers = speedRatio
                        * ApplyEvent.instance.giantShadowRewardPlayerValue
                        * ApplyEvent.instance.powderKegRewardValue
                        * ApplyEvent.instance.chainShackleRewardValue
                        * ApplyEvent.instance.bulletHellOverdriveRewardValue
                        * ApplyEvent.instance.berserkRewardValue;

        // ���� �̵� �ӵ�
        float finalSpeed = baseSpeed * modifiers * Time.fixedDeltaTime;


        if (!ApplyEvent.instance.bindingRage)//12�� �̺�Ʈ �������� �ƴϸ� ��
        { // ��ǥ ��ġ
            targetPosition += input * finalSpeed;
        }


        /*  Vector3 targetPosition = rb.position + input * ((speed + walkAndRunspeed + EventManager.Instance.bonusMoveSpeed) * (speedRatio * ApplyEvent.instance.giantShadowRewardPlayerValue * ApplyEvent.instance.powderKegRewardValue)) * Time.fixedDeltaTime;*/

        rb.MovePosition(targetPosition);

        if (input != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(input);
            rb.MoveRotation(targetRotation);
        }

        animator.SetFloat("WalkType", shild ? 1 : 0);
        animator.SetFloat("Walk", speed);
        animator.SetFloat("ShildDown", s ? -1 : 1);
        animator.SetFloat("ShildUp", s ? -1 : 1);
    }


    void Rotation()
    {
        float mouseDeltaX = Input.GetAxis("Mouse X"); // �¿� ���콺 �̵���
        float rotationAmount = mouseDeltaX * rotationSpeed;
        Quaternion delta = Quaternion.Euler(0, rotationAmount, 0);

        currentRotation *= delta; // ���� ȸ��

        rb.MoveRotation(currentRotation);
    }

    public IEnumerator Knock()
    {
        knockCorutin = true;
        playerinvincibility = true;
        animator.SetBool("Knock", true);
        dontMove = true;

        while (!IsAnimationClipFinished("Knock Out"))
        {
            yield return null;
        }
        playerinvincibility = false;
        animator.SetBool("Knock", false);
        dontMove = false;
        knockCorutin = false;
    }
    public void PushPaticle()
    {
        for (int i = 0; i < pushPaticle.Length; i++)
        {
            pushPaticle[i].SetActive(push);
        }
    }
    public void UI()
    {
        HpText[1].text = hp.ToString("F0");
        HpText[0].text = maxhp.ToString() + "  / ";
        StaminaText[0].text = stamina.ToString("F0");
        StaminaText[1].text = staminaMax.ToString("F0") + "  / ";
        hpSlider.value = hp;
        staminaSlider.value = stamina;

        xpBarMaterial.SetFloat("_segmentAmount", maxExp[level]);
        float currentLevelBaseExp = maxExp[level];

        float nextLevelBaseExp = (level + 1 < maxExp.Length) ? maxExp[level + 1] : maxExp[maxExp.Length - 1];
        if (level >= maxExp.Length - 1) // ������ ���
        {
            expSilder.maxValue = 1;
            expSilder.value = 1;
        }
        else // ������ �ƴ� ���
        {
            expSilder.maxValue = nextLevelBaseExp - currentLevelBaseExp;
            expSilder.value = currentExp - currentLevelBaseExp;
        }
    }

    public void WallCheck()
    {
        float rayDistance = 1f;
        Vector3 origin = ray.transform.position;

        // �̵� ������ 0�̸� �̵� ���� ����
        if (deshdir.magnitude < 0.1f)
        {
            dontMove = false;
            return;
        }

        // �̵� �������θ� ����ĳ��Ʈ
        if (Physics.Raycast(origin, deshdir.normalized, out RaycastHit hit, rayDistance, 1 << 15))
        {
            dontMove = true;
            Debug.DrawRay(origin, deshdir.normalized * rayDistance, Color.red);
        }
        else
        {
            dontMove = false;
            Debug.DrawRay(origin, deshdir.normalized * rayDistance, Color.green);
        }
    }
    public void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;

        var clip = footstepClips[Random.Range(0, footstepClips.Length)];
        playerSound.pitch = Random.Range(0.95f, 1.05f);
        playerSound.PlayOneShot(clip, Sound.Instance.playerSound);
    }
    public void PlayAttackVoice()
    {
        if (attackVoiceClip.Length == 0) return;

        var clip = attackVoiceClip[physicsType == true ? attackSoundCount : 0];
        playerSound.pitch = Random.Range(0.95f, 1.05f);
        playerSound.PlayOneShot(clip, Sound.Instance.playerSound);
    }

    public void Shoot()
    {
        shoot.playerShoot = true;
        shoot.PlayerShoot();
    }
    public IEnumerator PlayerMagicShoot()
    {
        for (int i = 0; i < combo4SkillPluse; i++)
        {
            playerSound.PlayOneShot(magicBulletSound[magicPropertyindex]);
            var magicBullet = Instantiate(magicBulletPrefab[magicPropertyindex], MagicGameObjectPos.transform.position, Quaternion.LookRotation(transform.forward)); // Y �����ϰ� ȸ��
            magicBullet.gameObject.SetActive(false);
            yield return null;
            magicBullet.gameObject.SetActive(true);
            MagicBullet _magicBullet = magicBullet.GetComponent<MagicBullet>();
            _magicBullet.saveCombo = magicCombo;
            _magicBullet.SetNormalizedDir(transform.forward, 40 * hitSize);
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void MagicShoot()
    {
        StartCoroutine(PlayerMagicShoot());
    }
    public void Hit(float enemyAttack, Enemy_Base enemy_Base)
    {
        float power = enemy_Base.monsterData.dangerLevel;
        hp -= enemyAttack;
        int randDamagedSound = Random.Range(0, damagedAudioClip.Length);
        playerSound.PlayOneShot(damagedAudioClip[randDamagedSound], Sound.Instance.playerSound);
        CameraShake.instance.Shake(0.02f * power, 0.03f * power, 0.03f * power);
        if (screenHitEffect == null)
        {
            screenHitEffect = StartCoroutine(ScreenHitEffect());
        }

    }
    public void DumyMonsterHit(float enemyAttack)
    {
        hp -= enemyAttack;
        int randDamagedSound = Random.Range(0, damagedAudioClip.Length);
        playerSound.PlayOneShot(damagedAudioClip[randDamagedSound], Sound.Instance.playerSound);
        CameraShake.instance.Shake(0.02f, 0.03f, 0.03f);
        if (screenHitEffect == null)
        {
            screenHitEffect = StartCoroutine(ScreenHitEffect());
        }

    }
    public void TickDamage(float enemyAttack)
    {
        hp -= enemyAttack;
        CameraShake.instance.Shake(0.0025f * power, 0.0025f * power, 0.002f * power);
        if (screenHitEffect == null)
        {
            screenHitEffect = StartCoroutine(ScreenHitEffect());
        }
    }
    IEnumerator ScreenHitEffect()
    {
        damaged = true;
        hitspriteRenderer.sprite = hitImage[1];
        float _vignettelne = 0.6f;
        while (_vignettelne < 1f)
        {
            _vignettelne += Time.deltaTime * 1.5f;
            shockwavePostEffect.hitMat.SetFloat("_Vignettelne", _vignettelne);
            yield return null;
        }

        while (_vignettelne > 0.6f)
        {
            _vignettelne -= Time.deltaTime * 1.5f;
            shockwavePostEffect.hitMat.SetFloat("_Vignettelne", _vignettelne);
            yield return null;
        }
        hitspriteRenderer.sprite = hitImage[0];
        damaged = false;
        screenHitEffect = null;
    }

    private bool IsAnimationClipFinished(string clipName)
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0); // �⺻ ���̾� 0
        if (stateInfo.IsName(clipName))
        {
            // �ִϸ��̼� Ŭ���� ���� ���°� �������� Ȯ�� (normalizedTime >= 1)
            return stateInfo.normalizedTime >= 1f;
        }
        return false;
    }
    public void Land()
    {
        land = false;  //���� �� ���� ������� ȿ���� 
        playerSound.PlayOneShot(landClip, Sound.Instance.playerSound);
    }

    public void SkillApply()
    {
        foreach (var skill in skills.activeSkills)
        {
            skill.Activate(this);
        }
    }
    public void SkillDeApply()
    {
        foreach (var skill in skills.activeSkills)
        {
            skill.Deactivate(this);
        }
    }
    public void OnLand()
    {
        if (isGround && !land)
        {
            land = true;
            playerSound.PlayOneShot(landClip, Sound.Instance.playerSound);
            animator.SetBool("IsAir", false);
        }
        else if (!isGround)
        {
            land = false;
            animator.SetBool("IsAir", true);
        }
    }

}
