using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    public string skillName; // 스킬명
    public int level; //스킬 레벨
    public float skillDamage; // 데미지
    public float cooldown; // 쿨타임

    public Skill(string name, float baseDamage, float baseCooldown) // 스킬 생성자, 모든 스킬을 레벨 0으로 하여 스킬 리스트에 스킬 객체 삽입(스킬 매니저에 존재)
    {
        this.skillName = name;
        this.skillDamage = baseDamage;
        this.cooldown = baseCooldown;
        this.level = 0;
    }

    public virtual void LevelUp() // 스킬 레벨업
    {
        level++;
    }

    public float GetDamage() // 스킬 데미지 리턴
    {
        return skillDamage;
    }

    public float GetCooldown() // 스킬 쿨타임, 각 스킬에서 오버라이딩 필요
    {
        return cooldown;
    }

    // 몬스터와 상호작용하는 추상 메서드
    public abstract void Activate(GameObject target);
}
