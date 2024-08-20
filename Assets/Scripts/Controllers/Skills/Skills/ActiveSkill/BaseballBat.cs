using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class BaseballBat : Skill
{
    public BaseballBat() : base("BaseballBat", 2f, 4f) { } // 생성자 : 스킬명, 1랩 데미지, 1랩 쿨타임

    private float angle = Mathf.PI / 8; // 부채꼴 범위의 반각
    private float radius = 1.5f; // 반지름
    private UnityEngine.Vector2 dir; // 플레이어 시선 방향
    private UnityEngine.Vector2 lastdir;
    WeaknessType weaknessType = WeaknessType.Blow;
    public override void Activate(GameObject target) // 몬스터와 상호 작용 로직
    {
        Monster monster;
        float totalDamage = 0f; // 몬스터가 입는 총 데미지
        dir = player.GetComponent<Rigidbody2D>().velocity.normalized; // 이동 방향 단위 벡터
        if(dir == UnityEngine.Vector2.zero) dir = lastdir;
        else lastdir = dir;

        // 범위 내의 모든 콜라이더를 가져옴 (히트스캔 방식)
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(player.transform.position, radius);

        foreach (var hitCollider in hitColliders) // 범위 안의 모든 몬스터에 대하여 반복문
        {
            UnityEngine.Vector2 directionToCollider = (hitCollider.transform.position - player.transform.position).normalized;
            float angleToCollider = UnityEngine.Vector2.Angle(dir, directionToCollider);

            // 부채꼴 범위 안에 있는지 확인
            if (angleToCollider <= angle * Mathf.Rad2Deg) 
            {
                if (hitCollider.CompareTag("Monster1") || hitCollider.CompareTag("Monster3") || hitCollider.CompareTag("Monster2"))
                {
                    monster = hitCollider.GetComponent<Monster>();
                    float weaknessMultipler = (hitCollider.CompareTag("Monster2") || hitCollider.CompareTag("Monster3")) ? 1.5f : 1f;

                    DamageInfo damageInfo = new DamageInfo
                    {
                        skillDamage = this.skillDamage,
                        playerDamage = player.playerStat.attackDamageByLevel,
                        weaknessMultipler = weaknessMultipler,
                        isCritical = player.playerStat.CheckCritical()
                    };

                    totalDamage = finalDamage(damageInfo);
                    monster.TakeDamage(totalDamage);
                }
            }
        }
        if(level == 8)
        {
            StartCoroutine(DoubleAttack(target));
        }
    }

    public override void LevelUp() // 야구 방망이 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.skillDamage += 1f;

        switch (level)
        {
            case 3: // 2->3랩: 쿨타임 1초 감소
                this.cooldown--;
                break;
            case 4: // 3->4랩: 반지름 2->2.25
                this.radius = 2f;
                break;
            case 6: //5->6랩: 쿨타임 1초 감소
                this.cooldown--;
                break;
            case 7: //6->7랩: 범위 증가
                angle = Mathf.PI / 3;
                break;
            case 8: //2회 공격
                this.skillDamage *= 2;
                break;

        }
    }
    private IEnumerator DoubleAttack(GameObject target) // 8레벨 2회 공격을 코루틴으로 구현
    {
        yield return new WaitForSeconds(0.25f); // 0.25초 대기
        Activate(target); // 두 번째 공격
    }
}
