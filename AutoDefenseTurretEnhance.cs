using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/*
# ��ȭ �뷱�� ǥ (���� �ͷ� ����)
#
# �ִ��� 96 ����, �� Ȱ��ȭ ���� + �� ���� ���� ����
# ��� ������ ���� ���� �ͷ� ��ȭ, �� 7�� ���� ��ȭ
# ���� ����: �� �ͷ��� ��� 50% �̻� ���� ��, �л� ����: 7�� �յ� �й�
#
# �� �׸� �ǹ�:
# - �ִ�ġ: �ش� �׸��� ������ �� �ִ� �ִ� ��
# - ����ġ: �ʱ� ��
# - ��: �ִ�ġ - ����ġ
# - ���ô� ȿ��: ��� 1���� ��ȭ��
# - �ʿ� ����: �� / ���ô� ȿ��
# - ���� ����: ���� �ð�(��) = �ʿ� ���� / ��� ���� �ӵ�(3/��)
# - �л� ����: ���� �ð�(��) = �ʿ� ���� / ��� ���� �ӵ�(0.857/��)


�׸�, �ִ�ġ, ����ġ, ��, ���ô� ȿ��, �ʿ� ����, ���� ����(��), �л� ����(��)
���ݷ�, 60, 10, 50, 0.5, 100, 33, 116
�ӵ�, 0.1, 1, 0.9, -0.01, 90, 30, 105
���ӽð�, 600, 100, 500, 5, 100, 33, 116
ȸ���ӵ�, 180, 100, 80, 1, 80, 26.7, 93.3
�ִ� ����ġ ����, 5000, 1000, 4000, 50, 80, 26.7, 93.3
��Ÿ�, 100, 30, 70, 1, 70, 23.3, 81.7

�׸�	�ִ�ġ	����ġ	��	���ô� ȿ��	�ʿ� ����	���� ����(3 ���/��)	�л� ����(0.857 ���/��)
���ݷ�	60	10	50	+0.5	100	33��	116��
�ӵ�(����)	0.1	1	0.9	-0.01	90	30��	105��
���ӽð�	600	100	500	+5��	100	33��	116��
ȸ�� �ӵ�	180	100	80	+1	80	26.7��	93.3��
�ִ� ����ġ ����	5000	1000	4000	+50	80	26.7��	93.3��
��Ÿ� ����	100	30	70	+1	70	23.3��	81.7��
*/
public class AutoDefenseTurretEnhance : MonoBehaviour
{
    public Item item;
    public ItemData data;

    public AutoDefenseTurret autoDefenseTurret;
    [Header("��� Ȱ����")]
    public GameObject[] ModulesActive;
    public GameObject[] ModulesActiveUI;
    public GameObject autoDefenseTurretActiveUI;
    public GameObject EnhanceUI;
    [Header("��� �ټ�")]
    public int cannonModuleMaxCount, MainModuleMaxCount, HoldModuleMaxCount, FloorModuleMaxCount;//Ȱ��ȭ�� �ʿ��� ����
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
    [Header("ȿ����")]
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
    //�����ͷ� Ȱ��ȭ
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



    //�����ͷ��� ���� ����ġ ���
    public void ExpGetButton()
    {
        if (autoDefenseTurret.currentSaveExp > 0)
        {
            Player.Instance.currentExp += autoDefenseTurret.currentSaveExp;
            autoDefenseTurret.currentSaveExp = 0f;
        }
    }

    //��ȭ
    public void EnhanceButton(int index)
    {
        switch (index)
        {
            case 0:
                //���ط� ���� : �ɳ�
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
                // ���� �ӵ� ���� : �ɳ�
                if (autoDefenseTurret.TotalAttackSpeed > autoDefenseTurret.MaxattackSpeed)
                {
                    if (data.cannonModuleCount > 0)
                    {
                        audioSource.PlayOneShot(audioClips[7], Sound.Instance.uiSound);

                        // ���ݼӵ�(��Ÿ��) ����
                        autoDefenseTurret.enhanceAttackSpeed += 0.01f;

                        // UI ǥ�ô� 1 / ��Ÿ�� ���� �����ִ� �� �������� (�߻� Ƚ��/��)
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
                //ȸ�� �ӵ� ���� : Ȧ��
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
                //��Ÿ� ���� : ���� ���
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
                //�ִ� ����ġ ������ ���� : ���� ���
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
                Debug.Log("EnhanceButton(5) ȣ���");
                //���ӽð� ���� :�ٴ� ���
                if (autoDefenseTurret.TotalDuration < autoDefenseTurret.Maxduration)
                {
                    if (data.FloorModuleCount > 0)
                    {
                        audioSource.PlayOneShot(audioClips[7], Sound.Instance.uiSound);
                        autoDefenseTurret.enhanceDuration += 5;
                        EnhanceText[5].text = (autoDefenseTurret.duration + autoDefenseTurret.enhanceDuration).ToString("F0") + "��";
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
    //����
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

        //������ ����� ����
        if (!moduleDecreased)
        {
            audioSource.PlayOneShot(audioClips[4], Sound.Instance.uiSound);
        }
        else//������ ����� ����
        {
            if (autoDefenseTurret.currentDuration >= repairAmount)
            {
                //������ �������� �� ����
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
        currentDuration.fillAmount = 1 - fill; // ���� �ð� ǥ��

    }
    //������
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

    //�ʱ�ȭ
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
