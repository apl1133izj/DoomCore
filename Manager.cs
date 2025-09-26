using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.AI.Navigation; // NavMeshSurface 클래스가
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
        Battle,     // 전투 중
        RepairTime,  // 정비 시간 중
        Tutorials, //튜토리얼
        Test
    }


    public bool testBool;

    Coroutine GarbageCollectorCollectIncrementalCoroutine;
    [Header("시작시 기본 설정")]
    public List<Base> baseActive;
    [Header("튜토리얼")]
    public GameObject nextButton;
    public GameObject playerUI;//투토리얼중에만 비활성화
    public GameObject tutorialBack;
    public GameObject keyTutorial;//키 설명 튜토리얼
    public GameObject directionGuide;//길 안내
    public GameObject coreBasrActive;//코어 활성화 튜토리얼
    public List<GameObject> coreBasTutorials;//1단계 코어가 모두 활성화 되어야함
    public GameObject spawPonsterPoint;//튜토리얼 중에는 몬스터 스폰을 막음
    public GameObject tutorialsCheckGameObject;//다음실행에도 튜토리얼을 실행할거냐는 안내문자
    public GameObject shildTutorials;
    public GameObject magicTutorials;
    Coroutine tutorialStartSound;
    [Header("전투")]
    public TurnState turnState = TurnState.Battle;


    [Header("게임 상태")]
    public bool GameClear;
    public bool GameOver;
    public bool gameClearEvent;
    public Coroutine warringCoroutine;
    public Coroutine gameClearCoroutine;
    public GameObject EndBoseMonster;//제한시간안에 몬스터를 처치하지못함
    public GameObject TowerEndBoseMonster;//제한시간안에 몬스터를 처치하지못함
    public bool gameOverSceneActive;//마법 : 게임오버 당했으면 게임오버 씬으로 넘어가짐
    Coroutine gameOverSceneActiveCoroutine;
    public Coroutine endMonster;
    public Player player;
    public MonsterSpaw monsterSpaw;
    public GameObject[] OnEnableCamera;//플레이어,타워 카메라
    public AudioSource gameOverCinematinSound;
    [SerializeField] private NavMeshSurface[] surface;
    public float totalDefenseTime;
    public GameObject[] gameoverEnableObject;//타워 전투에서 시간 초과를 당했을때 상태를 바꿔야하는 해야하는 게임오브젝트;
    [Header("턴")]
    public int Maxtrun;
    public int currentTrun;//현재 턴
    public int ReductionTrun;//감소된턴
    int lastCoreActiveCount = 0;//비교
    public bool physicalAttackTrun;//물리공격인 턴
    public ViewModeChanger shop;
    public DefenseBarrier[] defenseBarrier;//코어 활성화 여부 
    public int coreActiveCount;
    public GameObject eventManagerActive;//턴 시작시 이벤트 활성화

    [Header("처치 횟수")]
    public int spawMonsterCount;
    public int killedMonsterCount;

    [Header("시간")]
    public float turnTime;
    public float pluseTurnTime;
    public float repairTimer = 0f;
    public float repairlimitTime; //다음 정비시간까지 남은 시간

    public float timeStopTime;//상점 이용시 정비시간이 멈춰 시간이 멈춘거 같은 효과 값이 0이면 시간이 안감
    [Header("UI")]
    public bool uiBool;
    public bool showKeyUI;
    public Image announceTextPenel;
    public GameObject[] ShildUI;//0 : 물리 , 1 : 마법
    string[] developerAnnounceText = new string[]
{
    // 5턴
    "\n[코어 연구원 알림]\n\n" +
    "<color=#FF0000>[비물질 파동 억제 구조물]</color> 프로젝트가 개시되었습니다.\n" +
    "파동이 확산되면 물리 공격이 통하지 않는 몬스터가 나타납니다.\n" +
    "무기는 자동으로 <color=#00FFFF>마법 모드</color>로 전환됩니다.\n" +
    "구조물이 완성될 때까지 반드시 버텨야 합니다.\n",

    // 10턴
    "\n[코어 연구원 알림]\n\n" +
    "우리는 <color=#FF0000>[비물질 파동]</color>의 생성 원인을 추적 중입니다.\n" +
    "코어의 사용이 파동을 더욱 증폭시키는 것으로 추정됩니다.\n" +
    "무기는 자동으로 <color=#00FFFF>마법 모드</color>로 전환됩니다.\n",

    // 15턴
    "\n[코어 연구원 알림]\n\n" +
    "<color=#FF0000>[비물질 파동]</color>의 근원이 밝혀졌습니다.\n" +
    "파동은 점점 거세지며, 세상을 뒤흔들고 있습니다.\n" +
    "무기는 자동으로 <color=#00FFFF>마법 모드</color>로 전환됩니다.\n",

    // 20턴
    "\n[코어 연구원 알림]\n\n" +
    "<color=#FF0000>[비물질 파동 억제 구조물]</color>의 설계도가 완성 단계에 도달했습니다.\n" +
    "그러나 파장의 세기가 끊임없이 고조되고 있습니다.\n" +
    "무기는 자동으로 <color=#00FFFF>마법 모드</color>로 전환됩니다.\n",

    // 25턴
    "\n[코어 연구원 알림]\n\n" +
    "핵심 부품 제작에 돌입했습니다.\n" +
    "동시에 <color=#FF0000>[비물질 파동]</color>의 압력이 심해지고 있습니다.\n" +
    "무기는 자동으로 <color=#00FFFF>마법 모드</color>로 전환됩니다.\n",

    // 30턴
    "\n[코어 연구원 알림]\n\n" +
    "핵심 부품의 조립이 진행 중입니다.\n" +
    "<color=#FF0000>[비물질 파동]</color>의 거센 물결이 들이닥치고 있습니다.\n" +
    "무기는 자동으로 <color=#00FFFF>마법 모드</color>로 전환됩니다.\n",

    // 35턴
    "\n[코어 연구원 알림]\n\n" +
    "핵심 부품 조립이 완료되었습니다.\n" +
    "그러나 파동의 압력이 한층 더 강해지고 있습니다.\n" +
    "무기는 자동으로 <color=#00FFFF>마법 모드</color>로 전환됩니다.\n",

    // 40턴
    "\n[코어 연구원 알림]\n\n" +
    "억제 구조물의 뼈대가 드러났습니다.\n" +
    "<color=#FF0000>[비물질 파동]</color>이 폭주 단계에 접어들고 있습니다.\n" +
    "무기는 자동으로 <color=#00FFFF>마법 모드</color>로 전환됩니다.\n",

    // 45턴
    "\n[코어 연구원 알림]\n\n" +
    "몬스터들이 더욱 흉포해지고 있습니다.\n" +
    "<color=#FF0000>[비물질 파동]</color>의 세기가 한층 더 격렬해졌습니다.\n" +
    "무기는 자동으로 <color=#00FFFF>마법 모드</color>로 전환됩니다.\n",

    // 50턴
    "\n[코어 연구원 알림]\n\n" +
    "기계의 심장에서 이상이 발생했습니다. 제작이 지연되고 있습니다.\n" +
    "<color=#FF0000>[비물질 파동]</color>은 무자비하게 몰아치고 있습니다.\n" +
    "무기는 자동으로 <color=#00FFFF>마법 모드</color>로 전환됩니다.\n",

    // 55턴
    "\n[코어 연구원 알림]\n\n" +
    "문제를 해결했습니다. 제작을 서두르고 있습니다.\n" +
    "하지만 <color=#FF0000>[비물질 파동]</color>은 최고조에 도달했습니다.\n" +
    "무기는 자동으로 <color=#00FFFF>마법 모드</color>로 전환됩니다.\n",

    // 60턴
    "\n[코어 연구원 알림]\n\n" +
    "기계의 심장이 완성을 앞두고 있습니다.\n" +
    "<color=#FF0000>[비물질 파동]</color>은 마치 폭풍처럼 요동칩니다.\n" +
    "무기는 자동으로 <color=#00FFFF>마법 모드</color>로 전환됩니다.\n",

    // 65턴
    "\n[코어 연구원 알림]\n\n" +
    "결합 장치 설치가 마지막 단계에 도달했습니다.\n" +
    "<color=#FF0000>[비물질 파동]</color>은 극한을 향해 치닫고 있습니다.\n" +
    "무기는 자동으로 <color=#00FFFF>마법 모드</color>로 전환됩니다.\n",

    // 70턴
    "\n[코어 연구원 알림]\n\n" +
    "구조물의 윤곽이 완전히 드러났습니다.\n" +
    "<color=#FF0000>[비물질 파동]</color>은 압도적인 힘으로 몰려옵니다.\n" +
    "무기는 자동으로 <color=#00FFFF>마법 모드</color>로 전환됩니다.\n",

    // 75턴
    "\n[코어 연구원 알림]\n\n" +
    "최종 점검에 착수했습니다.\n" +
    "<color=#FF0000>[비물질 파동]</color>은 지금이 가장 강력한 순간입니다.\n" +
    "무기는 자동으로 <color=#00FFFF>마법 모드</color>로 전환됩니다.\n",

    // 80턴
    "\n[코어 연구원 알림]\n\n" +
    "억제 구조물의 완성이 눈앞입니다.\n" +
    "<color=#FF0000>[비물질 파동]</color>은 한계에 도달했습니다.\n" +
    "무기는 자동으로 <color=#00FFFF>마법 모드</color>로 전환됩니다.\n",

    // 85턴
    "\n[코어 연구원 알림]\n\n" +
    "마지막 결합 공정이 진행 중입니다.\n" +
    "<color=#FF0000>[비물질 파동]</color>이 미친 듯이 소용돌이칩니다.\n" +
    "무기는 자동으로 <color=#00FFFF>마법 모드</color>로 전환됩니다.\n",

    // 90턴
    "\n[코어 연구원 알림]\n\n" +
    "억제 장치의 핵심이 완성에 가까워지고 있습니다.\n" +
    "<color=#FF0000>[비물질 파동]</color>은 폭발 직전의 긴장감을 자아냅니다.\n" +
    "무기는 자동으로 <color=#00FFFF>마법 모드</color>로 전환됩니다.\n",

    // 95턴
    "\n[코어 연구원 알림]\n\n" +
    "최종 단계 돌입!\n" +
    "<color=#FF0000>[비물질 파동]</color>은 통제 불가능한 수준에 이르렀습니다.\n" +
    "무기는 자동으로 <color=#00FFFF>마법 모드</color>로 전환됩니다.\n",

    // 100턴 (완료)
    "\n[코어 연구원 알림]\n\n" +
    "<color=#00FF00>[비물질 파동 억제 구조물]</color>이 드디어 완전히 가동을 시작했습니다!\n" +
    "세상을 뒤흔들던 파동은 사라지고, 마침내 평화가 찾아왔습니다.\n" +
    "당신의 헌신과 용기에 깊은 감사를 드립니다.\n" +
    "<color=#FFD700>당신은 모든 것을 지켜냈습니다!</color>"
};


    public TextMeshProUGUI announceText;//안내
    public TextMeshProUGUI currentTrunUI;//현재 턴
    public TextMeshProUGUI spawMonsterCountUI;//현재턴에 스폰한 몬스터수
    public TextMeshProUGUI killedMonsterCountUI;//현재 턴에 처치한 몬스터수
    public TextMeshProUGUI turnTimeUI;  //다음 턴까지 시간
    public TextMeshProUGUI repairTimerUI;//정비 시간
    public TextMeshProUGUI repairStateUI;
    public TextMeshProUGUI[] coreCount;
    public GameObject physicalAttackTrunClearannouncement;//5턴 마법을 이용한 전투시 클리어 했을경우 나오는 UI 게임오브젝트 활성화
    public GameObject endTrunGameObject;
    public GameObject[] gameOverUI; //0:playerStatusUI 1:monsterUI
    public GameObject toggle_Controls;
    public Animator gameOverAnimator;
    public GameObject warringUI;
    public enum BGMType
    {
        Battle_Normal,       // 일반 전투 (체력 100~30%)
        Battle_Critical,     // 위기 전투 (체력 30% 이하)
        Shop,                // 상점
        GameOver,            // 게임 오버 (결과창 직전)
        Title,               // 타이틀 / 로비
        Boss_Appear,         // 보스 등장
        Preparation,         // 정비 시간
        Time_AlmostOver,      // 타임 오버 임박
        GameClear,            // 게임 클리어 (결과창 직전)
    }
    public BGMType bgmType;
    public int currentBGM;
    public AudioSource backgroundSound;
    public List<AudioClip> backgroundClip;// 0 :일반 전투 (체력 100~30%)1:위기 전투 (체력 30% 이하) 2:상점 3:게임 오버 직후 4: 타이틀/로비
                                          //  5:보스 등장 시 6:정비 시간 7:타임오버 임박 8:타임 오버 효과음
    public AudioSource audioSource;
    public List<AudioClip> pointSoundClip;
    public AudioClip levelUpSound;
    public AudioSource hitAudioSource;

    [Header("난이도")]
    public int difficulty;//스폰 딜레이로 난이도 설정
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
                //    빌드시 활성화
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
        int baseLimitTime = 320; // 기본값 320
        int increasePerTurn = 10; // 턴당 증가량

        for (int i = 30; i < turnDatas.Count; i++)
        {

            // i번째 턴 limitTime = 기본값 + (턴 수 - 30) * 증가량
            turnDatas[i].limitTime = baseLimitTime + (i - 30) * increasePerTurn;

            turnDatas[i].turn = i;
            turnDatas[i].monsterCount += i;
        }

        // 현재 턴 limitTime 갱신
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
                announceText.text = "\n시간안에 턴을 종료하지 못해 보스몬스터가 생성됩니다";
                return;
            }
            spawPonsterPoint.SetActive(turnState != TurnState.Tutorials);
            GameOVERAndClear();
            if (uiBool)
            {
                Cursor.lockState = CursorLockMode.Confined; // 커서가 화면 밖으로 나가지 못하도록
                Cursor.visible = true; // 커서 보임
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked; // 커서를 화면 중앙에 고정
                Cursor.visible = false; // 커서 숨김
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

            // 레벨이 변경되었는지 확인
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

        // 마지막 값보다 크면 최종 레벨 반환
        return maxExp.Length - 1;
    }
    public void LevelUpActions()
    {
        player.saveLevel = player.level; // 레벨업했음을 저장
        audioSource.PlayOneShot(levelUpSound, Sound.Instance.playerSound);
    }
    public void True()
    {
        switch (turnState)
        {
            case TurnState.Battle:
                totalDefenseTime += Time.deltaTime;
                //제한 시간이 0이 될때 까지 시간 감소
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
                        // 현재 턴을 5로 나눈 값으로 메시지 인덱스 계산
                        //int currentMessage = Mathf.Min((currentTrun / 4) - 1, developerAnnounceText.Length - 1);
                        // 연구에 필요한 총 턴 (감소 포함 반영)
                        int totalTurn = Maxtrun - ReductionTrun;

                        // 진행된 턴
                        int progressedTurn = currentTrun;

                        // 진행률 (0 ~ 1)
                        float progress = Mathf.Clamp01((float)progressedTurn / totalTurn);

                        // 진행률을 메시지 인덱스로 변환
                        int currentMessage = Mathf.Min(
                            Mathf.FloorToInt(progress * (developerAnnounceText.Length - 1)),
                            developerAnnounceText.Length - 1);

                        // UI에 메시지 표시
                        turnTimeUI.text = developerAnnounceText[currentMessage];

                    }
                    else
                    {
                        ShildUI[0].SetActive(true);
                        ShildUI[1].SetActive(false);
                        repairTimer = 45f;
                        turnTimeUI.text =
                            "\n정비 시간입니다.\n" +
                            "코어 제어실로 이동해 코어를 활성화하세요.\n";
                    }
                }
                break;

            case TurnState.RepairTime:
                //5턴마다 클리어UI
                if (currentTrun == Maxtrun - ReductionTrun || gameClearEvent)
                {
                    if (gameClearCoroutine != null) return;
                    gameClearCoroutine = StartCoroutine(GameClearEvent());
                }

                repairTimer -= timeStopTime * Time.deltaTime;
                totalDefenseTime = 0f;
                if (repairTimer <= 0)
                {
                    // 정비 시간 끝 → 다음 턴 시작
                    eventManagerActive.SetActive(true);
                    currentTrun++;
                  
                    turnTimeUI.text = "";
                    repairTimer = GetTurnLimitTime(currentTrun);
                    turnTime = GetTurnLimitTime(currentTrun);
                    spawMonsterCount = 0;
                    killedMonsterCount = 0;
                    //활성화된 코어 횟수 7회마다 종료까지 필요한 턴이 감소함
                    for (int i = 0; i < defenseBarrier.Length; i++)
                    {
                        if (defenseBarrier[i].gameObject.activeSelf)
                        {
                            coreActiveCount++;
                        }
                    }

                    // 이번에 새로 추가된 코어 개수만큼 차이 계산
                    int newlyActivated = coreActiveCount - lastCoreActiveCount;

                    // 7개 단위마다 ReductionTurn 증가
                    if (newlyActivated > 0)
                    {
                        int prevBonus = lastCoreActiveCount / 7;
                        int newBonus = coreActiveCount / 7;

                        // 차이가 있다면 그만큼 누적
                        if (newBonus > prevBonus)
                        {
                            ReductionTrun += newBonus - prevBonus;
                        }
                    }

                    // 마지막 상태 저장
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
    //몬스터를 다 처치후 시간이 남았다면 그 시간 스킵 - 기지에서만 가능
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
            announceText.text = "\n몬스터를 전부 처치해야 합니다";
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
            repairStateUI.text = "남은 정비시간  ";
            repairTimerUI.text = repairTimer.ToString("F1");
        }
        else
        {
            repairStateUI.text = "다음 정비까지 남은시간  ";
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
    //게임 오버시 게임오버 포인트 연출
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
            // 둘 다 false일 때만 return
            goTite.SetActive(false);
            gameOverAnimator.enabled = false;
            return;
        }
        //게임 클리어,오버 일경우만
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
    // 0 :일반 전투 (체력 100~30%)1:위기 전투 (체력 30% 이하) 2:상점 3:게임 오버 직후 4: 타이틀/로비
    // 5 :보스 등장 시 6:정비 시간 7:타임오버 임박 8:타임 오버 효과음
    public void PlayBGM(BGMType bgmType)
    {

        // 중복 타입이면 무시
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
            case BGMType.Preparation://정비
                bgmNumber = 6;
                break;
            case BGMType.Time_AlmostOver://턴 종료 까지 시간이 얼마 남지 않음 30%이하임
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
        // 이미 재생 중인 곡이면 무시
        if (backgroundSound.clip == backgroundClip[bgmNumber] && backgroundSound.isPlaying)
            return;

        // 현재 진행 중인 페이드가 있으면 중단하고 새 전환 시작
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

            //  페이드 아웃 기존곡
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                backgroundSound.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
                yield return null;
            }

            // 기존곡 정지 및 새로운 곡 교체
            backgroundSound.Stop();
            backgroundSound.clip = newClip;
            backgroundSound.loop = true;
            backgroundSound.Play();

            //  페이드 인 새로운 곡
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

        //키안내
        bool nextTutorial1 = false;
        tutorialBack.SetActive(true);
        player.physicsType = true;
        int key = 0;
        Play(9);
        announceText.text = "\n[T] 키를 눌러 조작법을 확인할 수 있습니다.\n다음 튜토리얼로 진행하려면 [T] 키를 입력하세요.";
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




        //길 안내
        bool nextTutorial2 = false;
        int key2 = 0;
        announceText.text = "\n[숫자패드]를 누르면 코어 제어실까지 길안내가 표시되고 8번을 누르면 안내를 종료 합니다.\n다음 튜토리얼로 진행하려면[숫자패드] 키를 입력하세요.";
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

        //방패 사용 튜토리얼
        shildTutorials.SetActive(true);
        announceText.text = "\n방패를 사용 하려면 마우스 우클릭 을 사용 해야합니다.\n 10번 탄막을 막게 되면 다음 튜토리얼이 진행됩니다";
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

        //코어 활성화 안내
        bool nextTutorial3 = false;
        Player.Instance.data.RareCoreCount[0] = 175;
        announceText.text = "\n코어 제어실에 접근하면  \n코어 활성화 UI가 자동으로 표시됩니다.\n[코어 조각 지급 완료 - 모든 코어를 활성화 시켜야 튜토리얼이 종료됩니다]\n게임 시작시 175개의 하급 코어가 지급됨니다.";
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
        //t : 멈춤 f: 움직임 
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
        GarbageCollector.CollectIncremental(1000000); // 1MB씩 수집
        yield return new WaitForSeconds(30);
        GarbageCollectorCollectIncrementalCoroutine = null;
    }
}

[System.Serializable]
public class TurnData
{
    public int turn;           // 턴 번호
    public int monsterCount;   // 해당 턴 몬스터 수
    public float limitTime;    // 제한 시간(초)
}
