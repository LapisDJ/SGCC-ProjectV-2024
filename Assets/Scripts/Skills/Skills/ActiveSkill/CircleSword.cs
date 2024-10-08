using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSword : Skill
{
    protected override void Awake()
    {
        base.Awake();
        skillName = "칼날 의수";
        skillDamage = 60f;
        cooldown = 4f;
        icon = Resources.Load<Sprite>("UI/Icon/11");
        levelupguide = "원 범위로 피해를 줍니다";
        animator = GetComponent<Animator>();
    }

    private float circleAttackRadius = 2f; // 원 범위 반지름
    WeaknessType weaknessType = WeaknessType.Slash;
    public override void Activate() // 몬스터와 상호 작용 로직
    {
        transform.localScale = new Vector3(circleAttackRadius, circleAttackRadius, 1f); // 원하는 크기로 설정, 예시로 2배 크기

        transform.position = player.transform.position;
        animator.Play("bladestorm");
        Monster monster;
        float totalDamage = 0f; // 몬스터가 입는 총 데미지

        // 범위 내의 모든 콜라이더를 가져옴 (히트스캔 방식)
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(player.transform.position, circleAttackRadius);

        foreach (var hitCollider in hitColliders) // 범위 안의 모든 몬스터에 대하여 반복문
        {
            if (hitCollider.CompareTag("Monster1") || hitCollider.CompareTag("Monster3") || hitCollider.CompareTag("Monster2"))
            {
                monster = hitCollider.GetComponent<Monster>();
                float weaknessMultipler = (hitCollider.CompareTag("Monster1") || hitCollider.CompareTag("Monster3")) ? 1.5f : 1f;

                DamageInfo damageInfo = new DamageInfo
                {
                    skillDamage = this.skillDamage,
                    playerDamage = Player_Stat.instance.attackDamageByLevel,
                    weaknessMultipler = weaknessMultipler,
                    isCritical = Player_Stat.instance.CheckCritical()
                };

                totalDamage = finalDamage(damageInfo);
                monster.TakeDamage(totalDamage);
            }
        }
        lastUsedTime = Time.time;
        StartCoroutine(Waitforseconds());
    }

    public override void LevelUp() // 칼날 의수(원형 참격) 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.skillDamage += 1f;

        switch (level)
        {
            case 1:
                this.levelupguide = "데미지 60 -> 61";
                break;
            case 2:
                this.levelupguide = "데미지 61 -> 62, 쿨타임 4 -> 3";
                break;
            case 3: // 2->3랩: 쿨타임 1초 감소
                this.cooldown--;
                this.levelupguide = "데미지 62 -> 63, 스킬범위 증가";
                break;
            case 5: // 3->4랩: 반지름 2->2.25
                this.circleAttackRadius = 2.25f;
                this.levelupguide = "데미지 63 -> 64, 쿨타임 3 -> 2";
                break;
            case 6: //5->6랩: 쿨타임 1초 감소
                this.cooldown--;
                this.levelupguide = "데미지 64 -> 65, 스킬범위 증가";
                break;
            case 7: //6->7랩: 반지름 2->2.25
                this.circleAttackRadius = 2.5f;
                this.levelupguide = "스킬 데미지 2배 증가";
                break;
            case 8: //7->8랩: 스킬 데미지 2배 증가
                this.skillDamage *= 2;
                break;

        }
    }
    IEnumerator Waitforseconds()
    {
        yield return new WaitForSeconds(1.0f);
    }
}
