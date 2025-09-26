using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*interface : Ŭ������ ����ü�� �ݵ�� ������ �ϴ� ���(�޼���, �Ӽ�)�� [���]�� �����ϴ� ��.
            �������̽��� ���(����)�� Ŭ������ �ݵ�� �� ���� �޼��带 ���� �����ؾ� ��.*/
public interface ISkill//���(�Ծ�) ����
{
    string NameDescription { get; }
    bool Use { get; }                // ���� ��� ��������
    void Activate(Player player);// ��ų�� ��ȹ��/�ο����� �� 1ȸ
    void Deactivate(Player player); // ��ų�� ������/�Ҹꡱ�� �� 1ȸ
    void OnHit(Player player, Enemy_Base enemy); // ���� ���� �ø��� �Ź�
}
