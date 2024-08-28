using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatalVirus : PassiveSkill
{
    protected override void Awake()
    {
        skillName = "치명적 바이러스";
        effect = 0.01f;
        cooldown = 0f;
        icon = Resources.Load<Sprite>("UI/Icon/6");
    }
    public override void LevelUp() // 치명적 바이러스 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.effect += 0.01f; // 1레벨을 제외한 모든 레벨에서 효과가 1%p씩 상승.
    }
}