using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MagicPropertyDrawing : MonoBehaviour
{
    [Header("��ȭ - ��")]
    public int Aqualdrawindex;
    public int totalAqualdraw;
    public int[] Aqualevel;        // ��: 3��
    public int[] AquaMaxlevel;     // ��: {30, 30, 5}
    public float arearsizeEnhance;
    public float speedRatioEnhance;
    public float slowDurationEnhance;
    public int AquaHitComboEnhance;

    [Header("��ȭ - Ǯ")]
    public int Verdantldrawindex;
    public int totalVerdantdraw;
    public int[] Verdantlevel;
    public int[] VerdantMaxlevel;
    public float maxRecoveryEnhance;
    public float RecoveryDurationEnhance;
    public int VerdantHitComboEnhance;

    [Header("��ȭ - ���")]
    public int Shadowldrawindex;
    public int totalShadowtdraw;
    public int[] Shadowlevel;
    public int[] ShadowMaxlevel;
    public float hideDurationEnhance;
    public int ShadowHitComboEnhance;

    [Header("��� Ȯ�� ����")]
    public float AquaRarityPower = 1f;
    public float VerdantRarityPower = 1f;
    public float ShadowRarityPower = 1f;

    [Header("�������̽�")]
    public TextMeshProUGUI[] AquaUI;
    public TextMeshProUGUI[] VerdantUI;
    public TextMeshProUGUI[] ShadowUI;
    public TextMeshProUGUI[] AquaLevelUI;
    public TextMeshProUGUI[] VerdantLevelUI;
    public TextMeshProUGUI[] ShadowLevelUI;
    [Header("ȿ����")]
    public AudioSource audioSource;
    public AudioClip[] soundEffects;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // ======================= ��ư �̱� =========================
    public void DrawAquaButton()
    {
        if (GetRemainingUpgrades(Aqualevel, AquaMaxlevel) <= 0)
        {
            audioSource.PlayOneShot(soundEffects[3], Sound.Instance.uiSound);
            Debug.Log("�� �Ӽ� ��� ��ȭ�� �ִ�ġ�Դϴ�!");
            return;
        }

        float drawCostExp = 5f;
        if (Player.Instance.currentExp >= drawCostExp)
        {
            Player.Instance.currentExp -= drawCostExp;
            audioSource.PlayOneShot(soundEffects[0], Sound.Instance.uiSound);
            Aqualdrawindex = DrawAqua();
            if (Aqualdrawindex != -1)
                AquaSPACUpgrade(Aqualdrawindex);
        }
        else
        {
            audioSource.PlayOneShot(soundEffects[3], Sound.Instance.uiSound);

        }
    }

    public void DrawVerdantButton()
    {
        if (GetRemainingUpgrades(Verdantlevel, VerdantMaxlevel) <= 0)
        {
            audioSource.PlayOneShot(soundEffects[3], Sound.Instance.uiSound);
            Debug.Log("Ǯ �Ӽ� ��� ��ȭ�� �ִ�ġ�Դϴ�!");
            return;
        }

        float drawCostExp = 8f;
        if (Player.Instance.currentExp >= drawCostExp)
        {
            Player.Instance.currentExp -= drawCostExp;
            audioSource.PlayOneShot(soundEffects[1], Sound.Instance.uiSound);
            Verdantldrawindex = DrawVerdant();
            if (Verdantldrawindex != -1)
                VerdantSPACUpgrade(Verdantldrawindex);
        }else
        {
            audioSource.PlayOneShot(soundEffects[3], Sound.Instance.uiSound);

        }
    }

    public void DrawShadowButton()
    {
        if (GetRemainingUpgrades(Shadowlevel, ShadowMaxlevel) <= 0)
        {
            audioSource.PlayOneShot(soundEffects[3], Sound.Instance.uiSound);
            Debug.Log("��� �Ӽ� ��� ��ȭ�� �ִ�ġ�Դϴ�!");
            return;
        }

        float drawCostExp = 12f;
        if (Player.Instance.currentExp >= drawCostExp)
        {
            Player.Instance.currentExp -= drawCostExp;
            audioSource.PlayOneShot(soundEffects[2], Sound.Instance.uiSound);
            Shadowldrawindex = DrawShadow();
            if (Shadowldrawindex != -1)
                ShadowSPACUpgrade(Shadowldrawindex);
        }
        else
        {
            audioSource.PlayOneShot(soundEffects[3], Sound.Instance.uiSound);

        }
    }

    // ======================= �̱� =========================
    public int DrawAqua()
    {
        totalAqualdraw++;
        return RandomIndexWithUpgrade(Aqualevel, AquaMaxlevel, AquaRarityPower);
    }

    public int DrawVerdant()
    {
        totalVerdantdraw++;
        return RandomIndexWithUpgrade(Verdantlevel, VerdantMaxlevel, VerdantRarityPower);
    }

    public int DrawShadow()
    {
        totalShadowtdraw++;
        return RandomIndexWithUpgrade(Shadowlevel, ShadowMaxlevel, ShadowRarityPower);
    }

    int RandomIndexWithUpgrade(int[] levels, int[] maxLevels, float rarityPower = 1f)
    {
        List<int> possibleIndexes = new List<int>();
        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i] < maxLevels[i])
                possibleIndexes.Add(i);
        }

        if (possibleIndexes.Count == 0)
            return -1; // ��� ���� �ִ�ġ

        float r = Mathf.Pow(Random.value, rarityPower);
        int selectedIndex = Mathf.FloorToInt(r * possibleIndexes.Count);
        selectedIndex = Mathf.Clamp(selectedIndex, 0, possibleIndexes.Count - 1);

        int finalIndex = possibleIndexes[selectedIndex];

        // ���⼭�� ���� ����
        levels[finalIndex] = Mathf.Min(levels[finalIndex] + 1, maxLevels[finalIndex]);

        Debug.Log($"�Ӽ� �ε��� {finalIndex} ���� �� �� ���� ���� {levels[finalIndex]}/{maxLevels[finalIndex]}");

        return finalIndex;
    }

    // ======================= ��ȭ ���� =========================
    public void AquaSPACUpgrade(int index)
    {
        if (index < 0 || index >= Aqualevel.Length) return;

        if (index == 0) speedRatioEnhance += 0.015f;
        else if (index == 1) slowDurationEnhance += 0.2f;
        else if (index == 2) AquaHitComboEnhance--;

        if (totalAqualdraw % 15 == 0)
            MagicDefense.Instance.AquaDefenceMaxCount++; //.�ִ� ���� Ƚ��

        MagicDefense.Instance.AquaHitCombo = AquaHitComboEnhance;
        MagicDefense.Instance.speedRatio = speedRatioEnhance;
        MagicDefense.Instance.slowDuration = slowDurationEnhance;

        AquaUI[0].text = speedRatioEnhance.ToString("F2");
        AquaUI[1].text = slowDurationEnhance.ToString("F1");
        AquaUI[2].text = AquaHitComboEnhance.ToString("F0");
        AquaUI[3].text = MagicDefense.Instance.AquaDefenceMaxCount.ToString("F0");
        AquaUI[4].text = totalAqualdraw.ToString();

        for (int i = 0; i < AquaLevelUI.Length; i++)
        {
            AquaLevelUI[i].text = Aqualevel[i].ToString("F0");
        }
    }

    public void VerdantSPACUpgrade(int index)
    {
        if (index < 0 || index >= Verdantlevel.Length) return;

        if (index == 0) maxRecoveryEnhance += 1f;
        else if (index == 1) RecoveryDurationEnhance -= 0.2f;
        else if (index == 2) VerdantHitComboEnhance--;

        if (totalVerdantdraw % 15 == 0)
            MagicDefense.Instance.VerdantDefenceMaxCount++;//�ִ� ���� Ƚ��

        MagicDefense.Instance.VerdantHitCombo = VerdantHitComboEnhance;
        MagicDefense.Instance.maxRecovery = maxRecoveryEnhance;
        MagicDefense.Instance.RecoveryDuration = RecoveryDurationEnhance;

        VerdantUI[0].text = maxRecoveryEnhance.ToString("F2");
        VerdantUI[1].text = RecoveryDurationEnhance.ToString("F2");
        VerdantUI[2].text = VerdantHitComboEnhance.ToString("F0");
        VerdantUI[3].text = MagicDefense.Instance.VerdantDefenceMaxCount.ToString("F0");
        VerdantUI[4].text = totalVerdantdraw.ToString();
        for (int i = 0; i < VerdantLevelUI.Length; i++)
        {
            VerdantLevelUI[i].text = Verdantlevel[i].ToString("F0");
        }
    }

    public void ShadowSPACUpgrade(int index)
    {
        if (index < 0 || index >= Shadowlevel.Length) return;

        if (index == 0) hideDurationEnhance += 0.2f;
        else if (index == 1) ShadowHitComboEnhance--;

        if (totalShadowtdraw % 15 == 0)
            MagicDefense.Instance.ShadowDefenceMaxCount++;//�ִ� ���� Ƚ��

        MagicDefense.Instance.ShadowHitCombo = ShadowHitComboEnhance;
        MagicDefense.Instance.hideDuration = hideDurationEnhance;

        ShadowUI[0].text = hideDurationEnhance.ToString("F2");
        ShadowUI[1].text = ShadowHitComboEnhance.ToString("F0");
        ShadowUI[2].text = MagicDefense.Instance.ShadowDefenceMaxCount.ToString("F0");
        ShadowUI[3].text = totalShadowtdraw.ToString();

        for (int i = 0; i < ShadowLevelUI.Length; i++)
        {
            ShadowLevelUI[i].text = Shadowlevel[i].ToString("F0");
        }
    }

    // ======================= ���� �Լ� =========================
    int GetRemainingUpgrades(int[] levels, int[] maxLevels)
    {
        int remain = 0;
        for (int i = 0; i < levels.Length; i++)
            remain += (maxLevels[i] - levels[i]);
        return remain;
    }

}
