using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateKnifeDrone : Skill
{
    public RotateKnifeDrone() : base("RotateKnifeDrone", 1f, 0f) { } // 생성자 : 스킬명, 1랩 데미지, 1랩 쿨타임

    public GameObject knifeDronePrefab; // 칼날 드론 프리펩
    public float rotationSpeed = 90f; // 1랩 칼날 회전 속도
    public float knifeRadius = 2f; // 1랩 칼날 회전 반지름
    private List<GameObject> knifeDrones = new List<GameObject>(); // 칼날 개수 리스트
    private int knifeCount = 2; // 1랩 칼날 개수


    public override void Activate(GameObject target) // 몬스터와 상호 작용 로직
    {

    }

    public override void LevelUp() // 칼날 드론 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.skillDamage += 1f;

        switch(level)
        {
            case 1: // 0 -> 1랩 : 기본값으로 칼날 생성
                GenerateKnives(this.knifeCount); // 칼날 2개
                break;
            case 3: // 2->3랩 : 칼날 2개 증가
                this.knifeCount += 2;
                GenerateKnives(this.knifeCount); // 칼날 4개
                break;
            case 4: // 3->4랩 : 칼날 회전 속도 2배 증가
                this.rotationSpeed *= 2;
                break;
            case 6: // 5->6랩 : 칼날 2개 증가
                this.knifeCount += 2;
                GenerateKnives(this.knifeCount);
                break;
            case 7: // 6->7랩 : 칼날 2개 증가
                this.knifeCount += 2;
                GenerateKnives(this.knifeCount);
                break;
            case 8: // 7->8랩 : 칼날 회전 속도 2배 증가
                this.rotationSpeed *= 2;
                break;
        }
    }

    void GenerateKnives(int knifeCount)
    {
        for (int i = 0; i < knifeCount; i++)
        {
            float angle = i * Mathf.PI * 2f / knifeCount;
            Vector3 position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * knifeRadius;
            GameObject knife = Instantiate(knifeDronePrefab, transform.position + position, Quaternion.identity);
            knife.transform.SetParent(transform);
            knifeDrones.Add(knife);
        }
    }

    void DestroyKnives()
    {
        foreach (GameObject knife in knifeDrones)
        {
            Destroy(knife);
        }
        knifeDrones.Clear();
    }

    public void RotateKnives()
    {
        foreach (GameObject knife in knifeDrones)
        {
            knife.transform.RotateAround(transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }
}