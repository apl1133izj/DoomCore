using System.Collections;
using TMPro;
using UnityEngine;

public class ApplyEvent : MonoBehaviour
{
    public static ApplyEvent instance;

    // �̺�Ʈ ������
    public bool toxicWind;        // �͵��� �ٶ�
    public bool nearPowderKeg;    // ȭ��� ��ó
    public bool smallShadow;      // ���� �׸���
    public bool giantShadow;      // �Ŵ��� �׸���
    public bool chainShackle;     // ��罽�� ����
    public bool suppressedLeap;   // ������ ����
    public bool twistedHarvest;   // ��Ʋ�� ��Ȯ
    public bool lurkingHarvest;   // �ẹ�� ��Ȯ
    public bool bulletHellOverdrive; // ź�� ����
    public bool ironMonster;      // ��ö ����
    public bool berserk;          // ����ȭ
    public bool shopSealed;       // ���� ����
    public bool bindingRage;      // �ӹ��� �ݳ�
    public bool stoneCurse;       // ���μ�
    public bool bloodlessHunt;    // �� ���� ���


    public GameObject guidedMissile;
    private void Awake()
    {
        instance = this;
        ApplyEventValuesInitialization();
    }


    //���� �Ϸ� -> �׽�Ʈ �Ϸ�
    [Header("�͵��� �ٶ� #0")]
    public float toxicWindCurseValue;   // ü�� ���ҷ� (3�ʸ��� 1 �� ����ȿ��)
    public float toxicWindRewardValue;  // ȸ���� ���� (%)
    public IEnumerator ToxicWindCoroutine()
    {
        toxicWind = true;
        ApplyEventValues(0, EventManager.Instance.curse, EventManager.Instance.skipCount);
        while (Manager.Instance.turnState == Manager.TurnState.Battle)
        {

            Player.Instance.hp -= 2;
            yield return new WaitForSeconds(toxicWindCurseValue);
        }
        toxicWind = false;
        ApplyEventValuesInitialization();
        ApplyCompensation(EventManager.Instance.Level1compensation);
    }


    //���� �Ϸ� -> �׽�Ʈ �Ϸ�
    [Header("ȭ��� ��ó #1")]
    public int powderKegCurseValue;     // ����ź ���� (5 �� ����ȿ��)
    public float powderKegRewardValue;  // ���� ȿ�� (�߰� �ɼ�)
    public IEnumerator NearPowderKegCoroutine()
    {
        nearPowderKeg = true;
        ApplyEventValues(1, EventManager.Instance.curse, EventManager.Instance.skipCount);
        int bulletCount = 0;
        GameObject bullet = null;
        while (Manager.Instance.turnState == Manager.TurnState.Battle)
        {
            if (bulletCount < powderKegCurseValue)
            {
                bulletCount++;
                bullet = Instantiate(guidedMissile);
                bullet.transform.position = Player.Instance.transform.position + new Vector3(10, 0, 10);
            }
            yield return new WaitForSeconds(5);
        }
        ApplyEventValuesInitialization();
        nearPowderKeg = false;

        ApplyCompensation(EventManager.Instance.Level1compensation);
    }


    //�����Ϸ� -> �׽�Ʈ �Ϸ�
    [Header("���� �׸��� #2")]
    public float smallShadowCurseValue; // ���� �̵��ӵ� ���� (% �� ����ȿ��)
    public float smallShadowRewardValue;// ���ݷ� ���� (%)
    public IEnumerator SmallShadowCoroutine()
    {
        giantShadow = true;
        ApplyEventValues(2, EventManager.Instance.curse, EventManager.Instance.skipCount);
        while (Manager.Instance.turnState == Manager.TurnState.Battle)
            yield return null;
        giantShadow = false;
        ApplyEventValuesInitialization();
        ApplyCompensation(EventManager.Instance.Level1compensation);
    }


    //���� �Ϸ� -> �׽�Ʈ �Ϸ�
    [Header("�Ŵ��� �׸��� #3")]
    public float giantShadowCurseValue;          // ���� ũ�� ���� (1.4�� �� ����ȿ��)
    public float giantShadowRewardMonsterValue;  // ���� �̵��ӵ� ���� (%)
    public float giantShadowRewardPlayerValue;   // �÷��̾� �̵��ӵ� ���� (%)
    public IEnumerator GiantShadowCoroutine()
    {
        giantShadow = true;
        ApplyEventValues(3, EventManager.Instance.curse, EventManager.Instance.skipCount);
        while (Manager.Instance.turnState == Manager.TurnState.Battle)
            yield return null;
        giantShadow = false;
        ApplyEventValuesInitialization();
        ApplyCompensation(EventManager.Instance.Level1compensation);
    }


    //���� �Ϸ� -> �׽�Ʈ �Ϸ�
    [Header("��罽�� ���� #4")]
    public float chainShackleCurseValue; // ȸ�� ��Ÿ�� ���� (�� �� ����ȿ��)
    public float chainShackleRewardValue;// �̵��ӵ� ���� (%)
    public IEnumerator ChainShackleCoroutine()
    {
        chainShackle = true;
        ApplyEventValues(4, EventManager.Instance.curse, EventManager.Instance.skipCount);
        while (Manager.Instance.turnState == Manager.TurnState.Battle)
            yield return null;
        chainShackle = false;
        ApplyEventValuesInitialization();
        ApplyCompensation(EventManager.Instance.Level1compensation);
    }


    //���� �Ϸ� -> �׽�Ʈ �Ϸ�
    [Header("������ ���� #5")]
    // (���� ������ bool ó���� �� ����)
    public float suppressedLeapRewardValue; // ȸ�� ��Ÿ�� ���� (% �� -����ȿ��)
    public IEnumerator SuppressedLeapCoroutine()
    {
        suppressedLeap = true;
        ApplyEventValues(5, EventManager.Instance.curse, EventManager.Instance.skipCount);
        while (Manager.Instance.turnState == Manager.TurnState.Battle)
            yield return null;
        suppressedLeap = false;
        ApplyEventValuesInitialization();
        ApplyCompensation(EventManager.Instance.Level2compensation);
    }


    //���� �Ϸ� ->�׽�Ʈ �Ϸ�
    [Header("��Ʋ�� ��Ȯ #6")]
    // ����� �νǽ� ���� ��� ���ͷ� ����
    // (��� ��� ����ȭ �� bool)
    public int twistedHarvestRewardValue; // �ִ� ��� ��� ���� (7 �� -����ȿ��)
    public IEnumerator TwistedHarvestCoroutine()
    {
        ModuleSpawManager.moduleSpawManager.ModuleReSpaw();
        twistedHarvest = true;
        ApplyEventValues(6, EventManager.Instance.curse, EventManager.Instance.skipCount);
        while (Manager.Instance.turnState == Manager.TurnState.Battle)
            yield return null;
        twistedHarvest = false;
        ApplyEventValuesInitialization();
        ApplyCompensation(EventManager.Instance.Level2compensation);
    }



    //���� �Ϸ� -> �׽�Ʈ �Ϸ�
    [Header("�ẹ�� ��Ȯ#7")]
    //�ν��� �ʾƵ� ��� ����� ���ͷ� ����
    public int lurkingHarvestCurseValue;  // ���� óġ ��ǥ �� (30 �� ����ȿ��)
                                          // ���� ����
    public GameObject countUI;
    public TextMeshProUGUI event7ModuleCountText;
    public int maxSpaw;
    public IEnumerator LurkingHarvestCoroutine()
    {
        lurkingHarvest = true;
        ModuleSpawManager.moduleSpawManager.ModuleReSpaw();
        maxSpaw = 12;
        countUI.SetActive(true);
        ApplyEventValues(7, EventManager.Instance.curse, EventManager.Instance.skipCount);
        event7ModuleCountText.text = $"óġ Ƚ��[{lurkingHarvestCurseValue}]";
        while (Manager.Instance.turnState == Manager.TurnState.Battle)
        {
            if (lurkingHarvestCurseValue <= 0)
            {
                break;
            }
            yield return null;
        }
        countUI.SetActive(false);
        Manager.Instance.killedMonsterCount = Manager.Instance.GetTurnMonsterCount(Manager.Instance.currentTrun);
        Manager.Instance.monsterSpaw.spawCount = 0;
        lurkingHarvest = false;
        ApplyEventValuesInitialization();
        ApplyCompensation(EventManager.Instance.Level2compensation);
    }

    //���� �Ϸ� -> �׽�Ʈ �Ϸ�
    [Header("ź�� ����#8")]
    public float bulletHellOverdriveCurseValue; // �� ź�� �� ���� (% �� ����ȿ��)
    public float bulletHellOverdriveRewardValue;// �÷��̾� �̵��ӵ� ���� (%)
    public IEnumerator BulletHellOverdriveCoroutine()
    {
        bulletHellOverdrive = true;
        ApplyEventValues(8, EventManager.Instance.curse, EventManager.Instance.skipCount);
        while (Manager.Instance.turnState == Manager.TurnState.Battle)
            yield return null;
        bulletHellOverdrive = false;
        ApplyEventValuesInitialization();
        ApplyCompensation(EventManager.Instance.Level2compensation);
    }

    //���� �Ϸ� ->�׽�Ʈ �Ϸ�
    [Header("��ö ����#9")]
    public float ironMonsterCurseValue;   // ���� ü�� ���� (% �� ����ȿ��)
    public int ironMonsterRewardValue;    // �߰� ��� �ھ� ����
    public IEnumerator IronMonsterCoroutine()
    {
        ironMonster = true;
        ApplyEventValues(9, EventManager.Instance.curse, EventManager.Instance.skipCount);
        while (Manager.Instance.turnState == Manager.TurnState.Battle)
            yield return null;
        ironMonster = false;
        ApplyEventValuesInitialization();
        ApplyCompensation(EventManager.Instance.Level2compensation);
    }

    //���� �Ϸ� ->�׽�Ʈ �Ϸ�
    [Header("����ȭ#10")]
    public float berserkCurseValue;       // ���� ���ݼӵ� ���� (% �� ����ȿ��)
    public float berserkRewardValue;      // �÷��̾� �̵��ӵ� ���� (%)
    public IEnumerator BerserkCoroutine()
    {
        berserk = true;
        ApplyEventValues(10, EventManager.Instance.curse, EventManager.Instance.skipCount);
        while (Manager.Instance.turnState == Manager.TurnState.Battle)
            yield return null;
        berserk = false;
        ApplyEventValuesInitialization();
        ApplyCompensation(EventManager.Instance.Level3compensation);
    }


    //���� �Ϸ� -> �׽�Ʈ �Ϸ�
    [Header("���� ����#11")]
    public int shopSealedCurseValue;       // ���� �� �� (1�� �� ����ȿ��)
    public float shopSealedRewardExpValue; // ����ġ ������ (%)
    public float shopSealedRewardCoreValue;// �ھ� ���� ������ (%)

    public IEnumerator ShopSealedCoroutine()
    {
        shopSealed = true;
        ApplyEventValues(11, EventManager.Instance.curse, EventManager.Instance.skipCount);
        while (Manager.Instance.turnState == Manager.TurnState.Battle || shopSealedCurseValue > 0)
            yield return null;
        shopSealed = false;
        ApplyEventValuesInitialization();
        ApplyCompensation(EventManager.Instance.Level3compensation);
    }

    //�����Ϸ� -> �׽�Ʈ �Ϸ�
    // �ӹ��� �ݳ�#12
    [Header("�ӹ��� �ݳ�#12")]
    // (�̵� ������ bool ó��)
    public float bindingRageRewardValue;  // ���ݷ� ���� (% �� ����ȿ��)

    public IEnumerator BindingRageCoroutine()
    {
        bindingRage = true;
        ApplyEventValues(12, EventManager.Instance.curse, EventManager.Instance.skipCount);
        while (Manager.Instance.turnState == Manager.TurnState.Battle)
            yield return null;
        bindingRage = false;
        ApplyEventValuesInitialization();
        ApplyCompensation(EventManager.Instance.Level3compensation);
    }
    //�����Ϸ� -> �׽�Ʈ �Ϸ�
    // (��Ż �Ұ� �� bool)
    [Header("���μ�#13")]
    public float stoneCurseRewardMoveValue; // �̵��ӵ� ���� (% �� -����ȿ��)
    public float stoneCurseRewardHpValue;   // �ִ� ü�� ���� (��ġ �� -����ȿ��)
    public GameObject Event13GameObject;
    public IEnumerator StoneCurseCoroutine()
    {
        stoneCurse = true;
        Event13GameObject.SetActive(true);
        ApplyEventValues(13, EventManager.Instance.curse, EventManager.Instance.skipCount);
        while (Manager.Instance.turnState == Manager.TurnState.Battle)
            yield return null;
        stoneCurse = false;
        Event13GameObject.SetActive(false);
        ApplyEventValuesInitialization();
        ApplyCompensation(EventManager.Instance.Level3compensation);
    }
    [Header("�� ���� ���#14")]
    // (ȸ�� ���� �� bool)
    public float bloodlessHuntRewardValue;  // �ʴ� ȸ���� (1 �� -����ȿ��)
    public IEnumerator BloodlessHuntCoroutine()
    {
        bloodlessHunt = true;
        ApplyEventValues(14, EventManager.Instance.curse, EventManager.Instance.skipCount);
        while (Manager.Instance.turnState == Manager.TurnState.Battle)
        {
            if (Player.Instance.hp < Player.Instance.maxhp)
            {
                Player.Instance.hp += bloodlessHuntRewardValue;
            }
            yield return new WaitForSeconds(1);
        }

        bloodlessHunt = false;
        ApplyEventValuesInitialization();
        ApplyCompensation(EventManager.Instance.Level3compensation);
    }
    public void ApplyEventValuesInitialization()
    {
        toxicWindCurseValue = 1f;
        toxicWindRewardValue = 1f;
        powderKegCurseValue = 1;
        powderKegRewardValue = 1f;
        smallShadowCurseValue = 1f;
        smallShadowRewardValue = 1f;
        giantShadowCurseValue = 1f;
        giantShadowRewardMonsterValue = 1f;
        giantShadowRewardPlayerValue = 1f;
        chainShackleCurseValue = 1f;
        chainShackleRewardValue = 1f;
        suppressedLeapRewardValue = 1f;
        twistedHarvestRewardValue = 0;
        lurkingHarvestCurseValue = 0;
        bulletHellOverdriveCurseValue = 1f;
        bulletHellOverdriveRewardValue = 1f;
        ironMonsterCurseValue = 1f;
        ironMonsterRewardValue = 0;
        berserkCurseValue = 1f;
        berserkRewardValue = 1f;
        shopSealedCurseValue = 0;
        shopSealedRewardExpValue = 1f;
        shopSealedRewardCoreValue = 1f;
        bindingRageRewardValue = 1f;
        stoneCurseRewardMoveValue = 1f;
        stoneCurseRewardHpValue = 1f;
        bloodlessHuntRewardValue = 1f;
    }
    public void ApplyEventValues(int eventId, float curse, int skipCount)
    {
        switch (eventId)
        {
            // �͵��� �ٶ� #0
            case 0:
                toxicWindCurseValue = 2 * (1f + curse / 100f);   // 3�ʸ��� ü�� ����
                toxicWindRewardValue = 1.45f;                    // ȸ���� +40%
                break;

            // ȭ��� ��ó #1
            case 1:
                powderKegCurseValue = Mathf.RoundToInt(5 + skipCount); // ����ź ��
                powderKegRewardValue = 1.3f;                          // �̵� �ӵ� +30%
                break;

            // ���� �׸��� #2
            case 2:
                smallShadowCurseValue = 1.4f * (1f + curse / 100f); // ���� �̵��ӵ� ���� %
                smallShadowRewardValue = 1.20f;                    // ���ݷ� ���� %
                break;

            // �Ŵ��� �׸��� #3
            case 3:
                giantShadowCurseValue = 1.4f * (1f + curse / 100f);  // ���� ũ�� ����
                giantShadowRewardMonsterValue = -1.20f;            // ���� �̵��ӵ� ���� %
                giantShadowRewardPlayerValue = 1.30f;              // �÷��̾� �̵��ӵ� ���� %
                break;

            // ��罽�� ���� #4
            case 4:
                chainShackleCurseValue = 15 * (1f + curse / 100f); // ȸ�� ��Ÿ�� ����
                chainShackleRewardValue = 1.30f;                    // �̵��ӵ� ���� %
                break;

            // ������ ���� #5
            case 5:
                suppressedLeapRewardValue = Player.Instance.dashCooldown * (1f - curse / 100f); // ȸ�� ��Ÿ�� ���� %
                break;

            // ��Ʋ�� ��Ȯ #6
            case 6:
                twistedHarvestRewardValue = 7 - skipCount; // �ִ� ��� ��� ���� ����
                break;

            // �ẹ�� ��Ȯ #7
            case 7:
                lurkingHarvestCurseValue = Mathf.RoundToInt(10 * (1f + curse / 100f)); // ���� óġ ��ǥ ��
                break;

            // ź�� ���� #8
            case 8:
                bulletHellOverdriveCurseValue = 1.30f * (1f + curse / 100f); // �� ź�� �� ���� %
                bulletHellOverdriveRewardValue = 1.30f;                      // �÷��̾� �̵��ӵ� ���� %
                break;

            // ��ö ���� #9
            case 9:
                ironMonsterCurseValue = 1.4f * (1f + curse / 100f); // ���� �ִ� ü�� ���� %
                ironMonsterRewardValue = 1;                         // �ھ� �߰� ��� (����)
                break;

            // ����ȭ #10
            case 10:
                berserkCurseValue = 1.3f * (1f + curse / 100f); // ���� ���ݼӵ� ���� %
                berserkRewardValue = 1.30f;                     // �÷��̾� �̵��ӵ� ���� %
                break;

            // ���� ���� #11
            case 11:
                shopSealedCurseValue = 1 + skipCount;    // ���� �� ��
                shopSealedRewardExpValue = 1.30f;        // ����ġ ���� %
                shopSealedRewardCoreValue = 1.30f;       // �ھ� ���� ���� %
                break;

            // �ӹ��� �ݳ� #12
            case 12:
                bindingRageRewardValue = 2.0f * (1f + curse / 100f); // ���ݷ� ���� %
                break;

            // ���μ� #13
            case 13:
                stoneCurseRewardMoveValue = 1.40f * (1f - curse / 100f); // �̵��ӵ� ���� %
                stoneCurseRewardHpValue = 2.0f * (1f - curse / 100f);    // �ִ� ü�� ����
                break;

            // �� ���� ��� #14
            case 14:
                bloodlessHuntRewardValue = 1f * (1f - curse / 100f); // �ʴ� ü�� ȸ����
                break;

            default:
                Debug.LogWarning($"�� �� ���� �̺�Ʈ ID: {eventId}");
                break;
        }
    }


    public void ApplyCompensation(int Compensationindex)
    {
        EventManager.Instance.CompensationApply(Compensationindex);
    }
}
