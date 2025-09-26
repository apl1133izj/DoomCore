using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*interface : 클래스나 구조체가 반드시 가져야 하는 기능(메서드, 속성)의 [약속]을 정의하는 것.
            인터페이스를 상속(구현)한 클래스는 반드시 그 안의 메서드를 전부 구현해야 함.*/
public interface ISkill//계약(규약) 역할
{
    string NameDescription { get; }
    bool Use { get; }                // 현재 사용 가능한지
    void Activate(Player player);// 스킬이 “획득/부여”될 때 1회
    void Deactivate(Player player); // 스킬이 “해제/소멸”될 때 1회
    void OnHit(Player player, Enemy_Base enemy); // 공격 적중 시마다 매번
}
