using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnhanceData
{
    public int id;                        // 강화 ID
    public bool hasEnhance;              // 강화 가능 여부
    public List<int> enhanceLevel;             // 현재 강화 레벨
    public float[] expPricce;
    [Header("거점")]
    [Tooltip("방어막 증폭기 - 거점 보호막 최대 수치가 증가합니다")]
    public List<float> ShieldAmplifier;
    [Tooltip("자기장 반격 장치 -  거점 주변 적에게 주기적으로 피해를 줍니다.")]
    public List<float> MagneticRetaliator;
    [Tooltip("자동 수리 모듈 - ~턴 마다 방어막 을 회복합니다.")]
    public List<float> AutoRepairModule;
    [Tooltip("코어 조각 변환기 - 남은 코어 조각을 갈아 경험치로 변환 합니다.")]
    public List<float> CoreTransmuter;

    [Header("타워")]
    [Tooltip("고급 냉각 시스템 - 과열 없이 더 오래 공격할 수 있습니다..")]
    public List<float> CoolingSystem;
    [Tooltip("출력 강화 모듈 - 타워의 기본 공격력이 증가합니다.")]
    public List<float> PowerOutputBooster;
    [Tooltip("마법 증폭 카드 -  마법 카드의 지속 시간이 늘어납니다.")]
    public List<float> MagicCardAmplifier;
    [Tooltip("과열 폭주 해방 - 과열시 회전감소량이 감소합니다.")]
    public List<float> OverheatBurstRelease;

    [Header("플레이어")]
    [Tooltip("기초 체력 훈련 - 플레이어의 체력 과 기력이 증가합니다.")]
    public List<float> PhysicalTraining;
    [Tooltip("방패 내구도 향상 - 방패의 방어력이 증가합니다")]
    public List<float> ShieldDurability;
    [Tooltip("코어 채집 숙련 - 채집 범위가 증가 합니다.")]
    public List<float> CoreHarvestSkill;
    [Tooltip("섬광 리붓 모듈 - 대쉬를 더욱 빠르게 사용 할수 있도록 합니다.")]
    public List<float> FlashRebootModule;


    public float[] BaseEnhanceIncreaseLevel;//강화 증가율 레벨 : 레벨이 높을수록 강화시 스펙이 더 증가
    public float[] TowerEnhanceIncreaseLevel;//강화 증가율 레벨 : 레벨이 높을수록 강화시 스펙이 더 증가
    public float[] PlayerEnhanceIncreaseLevel;//강화 증가율 레벨 : 레벨이 높을수록 강화시 스펙이 더 증가

    public float EXPGrowthAmplifier;//강화 증가율 레벨 : 레벨이 높을수록 강화시 스펙이 더 증가    

    public bool canBeAttacked => levelCheck();//무적인 몬스터 공격가능
    public bool levelCheck()
    {
        if (id == 2)
        {
            int totalLevel = 0;
            for (int i = 0; i < enhanceLevel.Count; i++)
            {
                totalLevel += enhanceLevel[i]; // 전체 합산
            }

            return totalLevel > 5; // 합계가 5 이상이면 공격 가능
        }
        return false;
    }
}

