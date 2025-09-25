using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ItemData
{
    public int[] RareCoreCount;//아이템 레어도 0 : 기본, 1 : 희귀 , 2 : 전설
    public int[] Activation_itemCount;//활성화 아이템 0: 하나당 1 , 1: 하나당 2개 2:하나당 3개
    public int  FireCard, IceCard, electricityCard;
    public int cannonModuleCount, MainModuleCount, HoldModuleCount, FloorModuleCount;

    public bool hasCompletedTutorial;//튜토리얼을 완료 한적이 있는가?
    public float GameOverPoint;//살아남은턴 = 게임끝나고 강화가능하게 하는 포인트
}
