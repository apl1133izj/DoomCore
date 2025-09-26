using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.AI.Navigation; // NavMeshSurface Ŭ������
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngine.UI;
public class Manager : MonoBehaviour
{
    public static Manager Instance;
    public List<TurnData> turnDatas = new List<TurnData>();
    public enum TurnState
    {
        Battle,     // ���� ��
        RepairTime,  // ���� �ð� ��
        Tutorials, //Ʃ�丮��
        Test
    }


    public bool testBool;

    Coroutine GarbageCollectorCollectIncrementalCoroutine;
    [Header("���۽� �⺻ ����")]
    public List<Base> baseActive;
    [Header("Ʃ�丮��")]
    public GameObject nextButton;
    public GameObject playerUI;//���丮���߿��� ��Ȱ��ȭ
    public GameObject tutorialBack;
    public GameObject keyTutorial;//Ű ���� Ʃ�丮��
    public GameObject directionGuide;//�� �ȳ�
    public GameObject coreBasrActive;//�ھ� Ȱ��ȭ Ʃ�丮��
    public List<GameObject> coreBasTutorials;//1�ܰ� �ھ ��� Ȱ��ȭ �Ǿ����
    public GameObject spawPonsterPoint;//Ʃ�丮�� �߿��� ���� ������ ����
    public GameObject tutorialsCheckGameObject;//�������࿡�� Ʃ�丮���� �����ҰųĴ� �ȳ�����
    public GameObject shildTutorials;
    public GameObject magicTutorials;
    Coroutine tutorialStartSound;
    [Header("����")]
    public TurnState turnState = TurnState.Battle;


    [Header("���� ����")]
    public bool GameClear;
    public bool GameOver;
    public bool gameClearEvent;
    public Coroutine warringCoroutine;
    public Coroutine gameClearCoroutine;
    public GameObject EndBoseMonster;//���ѽð��ȿ� ���͸� óġ��������
    public GameObject TowerEndBoseMonster;//���ѽð��ȿ� ���͸� óġ��������
    public bool gameOverSceneActive;//���� : ���ӿ��� �������� ���ӿ��� ������ �Ѿ��
    Coroutine gameOverSceneActiveCoroutine;
    public Coroutine endMonster;
    public Player player;
    public MonsterSpaw monsterSpaw;
    public GameObject[] OnEnableCamera;//�÷��̾�,Ÿ�� ī�޶�
    public AudioSource gameOverCinematinSound;
    [SerializeField] private NavMeshSurface[] surface;
    public float totalDefenseTime;
    public GameObject[] gameoverEnableObject;//Ÿ�� �������� �ð� �ʰ��� �������� ���¸� �ٲ���ϴ� �ؾ��ϴ� ���ӿ�����Ʈ;
    [Header("��")]
    public int Maxtrun;
    public int currentTrun;//���� ��
    public int ReductionTrun;//���ҵ���
    int lastCoreActiveCount = 0;//��
    public bool physicalAttackTrun;//���������� ��
    public ViewModeChanger shop;
    public DefenseBarrier[] defenseBarrier;//�ھ� Ȱ��ȭ ���� 
    public int coreActiveCount;
    public GameObject eventManagerActive;//�� ���۽� �̺�Ʈ Ȱ��ȭ

    [Header("óġ Ƚ��")]
    public int spawMonsterCount;
    public int killedMonsterCount;

    [Header("�ð�")]
    public float turnTime;
    public float pluseTurnTime;
    public float repairTimer = 0f;
    public float repairlimitTime; //���� ����ð����� ���� �ð�

    public float timeStopTime;//���� �̿�� ����ð��� ���� �ð��� ����� ���� ȿ�� ���� 0�̸� �ð��� �Ȱ�
    [Header("UI")]
    public bool uiBool;
    public bool showKeyUI;
    public Image announceTextPenel;
    public GameObject[] ShildUI;//0 : ���� , 1 : ����
    string[] developerAnnounceText = new string[]
{
    // 5��
    "\n[�ھ� ������ �˸�]\n\n" +
    "<color=#FF0000>[���� �ĵ� ���� ������]</color> ������Ʈ�� ���õǾ����ϴ�.\n" +
    "�ĵ��� Ȯ��Ǹ� ���� ������ ������ �ʴ� ���Ͱ� ��Ÿ���ϴ�.\n" +
    "����� �ڵ����� <color=#00FFFF>���� ���</color>�� ��ȯ�˴ϴ�.\n" +
    "�������� �ϼ��� ������ �ݵ�� ���߾� �մϴ�.\n",

    // 10��
    "\n[�ھ� ������ �˸�]\n\n" +
    "�츮�� <color=#FF0000>[���� �ĵ�]</color>�� ���� ������ ���� ���Դϴ�.\n" +
    "�ھ��� ����� �ĵ��� ���� ������Ű�� ������ �����˴ϴ�.\n" +
    "����� �ڵ����� <color=#00FFFF>���� ���</color>�� ��ȯ�˴ϴ�.\n",

    // 15��
    "\n[�ھ� ������ �˸�]\n\n" +
    "<color=#FF0000>[���� �ĵ�]</color>�� �ٿ��� ���������ϴ�.\n" +
    "�ĵ��� ���� �ż�����, ������ ������ �ֽ��ϴ�.\n" +
    "����� �ڵ����� <color=#00FFFF>���� ���</color>�� ��ȯ�˴ϴ�.\n",

    // 20��
    "\n[�ھ� ������ �˸�]\n\n" +
    "<color=#FF0000>[���� �ĵ� ���� ������]</color>�� ���赵�� �ϼ� �ܰ迡 �����߽��ϴ�.\n" +
    "�׷��� ������ ���Ⱑ ���Ӿ��� �����ǰ� �ֽ��ϴ�.\n" +
    "����� �ڵ����� <color=#00FFFF>���� ���</color>�� ��ȯ�˴ϴ�.\n",

    // 25��
    "\n[�ھ� ������ �˸�]\n\n" +
    "�ٽ� ��ǰ ���ۿ� �����߽��ϴ�.\n" +
    "���ÿ� <color=#FF0000>[���� �ĵ�]</color>�� �з��� �������� �ֽ��ϴ�.\n" +
    "����� �ڵ����� <color=#00FFFF>���� ���</color>�� ��ȯ�˴ϴ�.\n",

    // 30��
    "\n[�ھ� ������ �˸�]\n\n" +
    "�ٽ� ��ǰ�� ������ ���� ���Դϴ�.\n" +
    "<color=#FF0000>[���� �ĵ�]</color>�� �ż� ������ ���̴�ġ�� �ֽ��ϴ�.\n" +
    "����� �ڵ����� <color=#00FFFF>���� ���</color>�� ��ȯ�˴ϴ�.\n",

    // 35��
    "\n[�ھ� ������ �˸�]\n\n" +
    "�ٽ� ��ǰ ������ �Ϸ�Ǿ����ϴ�.\n" +
    "�׷��� �ĵ��� �з��� ���� �� �������� �ֽ��ϴ�.\n" +
    "����� �ڵ����� <color=#00FFFF>���� ���</color>�� ��ȯ�˴ϴ�.\n",

    // 40��
    "\n[�ھ� ������ �˸�]\n\n" +
    "���� �������� ���밡 �巯�����ϴ�.\n" +
    "<color=#FF0000>[���� �ĵ�]</color>�� ���� �ܰ迡 ������ �ֽ��ϴ�.\n" +
    "����� �ڵ����� <color=#00FFFF>���� ���</color>�� ��ȯ�˴ϴ�.\n",

    // 45��
    "\n[�ھ� ������ �˸�]\n\n" +
    "���͵��� ���� ���������� �ֽ��ϴ�.\n" +
    "<color=#FF0000>[���� �ĵ�]</color>�� ���Ⱑ ���� �� �ݷ��������ϴ�.\n" +
    "����� �ڵ����� <color=#00FFFF>���� ���</color>�� ��ȯ�˴ϴ�.\n",

    // 50��
    "\n[�ھ� ������ �˸�]\n\n" +
    "����� ���忡�� �̻��� �߻��߽��ϴ�. ������ �����ǰ� �ֽ��ϴ�.\n" +
    "<color=#FF0000>[���� �ĵ�]</color>�� ���ں��ϰ� ����ġ�� �ֽ��ϴ�.\n" +
    "����� �ڵ����� <color=#00FFFF>���� ���</color>�� ��ȯ�˴ϴ�.\n",

    // 55��
    "\n[�ھ� ������ �˸�]\n\n" +
    "������ �ذ��߽��ϴ�. ������ ���θ��� �ֽ��ϴ�.\n" +
    "������ <color=#FF0000>[���� �ĵ�]</color>�� �ְ����� �����߽��ϴ�.\n" +
    "����� �ڵ����� <color=#00FFFF>���� ���</color>�� ��ȯ�˴ϴ�.\n",

    // 60��
    "\n[�ھ� ������ �˸�]\n\n" +
    "����� ������ �ϼ��� �յΰ� �ֽ��ϴ�.\n" +
    "<color=#FF0000>[���� �ĵ�]</color>�� ��ġ ��ǳó�� �䵿Ĩ�ϴ�.\n" +
    "����� �ڵ����� <color=#00FFFF>���� ���</color>�� ��ȯ�˴ϴ�.\n",

    // 65��
    "\n[�ھ� ������ �˸�]\n\n" +
    "���� ��ġ ��ġ�� ������ �ܰ迡 �����߽��ϴ�.\n" +
    "<color=#FF0000>[���� �ĵ�]</color>�� ������ ���� ġ�ݰ� �ֽ��ϴ�.\n" +
    "����� �ڵ����� <color=#00FFFF>���� ���</color>�� ��ȯ�˴ϴ�.\n",

    // 70��
    "\n[�ھ� ������ �˸�]\n\n" +
    "�������� ������ ������ �巯�����ϴ�.\n" +
    "<color=#FF0000>[���� �ĵ�]</color>�� �е����� ������ �����ɴϴ�.\n" +
    "����� �ڵ����� <color=#00FFFF>���� ���</color>�� ��ȯ�˴ϴ�.\n",

    // 75��
    "\n[�ھ� ������ �˸�]\n\n" +
    "���� ���˿� �����߽��ϴ�.\n" +
    "<color=#FF0000>[���� �ĵ�]</color>�� ������ ���� ������ �����Դϴ�.\n" +
    "����� �ڵ����� <color=#00FFFF>���� ���</color>�� ��ȯ�˴ϴ�.\n",

    // 80��
    "\n[�ھ� ������ �˸�]\n\n" +
    "���� �������� �ϼ��� �����Դϴ�.\n" +
    "<color=#FF0000>[���� �ĵ�]</color>�� �Ѱ迡 �����߽��ϴ�.\n" +
    "����� �ڵ����� <color=#00FFFF>���� ���</color>�� ��ȯ�˴ϴ�.\n",

    // 85��
    "\n[�ھ� ������ �˸�]\n\n" +
    "������ ���� ������ ���� ���Դϴ�.\n" +
    "<color=#FF0000>[���� �ĵ�]</color>�� ��ģ ���� �ҿ뵹��Ĩ�ϴ�.\n" +
    "����� �ڵ����� <color=#00FFFF>���� ���</color>�� ��ȯ�˴ϴ�.\n",

    // 90��
    "\n[�ھ� ������ �˸�]\n\n" +
    "���� ��ġ�� �ٽ��� �ϼ��� ��������� �ֽ��ϴ�.\n" +
    "<color=#FF0000>[���� �ĵ�]</color>�� ���� ������ ���尨�� �ھƳ��ϴ�.\n" +
    "����� �ڵ����� <color=#00FFFF>���� ���</color>�� ��ȯ�˴ϴ�.\n",

    // 95��
    "\n[�ھ� ������ �˸�]\n\n" +
    "���� �ܰ� ����!\n" +
    "<color=#FF0000>[���� �ĵ�]</color>�� ���� �Ұ����� ���ؿ� �̸������ϴ�.\n" +
    "����� �ڵ����� <color=#00FFFF>���� ���</color>�� ��ȯ�˴ϴ�.\n",

    // 100�� (�Ϸ�)
    "\n[�ھ� ������ �˸�]\n\n" +
    "<color=#00FF00>[���� �ĵ� ���� ������]</color>�� ���� ������ ������ �����߽��ϴ�!\n" +
    "������ ������ �ĵ��� �������, ��ħ�� ��ȭ�� ã�ƿԽ��ϴ�.\n" +
    "����� ��Ű� ��⿡ ���� ���縦 �帳�ϴ�.\n" +
    "<color=#FFD700>����� ��� ���� ���ѳ½��ϴ�!</color>"
};


    public TextMeshProUGUI announceText;//�ȳ�
    public TextMeshProUGUI currentTrunUI;//���� ��
    public TextMeshProUGUI spawMonsterCountUI;//�����Ͽ� ������ ���ͼ�
    public TextMeshProUGUI killedMonsterCountUI;//���� �Ͽ� óġ�� ���ͼ�
    public TextMeshProUGUI turnTimeUI;  //���� �ϱ��� �ð�
    public TextMeshProUGUI repairTimerUI;//���� �ð�
    public TextMeshProUGUI repairStateUI;
    public TextMeshProUGUI[] coreCount;
    public GameObject physicalAttackTrunClearannouncement;//5�� ������ �̿��� ������ Ŭ���� ������� ������ UI ���ӿ�����Ʈ Ȱ��ȭ
    public GameObject endTrunGameObject;
    public GameObject[] gameOverUI; //0:playerStatusUI 1:monsterUI
    public GameObject toggle_Controls;
    public Animator gameOverAnimator;
    public GameObject warringUI;
    public enum BGMType
    {
        Battle_Normal,       // �Ϲ� ���� (ü�� 100~30%)
        Battle_Critical,     // ���� ���� (ü�� 30% ����)
        Shop,                // ����
        GameOver,            // ���� ���� (���â ����)
        Title,               // Ÿ��Ʋ / �κ�
        Boss_Appear,         // ���� ����
        Preparation,         // ���� �ð�
        Time_AlmostOver,      // Ÿ�� ���� �ӹ�
        GameClear,            // ���� Ŭ���� (���â ����)
    }
    public BGMType bgmType;
    public int currentBGM;
    public AudioSource backgroundSound;
    public List<AudioClip> backgroundClip;// 0 :�Ϲ� ���� (ü�� 100~30%)1:���� ���� (ü�� 30% ����) 2:���� 3:���� ���� ���� 4: Ÿ��Ʋ/�κ�
                                          //  5:���� ���� �� 6:���� �ð� 7:Ÿ�ӿ��� �ӹ� 8:Ÿ�� ���� ȿ����
    public AudioSource audioSource;
    public List<AudioClip> pointSoundClip;
    public AudioClip levelUpSound;
    public AudioSource hitAudioSource;

    [Header("���̵�")]
    public int difficulty;//���� �����̷� ���̵� ����
    public Enhance enhance;
    public EnhanceData enhanceData;


    float nextframe;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayBGM(BGMType.Title);
        backgroundSound.loop = true;
        enhanceData = enhance.GetEnhanceData(2);

        if (!Player.Instance.data.hasCompletedTutorial)
        {
            playerUI.SetActive(false);
            turnState = TurnState.Tutorials;
            StartCoroutine(TutorialStart());
        }
        else
        {
            if (turnState == TurnState.Tutorials)
            {
                //    ����� Ȱ��ȭ
                for (int r = 0; r < Player.Instance.data.RareCoreCount.Length; r++)
                {
                    if (r == 0)
                    {
                        Player.Instance.data.RareCoreCount[0] = 175;
                        continue;
                    }
                    Player.Instance.data.RareCoreCount[r] = 0;
                }
            }
            else
            {
                Player.Instance.data.RareCoreCount[0] = 25;
            }
            playerUI.SetActive(true);
            turnState = TurnState.Battle;
        }
        int baseLimitTime = 320; // �⺻�� 320
        int increasePerTurn = 10; // �ϴ� ������

        for (int i = 30; i < turnDatas.Count; i++)
        {

            // i��° �� limitTime = �⺻�� + (�� �� - 30) * ������
            turnDatas[i].limitTime = baseLimitTime + (i - 30) * increasePerTurn;

            turnDatas[i].turn = i;
            turnDatas[i].monsterCount += i;
        }

        // ���� �� limitTime ����
        repairlimitTime = turnDatas[currentTrun].limitTime;
        turnTime = GetTurnLimitTime(currentTrun) + pluseTurnTime;
    }
    void Update()
    {
        /*        if (GarbageCollectorCollectIncrementalCoroutine == null)
                {
                    GarbageCollectorCollectIncrementalCoroutine = StartCoroutine(GarbageCollectorCollectIncremental());
                }*/
        if (eventManagerActive.activeSelf) return;
        if (Time.time > nextframe)
        {
            nextframe += 0.03f;
            if (TowerEndBoseMonster.activeSelf)
            {
                if (!GameOver || !GameClear)
                {
                    for (int i = 0; i < OnEnableCamera.Length; i++)
                    {
                        OnEnableCamera[i].SetActive(false);
                    }
                }
                else
                {
                    OnEnableCamera[7].SetActive(true);
                }

            }
            if (gameOverSceneActive)
            {
                if (gameOverSceneActiveCoroutine != null) return;
                gameOverSceneActiveCoroutine = StartCoroutine(GameOverSceneActiveCoroutine());
                return;
            }
            BackGroundType();

            if (!string.IsNullOrWhiteSpace(announceText.text))
            {
                announceTextPenel.enabled = true;
            }
            else
            {
                announceTextPenel.enabled = false;
            }
            if (endMonster != null)
            {
                announceText.text = "\n�ð��ȿ� ���� �������� ���� �������Ͱ� �����˴ϴ�";
                return;
            }
            spawPonsterPoint.SetActive(turnState != TurnState.Tutorials);
            GameOVERAndClear();
            if (uiBool)
            {
                Cursor.lockState = CursorLockMode.Confined; // Ŀ���� ȭ�� ������ ������ ���ϵ���
                Cursor.visible = true; // Ŀ�� ����
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked; // Ŀ���� ȭ�� �߾ӿ� ����
                Cursor.visible = false; // Ŀ�� ����
            }
            if (GameOver || GameClear) return;
            True();
            UI();
            int calculatedLevel = GetLevel(player.currentExp, player.maxExp);
            if (turnState == TurnState.RepairTime)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    repairTimer = 0.1f;
                }
            }

            // ������ ����Ǿ����� Ȯ��
            if (player.level != calculatedLevel)
            {
                player.level = calculatedLevel;
                LevelUpActions();
            }
        }
    }
    int GetLevel(float currentExp, float[] maxExp)
    {
        for (int i = 0; i < maxExp.Length - 1; i++)
        {
            if (currentExp >= maxExp[i] && currentExp < maxExp[i + 1])
            {
                return i;
            }
        }

        // ������ ������ ũ�� ���� ���� ��ȯ
        return maxExp.Length - 1;
    }
    public void LevelUpActions()
    {
        player.saveLevel = player.level; // ������������ ����
        audioSource.PlayOneShot(levelUpSound, Sound.Instance.playerSound);
    }
    public void True()
    {
        switch (turnState)
        {
            case TurnState.Battle:
                totalDefenseTime += Time.deltaTime;
                //���� �ð��� 0�� �ɶ� ���� �ð� ����
                if (turnTime > 0)
                {
                    turnTime -= Time.deltaTime;

                }
                else
                {
                    if (endMonster == null)
                    {
                        endMonster = StartCoroutine(ENDMonster());
                    }

                    return;
                }
                if (killedMonsterCount == GetTurnMonsterCount(currentTrun))
                {

                    endTrunGameObject.SetActive(true);
                    turnState = TurnState.RepairTime;
                    if (currentTrun % 4 == 0 && turnState == TurnState.RepairTime)
                    {
                        ShildUI[0].SetActive(false);
                        ShildUI[1].SetActive(true);
                        repairTimer = 45f * 2;
                        // ���� ���� 5�� ���� ������ �޽��� �ε��� ���
                        //int currentMessage = Mathf.Min((currentTrun / 4) - 1, developerAnnounceText.Length - 1);
                        // ������ �ʿ��� �� �� (���� ���� �ݿ�)
                        int totalTurn = Maxtrun - ReductionTrun;

                        // ����� ��
                        int progressedTurn = currentTrun;

                        // ����� (0 ~ 1)
                        float progress = Mathf.Clamp01((float)progressedTurn / totalTurn);

                        // ������� �޽��� �ε����� ��ȯ
                        int currentMessage = Mathf.Min(
                            Mathf.FloorToInt(progress * (developerAnnounceText.Length - 1)),
                            developerAnnounceText.Length - 1);

                        // UI�� �޽��� ǥ��
                        turnTimeUI.text = developerAnnounceText[currentMessage];

                    }
                    else
                    {
                        ShildUI[0].SetActive(true);
                        ShildUI[1].SetActive(false);
                        repairTimer = 45f;
                        turnTimeUI.text =
                            "\n���� �ð��Դϴ�.\n" +
                            "�ھ� ����Ƿ� �̵��� �ھ Ȱ��ȭ�ϼ���.\n";
                    }
                }
                break;

            case TurnState.RepairTime:
                //5�ϸ��� Ŭ����UI
                if (currentTrun == Maxtrun - ReductionTrun || gameClearEvent)
                {
                    if (gameClearCoroutine != null) return;
                    gameClearCoroutine = StartCoroutine(GameClearEvent());
                }

                repairTimer -= timeStopTime * Time.deltaTime;
                totalDefenseTime = 0f;
                if (repairTimer <= 0)
                {
                    // ���� �ð� �� �� ���� �� ����
                    eventManagerActive.SetActive(true);
                    currentTrun++;
                  
                    turnTimeUI.text = "";
                    repairTimer = GetTurnLimitTime(currentTrun);
                    turnTime = GetTurnLimitTime(currentTrun);
                    spawMonsterCount = 0;
                    killedMonsterCount = 0;
                    //Ȱ��ȭ�� �ھ� Ƚ�� 7ȸ���� ������� �ʿ��� ���� ������
                    for (int i = 0; i < defenseBarrier.Length; i++)
                    {
                        if (defenseBarrier[i].gameObject.activeSelf)
                        {
                            coreActiveCount++;
                        }
                    }

                    // �̹��� ���� �߰��� �ھ� ������ŭ ���� ���
                    int newlyActivated = coreActiveCount - lastCoreActiveCount;

                    // 7�� �������� ReductionTurn ����
                    if (newlyActivated > 0)
                    {
                        int prevBonus = lastCoreActiveCount / 7;
                        int newBonus = coreActiveCount / 7;

                        // ���̰� �ִٸ� �׸�ŭ ����
                        if (newBonus > prevBonus)
                        {
                            ReductionTrun += newBonus - prevBonus;
                        }
                    }

                    // ������ ���� ����
                    lastCoreActiveCount = coreActiveCount;
                    warringCoroutine = null;
                }
                break;
        }
        bool isMagicTurn = currentTrun % 4 == 0;

        if (isMagicTurn && turnState == TurnState.RepairTime)
        {
            physicalAttackTrun = false;
        }
        else if (!isMagicTurn)
        {
            physicalAttackTrun = true;
        }

    }
    IEnumerator GameClearEvent()
    {
        gameClearEvent = true;
        yield return new WaitForSeconds(8);
        GameClear = true;
    }
    IEnumerator Warring()
    {
        backgroundSound.PlayOneShot(backgroundClip[8], Sound.Instance.backSound);
        warringUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        warringUI.gameObject.SetActive(false);
    }
    IEnumerator GameOverSceneActiveCoroutine()
    {
        StartCoroutine(ENDMonster());
        yield return null;
    }
    IEnumerator ENDMonster()
    {
        GameObject ENDBOSE = null;
        if (currentTrun % 5 != 0)
        {
            surface[0].enabled = false;
            surface[1].enabled = true;
            ENDBOSE = Instantiate(EndBoseMonster, monsterSpaw.closePlayerPoint.transform.position, Quaternion.identity);
        }
        else
        {
            if (!GameOver)
            {
                for (int i = 0; i < OnEnableCamera.Length; i++)
                {
                    OnEnableCamera[i].SetActive(false);
                }
                TowerEndBoseMonster.SetActive(true);
            }
            else
            {
                OnEnableCamera[7].SetActive(true);
            }

        }

        if (ENDBOSE != null)
        {
            yield return new WaitUntil(() => ENDBOSE.activeSelf == false);
        }
        endMonster = null;
    }
    //���͸� �� óġ�� �ð��� ���Ҵٸ� �� �ð� ��ŵ - ���������� ����
    public void TimeSkipButton()
    {
        if (turnState == TurnState.Battle &&
            GetTurnMonsterCount(currentTrun) == spawMonsterCount &&
            spawMonsterCount == killedMonsterCount)
        {
            turnState = TurnState.RepairTime;
            repairTimer = 0f;
            turnTime = 0f;
            spawMonsterCount = 0;
            killedMonsterCount = 0;
            repairlimitTime = turnDatas[currentTrun].limitTime;
        }
        else
        {
            announceText.text = "\n���͸� ���� óġ�ؾ� �մϴ�";
        }
    }
    public float GetTurnLimitTime(int currentTurn)
    {
        var data = turnDatas.Find(x => x.turn == currentTurn);
        if (data != null)
        {
            return data.limitTime;
        }
        return 0f;
    }

    public int GetTurnMonsterCount(int currentTurn)
    {

        var data = turnDatas.Find(x => x.turn == currentTurn);
        if (data != null)
        {
            return data.monsterCount;
        }
        return 0;
    }

    public void UI()
    {

        currentTrunUI.text = ((Maxtrun - ReductionTrun) - currentTrun).ToString();
        spawMonsterCountUI.text = (GetTurnMonsterCount(currentTrun) - killedMonsterCount).ToString();

        killedMonsterCountUI.text = killedMonsterCount.ToString();

        repairlimitTime -= Time.deltaTime;
        if (turnState == TurnState.RepairTime)
        {
            repairStateUI.text = "���� ����ð�  ";
            repairTimerUI.text = repairTimer.ToString("F1");
        }
        else
        {
            repairStateUI.text = "���� ������� �����ð�  ";
            repairTimerUI.text = turnTime.ToString("F1");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            showKeyUI = !showKeyUI;
            toggle_Controls.SetActive(showKeyUI);
        }
        for (int i = 0; i < 3; i++)
        {
            coreCount[i].text = Player.Instance.item.itemData.RareCoreCount[i].ToString();
        }
    }

    public Slider slider;
    public TextMeshProUGUI survivalTurn;
    public TextMeshProUGUI point;
    public Coroutine pointSliderCoroutine;
    public GameObject goTite;

    public void PointSlider()
    {
        if (goTite.activeSelf || pointSliderCoroutine != null) return;
        pointSliderCoroutine = StartCoroutine(PointSliderCoroutine());
    }
    //���� ������ ���ӿ��� ����Ʈ ����
    IEnumerator PointSliderCoroutine()
    {
        yield return new WaitUntil(() => point.gameObject.activeSelf);
        slider.value = 0;
        survivalTurn.text = currentTrun.ToString("F0");
        point.text = "0";
        yield return new WaitForSeconds(1f);
        audioSource.clip = pointSoundClip[0];
        audioSource.Play();
        audioSource.loop = true;

        int gameoverpointCount = GameOver == true ? currentTrun : 1000;
        slider.maxValue = GameOver == true ? 96 * 2 : 1000;
        while (slider.value < gameoverpointCount)
        {
            survivalTurn.text = currentTrun.ToString("F0");
            slider.value += Time.deltaTime * 100;
            yield return null;
            point.text = (slider.value * 2).ToString("F0");

        }
        audioSource.clip = pointSoundClip[1];
        audioSource.Play();
        audioSource.loop = false;
        Player.Instance.data.GameOverPoint += GameOver == true ? currentTrun * 2 : 1000;
        Player.Instance.item.Save();
        survivalTurn.text = "0";
        point.text = (GameOver == true ? currentTrun * 2 : 1000).ToString("F0");
        goTite.SetActive(true);
        pointSliderCoroutine = null;
    }
    public void GameOVERAndClear()
    {

        if (!GameOver && !GameClear)
        {
            // �� �� false�� ���� return
            goTite.SetActive(false);
            gameOverAnimator.enabled = false;
            return;
        }
        //���� Ŭ����,���� �ϰ�츸
        turnState = TurnState.RepairTime;
        uiBool = true;
        PointSlider();
        for (int i = 0; i < gameOverUI.Length; i++)
        {
            gameOverUI[i].SetActive(false);
        }

        gameOverAnimator.enabled = true;
        if (GameOver || GameClear)
        {
            gameoverEnableObject[0].SetActive(true);
            OnEnableCamera[7].SetActive(true);
        }
    }

    Coroutine bgmFadeCoroutine;
    private void BackGroundType()
    {
        backgroundSound.volume = Sound.Instance.backSound;
        Player.Instance.playerSound.volume = Sound.Instance.playerSound;


        BGMType newBgmType = bgmType;

        if (GameOver)
        {
            newBgmType = BGMType.GameOver;
        }
        else if (GameClear)
        {
            newBgmType = BGMType.GameClear;
        }
        else if (!Player.Instance.gameObject.activeSelf)
        {
            newBgmType = BGMType.Shop;
        }
        else
        {
            bool isBossAlive = GameObject.FindGameObjectWithTag("BoseArear") != null;

            if (isBossAlive)
            {
                newBgmType = BGMType.Boss_Appear;
            }
            else if (turnState == TurnState.RepairTime)
            {
                newBgmType = BGMType.Preparation;
            }
            else if (turnState == TurnState.Battle)
            {
                // Highest priority: Time almost over
                if (turnTime <= turnDatas[currentTrun].limitTime * 0.3f)
                {
                    newBgmType = BGMType.Time_AlmostOver;
                }
                // Next priority: Critical health
                else if (Player.Instance.hp < Player.Instance.maxhp * 0.3f)
                {
                    newBgmType = BGMType.Battle_Critical;
                }
                // Lowest priority: Normal battle
                else if (Player.Instance.hp > Player.Instance.maxhp * 0.3f)
                {
                    newBgmType = BGMType.Battle_Normal;
                }
            }
        }

        if (bgmType != newBgmType)
        {
            bgmType = newBgmType;
            PlayBGM(bgmType);
        }
    }
    // 0 :�Ϲ� ���� (ü�� 100~30%)1:���� ���� (ü�� 30% ����) 2:���� 3:���� ���� ���� 4: Ÿ��Ʋ/�κ�
    // 5 :���� ���� �� 6:���� �ð� 7:Ÿ�ӿ��� �ӹ� 8:Ÿ�� ���� ȿ����
    public void PlayBGM(BGMType bgmType)
    {

        // �ߺ� Ÿ���̸� ����
        if (currentBGM == (int)bgmType)
            return;

        currentBGM = (int)bgmType;

        int bgmNumber = -1;

        switch (bgmType)
        {
            case BGMType.Battle_Normal:
                bgmNumber = 0;
                break;
            case BGMType.Battle_Critical:
                bgmNumber = 1;
                break;
            case BGMType.Shop:
                bgmNumber = 2;
                break;
            case BGMType.GameOver:
                bgmNumber = 3;
                break;
            case BGMType.Title:
                bgmNumber = 4;
                break;
            case BGMType.Boss_Appear:
                bgmNumber = 5;
                break;
            case BGMType.Preparation://����
                bgmNumber = 6;
                break;
            case BGMType.Time_AlmostOver://�� ���� ���� �ð��� �� ���� ���� 30%������
                bgmNumber = 7;
                break;
            case BGMType.GameClear:
                bgmNumber = 10;
                break;
        }

        if (bgmNumber >= 0)
        {
            Play(bgmNumber);
        }
    }

    private void Play(int bgmNumber)
    {
        // �̹� ��� ���� ���̸� ����
        if (backgroundSound.clip == backgroundClip[bgmNumber] && backgroundSound.isPlaying)
            return;

        // ���� ���� ���� ���̵尡 ������ �ߴ��ϰ� �� ��ȯ ����
        if (bgmFadeCoroutine != null)
        {
            StopCoroutine(bgmFadeCoroutine);
            bgmFadeCoroutine = null;
        }

        bgmFadeCoroutine = StartCoroutine(CrossFadeToNewBGM(bgmNumber));
    }

    private IEnumerator CrossFadeToNewBGM(int newBgmNumber)
    {
        if (GameClear)
        {
            backgroundSound.clip = backgroundClip[10];
            backgroundSound.Play();
        }
        else
        {
            float fadeDuration = 1f;
            float startVolume = backgroundSound.volume;

            AudioClip newClip = backgroundClip[newBgmNumber];

            //  ���̵� �ƿ� ������
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                backgroundSound.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
                yield return null;
            }

            // ������ ���� �� ���ο� �� ��ü
            backgroundSound.Stop();
            backgroundSound.clip = newClip;
            backgroundSound.loop = true;
            backgroundSound.Play();

            //  ���̵� �� ���ο� ��
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                backgroundSound.volume = Mathf.Lerp(0f, Sound.Instance.backSound, t / fadeDuration);
                yield return null;
            }

            backgroundSound.volume = Sound.Instance.backSound;
        }

        bgmFadeCoroutine = null;
    }
    public void GoTitleButton()
    {
        SceneManager.LoadScene(0);
    }

    IEnumerator TutorialStartSound()
    {
        audioSource.clip = pointSoundClip[2];
        audioSource.Play();
        audioSource.loop = false;
        yield return new WaitUntil(() => tutorialStartSound == null);
    }
    IEnumerator TutorialStart()
    {
        tutorialStartSound = null;

        //Ű�ȳ�
        bool nextTutorial1 = false;
        tutorialBack.SetActive(true);
        player.physicsType = true;
        int key = 0;
        Play(9);
        announceText.text = "\n[T] Ű�� ���� ���۹��� Ȯ���� �� �ֽ��ϴ�.\n���� Ʃ�丮��� �����Ϸ��� [T] Ű�� �Է��ϼ���.";
        while (!nextTutorial1)
        {
            if (key > 1)
            {
                nextButton.SetActive(true);
                if (tutorialStartSound == null)
                {
                    tutorialStartSound = StartCoroutine(TutorialStartSound());
                }

                nextTutorial1 = Input.GetKeyDown(KeyCode.Q);
            }
            else
            {
                if (Input.GetKey(KeyCode.T))
                {
                    key++;
                }
                nextButton.SetActive(false);
            }
            keyTutorial.SetActive(true);

            yield return null;
        }
        tutorialStartSound = null;
        nextButton.SetActive(false);
        keyTutorial.SetActive(false);




        //�� �ȳ�
        bool nextTutorial2 = false;
        int key2 = 0;
        announceText.text = "\n[�����е�]�� ������ �ھ� ����Ǳ��� ��ȳ��� ǥ�õǰ� 8���� ������ �ȳ��� ���� �մϴ�.\n���� Ʃ�丮��� �����Ϸ���[�����е�] Ű�� �Է��ϼ���.";
        while (!nextTutorial2)
        {
            if (key2 > 1)
            {
                if (tutorialStartSound == null)
                {
                    tutorialStartSound = StartCoroutine(TutorialStartSound());
                }
                nextButton.SetActive(true);
                nextTutorial2 = Input.GetKeyDown(KeyCode.Q);
            }
            else
            {
                if (Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Alpha3)
                 || Input.GetKey(KeyCode.Alpha4) || Input.GetKey(KeyCode.Alpha5) || Input.GetKey(KeyCode.Alpha6) || Input.GetKey(KeyCode.Alpha1))
                {
                    key2++;
                }

                nextButton.SetActive(false);
            }
            directionGuide.SetActive(true);

            yield return null;
        }
        tutorialStartSound = null;
        nextButton.SetActive(false);
        directionGuide.SetActive(false);

        //���� ��� Ʃ�丮��
        shildTutorials.SetActive(true);
        announceText.text = "\n���и� ��� �Ϸ��� ���콺 ��Ŭ�� �� ��� �ؾ��մϴ�.\n 10�� ź���� ���� �Ǹ� ���� Ʃ�丮���� ����˴ϴ�";
        while (shildTutorials.activeSelf)
        {
            yield return null;
        }
        announceText.text = "";
        magicTutorials.SetActive(true);
        while (magicTutorials.activeSelf)
        {
            yield return null;
        }

        //�ھ� Ȱ��ȭ �ȳ�
        bool nextTutorial3 = false;
        Player.Instance.data.RareCoreCount[0] = 175;
        announceText.text = "\n�ھ� ����ǿ� �����ϸ�  \n�ھ� Ȱ��ȭ UI�� �ڵ����� ǥ�õ˴ϴ�.\n[�ھ� ���� ���� �Ϸ� - ��� �ھ Ȱ��ȭ ���Ѿ� Ʃ�丮���� ����˴ϴ�]\n���� ���۽� 175���� �ϱ� �ھ ���޵ʴϴ�.";
        while (!nextTutorial3)
        {
            for (int i = 0; i < coreBasTutorials.Count; i++)
            {
                bool allActive = coreBasTutorials.All(t => t.activeSelf);
                if (allActive)
                {
                    if (tutorialStartSound == null)
                    {
                        tutorialStartSound = StartCoroutine(TutorialStartSound());
                    }
                    nextButton.SetActive(true);

                    while (!nextTutorial3)
                    {
                        if (Input.GetKey(KeyCode.Q))
                        {
                            nextTutorial3 = true;
                        }
                        yield return null;
                    }
                }
            }
            coreBasrActive.SetActive(true);
            yield return null;
        }
        announceText.text = "";
        yield return new WaitForSeconds(0.25f);
        tutorialBack.SetActive(false);
        coreBasrActive.SetActive(false);
        tutorialsCheckGameObject.SetActive(true);
        yield return null;
        while (!Check)
        {

            if (Input.GetKey(KeyCode.Q))
            {
                if (tutorialStartSound == null)
                {
                    tutorialStartSound = StartCoroutine(TutorialStartSound());
                }
                TutorialsCheck(false);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                if (tutorialStartSound == null)
                {
                    tutorialStartSound = StartCoroutine(TutorialStartSound());
                }
                TutorialsCheck(true);
            }
            if (!Check)
            {
                tutorialsCheckGameObject.SetActive(true);
            }
            else
            {
                break;
            }
            tutorialStartSound = null;
            yield return null;
        }
        announceText.text = "";
        playerUI.SetActive(true);
        turnState = TurnState.Battle;
    }
    private bool Check = false;
    public void TutorialsCheck(bool tf)
    {
        Player.Instance.data.hasCompletedTutorial = tf;
        Check = true;
        tutorialsCheckGameObject.SetActive(false);
    }

    public void currentTrunChange(bool tf)
    {
        if (tf)
        {
            if (currentTrun < 99)
            {
                currentTrun += 1;
            }

        }
        else
        {
            if (currentTrun > 1)
            {
                currentTrun -= 1;
            }
        }
        turnState = TurnState.RepairTime;
        repairTimer = 0f;
        turnTime = 0f;
        spawMonsterCount = 0;
        killedMonsterCount = 0;
        repairlimitTime = turnDatas[currentTrun].limitTime;
    }

    public void PlayAndStop(bool _stay)
    {
        //t : ���� f: ������ 
        if (_stay)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }



    IEnumerator GarbageCollectorCollectIncremental()
    {
        GarbageCollector.CollectIncremental(1000000); // 1MB�� ����
        yield return new WaitForSeconds(30);
        GarbageCollectorCollectIncrementalCoroutine = null;
    }
}

[System.Serializable]
public class TurnData
{
    public int turn;           // �� ��ȣ
    public int monsterCount;   // �ش� �� ���� ��
    public float limitTime;    // ���� �ð�(��)
}
