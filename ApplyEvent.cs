using System.Collections;
using TMPro;
using UnityEngine;

public class ApplyEvent : MonoBehaviour
{
    public static ApplyEvent instance;

    // 이벤트 변수들
    public bool toxicWind;        // 맹독의 바람
    public bool nearPowderKeg;    // 화약고 근처
    public bool smallShadow;      // 작은 그림자
    public bool giantShadow;      // 거대한 그림자
    public bool chainShackle;     // 쇠사슬의 족쇄
    public bool suppressedLeap;   // 억제된 도약
    public bool twistedHarvest;   // 뒤틀린 수확
    public bool lurkingHarvest;   // 잠복의 수확
    public bool bulletHellOverdrive; // 탄막 폭주
    public bool ironMonster;      // 강철 몬스터
    public bool berserk;          // 광폭화
    public bool shopSealed;       // 상점 봉쇄
    public bool bindingRage;      // 속박의 격노
    public bool stoneCurse;       // 망부석
    public bool bloodlessHunt;    // 피 없는 사냥


    public GameObject guidedMissile;
    private void Awake()
    {
        instance = this;
        ApplyEventValuesInitialization();
    }


    //구현 완료 -> 테스트 완료
    [Header("맹독의 바람 #0")]
    public float toxicWindCurseValue;   // 체력 감소량 (3초마다 1 × 저주효과)
    public float toxicWindRewardValue;  // 회복량 증가 (%)
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


    //구현 완료 -> 테스트 완료
    [Header("화약고 근처 #1")]
    public int powderKegCurseValue;     // 유도탄 개수 (5 × 저주효과)
    public float powderKegRewardValue;  // 보상 효과 (추가 옵션)
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


    //구현완료 -> 테스트 완료
    [Header("작은 그림자 #2")]
    public float smallShadowCurseValue; // 몬스터 이동속도 증가 (% × 저주효과)
    public float smallShadowRewardValue;// 공격력 증가 (%)
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


    //구현 완료 -> 테스트 완료
    [Header("거대한 그림자 #3")]
    public float giantShadowCurseValue;          // 몬스터 크기 배율 (1.4배 × 저주효과)
    public float giantShadowRewardMonsterValue;  // 몬스터 이동속도 감소 (%)
    public float giantShadowRewardPlayerValue;   // 플레이어 이동속도 증가 (%)
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


    //구현 완료 -> 테스트 완료
    [Header("쇠사슬의 족쇄 #4")]
    public float chainShackleCurseValue; // 회피 쿨타임 증가 (초 × 저주효과)
    public float chainShackleRewardValue;// 이동속도 증가 (%)
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


    //구현 완료 -> 테스트 완료
    [Header("억제된 도약 #5")]
    // (점프 제한은 bool 처리라 값 없음)
    public float suppressedLeapRewardValue; // 회피 쿨타임 감소 (% × -저주효과)
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


    //구현 완료 ->테스트 완료
    [Header("뒤틀린 수확 #6")]
    // 모듈을 부실시 전부 모듈 몬스터로 생성
    // (모든 모듈 몬스터화 → bool)
    public int twistedHarvestRewardValue; // 최대 모듈 드롭 개수 (7 × -저주효과)
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



    //구현 완료 -> 테스트 완료
    [Header("잠복의 수확#7")]
    //부시지 않아도 모든 모듈이 몬스터로 변함
    public int lurkingHarvestCurseValue;  // 몬스터 처치 목표 수 (30 × 저주효과)
                                          // 보상 없음
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
        event7ModuleCountText.text = $"처치 횟수[{lurkingHarvestCurseValue}]";
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

    //구현 완료 -> 테스트 완료
    [Header("탄막 폭주#8")]
    public float bulletHellOverdriveCurseValue; // 적 탄막 빈도 증가 (% × 저주효과)
    public float bulletHellOverdriveRewardValue;// 플레이어 이동속도 증가 (%)
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

    //구현 완료 ->테스트 완료
    [Header("강철 몬스터#9")]
    public float ironMonsterCurseValue;   // 몬스터 체력 증가 (% × 저주효과)
    public int ironMonsterRewardValue;    // 추가 드롭 코어 개수
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

    //구현 완료 ->테스트 완료
    [Header("광폭화#10")]
    public float berserkCurseValue;       // 몬스터 공격속도 증가 (% × 저주효과)
    public float berserkRewardValue;      // 플레이어 이동속도 증가 (%)
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


    //구현 완료 -> 테스트 완료
    [Header("상점 봉쇄#11")]
    public int shopSealedCurseValue;       // 봉쇄 턴 수 (1턴 × 저주효과)
    public float shopSealedRewardExpValue; // 경험치 증가량 (%)
    public float shopSealedRewardCoreValue;// 코어 조각 증가량 (%)

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

    //구현완료 -> 테스트 완료
    // 속박의 격노#12
    [Header("속박의 격노#12")]
    // (이동 제한은 bool 처리)
    public float bindingRageRewardValue;  // 공격력 증가 (% × 저주효과)

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
    //구현완료 -> 테스트 완료
    // (이탈 불가 → bool)
    [Header("망부석#13")]
    public float stoneCurseRewardMoveValue; // 이동속도 증가 (% × -저주효과)
    public float stoneCurseRewardHpValue;   // 최대 체력 증가 (수치 × -저주효과)
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
    [Header("피 없는 사냥#14")]
    // (회복 차단 → bool)
    public float bloodlessHuntRewardValue;  // 초당 회복량 (1 × -저주효과)
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
            // 맹독의 바람 #0
            case 0:
                toxicWindCurseValue = 2 * (1f + curse / 100f);   // 3초마다 체력 감소
                toxicWindRewardValue = 1.45f;                    // 회복량 +40%
                break;

            // 화약고 근처 #1
            case 1:
                powderKegCurseValue = Mathf.RoundToInt(5 + skipCount); // 유도탄 수
                powderKegRewardValue = 1.3f;                          // 이동 속도 +30%
                break;

            // 작은 그림자 #2
            case 2:
                smallShadowCurseValue = 1.4f * (1f + curse / 100f); // 몬스터 이동속도 증가 %
                smallShadowRewardValue = 1.20f;                    // 공격력 증가 %
                break;

            // 거대한 그림자 #3
            case 3:
                giantShadowCurseValue = 1.4f * (1f + curse / 100f);  // 몬스터 크기 배율
                giantShadowRewardMonsterValue = -1.20f;            // 몬스터 이동속도 감소 %
                giantShadowRewardPlayerValue = 1.30f;              // 플레이어 이동속도 증가 %
                break;

            // 쇠사슬의 족쇄 #4
            case 4:
                chainShackleCurseValue = 15 * (1f + curse / 100f); // 회피 쿨타임 증가
                chainShackleRewardValue = 1.30f;                    // 이동속도 증가 %
                break;

            // 억제된 도약 #5
            case 5:
                suppressedLeapRewardValue = Player.Instance.dashCooldown * (1f - curse / 100f); // 회피 쿨타임 감소 %
                break;

            // 뒤틀린 수확 #6
            case 6:
                twistedHarvestRewardValue = 7 - skipCount; // 최대 모듈 드롭 개수 증가
                break;

            // 잠복의 수확 #7
            case 7:
                lurkingHarvestCurseValue = Mathf.RoundToInt(10 * (1f + curse / 100f)); // 몬스터 처치 목표 수
                break;

            // 탄막 폭주 #8
            case 8:
                bulletHellOverdriveCurseValue = 1.30f * (1f + curse / 100f); // 적 탄막 빈도 증가 %
                bulletHellOverdriveRewardValue = 1.30f;                      // 플레이어 이동속도 증가 %
                break;

            // 강철 몬스터 #9
            case 9:
                ironMonsterCurseValue = 1.4f * (1f + curse / 100f); // 몬스터 최대 체력 증가 %
                ironMonsterRewardValue = 1;                         // 코어 추가 드롭 (개수)
                break;

            // 광폭화 #10
            case 10:
                berserkCurseValue = 1.3f * (1f + curse / 100f); // 몬스터 공격속도 증가 %
                berserkRewardValue = 1.30f;                     // 플레이어 이동속도 증가 %
                break;

            // 상점 봉쇄 #11
            case 11:
                shopSealedCurseValue = 1 + skipCount;    // 봉쇄 턴 수
                shopSealedRewardExpValue = 1.30f;        // 경험치 증가 %
                shopSealedRewardCoreValue = 1.30f;       // 코어 조각 증가 %
                break;

            // 속박의 격노 #12
            case 12:
                bindingRageRewardValue = 2.0f * (1f + curse / 100f); // 공격력 증가 %
                break;

            // 망부석 #13
            case 13:
                stoneCurseRewardMoveValue = 1.40f * (1f - curse / 100f); // 이동속도 증가 %
                stoneCurseRewardHpValue = 2.0f * (1f - curse / 100f);    // 최대 체력 증가
                break;

            // 피 없는 사냥 #14
            case 14:
                bloodlessHuntRewardValue = 1f * (1f - curse / 100f); // 초당 체력 회복량
                break;

            default:
                Debug.LogWarning($"알 수 없는 이벤트 ID: {eventId}");
                break;
        }
    }


    public void ApplyCompensation(int Compensationindex)
    {
        EventManager.Instance.CompensationApply(Compensationindex);
    }
}
