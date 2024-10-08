using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Pistol : Skill
{
    WeaknessType weaknessType = WeaknessType.Blow; // 공격 타임 : 타격
    protected override void Awake()
    {
        this.level = 1;
        base.Awake();
        skillName = "권총";
        skillDamage = 29f;
        cooldown = 1.5f;
        icon = Resources.Load<Sprite>("UI/Icon/13");
        levelupguide = "권총을 쏴요";
    }

    [SerializeField] public GameObject pistolPrefab;// 권총 프리펩
    public float bulletSpeed = 10.0f;
    public bool canPenetrate = false;    // 총알이 관통하는지 여부를 결정


    public override void Activate() // 몬스터와 상호 작용 로직
    {
        GameObject nearestMonster = FindNearestMonster();
        if (nearestMonster != null)
        {
            Vector3 direction = (nearestMonster.transform.position - player.transform.position).normalized;
            Vector3 bulletSpawnPosition = player.transform.position + direction * 0.5f;  // 플레이어 위치에서 약간 떨어진 위치
            GameObject pistolBullet = Instantiate(pistolPrefab, bulletSpawnPosition, Quaternion.identity);
            Debug.Log("bullet 프리펩 생성 완료");

            // Bullet 컴포넌트 가져오기
            PistolBullet PistolBulletScript = pistolBullet.GetComponent<PistolBullet>();
            if (PistolBulletScript != null)
            {
                float totalDamage;
                float weaknessMultipler = (nearestMonster.CompareTag("Monster2") || nearestMonster.CompareTag("Monster3")) ? 1.5f : 1f;

                DamageInfo damageInfo = new DamageInfo
                {
                    skillDamage = this.skillDamage,
                    playerDamage = Player_Stat.instance.attackDamageByLevel,
                    weaknessMultipler = weaknessMultipler,
                    isCritical = Player_Stat.instance.CheckCritical()
                };

                totalDamage = finalDamage(damageInfo);
                PistolBulletScript.damage = totalDamage;
            }

            Rigidbody2D bulletRb = PistolBulletScript.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.velocity = direction * bulletSpeed;
                Debug.Log("bullet 속도 계산 완료");
            }

            // 플레이어와 총알 간의 충돌을 무시
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            Collider2D bulletCollider = pistolBullet.GetComponent<Collider2D>();
            if (playerCollider != null && bulletCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, bulletCollider);
            }
        }

        // 레벨이 8 이상일 경우 2회 공격
        if (level >= 8)
        {
            StartCoroutine(DoubleAttack());
        }
        lastUsedTime = Time.time;
    }


    public override void LevelUp() // 권총 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.skillDamage += 1f;

        switch (level)
        {
            case 1:
                this.levelupguide = "스킬 데미지 29 -> 30";
                break;
            case 2:
                this.levelupguide = "스킬 데미지 30 -> 31, 쿨타임 1.5 -> 1.25";
                break;
            case 3: // 2->3랩 : 쿨타임 1초 감소
                this.cooldown -= 0.25f;
                this.levelupguide = "스킬 데미지 31 -> 32";
                break;
            case 4:
                this.levelupguide = "스킬 데미지 32 -> 33, 쿨타임 1.25 -> 1";
                break;
            case 5: // 4->5랩 : 쿨타임 1초 감소
                this.cooldown -= 0.25f;
                this.levelupguide = "스킬 데미지 33 -> 34";
                break;
            case 6:
                this.levelupguide = "스킬 데미지 34 -> 36";
                break;
            case 7: // 6->7랩 : 데미지 2 증가
                this.skillDamage += 2f;
                this.levelupguide = "이제부터 권총을 2발 연속으로 사용합니다";
                break;
        }
    }

    private IEnumerator DoubleAttack() // 8레벨 2회 공격을 코루틴으로 구현
    {
        yield return new WaitForSeconds(0.25f); // 0.25초 대기
        Activate(); // 두 번째 공격
    }


    GameObject FindNearestMonster()
    {
        // 찾고자 하는 레이어들을 정의 (예시로 몬스터들이 속한 레이어)
        int monsterLayer1 = LayerMask.NameToLayer("Monster");

        GameObject nearestMonster = null;
        float minDistance = Mathf.Infinity;

        // 씬 내의 모든 활성화된 게임 오브젝트를 가져옴
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // 오브젝트가 몬스터 레이어에 속해 있는지 확인
            if (obj.layer == monsterLayer1)
            {
                float distance = Vector3.Distance(player.transform.position, obj.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestMonster = obj;
                }
            }
        }

        return nearestMonster;
    }
}