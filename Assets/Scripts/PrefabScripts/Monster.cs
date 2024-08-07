using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class Monster : MonoBehaviour
{
    private float attackRange = Mathf.PI * 2;
    [SerializeField] Monster_Manager mm;
    [SerializeField] float CurrentHP = 0.0f;
    [SerializeField] float AD = 0.0f;
    [SerializeField] float Speed = 0.0f;
    [SerializeField] float lastAttacktime;
    [SerializeField] float AttackCooldown = 1.0f;
    void Awake()
    {
        mm = GetComponent<Monster_Manager>();
        AD = mm.GetAD();
        CurrentHP = mm.GetHP();
        Speed = mm.GetSpeed();
        lastAttacktime = Time.time;
    }
    public float GetCurrentSpeed()
    {
        return this.Speed;
    }
    void Attack()
    {
        UnityEngine.Vector2 attackPosition = transform.position; // 몬스터의 현재 위치를 공격 중심으로 설정
        attackPosition.y += 0.5f;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPosition, attackRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Player player = hitCollider.GetComponent<Player>();
                if (player != null)
                {
                    player.TakeDamage(AD); // 몬스터의 공격력만큼 피해를 줌
                }
            }
        }
    }
    void FixedUpdate()
    {
        if (Time.time >= lastAttacktime + AttackCooldown)
        {
            Attack();
            lastAttacktime = Time.time; // 마지막 근접 공격 시간 업데이트
        }
    }

    public void TakeDamage(float damage)
    {
        CurrentHP -= damage;
        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
