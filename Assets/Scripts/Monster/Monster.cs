using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public enum WeaknessType
{
    Slash, // 참격
    Blow, // 타격
    All // 참격, 타격
};

public struct MonsterStats
{
    public float initialHP;        // 초기 체력
    public float initialAttackDamage;  // 초기 공격력
    public float initialSpeed;     // 초기 이동속도
}

public class Monster : MonoBehaviour
{
    private float attackRange = Mathf.PI * 2; // 공격 범위
    protected MonsterStats stats;
    [SerializeField] protected float currentHP; // 현재 체력
    [SerializeField] protected float attackDamage; // 공격력 
    [SerializeField] protected float speed; // 이동속도
    [SerializeField] protected float lastAttacktime; // 최근 공격 시각
    [SerializeField] protected float attackCooldown; // 공격 쿨타임
    [SerializeField] Animator animator;
    [SerializeField] public string key;
    protected WeaknessType weakness; // 약점 타입
    public float fadeDuration = 1.0f; // 알파값이 줄어드는 시간 (초)
    protected SpriteRenderer spriteRenderer;

    // ( 플레이어 , 엔지지어 ) 충돌 피해 변수
    private float lastPlayerDamageTime = -1f;
    private float lastEngineerDamageTime = -1f;

    protected virtual void Awake()
    {
        // 초기화
        InitializeStats();
    }

    public float GetCurrentSpeed()
    {
        return this.speed;
    }
    void Attack()       // 총 쏘는 몬스터들 추가로 피해주기 구현하기!!
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
                    player.TakeDamage(attackDamage); // 몬스터의 공격력만큼 피해를 줌
                }
            }
        }
    }
    void FixedUpdate()
    {
        if (Time.time >= lastAttacktime + attackCooldown)
        {
            Attack();
            lastAttacktime = Time.time; // 마지막 근접 공격 시간 업데이트
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            damageToObjects(collision, "Player");
        }

        if (collision.CompareTag("Engineer"))
        {
            damageToObjects(collision, "Engineer");
        }
    }
    private void damageToObjects(Collider2D collision, string tag)
    {
        if (tag == "Player")
        {
            // 마지막으로 플레이어에게 피해를 준 시간이 0.1초 이내인지 확인 후 충돌시 피해주기
            if (Time.time - lastPlayerDamageTime >= 0.1f)
            {
                Player__ player__ = collision.GetComponent<Player__>();
                if (player__ != null)
                {
                    player__.TakeDamage(attackDamage);  // 계산된 데미지를 적용
                    Debug.LogWarning("플레이어가 " + attackDamage + "를 입었습니다");
                    lastPlayerDamageTime = Time.time; // 피해를 준 시간 기록
                }
            }
        }
        else if (tag == "Engineer")
        {
            // 마지막으로 엔지니어에게 피해를 준 시간이 0.1초 이내인지 확인 후 충돌시 피해주기
            if (Time.time - lastEngineerDamageTime >= 0.1)
            {
                Engineer engineer = collision.GetComponent<Engineer>();
                if (engineer != null)
                {
                    engineer.TakeDamage(attackDamage);  // 계산된 데미지를 적용
                    Debug.LogWarning("엔지니어가 피해를 입었습니다");
                    lastEngineerDamageTime = Time.time; // 피해를 준 시간 기록
                }
            }
        }
    }
    public void TakeDamage(float damage)
    {
        float randomValue = Random.Range(0f, 1f); // 0에서 1 사이의 랜덤 값을 생성
        if(randomValue < Player_Stat.instance.instancedeathchance)
        {
            damage = 2147483647;
        }
        currentHP -= damage;
        if (currentHP <= 0)
        {
            this.speed = 0;
            this.attackDamage = 0;
            animator.SetTrigger("isDie");
            StartCoroutine(FadeOutAndDie());
            RealtimeManager.instance.Monsterkill();
        }
    }

    private IEnumerator FadeOutAndDie()
    {
        yield return FadeOutCoroutine();
        Die();
    }

    public void Die()
    {
        InitializeStats();
        gameObject.SetActive(false);
        Destroy(gameObject);
        SpawnManager.instance.objectPools[this.key].Enqueue(gameObject);
    }
    IEnumerator FadeOutCoroutine()
    {
        Color color = spriteRenderer.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            color.a = newAlpha;
            spriteRenderer.color = color;
            yield return null;
        }

        // 최종적으로 알파값을 0으로 설정
        color.a = 0f;
        spriteRenderer.color = color;
    }

    protected void InitializeStats()
    {
        // 구조체에 정의된 초기 값을 멤버 변수에 할당
        currentHP = stats.initialHP;
        attackDamage = stats.initialAttackDamage;
        speed = stats.initialSpeed;
        lastAttacktime = 0f; // 초기 쿨타임 시간은 0으로 설정
        Color color = spriteRenderer.color;
        color.a = 1f;
        spriteRenderer.color = color;
    }

}
