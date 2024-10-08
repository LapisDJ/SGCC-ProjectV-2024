using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazookaBullet : Bullet
{
    public float explosionRadius;
    public DamageInfo damageInfo;

    void Awake()
    {
        canPenetrate = false;
        // 총알의 Collider2D가 트리거로 설정되어 있어야 함
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 몬스터와 충돌할 경우에만 데미지 처리
        if (collision.CompareTag("Monster1") || collision.CompareTag("Monster2") || collision.CompareTag("Monster3"))
        {
            ExplodeAndDamage(collision.transform.position, explosionRadius, damageInfo);
            Destroy(gameObject); // 충돌 후 총알 제거
        }
    }

    void ExplodeAndDamage(Vector3 explosionCenter, float radius, DamageInfo damageInfo)
{
    int monsterLayerMask = LayerMask.GetMask("Monster"); // "MonsterLayer"는 몬스터가 속한 레이어 이름

    Collider2D[] hitMonsters = Physics2D.OverlapCircleAll(explosionCenter, radius, monsterLayerMask);

    foreach (Collider2D hitMonster in hitMonsters)
    {
        Monster monster = hitMonster.GetComponent<Monster>();
        if (monster != null)
        {
            float totalDamage;
            float weaknessMultiplier = (hitMonster.CompareTag("Monster1") || hitMonster.CompareTag("Monster3")) ? 1.5f : 1f;

            damageInfo.weaknessMultipler = weaknessMultiplier;

            totalDamage = finalDamage(damageInfo);
            monster.TakeDamage(totalDamage);
        }
    }
}


    protected float finalDamage(DamageInfo damageInfo)
    {
        float basicDamage = (damageInfo.playerDamage + damageInfo.skillDamage) * damageInfo.weaknessMultipler;
        if (damageInfo.isCritical)
            return basicDamage * 1.5f;
        else
            return basicDamage;
    }
}
