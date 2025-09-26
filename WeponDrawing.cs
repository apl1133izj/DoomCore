using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class WeponDrawing : MonoBehaviour
{
    public GameObject enhanceWindow;

    public Sword sword;
    public SwordManager swordManager;


    public GameObject DirectoGameObject;//���� ���̴� �θ�
    public GameObject drawingDirectorSwordGameObject;//���� �˸�糢�� ���� ����
    public Image[] DirectorImage;//����

    public GameObject[] DrawingDirectorSword;//���⿡ ����Ѵ� �˵�
    public Coroutine DrawingDirectorCoroutine;

    [Header("����")]
    int page;
    public GameObject[] PageUI;
    public GameObject[] DrawViewSword;//�̱⿡�� ���°�
    public GameObject[] lockSword;//��� �ִ� ��
    public GameObject[] ViewWepon;//���� ���� �̸�����
    public GameObject SPACUI;


    public AudioSource audioSource;
    public AudioClip[] clip;
    public int drawCount = 0; // �� �̱� Ƚ��
    public string[] Rares;

    public TextMeshProUGUI drawCountText;
    public TextMeshProUGUI weponDamageText;
    public TextMeshProUGUI weponAttackSpeedText;
    public TextMeshProUGUI weponCriticalText;
    public TextMeshProUGUI weponCriticalDamageText;
    public TextMeshProUGUI RareText;
    
    public void SelectSword(int index)
    {
      
        if (lockSword[index].activeSelf)//��� ��
        {
            audioSource.PlayOneShot(clip[3], Sound.Instance.uiSound);
        }
        else if (!lockSword[index].activeSelf)//������� �� ��
        {
            for (int i = 0; i < ViewWepon.Length; i++)
            {
                if (i == index)
                {
                    ViewWepon[i].SetActive(true);
                }
                else
                {
                    ViewWepon[i].SetActive(false);
                }
            }
            UIUpdate(index);
            int grade;
            if (index <= 11)
                grade = 0; // �Ϲ�
            else if (index <= 24)
                grade = 1; // ���
            else if (index <= 35)
                grade = 2; // ����
            else
                grade = 3; // ����
            RareText.text = Rares[grade];            // �迭���� ��� ǥ��

            audioSource.PlayOneShot(clip[2], Sound.Instance.uiSound);
            sword.swordIndex = index;//������ �� ����
        }
    }


    public void Draw()
    {
        if (DrawingDirectorCoroutine == null)
        {
            float drawCostExp = 15f; // �� �� �̱� �� ����ġ �Ҹ�
            if (Player.Instance.currentExp >= drawCostExp)
            {
                Player.Instance.currentExp -= drawCostExp;
                audioSource.PlayOneShot(clip[0], Sound.Instance.uiSound);
                DrawingDirectorCoroutine = StartCoroutine(DrawingDirector1());
            }
            else
            {
                audioSource.PlayOneShot(clip[8], Sound.Instance.uiSound);
                Debug.Log("����ġ ����! ���͸� �� ��������.");
            }
        }
        
    }
    //�̱� 1�� ����
    IEnumerator DrawingDirector1()
    {
        DirectorInitialization();
        drawingDirectorSwordGameObject.SetActive(true);
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < DrawingDirectorSword.Length; j++)
            {
                DrawingDirectorSword[j].SetActive(true);

                if (i >= 2 && j == 0)
                {
                    audioSource.PlayOneShot(clip[1], Sound.Instance.uiSound);
                }
                else
                {
                    audioSource.PlayOneShot(clip[1], Sound.Instance.uiSound);
                }

                float waitTime = Mathf.Max(0.05f, 0.2f - i * 0.1f);
                yield return new WaitForSeconds(waitTime);

                DrawingDirectorSword[j].SetActive(false);
            }
            yield return null;
        }
        audioSource.PlayOneShot(clip[4], Sound.Instance.uiSound);

        drawingDirectorSwordGameObject.SetActive(false);
        int swordIndex = RandomSword();                // ���� �� �ε���
        DirectorImage[0].enabled = false;
        DirectorImage[1].enabled = true;
        DrawViewSword[swordIndex].SetActive(true);
        RectTransform rt = DirectorImage[1].rectTransform;
        while (rt.sizeDelta.y > 0f)
        {
            Vector2 size = rt.sizeDelta;       // ���� sizeDelta ��������
            size.y -= Time.deltaTime * 125;   // 1�ʿ� 20�� �پ��
            size.y = Mathf.Max(size.y, 0f);   // 0 �Ʒ��� �� ��������
            rt.sizeDelta = size;               // �ٽ� �Ҵ�
            yield return null;                 // ���� �����ӱ��� ���
        }
        SPACUI.SetActive(true);
        DirectoGameObject.SetActive(false);
        audioSource.PlayOneShot(clip[5], Sound.Instance.uiSound);
        weponDamageText.text = swordManager.weponDamage[swordIndex].ToString("F2");//������
        yield return new WaitForSeconds(0.25f);
        audioSource.PlayOneShot(clip[5], Sound.Instance.uiSound);
        weponAttackSpeedText.text = swordManager.weponAttackSpeed[swordIndex].ToString("F2");//���ݼӵ�
        yield return new WaitForSeconds(0.25f);
        audioSource.PlayOneShot(clip[5], Sound.Instance.uiSound);
        weponCriticalText.text = swordManager.weponCritical[swordIndex].ToString("F2");//ũ��Ƽ�� Ȯ��
        yield return new WaitForSeconds(0.25f);
        audioSource.PlayOneShot(clip[5], Sound.Instance.uiSound);
        weponCriticalDamageText.text = swordManager.weponCriticalDamage[swordIndex].ToString("F2");//ũ��Ƽ�� ������
        int grade;
        if (swordIndex <= 11)
            grade = 0; // �Ϲ�
        else if (swordIndex <= 24)
            grade = 1; // ���
        else if (swordIndex <= 35)
            grade = 2; // ����
        else
            grade = 3; // ����
        RareText.text = Rares[grade];            // �迭���� ��� ǥ��
                                                 // �ߺ� üũ + ����ġ ��ȯ
        drawCountText.text = drawCount.ToString();//���� Ƚ��
        //�ߺ��� ��� ����ġ�� ��ȯ��
        if (!lockSword[swordIndex].activeSelf)
        {
            float expGain = Player.Instance.currentExp * 1.5f;
            Player.Instance.currentExp += expGain;
        }
        else
        {
            lockSword[swordIndex].SetActive(false); // �ű� ���� ��� ����
        }

        DrawingDirectorCoroutine = null;
    }


    private void DirectorInitialization()
    {
        SPACUI.SetActive(false);
        DirectoGameObject.SetActive(true);
        DirectorImage[0].enabled = true;
        DirectorImage[1].enabled = false;
        RectTransform rt = DirectorImage[1].rectTransform;
        Vector2 size = rt.sizeDelta;       // ���� sizeDelta ��������
        size.y = 234f;
    }

    /// <summary>
    /// p > 1 �� 44 �̱� �� ����
    //p = 1 �� ��� ��� ���� Ȯ��(2.22%)
    //p< 1 �� 44 �̱� �� ����
    /// </summary>
    /// <returns></returns>
    int RandomSword()
    {
        drawCount++;

        // 50ȸ ���� ����
        if (drawCount % 50 == 0)
            return Random.Range(36, 45);

        float r = Random.value;
        r = Mathf.Pow(r, 5.3f); // ���� Ȯ�� �� 5%
        int index = Mathf.FloorToInt(r * (lockSword.Length - 1)) + 1; // 1 ~ 44

        // �����ϰ� ���� ����
        index = Mathf.Clamp(index, 1, lockSword.Length - 1);

        return index;
    }




    public void UIUpdate(int index)
    {
        SPACUI.SetActive(true);

        weponDamageText.text = swordManager.weponDamage[index].ToString("F2");//������
        weponAttackSpeedText.text = swordManager.weponAttackSpeed[index].ToString("F2");//���ݼӵ�
        weponCriticalText.text = swordManager.weponCritical[index].ToString("F2");//ũ��Ƽ�� Ȯ��
        weponCriticalDamageText.text = swordManager.weponCriticalDamage[index].ToString("F2");//ũ��Ƽ�� ������
    }

    public void PageNextPrevious(int np)
    {
        if (np == 0)
        {
            page += 1;
            audioSource.PlayOneShot(clip[6], Sound.Instance.uiSound);
        }
        else
        {
            audioSource.PlayOneShot(clip[7], Sound.Instance.uiSound);
            page -= 1;
        }


        // ������ ���� ����
        page = Mathf.Clamp(page, 0, PageUI.Length - 1);

        // ������ UI Ȱ��ȭ
        for (int i = 0; i < PageUI.Length; i++)
        {
            PageUI[i].SetActive(i == page);
        }
    }

    public void BackEnhanceWindow()
    {
        audioSource.PlayOneShot(clip[9], Sound.Instance.uiSound);
        enhanceWindow.SetActive(true);
        gameObject.SetActive(false);
    }
}
