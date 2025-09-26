using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class WeponDrawing : MonoBehaviour
{
    public GameObject enhanceWindow;

    public Sword sword;
    public SwordManager swordManager;


    public GameObject DirectoGameObject;//연출 쉐이더 부모
    public GameObject drawingDirectorSwordGameObject;//같은 검모양끼리 모인 보무
    public Image[] DirectorImage;//연출

    public GameObject[] DrawingDirectorSword;//연출에 사용한는 검들
    public Coroutine DrawingDirectorCoroutine;

    [Header("가방")]
    int page;
    public GameObject[] PageUI;
    public GameObject[] DrawViewSword;//뽑기에서 나온검
    public GameObject[] lockSword;//잠겨 있는 검
    public GameObject[] ViewWepon;//가방 에서 미리보기
    public GameObject SPACUI;


    public AudioSource audioSource;
    public AudioClip[] clip;
    public int drawCount = 0; // 총 뽑기 횟수
    public string[] Rares;

    public TextMeshProUGUI drawCountText;
    public TextMeshProUGUI weponDamageText;
    public TextMeshProUGUI weponAttackSpeedText;
    public TextMeshProUGUI weponCriticalText;
    public TextMeshProUGUI weponCriticalDamageText;
    public TextMeshProUGUI RareText;
    
    public void SelectSword(int index)
    {
      
        if (lockSword[index].activeSelf)//잠긴 검
        {
            audioSource.PlayOneShot(clip[3], Sound.Instance.uiSound);
        }
        else if (!lockSword[index].activeSelf)//잠금해제 된 검
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
                grade = 0; // 일반
            else if (index <= 24)
                grade = 1; // 희귀
            else if (index <= 35)
                grade = 2; // 영웅
            else
                grade = 3; // 전설
            RareText.text = Rares[grade];            // 배열에서 등급 표시

            audioSource.PlayOneShot(clip[2], Sound.Instance.uiSound);
            sword.swordIndex = index;//선택한 검 장착
        }
    }


    public void Draw()
    {
        if (DrawingDirectorCoroutine == null)
        {
            float drawCostExp = 15f; // 한 번 뽑기 시 경험치 소모
            if (Player.Instance.currentExp >= drawCostExp)
            {
                Player.Instance.currentExp -= drawCostExp;
                audioSource.PlayOneShot(clip[0], Sound.Instance.uiSound);
                DrawingDirectorCoroutine = StartCoroutine(DrawingDirector1());
            }
            else
            {
                audioSource.PlayOneShot(clip[8], Sound.Instance.uiSound);
                Debug.Log("경험치 부족! 몬스터를 더 잡으세요.");
            }
        }
        
    }
    //뽑기 1번 연출
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
        int swordIndex = RandomSword();                // 뽑힌 검 인덱스
        DirectorImage[0].enabled = false;
        DirectorImage[1].enabled = true;
        DrawViewSword[swordIndex].SetActive(true);
        RectTransform rt = DirectorImage[1].rectTransform;
        while (rt.sizeDelta.y > 0f)
        {
            Vector2 size = rt.sizeDelta;       // 현재 sizeDelta 가져오기
            size.y -= Time.deltaTime * 125;   // 1초에 20씩 줄어듦
            size.y = Mathf.Max(size.y, 0f);   // 0 아래로 안 내려가게
            rt.sizeDelta = size;               // 다시 할당
            yield return null;                 // 다음 프레임까지 대기
        }
        SPACUI.SetActive(true);
        DirectoGameObject.SetActive(false);
        audioSource.PlayOneShot(clip[5], Sound.Instance.uiSound);
        weponDamageText.text = swordManager.weponDamage[swordIndex].ToString("F2");//데미지
        yield return new WaitForSeconds(0.25f);
        audioSource.PlayOneShot(clip[5], Sound.Instance.uiSound);
        weponAttackSpeedText.text = swordManager.weponAttackSpeed[swordIndex].ToString("F2");//공격속도
        yield return new WaitForSeconds(0.25f);
        audioSource.PlayOneShot(clip[5], Sound.Instance.uiSound);
        weponCriticalText.text = swordManager.weponCritical[swordIndex].ToString("F2");//크리티컬 확률
        yield return new WaitForSeconds(0.25f);
        audioSource.PlayOneShot(clip[5], Sound.Instance.uiSound);
        weponCriticalDamageText.text = swordManager.weponCriticalDamage[swordIndex].ToString("F2");//크리티컬 데미지
        int grade;
        if (swordIndex <= 11)
            grade = 0; // 일반
        else if (swordIndex <= 24)
            grade = 1; // 희귀
        else if (swordIndex <= 35)
            grade = 2; // 영웅
        else
            grade = 3; // 전설
        RareText.text = Rares[grade];            // 배열에서 등급 표시
                                                 // 중복 체크 + 경험치 변환
        drawCountText.text = drawCount.ToString();//뽑은 횟수
        //중복인 경우 경험치를 반환함
        if (!lockSword[swordIndex].activeSelf)
        {
            float expGain = Player.Instance.currentExp * 1.5f;
            Player.Instance.currentExp += expGain;
        }
        else
        {
            lockSword[swordIndex].SetActive(false); // 신규 무기 잠금 해제
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
        Vector2 size = rt.sizeDelta;       // 현재 sizeDelta 가져오기
        size.y = 234f;
    }

    /// <summary>
    /// p > 1 → 44 뽑기 더 힘듦
    //p = 1 → 모든 등급 동일 확률(2.22%)
    //p< 1 → 44 뽑기 더 쉬움
    /// </summary>
    /// <returns></returns>
    int RandomSword()
    {
        drawCount++;

        // 50회 보장 전설
        if (drawCount % 50 == 0)
            return Random.Range(36, 45);

        float r = Random.value;
        r = Mathf.Pow(r, 5.3f); // 전설 확률 약 5%
        int index = Mathf.FloorToInt(r * (lockSword.Length - 1)) + 1; // 1 ~ 44

        // 안전하게 범위 제한
        index = Mathf.Clamp(index, 1, lockSword.Length - 1);

        return index;
    }




    public void UIUpdate(int index)
    {
        SPACUI.SetActive(true);

        weponDamageText.text = swordManager.weponDamage[index].ToString("F2");//데미지
        weponAttackSpeedText.text = swordManager.weponAttackSpeed[index].ToString("F2");//공격속도
        weponCriticalText.text = swordManager.weponCritical[index].ToString("F2");//크리티컬 확률
        weponCriticalDamageText.text = swordManager.weponCriticalDamage[index].ToString("F2");//크리티컬 데미지
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


        // 페이지 범위 제한
        page = Mathf.Clamp(page, 0, PageUI.Length - 1);

        // 페이지 UI 활성화
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
