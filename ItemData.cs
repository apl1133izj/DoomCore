using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ItemData
{
    public int[] RareCoreCount;//������ ��� 0 : �⺻, 1 : ��� , 2 : ����
    public int[] Activation_itemCount;//Ȱ��ȭ ������ 0: �ϳ��� 1 , 1: �ϳ��� 2�� 2:�ϳ��� 3��
    public int  FireCard, IceCard, electricityCard;
    public int cannonModuleCount, MainModuleCount, HoldModuleCount, FloorModuleCount;

    public bool hasCompletedTutorial;//Ʃ�丮���� �Ϸ� ������ �ִ°�?
    public float GameOverPoint;//��Ƴ����� = ���ӳ����� ��ȭ�����ϰ� �ϴ� ����Ʈ
}
