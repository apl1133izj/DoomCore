using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    [Header("게임 상태")]
    public int currentDanger;//레벨 -> 카드 번호 위로 갈수록 높은 난이도
    public float difficultyIncrease;//난이도
    public int skipCount;//스킵 횟수
    public float curse;//저주
    public GameObject eventPenelGameObject;

    [Header("리롤")]
    public int[] Re_roll;
    public TextMeshProUGUI[] Re_rollUI;

    public Image rerool1ButtonImage;
    public Image rerool2ButtonImage;
    public Image rerool3ButtonImage;
    public Material rerool1ButtonMaterial;
    public Material rerool2ButtonMaterial;
    public Material rerool3ButtonMaterial;
    [Header("카드")]
    public GameObject[] index1card;
    public GameObject[] index2card;
    public GameObject[] index3card;
    public GameObject skipButton;

    [Header("이벤트 정보")]
    public string[] EventName;      // 이벤트 이름
    [TextArea]
    public string[] EventContent;   // 이벤트 내용

    [TextArea]
    public string[] compensationString;//보상 내용


    [Header("이벤트 보상")]
    public int Level1compensation;
    public int Level2compensation;
    public int Level3compensation;

    //코어1 10개 보상
    public int core1 = 10;
    //코어2 10개 보상
    public int core2 = 10;
    //코어3 10개 보상
    public int core3 = 10;

    //모듈 포2개
    public int muduleFire = 2;
    //모듈 메인2개
    public int muduleMain = 2;
    //모듈 포2개
    public int muduleHold = 1;
    //모듈 포2개
    public int muduleFloor = 1;


    // 공격력 강화
    public float bonusAttackPower = 0f;           // 다음 턴 동안 검 및 일반 공격력 증가(적용)

    // 광역 데미지 확대
    public float bonusAttackRange = 0f;           // 공격 범위 증가 * 사용됨 + 0.1 이런식으로 사용(적용)

    // 크리티컬 확률 증가
    public float bonusCriticalChance = 0f;        // 크리티컬 확률 증가 + 0.1f(적용)

    // 경험치 획득량 증가
    public float bonusExpGain = 0f;               // 경험치 획득량 증가(적용)

    // 이동 속도 증가
    public float bonusMoveSpeed = 0f;             // 플레이어 이동 속도 증가(적용)

    // 체력 회복 강화
    public float bonusHealOnKill = 0f;            // 몬스터 처치 시 회복량 증가(적용)

    public int bonusCoreDropCount = 0;            //몬스터 처치시 최대 코어 겟수 증가(적용)

    // 자동 터렛 강화
    public float bonusTurretPower = 0f;           // 설치된 터렛 공격력(적용)

    // 포탑 사거리 증가
    public float bonusTurretRange = 0f;           // 설치된 모든 터렛 사거리 증가(적용)

    // 포탑 회전 속도 증가
    public float bonusTurretRotationSpeed = 0f;   // 설치된 모든 터렛 회전 속도 증가(적용)

    // 코어 보호 강화
    public float bonusCoreDefense = 0f;           // 코어 주변 몬스터 감지 범위 증가(적용)

    // 모듈 드랍 증가
    public float bonusModuleDropRate = 0f;        // 모듈더미 모듈 드랍 확률 증가(적용)

    // 저주 누적 완화
    public float bonusCurseReduction = 0f;        // 디메리트(저주) 효과 감소 -> 스킵시 저주량 감소(적용)



    public Item item;
    public ItemData itemData;
    public int[] EventCompensation;
    public int selectEventNumber;
    [Header("UI - 카드창")]
    public TextMeshProUGUI[] card1NameUI;
    public TextMeshProUGUI[] card2NameUI;
    public TextMeshProUGUI[] card3NameUI;
    //내용
    public TextMeshProUGUI[] cardCompensation1UI;
    public TextMeshProUGUI[] cardCompensation2UI;
    public TextMeshProUGUI[] cardCompensation3UI;
    //저주
    public TextMeshProUGUI curseUI;
    [Header("UI - 카드 정보 창")]
    public TextMeshProUGUI skipCountUI;
    public TextMeshProUGUI difficultyIncreaseUI;
    public TextMeshProUGUI EventNameUI;
    public TextMeshProUGUI EventContentUI;
    public TextMeshProUGUI EventCompensationNameUI;
    public TextMeshProUGUI EventCompensationUI;
    [Header("난이도 - 리롤버튼 색상 (HDR 지원)")]
    [ColorUsage(true, true)] public Color level1Color = Color.green;
    [ColorUsage(true, true)] public Color level2Color = Color.yellow;
    [ColorUsage(true, true)] public Color level3Color = Color.red;


    public List<int> saveRandomValue = new List<int>();
    private List<int> candidates = new List<int>();
    [Header("이벤트 적용")]
    public ApplyEvent applyEvent;

    [Header("이벤트 사운드")]
    public AudioSource audioSource;
    public AudioClip[] audioClips;

    public GameObject eventActive;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        compensationString = new string[20];
    }
    public void EventManagerOnEnable()
    {
        InitializeUI();
        StartCoroutine(DrawEvent());
    }
    public void EventManagerOnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked; // 커서를 화면 중앙에 고정
        Cursor.visible = false; // 커서 숨김
        saveRandomValue.Clear();
        candidates.Clear();
        ResetReRoll();
        InitializeUI();
        ResetCards();
    }
    #region 카드 드로우
    IEnumerator DrawEvent()
    {
        for (int i = 0; i < 3; i++)
        {
            CardDraw(i);
            yield return new WaitForSecondsRealtime(0.5f);
            Cursor.lockState = CursorLockMode.Confined; // 커서가 화면 밖으로 나가지 못하도록
            Cursor.visible = true; // 커서 보임

        }
    }

    public void CardDraw(int cardIndex)
    {
        // 리롤권 없으면 드로우 불가
        if (Re_roll[cardIndex] <= 0) return;

        // 리롤권 차감
        Re_roll[cardIndex]--;
        Re_rollUI[cardIndex].text = $"{Re_roll[cardIndex]}";

        // 후보 초기화
        candidates.Clear();
        for (int i = 0; i < EventName.Length; i++)
        {
            // 이미 선택된 값은 후보에서 제외
            if (!saveRandomValue.Contains(i))
                candidates.Add(i);
        }

        // 후보가 비었으면 리턴 (예외 처리)
        if (candidates.Count == 0) return;

        // 랜덤 선택
        int randIdx = Random.Range(0, candidates.Count);
        int selected = candidates[randIdx];
        candidates.RemoveAt(randIdx);

        // 기존 카드 오브젝트 비활성화 (리롤 시)
        DeactivateCardVisual(cardIndex);

        // saveRandomValue 갱신
        if (saveRandomValue.Count > cardIndex)
            saveRandomValue[cardIndex] = selected; // 기존 값 덮어쓰기
        else
            saveRandomValue.Add(selected);        // 처음 드로우 시는 추가

        // 카드 오브젝트 활성화
        ActivateCardVisual(cardIndex, selected);

        // UI 적용
        ApplyCardUI(cardIndex, selected);
        CardUIUpdate(cardIndex);

    }

    // 카드 오브젝트 활성화
    private void ActivateCardVisual(int cardIndex, int value)
    {
        // 난이도 단계 계산 (배열 크기에 따라 자동 3등분)
        int sectionSize = Mathf.CeilToInt(EventName.Length / 3f);
        int difficultyLevel = Mathf.Clamp((value / sectionSize) + 1, 1, 3);
        List<int> compensationSave = new List<int>();
        for (int i = 0; i < EventName.Length; i++)
        {
            compensationSave.Add(i);
        }
        UpdateCompensationStrings();
        switch (cardIndex)
        {
            case 0:
                index1card[difficultyLevel - 1].SetActive(true);
                card1NameUI[difficultyLevel - 1].text = EventName[value];
                cardCompensation1UI[difficultyLevel - 1].text = EventContent[value];
                difficultyIncreaseUI.text = $"LV.{difficultyLevel}";
                Level1compensation = PickFromRange(compensationSave, difficultyLevel);
                break;

            case 1:
                index2card[difficultyLevel - 1].SetActive(true);
                card2NameUI[difficultyLevel - 1].text = EventName[value];
                cardCompensation2UI[difficultyLevel - 1].text = EventContent[value];
                difficultyIncreaseUI.text = $"LV.{difficultyLevel}";
                Level2compensation = PickFromRange(compensationSave, difficultyLevel);
                break;

            case 2:
                index3card[difficultyLevel - 1].SetActive(true);
                card3NameUI[difficultyLevel - 1].text = EventName[value];
                cardCompensation3UI[difficultyLevel - 1].text = EventContent[value];
                difficultyIncreaseUI.text = $"LV.{difficultyLevel}";
                Level3compensation = PickFromRange(compensationSave, difficultyLevel);
                break;

        }
    }
    private int PickFromRange(List<int> list, int difficultyLevel)
    {
        // 난이도별 구간 값 계산
        int start, end;
        if (difficultyLevel == 1) { start = 0; end = 7; }
        else if (difficultyLevel == 2) { start = 7; end = 13; }
        else { start = 13; end = 19; }

        // 현재 리스트에서 해당 구간에 포함된 값만 후보로 추림
        List<int> candidates = list.FindAll(x => x >= start && x < end);

        if (candidates.Count == 0)
        {
            Debug.LogWarning($"난이도 {difficultyLevel} 보상 후보가 더 이상 없음!");
            return -1; // 안전장치
        }

        // 후보 중 랜덤 뽑기
        int result = candidates[Random.Range(0, candidates.Count)];

        // 원본 리스트에서도 제거 (중복 방지)
        list.Remove(result);

        return result;
    }



    //카드UI업데이트
    public void CardUIUpdate(int index)
    {
        int value = saveRandomValue[index];
        selectEventNumber = value;//선택한 이벤트 인덱스 -> 선택 버튼에 적용될 인덱스
        EventNameUI.text = EventName[value];
        EventContentUI.text = EventContent[value];
        // 배열 크기에 따라 자동 3등분
        int sectionSize = Mathf.CeilToInt((float)EventName.Length / 3f);
        int difficultyLevel = Mathf.Clamp((value / sectionSize) + 1, 1, 3);

        // 난이도에 따른 색상 선택
        Color chosenColor = level1Color;
        switch (difficultyLevel)
        {
            case 2: chosenColor = level2Color; break;
            case 3: chosenColor = level3Color; break;
        }

        // 선택된 색상 적용
        EventNameUI.color = chosenColor;
        EventContentUI.color = chosenColor;
        EventCompensationNameUI.color = chosenColor;
        EventCompensationUI.color = chosenColor;
        difficultyIncreaseUI.color = chosenColor;
        if (index == 0)
        {
            Re_rollUI[0].color = chosenColor;
            rerool1ButtonImage.color = chosenColor;
            rerool1ButtonMaterial.SetColor("_OutlineGlowColor", chosenColor);
            //보상
            EventCompensationUI.text = compensationString[Level1compensation];
        }
        else if (index == 1)
        {
            Re_rollUI[1].color = chosenColor;
            rerool2ButtonImage.color = chosenColor;
            rerool2ButtonMaterial.SetColor("_OutlineGlowColor", chosenColor);
            //보상
            EventCompensationUI.text = compensationString[Level2compensation];
        }
        else if (index == 2)
        {
            Re_rollUI[2].color = chosenColor;
            rerool3ButtonImage.color = chosenColor;
            rerool3ButtonMaterial.SetColor("_OutlineGlowColor", chosenColor);
            //보상
            EventCompensationUI.text = compensationString[Level3compensation];
        }
        // 난이도 표시
        difficultyIncreaseUI.text = $"LV.{difficultyLevel}";
        audioSource.PlayOneShot(audioClips[1], Sound.Instance.uiSound);
        UpdateCompensationStrings();
    }

    // 카드 오브젝트 비활성화
    private void DeactivateCardVisual(int cardIndex)
    {
        switch (cardIndex)
        {
            case 0:
                foreach (var c in index1card) c.SetActive(false);
                break;
            case 1:
                foreach (var c in index2card) c.SetActive(false);
                break;
            case 2:
                foreach (var c in index3card) c.SetActive(false);
                break;
        }
    }
    //카드 UI적용
    private void ApplyCardUI(int cardIndex, int selected)
    {
        skipCountUI.text = skipCount.ToString();
        difficultyIncreaseUI.text = difficultyIncrease.ToString("F0");
        EventNameUI.text = EventName[selected];
        EventContentUI.text = EventContent[selected];

        if (EventCompensationUI != null && EventCompensation.Length > selected)
            EventCompensationUI.text = EventCompensation[selected].ToString();
    }
    #endregion

    #region 초기화
    private void InitializeUI()
    {
        skipCountUI.text = skipCount.ToString();
        difficultyIncreaseUI.text = difficultyIncrease.ToString("F0");
        EventNameUI.text = "";
        EventContentUI.text = "";
        EventCompensationUI.text = "";
        skipCountUI.text = skipCount.ToString();
        curseUI.text = $"[저주 효과{curse}]%" + $" - 저주 감소량[{bonusCurseReduction}]%";
        if (skipCount <= 2)
        {
            skipButton.SetActive(true);
        }
        else
        {
            skipButton.SetActive(false);
        }
    }
    //리롤 횟수 리셋 - 턴이 끝날때마다
    private void ResetReRoll()
    {
        for (int i = 0; i < Re_roll.Length; i++)
        {
            Re_roll[i] = 2;
            Re_rollUI[i].text = Re_roll[i].ToString();
        }
    }

    //카드 비활성화
    private void ResetCards()
    {
        foreach (var card in index1card) card.SetActive(false);
        foreach (var card in index2card) card.SetActive(false);
        foreach (var card in index3card) card.SetActive(false);
    }
    #endregion

    public void ReRollSound()
    {
        audioSource.PlayOneShot(audioClips[2], Sound.Instance.uiSound);
    }
    public void SkipButton()
    {
        if (skipCount <= 2)
        {
            skipButton.SetActive(true);
            skipCount += 1;
            curse = (15 + bonusCurseReduction) * skipCount;
            eventPenelGameObject.SetActive(false);
            Sound.Instance.audioSource.PlayOneShot(audioClips[3], Sound.Instance.uiSound);
        }
    }
    //이벤트 선택
    public void SelectEventButton(int i)
    {
        eventPenelGameObject.SetActive(false);
        skipCount = 0;
        curse = 0;
        ApplyEvents();
        Sound.Instance.audioSource.PlayOneShot(audioClips[4], Sound.Instance.uiSound);
    }

    //이벤트 적용
    public void ApplyEvents()
    {
        Manager.Instance.turnState = Manager.TurnState.Battle;
        switch (selectEventNumber)
        {
            case 0: // 맹독의 바람
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.ToxicWindCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 1: // 화약고 근처
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.NearPowderKegCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 2: // 작은 그림자
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.SmallShadowCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 3: // 거대한 그림자
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.GiantShadowCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 4: // 쇠사슬의 족쇄
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.ChainShackleCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 5: // 억제된 도약
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.SuppressedLeapCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 6: // 뒤틀린 수확
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.TwistedHarvestCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 7: // 잠복의 수확
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.LurkingHarvestCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 8: // 탄막 폭주
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.BulletHellOverdriveCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 9: // 강철 몬스터
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.IronMonsterCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 10: // 광폭화
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.BerserkCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 11: // 상점 봉쇄
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.ShopSealedCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 12: // 속박의 격노
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.BindingRageCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 13: // 망부석
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.StoneCurseCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 14: // 피 없는 사냥
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.BloodlessHuntCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
        }
    }
    //보상 적용
    public void CompensationApply(int compensationIndex)
    {
        switch (compensationIndex)
        {
            // ===== 아이템 보상 (합연산: skipCount 적용) =====
            case 0:
                Player.Instance.data.RareCoreCount[0] += core1 + (core1 * skipCount);
                break;
            case 1:
                Player.Instance.data.RareCoreCount[1] += core2 + (core2 * skipCount);
                break;
            case 2:
                Player.Instance.data.RareCoreCount[2] += core3 + (core3 * skipCount);
                break;
            case 3:
                Player.Instance.data.cannonModuleCount += muduleFire + (muduleFire * skipCount);
                break;
            case 4:
                Player.Instance.data.MainModuleCount += muduleMain + (muduleMain * skipCount);
                break;
            case 5:
                Player.Instance.data.HoldModuleCount += muduleHold + (muduleHold * skipCount);
                break;
            case 6:
                Player.Instance.data.FloorModuleCount += muduleFloor + (muduleFloor * skipCount);
                break;

            // ===== 스탯 보상 (곱연산: curse 적용) =====
            case 7:
                bonusAttackPower += 2f * (1f + curse / 100f);
                break;
            case 8:
                bonusAttackRange += 0.25f * (1f + curse / 100f);
                break;
            case 9:
                bonusCriticalChance += 0.25f * (1f + curse / 100f);
                break;
            case 10:
                bonusExpGain += 0.0015f * (1f + curse / 100f);
                break;
            case 11:
                bonusMoveSpeed += 0.2f * (1f + curse / 100f);
                break;
            case 12:
                bonusHealOnKill += 0.05f * (1f + curse / 100f);
                break;
            case 13:
                bonusCoreDropCount += (int)(1 * (1f + curse / 100f));
                break;
            case 14:
                bonusTurretPower += 2f * (1f + curse / 100f);
                break;
            case 15:
                bonusTurretRange += 3f * (1f + curse / 100f);
                break;
            case 16:
                bonusTurretRotationSpeed += 2f * (1f + curse / 100f);
                break;
            case 17:
                bonusCoreDefense += 5f * (1f + curse / 100f);
                break;
            case 18:
                bonusModuleDropRate += 0.01f * (1f + curse / 100f);
                break;
            case 19:
                bonusCurseReduction += 0.5f * (1f + curse / 100f);
                break;

        }
    }
    public void UpdateCompensationStrings()
    {
        // EventContent 계산 (curse 적용)
        EventContent = new string[]
        {
        $"제한 효과: 2초마다 체력 {2 * (2f + curse / 100f):0.##} 감소\n\n보상 효과: 대신 회복량 +40%",
        $"제한 효과: 필드에 플레이어 유도 탄막이 {(5 + skipCount):0} 회 생성됩니다.\n\n보상 효과: 이동 속도 +30%",
        $"제한 효과: 몬스터의 이동속도가 {100 * (1f + curse / 100f):0.##}% 증가합니다.\n\n보상 효과: 공격력이 20% 증가합니다.",
        $"제한 효과: 모든 몬스터 크기 {1.4 * (1f + curse / 100f):0.##}배\n\n보상 효과: 몬스터 이동속도 -20%, 이동 속도 +30%",
        $"제한 효과: 회피[E] 쿨타임이 {15 * (1f + curse / 100f):0.##}초로 증가합니다.\n\n보상 효과: 이동속도 증가",
        $"제한 효과: 점프를 사용할 수 없습니다.\n\n보상 효과: 회피[E] 쿨타임이 {Player.Instance.dashCooldown * (1f - curse / 100f):0.##}% 감소합니다.",
        $"제한 효과: 이번 턴에 생성된 모듈더미에는 모듈 몬스터만 생성합니다.[저주효과로 변한 묘듈은 모듈을 부술때까지 유지 됩니다.]\n\n보상 효과: 최대 모듈 드롭 개수가 {7- skipCount:0}개로 증가합니다.",
        $"제한 효과: 이번 턴에는 일반 몬스터가 등장하지 않습니다. 대신 모든 모듈이 몬스터로 변하며, 해당 몬스터 {15 * (1f + curse / 100f):0} 처치 시 턴이 종료됩니다.\n\n보상 효과 : [\"모듈 획득 시 경험치와 함께 체력이 회복됩니다.회복된 체력은 최대 체력 을 초과해서 회복 할 수 있습니다. (경험치와 동일한 역할)\"].",
        $"제한 효과: 이번 턴 동안 적의 탄막 발사 빈도가 {30 * (1f + curse / 100f):0.##}% 증가합니다.\n\n보상 효과: 플레이어 이동 속도 증가",
        $"제한 효과: 몬스터의 최대 체력이 {40 * (1f + curse / 100f):0.##}% 증가합니다.\n\n보상 효과: 코어 조각 추가 드롭",
        $"제한 효과: 이번 턴 동안 모든 몬스터의 공격 속도가 {30 * (1f + curse / 100f):0.##}% 증가합니다.\n\n보상 효과: 플레이어 이동 속도 증가",
        $"제한 효과: {1 + skipCount}턴 동안 상점 이용 제한\n\n보상 효과: 경험치량과 코어 조각 드롭량 증가",
        $"제한 효과: 방향키로 움직일 수 없습니다. 공격 대쉬와 회피로만 이동 가능\n\n보상 효과: 대쉬 공격력이 {70 * (1f + curse / 100f):0.##}% 증가",
        $"제한 효과: 이번 턴 동안 제한된 영역에서 벗어날 수 없습니다.\n\n보상 효과: 이동 속도가 {40 * (1f - curse / 100f):0.##}% 증가, 최대 체력이 {100 * (1f - curse / 100f):0} 증가",
        $"제한 효과: 이번 턴, 몬스터 처치 시 체력 회복이 발생하지 않습니다.\n\n보상 효과: 전투 내내 체력이 초당 {1 * (1f - curse / 100f):0.##} 회복"
        };

        // compensationString 계산 (skipCount, curse 적용)
        compensationString[0] = $"하급 코어 : {core1 + (core1 * skipCount)}개";
        compensationString[1] = $"중급 코어 : {core2 + (core2 * skipCount)}개";
        compensationString[2] = $"고급 코어 : {core3 + (core3 * skipCount)}개";
        compensationString[3] = $"포 모듈 : {muduleFire + (muduleFire * skipCount)}개";
        compensationString[4] = $"메인 모듈 : {muduleMain + (muduleMain * skipCount)}개";
        compensationString[5] = $"홀드 모듈 : {muduleHold + (muduleHold * skipCount)}개";
        compensationString[6] = $"플로어 모듈 : {muduleFloor + (muduleFloor * skipCount)}개";

        for (int i = 7; i <= 19; i++)
        {
            float baseValue = i switch
            {
                7 => 2f,
                8 => 0.25f,
                9 => 0.25f,
                10 => 0.0015f,
                11 => 0.2f,
                12 => 0.05f,
                13 => 1f,
                14 => 2f,
                15 => 3f,
                16 => 2f,
                17 => 5f,
                18 => 0.01f,
                19 => 0.5f,
                _ => 0f
            };
            compensationString[i] = i switch
            {
                7 => $"공격력 강화 : {baseValue * (1f + curse / 100f):0.##}만큼 영구적으로 증가",
                8 => $"공격 범위 확대 : {baseValue * (1f + curse / 100f):0.##}만큼 영구적으로 증가",
                9 => $"크리티컬 확률 증가 : {baseValue * (1f + curse / 100f):0.##}만큼 영구적으로 증가",
                10 => $"획득 경험치 증가 : {baseValue * (1f + curse / 100f):0.####}만큼 영구적으로 증가",
                11 => $"이동 속도 증가 : {baseValue * (1f + curse / 100f):0.##}만큼 영구적으로 증가",
                12 => $"체력 회복 강화 : {baseValue * (1f + curse / 100f):0.##}만큼 영구적으로 증가[경험치로 인한 회복]",
                13 => $"코어 조각 드랍 수량 증가 : {baseValue * (1f + curse / 100f):0}개 영구 증가",
                14 => $"자동 포탑 공격력강화 : {baseValue * (1f + curse / 100f):0.##}만큼 영구 강화",
                15 => $"자동 포탑 사거리 증가 : {baseValue * (1f + curse / 100f):0.##}만큼 영구 증가",
                16 => $"자동 포탑 회전 속도 증가 : {baseValue * (1f + curse / 100f):0.##}만큼 영구 증가",
                17 => $"코어 보호 강화 : {baseValue * (1f + curse / 100f):0.##}만큼 영구 증가",
                18 => $"모듈 드랍 증가 : {baseValue * (1f + curse / 100f):0.###}만큼 영구 증가",
                19 => $"저주 누적 완화 : {baseValue * (1f + curse / 100f):0.##}만큼 영구 증가",
                _ => ""
            };
        }
    }

    public void CardEventClip()
    {
        audioSource.PlayOneShot(audioSource.clip, Sound.Instance.uiSound);
    }
}
