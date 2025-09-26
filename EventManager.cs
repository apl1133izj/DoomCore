using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    [Header("���� ����")]
    public int currentDanger;//���� -> ī�� ��ȣ ���� ������ ���� ���̵�
    public float difficultyIncrease;//���̵�
    public int skipCount;//��ŵ Ƚ��
    public float curse;//����
    public GameObject eventPenelGameObject;

    [Header("����")]
    public int[] Re_roll;
    public TextMeshProUGUI[] Re_rollUI;

    public Image rerool1ButtonImage;
    public Image rerool2ButtonImage;
    public Image rerool3ButtonImage;
    public Material rerool1ButtonMaterial;
    public Material rerool2ButtonMaterial;
    public Material rerool3ButtonMaterial;
    [Header("ī��")]
    public GameObject[] index1card;
    public GameObject[] index2card;
    public GameObject[] index3card;
    public GameObject skipButton;

    [Header("�̺�Ʈ ����")]
    public string[] EventName;      // �̺�Ʈ �̸�
    [TextArea]
    public string[] EventContent;   // �̺�Ʈ ����

    [TextArea]
    public string[] compensationString;//���� ����


    [Header("�̺�Ʈ ����")]
    public int Level1compensation;
    public int Level2compensation;
    public int Level3compensation;

    //�ھ�1 10�� ����
    public int core1 = 10;
    //�ھ�2 10�� ����
    public int core2 = 10;
    //�ھ�3 10�� ����
    public int core3 = 10;

    //��� ��2��
    public int muduleFire = 2;
    //��� ����2��
    public int muduleMain = 2;
    //��� ��2��
    public int muduleHold = 1;
    //��� ��2��
    public int muduleFloor = 1;


    // ���ݷ� ��ȭ
    public float bonusAttackPower = 0f;           // ���� �� ���� �� �� �Ϲ� ���ݷ� ����(����)

    // ���� ������ Ȯ��
    public float bonusAttackRange = 0f;           // ���� ���� ���� * ���� + 0.1 �̷������� ���(����)

    // ũ��Ƽ�� Ȯ�� ����
    public float bonusCriticalChance = 0f;        // ũ��Ƽ�� Ȯ�� ���� + 0.1f(����)

    // ����ġ ȹ�淮 ����
    public float bonusExpGain = 0f;               // ����ġ ȹ�淮 ����(����)

    // �̵� �ӵ� ����
    public float bonusMoveSpeed = 0f;             // �÷��̾� �̵� �ӵ� ����(����)

    // ü�� ȸ�� ��ȭ
    public float bonusHealOnKill = 0f;            // ���� óġ �� ȸ���� ����(����)

    public int bonusCoreDropCount = 0;            //���� óġ�� �ִ� �ھ� �ټ� ����(����)

    // �ڵ� �ͷ� ��ȭ
    public float bonusTurretPower = 0f;           // ��ġ�� �ͷ� ���ݷ�(����)

    // ��ž ��Ÿ� ����
    public float bonusTurretRange = 0f;           // ��ġ�� ��� �ͷ� ��Ÿ� ����(����)

    // ��ž ȸ�� �ӵ� ����
    public float bonusTurretRotationSpeed = 0f;   // ��ġ�� ��� �ͷ� ȸ�� �ӵ� ����(����)

    // �ھ� ��ȣ ��ȭ
    public float bonusCoreDefense = 0f;           // �ھ� �ֺ� ���� ���� ���� ����(����)

    // ��� ��� ����
    public float bonusModuleDropRate = 0f;        // ������ ��� ��� Ȯ�� ����(����)

    // ���� ���� ��ȭ
    public float bonusCurseReduction = 0f;        // ��޸�Ʈ(����) ȿ�� ���� -> ��ŵ�� ���ַ� ����(����)



    public Item item;
    public ItemData itemData;
    public int[] EventCompensation;
    public int selectEventNumber;
    [Header("UI - ī��â")]
    public TextMeshProUGUI[] card1NameUI;
    public TextMeshProUGUI[] card2NameUI;
    public TextMeshProUGUI[] card3NameUI;
    //����
    public TextMeshProUGUI[] cardCompensation1UI;
    public TextMeshProUGUI[] cardCompensation2UI;
    public TextMeshProUGUI[] cardCompensation3UI;
    //����
    public TextMeshProUGUI curseUI;
    [Header("UI - ī�� ���� â")]
    public TextMeshProUGUI skipCountUI;
    public TextMeshProUGUI difficultyIncreaseUI;
    public TextMeshProUGUI EventNameUI;
    public TextMeshProUGUI EventContentUI;
    public TextMeshProUGUI EventCompensationNameUI;
    public TextMeshProUGUI EventCompensationUI;
    [Header("���̵� - ���ѹ�ư ���� (HDR ����)")]
    [ColorUsage(true, true)] public Color level1Color = Color.green;
    [ColorUsage(true, true)] public Color level2Color = Color.yellow;
    [ColorUsage(true, true)] public Color level3Color = Color.red;


    public List<int> saveRandomValue = new List<int>();
    private List<int> candidates = new List<int>();
    [Header("�̺�Ʈ ����")]
    public ApplyEvent applyEvent;

    [Header("�̺�Ʈ ����")]
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
        Cursor.lockState = CursorLockMode.Locked; // Ŀ���� ȭ�� �߾ӿ� ����
        Cursor.visible = false; // Ŀ�� ����
        saveRandomValue.Clear();
        candidates.Clear();
        ResetReRoll();
        InitializeUI();
        ResetCards();
    }
    #region ī�� ��ο�
    IEnumerator DrawEvent()
    {
        for (int i = 0; i < 3; i++)
        {
            CardDraw(i);
            yield return new WaitForSecondsRealtime(0.5f);
            Cursor.lockState = CursorLockMode.Confined; // Ŀ���� ȭ�� ������ ������ ���ϵ���
            Cursor.visible = true; // Ŀ�� ����

        }
    }

    public void CardDraw(int cardIndex)
    {
        // ���ѱ� ������ ��ο� �Ұ�
        if (Re_roll[cardIndex] <= 0) return;

        // ���ѱ� ����
        Re_roll[cardIndex]--;
        Re_rollUI[cardIndex].text = $"{Re_roll[cardIndex]}";

        // �ĺ� �ʱ�ȭ
        candidates.Clear();
        for (int i = 0; i < EventName.Length; i++)
        {
            // �̹� ���õ� ���� �ĺ����� ����
            if (!saveRandomValue.Contains(i))
                candidates.Add(i);
        }

        // �ĺ��� ������� ���� (���� ó��)
        if (candidates.Count == 0) return;

        // ���� ����
        int randIdx = Random.Range(0, candidates.Count);
        int selected = candidates[randIdx];
        candidates.RemoveAt(randIdx);

        // ���� ī�� ������Ʈ ��Ȱ��ȭ (���� ��)
        DeactivateCardVisual(cardIndex);

        // saveRandomValue ����
        if (saveRandomValue.Count > cardIndex)
            saveRandomValue[cardIndex] = selected; // ���� �� �����
        else
            saveRandomValue.Add(selected);        // ó�� ��ο� �ô� �߰�

        // ī�� ������Ʈ Ȱ��ȭ
        ActivateCardVisual(cardIndex, selected);

        // UI ����
        ApplyCardUI(cardIndex, selected);
        CardUIUpdate(cardIndex);

    }

    // ī�� ������Ʈ Ȱ��ȭ
    private void ActivateCardVisual(int cardIndex, int value)
    {
        // ���̵� �ܰ� ��� (�迭 ũ�⿡ ���� �ڵ� 3���)
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
        // ���̵��� ���� �� ���
        int start, end;
        if (difficultyLevel == 1) { start = 0; end = 7; }
        else if (difficultyLevel == 2) { start = 7; end = 13; }
        else { start = 13; end = 19; }

        // ���� ����Ʈ���� �ش� ������ ���Ե� ���� �ĺ��� �߸�
        List<int> candidates = list.FindAll(x => x >= start && x < end);

        if (candidates.Count == 0)
        {
            Debug.LogWarning($"���̵� {difficultyLevel} ���� �ĺ��� �� �̻� ����!");
            return -1; // ������ġ
        }

        // �ĺ� �� ���� �̱�
        int result = candidates[Random.Range(0, candidates.Count)];

        // ���� ����Ʈ������ ���� (�ߺ� ����)
        list.Remove(result);

        return result;
    }



    //ī��UI������Ʈ
    public void CardUIUpdate(int index)
    {
        int value = saveRandomValue[index];
        selectEventNumber = value;//������ �̺�Ʈ �ε��� -> ���� ��ư�� ����� �ε���
        EventNameUI.text = EventName[value];
        EventContentUI.text = EventContent[value];
        // �迭 ũ�⿡ ���� �ڵ� 3���
        int sectionSize = Mathf.CeilToInt((float)EventName.Length / 3f);
        int difficultyLevel = Mathf.Clamp((value / sectionSize) + 1, 1, 3);

        // ���̵��� ���� ���� ����
        Color chosenColor = level1Color;
        switch (difficultyLevel)
        {
            case 2: chosenColor = level2Color; break;
            case 3: chosenColor = level3Color; break;
        }

        // ���õ� ���� ����
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
            //����
            EventCompensationUI.text = compensationString[Level1compensation];
        }
        else if (index == 1)
        {
            Re_rollUI[1].color = chosenColor;
            rerool2ButtonImage.color = chosenColor;
            rerool2ButtonMaterial.SetColor("_OutlineGlowColor", chosenColor);
            //����
            EventCompensationUI.text = compensationString[Level2compensation];
        }
        else if (index == 2)
        {
            Re_rollUI[2].color = chosenColor;
            rerool3ButtonImage.color = chosenColor;
            rerool3ButtonMaterial.SetColor("_OutlineGlowColor", chosenColor);
            //����
            EventCompensationUI.text = compensationString[Level3compensation];
        }
        // ���̵� ǥ��
        difficultyIncreaseUI.text = $"LV.{difficultyLevel}";
        audioSource.PlayOneShot(audioClips[1], Sound.Instance.uiSound);
        UpdateCompensationStrings();
    }

    // ī�� ������Ʈ ��Ȱ��ȭ
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
    //ī�� UI����
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

    #region �ʱ�ȭ
    private void InitializeUI()
    {
        skipCountUI.text = skipCount.ToString();
        difficultyIncreaseUI.text = difficultyIncrease.ToString("F0");
        EventNameUI.text = "";
        EventContentUI.text = "";
        EventCompensationUI.text = "";
        skipCountUI.text = skipCount.ToString();
        curseUI.text = $"[���� ȿ��{curse}]%" + $" - ���� ���ҷ�[{bonusCurseReduction}]%";
        if (skipCount <= 2)
        {
            skipButton.SetActive(true);
        }
        else
        {
            skipButton.SetActive(false);
        }
    }
    //���� Ƚ�� ���� - ���� ����������
    private void ResetReRoll()
    {
        for (int i = 0; i < Re_roll.Length; i++)
        {
            Re_roll[i] = 2;
            Re_rollUI[i].text = Re_roll[i].ToString();
        }
    }

    //ī�� ��Ȱ��ȭ
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
    //�̺�Ʈ ����
    public void SelectEventButton(int i)
    {
        eventPenelGameObject.SetActive(false);
        skipCount = 0;
        curse = 0;
        ApplyEvents();
        Sound.Instance.audioSource.PlayOneShot(audioClips[4], Sound.Instance.uiSound);
    }

    //�̺�Ʈ ����
    public void ApplyEvents()
    {
        Manager.Instance.turnState = Manager.TurnState.Battle;
        switch (selectEventNumber)
        {
            case 0: // �͵��� �ٶ�
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.ToxicWindCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 1: // ȭ��� ��ó
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.NearPowderKegCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 2: // ���� �׸���
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.SmallShadowCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 3: // �Ŵ��� �׸���
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.GiantShadowCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 4: // ��罽�� ����
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.ChainShackleCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 5: // ������ ����
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.SuppressedLeapCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 6: // ��Ʋ�� ��Ȯ
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.TwistedHarvestCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 7: // �ẹ�� ��Ȯ
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.LurkingHarvestCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 8: // ź�� ����
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.BulletHellOverdriveCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 9: // ��ö ����
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.IronMonsterCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 10: // ����ȭ
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.BerserkCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 11: // ���� ����
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.ShopSealedCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 12: // �ӹ��� �ݳ�
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.BindingRageCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 13: // ���μ�
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.StoneCurseCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
            case 14: // �� ���� ���
                ApplyEvent.instance.StartCoroutine(ApplyEvent.instance.BloodlessHuntCoroutine());
                eventPenelGameObject.SetActive(false);
                break;
        }
    }
    //���� ����
    public void CompensationApply(int compensationIndex)
    {
        switch (compensationIndex)
        {
            // ===== ������ ���� (�տ���: skipCount ����) =====
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

            // ===== ���� ���� (������: curse ����) =====
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
        // EventContent ��� (curse ����)
        EventContent = new string[]
        {
        $"���� ȿ��: 2�ʸ��� ü�� {2 * (2f + curse / 100f):0.##} ����\n\n���� ȿ��: ��� ȸ���� +40%",
        $"���� ȿ��: �ʵ忡 �÷��̾� ���� ź���� {(5 + skipCount):0} ȸ �����˴ϴ�.\n\n���� ȿ��: �̵� �ӵ� +30%",
        $"���� ȿ��: ������ �̵��ӵ��� {100 * (1f + curse / 100f):0.##}% �����մϴ�.\n\n���� ȿ��: ���ݷ��� 20% �����մϴ�.",
        $"���� ȿ��: ��� ���� ũ�� {1.4 * (1f + curse / 100f):0.##}��\n\n���� ȿ��: ���� �̵��ӵ� -20%, �̵� �ӵ� +30%",
        $"���� ȿ��: ȸ��[E] ��Ÿ���� {15 * (1f + curse / 100f):0.##}�ʷ� �����մϴ�.\n\n���� ȿ��: �̵��ӵ� ����",
        $"���� ȿ��: ������ ����� �� �����ϴ�.\n\n���� ȿ��: ȸ��[E] ��Ÿ���� {Player.Instance.dashCooldown * (1f - curse / 100f):0.##}% �����մϴ�.",
        $"���� ȿ��: �̹� �Ͽ� ������ �����̿��� ��� ���͸� �����մϴ�.[����ȿ���� ���� ������ ����� �μ������� ���� �˴ϴ�.]\n\n���� ȿ��: �ִ� ��� ��� ������ {7- skipCount:0}���� �����մϴ�.",
        $"���� ȿ��: �̹� �Ͽ��� �Ϲ� ���Ͱ� �������� �ʽ��ϴ�. ��� ��� ����� ���ͷ� ���ϸ�, �ش� ���� {15 * (1f + curse / 100f):0} óġ �� ���� ����˴ϴ�.\n\n���� ȿ�� : [\"��� ȹ�� �� ����ġ�� �Բ� ü���� ȸ���˴ϴ�.ȸ���� ü���� �ִ� ü�� �� �ʰ��ؼ� ȸ�� �� �� �ֽ��ϴ�. (����ġ�� ������ ����)\"].",
        $"���� ȿ��: �̹� �� ���� ���� ź�� �߻� �󵵰� {30 * (1f + curse / 100f):0.##}% �����մϴ�.\n\n���� ȿ��: �÷��̾� �̵� �ӵ� ����",
        $"���� ȿ��: ������ �ִ� ü���� {40 * (1f + curse / 100f):0.##}% �����մϴ�.\n\n���� ȿ��: �ھ� ���� �߰� ���",
        $"���� ȿ��: �̹� �� ���� ��� ������ ���� �ӵ��� {30 * (1f + curse / 100f):0.##}% �����մϴ�.\n\n���� ȿ��: �÷��̾� �̵� �ӵ� ����",
        $"���� ȿ��: {1 + skipCount}�� ���� ���� �̿� ����\n\n���� ȿ��: ����ġ���� �ھ� ���� ��ӷ� ����",
        $"���� ȿ��: ����Ű�� ������ �� �����ϴ�. ���� �뽬�� ȸ�Ƿθ� �̵� ����\n\n���� ȿ��: �뽬 ���ݷ��� {70 * (1f + curse / 100f):0.##}% ����",
        $"���� ȿ��: �̹� �� ���� ���ѵ� �������� ��� �� �����ϴ�.\n\n���� ȿ��: �̵� �ӵ��� {40 * (1f - curse / 100f):0.##}% ����, �ִ� ü���� {100 * (1f - curse / 100f):0} ����",
        $"���� ȿ��: �̹� ��, ���� óġ �� ü�� ȸ���� �߻����� �ʽ��ϴ�.\n\n���� ȿ��: ���� ���� ü���� �ʴ� {1 * (1f - curse / 100f):0.##} ȸ��"
        };

        // compensationString ��� (skipCount, curse ����)
        compensationString[0] = $"�ϱ� �ھ� : {core1 + (core1 * skipCount)}��";
        compensationString[1] = $"�߱� �ھ� : {core2 + (core2 * skipCount)}��";
        compensationString[2] = $"��� �ھ� : {core3 + (core3 * skipCount)}��";
        compensationString[3] = $"�� ��� : {muduleFire + (muduleFire * skipCount)}��";
        compensationString[4] = $"���� ��� : {muduleMain + (muduleMain * skipCount)}��";
        compensationString[5] = $"Ȧ�� ��� : {muduleHold + (muduleHold * skipCount)}��";
        compensationString[6] = $"�÷ξ� ��� : {muduleFloor + (muduleFloor * skipCount)}��";

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
                7 => $"���ݷ� ��ȭ : {baseValue * (1f + curse / 100f):0.##}��ŭ ���������� ����",
                8 => $"���� ���� Ȯ�� : {baseValue * (1f + curse / 100f):0.##}��ŭ ���������� ����",
                9 => $"ũ��Ƽ�� Ȯ�� ���� : {baseValue * (1f + curse / 100f):0.##}��ŭ ���������� ����",
                10 => $"ȹ�� ����ġ ���� : {baseValue * (1f + curse / 100f):0.####}��ŭ ���������� ����",
                11 => $"�̵� �ӵ� ���� : {baseValue * (1f + curse / 100f):0.##}��ŭ ���������� ����",
                12 => $"ü�� ȸ�� ��ȭ : {baseValue * (1f + curse / 100f):0.##}��ŭ ���������� ����[����ġ�� ���� ȸ��]",
                13 => $"�ھ� ���� ��� ���� ���� : {baseValue * (1f + curse / 100f):0}�� ���� ����",
                14 => $"�ڵ� ��ž ���ݷ°�ȭ : {baseValue * (1f + curse / 100f):0.##}��ŭ ���� ��ȭ",
                15 => $"�ڵ� ��ž ��Ÿ� ���� : {baseValue * (1f + curse / 100f):0.##}��ŭ ���� ����",
                16 => $"�ڵ� ��ž ȸ�� �ӵ� ���� : {baseValue * (1f + curse / 100f):0.##}��ŭ ���� ����",
                17 => $"�ھ� ��ȣ ��ȭ : {baseValue * (1f + curse / 100f):0.##}��ŭ ���� ����",
                18 => $"��� ��� ���� : {baseValue * (1f + curse / 100f):0.###}��ŭ ���� ����",
                19 => $"���� ���� ��ȭ : {baseValue * (1f + curse / 100f):0.##}��ŭ ���� ����",
                _ => ""
            };
        }
    }

    public void CardEventClip()
    {
        audioSource.PlayOneShot(audioSource.clip, Sound.Instance.uiSound);
    }
}
