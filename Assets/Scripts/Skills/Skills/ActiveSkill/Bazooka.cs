using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bazooka : Skill
{
    WeaknessType weaknessType = WeaknessType.Blow; // 공격 타임 : 타격
    [SerializeField] public GameObject bazookaPrefab; // 바주카포 프리펩
    public float bulletSpeed = 10.0f;
    public float exploreRadius = 0.5f;

    protected override void Awake()
    {
        base.Awake();
        skillName = "바주카포";
        skillDamage = 60f;
        cooldown = 5f;
        icon = Resources.Load<Sprite>("UI/Icon/20");
        levelupguide = "폭발을 일으키는 총알을 발사합니다";
    }
    public override void LevelUp() // 바주카포 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.skillDamage += 1f;

        switch (level)
        {
            case 1:
                this.levelupguide = "데미지 60 -> 61";
                break;
            case 2:
                this.levelupguide = "데미지 61 -> 62, 쿨타임 5 -> 4";
                break;
            case 3: // 2->3랩 : 쿨타임 1초 감소
                this.levelupguide = "데미지 62 -> 63, 폭발범위 증가";
                cooldown -= 1f;
                break;
            case 4: // 3->4랩 : 폭발 범위 반지름 증가(반지름 1→ 반지름 1.5)
                this.levelupguide = "데미지 63 -> 64, 쿨타임 4 -> 3";
                exploreRadius += 0.5f;
                break;
            case 5: // 4->5랩 : 쿨타임 1초 감소
                this.levelupguide = "데미지 64 -> 65, 폭발범위 증가";
                cooldown -= 1f;
                break;
            case 6: // 5->6랩 : 폭발 범위 반지름 증가 (1.5→2)
                this.levelupguide = "데미지 65 -> 66, 쿨타임 3 -> 2";
                exploreRadius += 0.5f;
                break;
            case 7: // 6->7랩 : 쿨타임 1초 감소
                this.levelupguide = "데미지 66 -> 67, 폭발범위 대폭증가";
                cooldown -= 1f;
                break;
            case 8: // 7->8랩 : 폭발 범위 반지름 (2→3)
                exploreRadius += 1f;
                break;
        }
    }


    public override void Activate() // 몬스터와 상호 작용 로직
    {
        GameObject nearestMonster = FindNearestMonster();
        if (nearestMonster != null)
        {
            
            Vector3 direction = (nearestMonster.transform.position - player.transform.position).normalized;
            Vector3 bulletSpawnPosition = player.transform.position + direction * 0.5f; // 플레이어 위치에서 약간 떨어진 위치
            GameObject bazookaBullet = Instantiate(bazookaPrefab, bulletSpawnPosition, Quaternion.identity);
            Debug.Log("Bazooka bullet 프리펩 생성 완료");

            BazookaBullet BazookaBulletScript = bazookaBullet.GetComponent<BazookaBullet>();

            // 플레이어와 총알 간의 충돌을 무시
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            Collider2D bulletCollider = bazookaBullet.GetComponent<Collider2D>();
            if (playerCollider != null && bulletCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, bulletCollider);
            }

            if (BazookaBulletScript != null)
            {
                DamageInfo damageInfo = new DamageInfo
                {
                    skillDamage = this.skillDamage,
                    playerDamage = Player_Stat.instance.attackDamageByLevel,
                    isCritical = Player_Stat.instance.CheckCritical()
                };

                BazookaBulletScript.damageInfo = damageInfo;
            }

            Rigidbody2D bulletRb = bazookaBullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.velocity = direction * bulletSpeed;
            }
        }
        lastUsedTime = Time.time;
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
