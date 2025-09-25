using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnhanceData
{
    public int id;                        // ��ȭ ID
    public bool hasEnhance;              // ��ȭ ���� ����
    public List<int> enhanceLevel;             // ���� ��ȭ ����
    public float[] expPricce;
    [Header("����")]
    [Tooltip("�� ������ - ���� ��ȣ�� �ִ� ��ġ�� �����մϴ�")]
    public List<float> ShieldAmplifier;
    [Tooltip("�ڱ��� �ݰ� ��ġ -  ���� �ֺ� ������ �ֱ������� ���ظ� �ݴϴ�.")]
    public List<float> MagneticRetaliator;
    [Tooltip("�ڵ� ���� ��� - ~�� ���� �� �� ȸ���մϴ�.")]
    public List<float> AutoRepairModule;
    [Tooltip("�ھ� ���� ��ȯ�� - ���� �ھ� ������ ���� ����ġ�� ��ȯ �մϴ�.")]
    public List<float> CoreTransmuter;

    [Header("Ÿ��")]
    [Tooltip("��� �ð� �ý��� - ���� ���� �� ���� ������ �� �ֽ��ϴ�..")]
    public List<float> CoolingSystem;
    [Tooltip("��� ��ȭ ��� - Ÿ���� �⺻ ���ݷ��� �����մϴ�.")]
    public List<float> PowerOutputBooster;
    [Tooltip("���� ���� ī�� -  ���� ī���� ���� �ð��� �þ�ϴ�.")]
    public List<float> MagicCardAmplifier;
    [Tooltip("���� ���� �ع� - ������ ȸ�����ҷ��� �����մϴ�.")]
    public List<float> OverheatBurstRelease;

    [Header("�÷��̾�")]
    [Tooltip("���� ü�� �Ʒ� - �÷��̾��� ü�� �� ����� �����մϴ�.")]
    public List<float> PhysicalTraining;
    [Tooltip("���� ������ ��� - ������ ������ �����մϴ�")]
    public List<float> ShieldDurability;
    [Tooltip("�ھ� ä�� ���� - ä�� ������ ���� �մϴ�.")]
    public List<float> CoreHarvestSkill;
    [Tooltip("���� ���� ��� - �뽬�� ���� ������ ��� �Ҽ� �ֵ��� �մϴ�.")]
    public List<float> FlashRebootModule;


    public float[] BaseEnhanceIncreaseLevel;//��ȭ ������ ���� : ������ �������� ��ȭ�� ������ �� ����
    public float[] TowerEnhanceIncreaseLevel;//��ȭ ������ ���� : ������ �������� ��ȭ�� ������ �� ����
    public float[] PlayerEnhanceIncreaseLevel;//��ȭ ������ ���� : ������ �������� ��ȭ�� ������ �� ����

    public float EXPGrowthAmplifier;//��ȭ ������ ���� : ������ �������� ��ȭ�� ������ �� ����    

    public bool canBeAttacked => levelCheck();//������ ���� ���ݰ���
    public bool levelCheck()
    {
        if (id == 2)
        {
            int totalLevel = 0;
            for (int i = 0; i < enhanceLevel.Count; i++)
            {
                totalLevel += enhanceLevel[i]; // ��ü �ջ�
            }

            return totalLevel > 5; // �հ谡 5 �̻��̸� ���� ����
        }
        return false;
    }
}

