using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/*
# 강화 밸런스 표 (개별 터렛 기준)
#
# 최대턴 96 기준, 방어막 활성화 유지 + 턴 감소 전략 적용
# 모듈 수집을 통한 개별 터렛 강화, 총 7대 개별 강화
# 집중 투자: 한 터렛에 모듈 50% 이상 투자 시, 분산 투자: 7대 균등 분배
#
# 각 항목별 의미:
# - 최대치: 해당 항목이 도달할 수 있는 최대 값
# - 현재치: 초기 값
# - Δ: 최대치 - 현재치
# - 스택당 효과: 모듈 1개당 강화량
# - 필요 스택: Δ / 스택당 효과
# - 집중 투자: 도달 시간(분) = 필요 스택 / 모듈 투입 속도(3/분)
# - 분산 투자: 도달 시간(분) = 필요 스택 / 모듈 투입 속도(0.857/분)


항목, 최대치, 현재치, Δ, 스택당 효과, 필요 스택, 집중 투자(분), 분산 투자(분)
공격력, 60, 10, 50, 0.5, 100, 33, 116
속도, 0.1, 1, 0.9, -0.01, 90, 30, 105
지속시간, 600, 100, 500, 5, 100, 33, 116
회전속도, 180, 100, 80, 1, 80, 26.7, 93.3
최대 경험치 저장, 5000, 1000, 4000, 50, 80, 26.7, 93.3
사거리, 100, 30, 70, 1, 70, 23.3, 81.7

항목	최대치	현재치	Δ	스택당 효과	필요 스택	집중 투자(3 모듈/분)	분산 투자(0.857 모듈/분)
공격력	60	10	50	+0.5	100	33분	116분
속도(간격)	0.1	1	0.9	-0.01	90	30분	105분
지속시간	600	100	500	+5초	100	33분	116분
회전 속도	180	100	80	+1	80	26.7분	93.3분
최대 경험치 저장	5000	1000	4000	+50	80	26.7분	93.3분
사거리 증가	100	30	70	+1	70	23.3분	81.7분
*/
public class AutoDefenseTurretEnhance : MonoBehaviour
{
    public Item item;
    public ItemData data;

    public AutoDefenseTurret autoDefenseTurret;
    [Header("모듈 활성도")]
    public GameObject[] ModulesActive;
    public GameObject[] ModulesActiveUI;
    public GameObject autoDefenseTurretActiveUI;
    public GameObject EnhanceUI;
    [Header("모듈 겟수")]
    public int cannonModuleMaxCount, MainModuleMaxCount, HoldModuleMaxCount, FloorModuleMaxCount;//활성화시 필요한 모듈수
    public int cannonModuleCount, MainModuleCount, HoldModuleCount, FloorModuleCount;
    public bool[] activeCheck;
    [Header("UI")]
    public GameObject ui;
    public TextMeshProUGUI[] ModuleCurrentCountTextMeshProUGUI;
    public TextMeshProUGUI[] EnhanceText;
    public TextMeshProUGUI[] collectionExp;
    public Image currentDuration;
    public GameObject DescriptionText;
    public GameObject AnnouncementMessageText;
    public Coroutine AnnouncementMessageCoroutine;
    [Header("효과음")]
    AudioSource audioSource;
    public AudioClip[] audioClips;
    public GameObject playerCamera;

    public float currentCollectionExp;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        data = item.GetItemData();
        Initialization();
    }
    private void OnEnable()
    {
        Initialization();
    }
    private void Update()
    {
        if (ui.activeSelf)
        {
            CurrentDurationUpdate();
        }
    }
    //오토터렛 활성화
    public void ModulesActiveButton(int i)
    {
        if (i == 0)
        {
            if (cannonModuleCount >= cannonModuleMaxCount)
            {
                audioSource.PlayOneShot(audioClips[0]);
                ModulesActiveUI[0].SetActive(false);
                activeCheck[0] = true;
                data.cannonModuleCount -= 2;
            }
            else
            {
                audioSource.PlayOneShot(audioClips[1]);
            }
        }
        else if (i == 1)
        {
            if (MainModuleCount >= MainModuleMaxCount)
            {
                audioSource.PlayOneShot(audioClips[0]);
                ModulesActiveUI[1].SetActive(false);
                activeCheck[1] = true;
                data.MainModuleCount -= 2;
            }
            else
            {
                audioSource.PlayOneShot(audioClips[1]);
            }
        }
        else if (i == 2)
        {
            if (HoldModuleCount >= HoldModuleMaxCount)
            {
                audioSource.PlayOneShot(audioClips[0]);
                ModulesActive[1].SetActive(true);
                ModulesActiveUI[2].SetActive(false);
                activeCheck[2] = true;
                data.HoldModuleCount -= 1;
            }
            else
            {
                audioSource.PlayOneShot(audioClips[1]);
            }
        }
        else if (i == 3)
        {
            if (FloorModuleCount >= FloorModuleMaxCount)
            {
                audioSource.PlayOneShot(audioClips[0], Sound.Instance.uiSound);
                ModulesActive[2].SetActive(true);
                ModulesActiveUI[3].SetActive(false);
                activeCheck[3] = true;
                FloorModuleCount = data.FloorModuleCount -= 1;
            }
            else
            {
                audioSource.PlayOneShot(audioClips[1], Sound.Instance.uiSound);
            }
        }
        Initialization();
        if (activeCheck[0] && activeCheck[1])
        {
            ModulesActive[0].SetActive(true);
        }
        if (activeCheck[0] && activeCheck[1] && activeCheck[2] && activeCheck[3])
        {
            audioSource.PlayOneShot(audioClips[2], Sound.Instance.uiSound);
            autoDefenseTurretActiveUI.SetActive(false);
            autoDefenseTurret.enabled = true;
            EnhanceUI.SetActive(true);
        }
    }



    //오토터렛이 얻은 경험치 얻기
    public void ExpGetButton()
    {
        if (autoDefenseTurret.currentSaveExp > 0)
        {
            Player.Instance.currentExp += autoDefenseTurret.currentSaveExp;
            autoDefenseTurret.currentSaveExp = 0f;
        }
    }

    //강화
    public void EnhanceButton(int index)
    {
        switch (index)
        {
            case 0:
                //피해량 증가 : 케논
                if (autoDefenseTurret.TotalDamage < autoDefenseTurret.Maxdamage)
                {
                    if (data.cannonModuleCount > 0)
                    {
                        audioSource.PlayOneShot(audioClips[7], Sound.Instance.uiSound);
                        autoDefenseTurret.enhanceDamage += 0.5f;
                        EnhanceText[0].text = (autoDefenseTurret.damage + autoDefenseTurret.enhanceDamage).ToString("F1");
                        data.cannonModuleCount--;
                    }
                    else
                    {
                        audioSource.PlayOneShot(audioClips[8], Sound.Instance.uiSound);
                    }
                }
                else
                {
                    audioSource.PlayOneShot(audioClips[8], Sound.Instance.uiSound);
                }
                Initialization();
                break;
            case 1:
                // 공격 속도 증가 : 케논
                if (autoDefenseTurret.TotalAttackSpeed > autoDefenseTurret.MaxattackSpeed)
                {
                    if (data.cannonModuleCount > 0)
                    {
                        audioSource.PlayOneShot(audioClips[7], Sound.Instance.uiSound);

                        // 공격속도(쿨타임) 감소
                        autoDefenseTurret.enhanceAttackSpeed += 0.01f;

                        // UI 표시는 1 / 쿨타임 으로 보여주는 게 직관적임 (발사 횟수/초)
                        EnhanceText[1].text = (1 - autoDefenseTurret.enhanceAttackSpeed).ToString("F2") + " /s";

                        data.cannonModuleCount--;
                    }
                    else
                    {
                        audioSource.PlayOneShot(audioClips[8], Sound.Instance.uiSound);
                    }
                }
                Initialization();
                break;
            case 2:
                //회전 속도 증가 : 홀드
                if (autoDefenseTurret.TotalRotationSpeed < autoDefenseTurret.MaxrotationSpeed)
                {
                    if (data.HoldModuleCount > 0)
                    {
                        audioSource.PlayOneShot(audioClips[7], Sound.Instance.uiSound);
                        autoDefenseTurret.enhanceRotationSpeed += 1;
                        EnhanceText[2].text = (autoDefenseTurret.rotationSpeed + autoDefenseTurret.enhanceRotationSpeed).ToString("F0");
                        data.HoldModuleCount--;
                    }
                    else
                    {
                        audioSource.PlayOneShot(audioClips[8], Sound.Instance.uiSound);
                    }
                }
                Initialization();
                break;
            case 3:
                //사거리 증가 : 메인 모듈
                if (autoDefenseTurret.TotalRange < autoDefenseTurret.Maxrange)
                {
                    if (data.MainModuleCount > 0)
                    {
                        audioSource.PlayOneShot(audioClips[7], Sound.Instance.uiSound);
                        autoDefenseTurret.enhanceRange += 1;
                        EnhanceText[3].text = (autoDefenseTurret.range + autoDefenseTurret.enhanceRange).ToString("F0");
                        data.MainModuleCount--;
                    }
                    else
                    {
                        audioSource.PlayOneShot(audioClips[8], Sound.Instance.uiSound);
                    }
                }
                Initialization();
                break;
            case 4:
                //최대 경험치 저정량 증가 : 메인 모듈
                if (autoDefenseTurret.TotalMaxSaveExp < autoDefenseTurret.MaxmaxSaveExp)
                {
                    if (data.MainModuleCount > 0)
                    {
                        audioSource.PlayOneShot(audioClips[7], Sound.Instance.uiSound);
                        autoDefenseTurret.enhanceMaxSaveExp += 50;
                        EnhanceText[4].text = (autoDefenseTurret.maxSaveExp + autoDefenseTurret.enhanceMaxSaveExp).ToString("F0");
                        data.MainModuleCount--;
                    }
                    else
                    {
                        audioSource.PlayOneShot(audioClips[8], Sound.Instance.uiSound);
                    }

                }
                Initialization();
                break;
            case 5:
                Debug.Log("EnhanceButton(5) 호출됨");
                //지속시간 증가 :바닥 모듈
                if (autoDefenseTurret.TotalDuration < autoDefenseTurret.Maxduration)
                {
                    if (data.FloorModuleCount > 0)
                    {
                        audioSource.PlayOneShot(audioClips[7], Sound.Instance.uiSound);
                        autoDefenseTurret.enhanceDuration += 5;
                        EnhanceText[5].text = (autoDefenseTurret.duration + autoDefenseTurret.enhanceDuration).ToString("F0") + "초";
                        data.FloorModuleCount--;
                    }
                    else
                    {
                        audioSource.PlayOneShot(audioClips[8], Sound.Instance.uiSound);
                    }

                }
                Initialization();
                break;
        }

    }
    //수리
    public void RepairTurret(float repairAmount)
    {
        if (autoDefenseTurret.currentDuration < 100)
        {
            if (AnnouncementMessageCoroutine == null)
            {
                AnnouncementMessageCoroutine = StartCoroutine(AnnouncementMessage());
            }
            audioSource.PlayOneShot(audioClips[4], Sound.Instance.uiSound);
            return;
        }

        int randModule = Random.Range(0, 4);
        bool moduleDecreased = false;

        for (int i = 0; i < 4; i++)
        {
            switch ((randModule + i) % 4)
            {
                case 0:
                    if (data.HoldModuleCount > 0) { data.HoldModuleCount--; moduleDecreased = true; }
                    break;
                case 1:
                    if (data.MainModuleCount > 0) { data.MainModuleCount--; moduleDecreased = true; }
                    break;
                case 2:
                    if (data.MainModuleCount > 0) { data.MainModuleCount--; moduleDecreased = true; }
                    break;
                case 3:
                    if (data.FloorModuleCount > 0) { data.FloorModuleCount--; moduleDecreased = true; }
                    break;
            }

            if (moduleDecreased) break;
        }

        //감소할 모듈이 없음
        if (!moduleDecreased)
        {
            audioSource.PlayOneShot(audioClips[4], Sound.Instance.uiSound);
        }
        else//감소할 모듈이 있음
        {
            if (autoDefenseTurret.currentDuration >= repairAmount)
            {
                //음수로 떨어지는 거 방지
                autoDefenseTurret.currentDuration = Mathf.Max(0, autoDefenseTurret.currentDuration - repairAmount);
                audioSource.PlayOneShot(audioClips[3], Sound.Instance.uiSound);
            }
            else
            {
                audioSource.PlayOneShot(audioClips[4], Sound.Instance.uiSound);
            }

        }
    }
    public void CurrentDurationUpdate()
    {
        float fill = Mathf.Clamp01(autoDefenseTurret.currentDuration / autoDefenseTurret.TotalDuration);
        currentDuration.fillAmount = 1 - fill; // 남은 시간 표시

    }
    //나가기
    public void ExitButton()
    {
        audioSource.PlayOneShot(audioClips[6], Sound.Instance.uiSound);
        ui.SetActive(false);
        playerCamera.SetActive(true);
        Manager.Instance.uiBool = false;
        Manager.Instance.timeStopTime = 1;
    }


    IEnumerator AnnouncementMessage()
    {
        AnnouncementMessageText.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        AnnouncementMessageText.SetActive(false);
        AnnouncementMessageCoroutine = null;
    }

    public void CollectionButton()
    {
        if (currentCollectionExp > 0)
        {
            Player.Instance.currentExp += currentCollectionExp;
            currentCollectionExp = 0f;
            audioSource.PlayOneShot(audioClips[7], Sound.Instance.uiSound);
        }
        else
        {
            audioSource.PlayOneShot(audioClips[8], Sound.Instance.uiSound);
        }

        collectionExp[0].text = autoDefenseTurret.TotalMaxSaveExp.ToString("F0") + "/";
        collectionExp[1].text = currentCollectionExp.ToString("F1");
    }

    //초기화
    public void Initialization()
    {
        cannonModuleCount = data.cannonModuleCount;
        MainModuleCount = data.MainModuleCount;
        HoldModuleCount = data.HoldModuleCount;
        FloorModuleCount = data.FloorModuleCount;
        ModuleCurrentCountTextMeshProUGUI[0].text = $"[{cannonModuleCount}]";
        ModuleCurrentCountTextMeshProUGUI[1].text = $"[{MainModuleCount}]";
        ModuleCurrentCountTextMeshProUGUI[2].text = $"[{HoldModuleCount}]";
        ModuleCurrentCountTextMeshProUGUI[3].text = $"[{FloorModuleCount}]";
        collectionExp[0].text = autoDefenseTurret.TotalMaxSaveExp.ToString("F0") + "/";
        collectionExp[1].text = currentCollectionExp.ToString("F1");
    }
    private void OnTriggerStay(Collider other)
    {
        if (Manager.Instance.turnState == Manager.TurnState.RepairTime || Manager.Instance.turnState == Manager.TurnState.Tutorials)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            if (Input.GetKey(KeyCode.F) && !ui.activeSelf)
            {
                Initialization();
                audioSource.clip = audioClips[5];
                audioSource.Play();
                audioSource.volume = Sound.Instance.uiSound;
                Manager.Instance.uiBool = true;
                ui.SetActive(true);
                playerCamera.SetActive(false);
                Manager.Instance.timeStopTime = 0;
            }
            if (Input.GetKey(KeyCode.Y))
            {
                Initialization();
                DescriptionText.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Manager.Instance.turnState == Manager.TurnState.RepairTime || Manager.Instance.turnState == Manager.TurnState.Tutorials)
        {
            Initialization();
            if (!other.gameObject.CompareTag("Player")) return;
            DescriptionText.SetActive(false);
        }
    }
}
