using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public int maxActiveSkills = 4; //최대 액티브 개수
    public int maxPassiveSkills = 6; //최대 패시브 개수

    private List<Skill> availableActiveSkills = new List<Skill>(); // 모든 액티브 스킬 리스트
    private List<PassiveSkill> availablePassiveSkills = new List<PassiveSkill>(); // 모든 패시브 스킬 리스트

    public static List<Skill> activeSkills = new List<Skill>(); // 현재 활성화된 액티브 스킬
    public static List<PassiveSkill> passiveSkills = new List<PassiveSkill>(); // 현재 활성화된 패시브 스킬
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start() // 모든 액티브, 패시브 스킬 초기화 후 담기
    {
        // 액티브 스킬 초기화
        availableActiveSkills.Add(CreateActiveSkill<BaseballBat>());
        availableActiveSkills.Add(CreateActiveSkill<Bazooka>());
        availableActiveSkills.Add(CreateActiveSkill<CircleSword>());
        availableActiveSkills.Add(CreateActiveSkill<ElectronicField>());
        availableActiveSkills.Add(CreateActiveSkill<Pistol>());
        availableActiveSkills.Add(CreateActiveSkill<RotateKnifeDrone>());
        availableActiveSkills.Add(CreateActiveSkill<ShotGun>());

        // 패시브 스킬 초기화
        availablePassiveSkills.Add(CreatePassiveSkill<Adrenaline>());
        availablePassiveSkills.Add(CreatePassiveSkill<CalculateHelper>());
        availablePassiveSkills.Add(CreatePassiveSkill<Coin>());
        availablePassiveSkills.Add(CreatePassiveSkill<FatalVirus>());
        availablePassiveSkills.Add(CreatePassiveSkill<LearnPill>());
        availablePassiveSkills.Add(CreatePassiveSkill<PoweredSkin>());
        availablePassiveSkills.Add(CreatePassiveSkill<ProstheticHand>());
        availablePassiveSkills.Add(CreatePassiveSkill<RocketBoots>());
        availablePassiveSkills.Add(CreatePassiveSkill<TeleportDevice>());
    }
    private T CreateActiveSkill<T>() where T : Skill
    {
        GameObject skillObject = new GameObject(typeof(T).Name);
        return skillObject.AddComponent<T>();
    }
    private T CreatePassiveSkill<T>() where T : PassiveSkill
    {
        GameObject skillObject = new GameObject(typeof(T).Name);
        return skillObject.AddComponent<T>();
    }
    public static List<Skill> skillchoices;
    public static List<PassiveSkill> passivechoices;
    public void LevelUp()
    {
        skillchoices = GetRandomSkillChoices(); // 레벨업 ui에 선택될 액티브 스킬들
        foreach (Skill active in skillchoices)
        {
            Debug.Log("ui에 표시되는 액티브 스킬 선택 목록" + active.skillName);//지금까지 나온 모든 액티브 스킬의 이름을 콘솔창에 띄움
        }
        passivechoices = GetRandomPassiveChoices(); // 레벨업 ui에 선택될 패시브 스킬들
        foreach (PassiveSkill passive in passivechoices)
        {
            Debug.Log("ui에 표시되는 액티브 스킬 선택 목록" + passive.skillName);//지금까지 나온 모든 액티브 스킬의 이름을 콘솔창에 띄움
        }
    }
    void Update()
    {
        // 디버깅용 코드입니다. Q, W, E, R 키를 눌렀을 때 스킬 발동
        if (Input.GetKeyDown(KeyCode.Q) && activeSkills.Count > 0)
        {
            activeSkills[0].Activate(); // Q에 해당하는 첫 번째 스킬 발동
            Debug.Log("Q 키로 " + activeSkills[0].skillName + " 스킬을 발동했습니다.");
        }

        if (Input.GetKeyDown(KeyCode.W) && activeSkills.Count > 1)
        {
            activeSkills[1].Activate(); // W에 해당하는 두 번째 스킬 발동
            Debug.Log("W 키로 " + activeSkills[1].skillName + " 스킬을 발동했습니다.");
        }

        if (Input.GetKeyDown(KeyCode.E) && activeSkills.Count > 2)
        {
            activeSkills[2].Activate(); // E에 해당하는 세 번째 스킬 발동
            Debug.Log("E 키로 " + activeSkills[2].skillName + " 스킬을 발동했습니다.");
        }

        if (Input.GetKeyDown(KeyCode.R) && activeSkills.Count > 3)
        {
            activeSkills[3].Activate(); // R에 해당하는 네 번째 스킬 발동
            Debug.Log("R 키로 " + activeSkills[3].skillName + " 스킬을 발동했습니다.");
        }
    }

    private List<Skill> GetRandomSkillChoices() // 로직에 따라 액티브 스킬 2개 반환
    {
        var retList = new List<Skill>();
        if (activeSkills.Count >= maxActiveSkills)//액티브 스킬 가짓수를 모두 채우면 가지고 있는 액티브 중에서 가져옴
        {
            int rand1 = UnityEngine.Random.Range(0, maxActiveSkills);
            int rand2 = UnityEngine.Random.Range(0, maxActiveSkills);
            while (rand1 == rand2)
            {
                rand2 = UnityEngine.Random.Range(0, maxActiveSkills);
            }
            retList.Add(availableActiveSkills[rand1]);
            retList.Add(availableActiveSkills[rand2]);
        }
        else//액티브 스킬 가짓수를 모두 채우지 않았으면 모든 리스트에서 가져옴
        {
            int rand1 = UnityEngine.Random.Range(0, 7);
            int rand2 = UnityEngine.Random.Range(0, 7);
            while (rand1 == rand2)
            {
                rand2 = UnityEngine.Random.Range(0, 7);
            }
            retList.Add(availableActiveSkills[rand1]);
            retList.Add(availableActiveSkills[rand2]);
        }
        return retList;
    }
    private List<PassiveSkill> GetRandomPassiveChoices() // 로직에 따라 패시브 스킬 2개 반환
    {
        //액티브와 방식은 같다.
        var retList = new List<PassiveSkill>();
        if (passiveSkills.Count >= maxPassiveSkills)
        {
            int rand1 = UnityEngine.Random.Range(0, maxPassiveSkills);
            int rand2 = UnityEngine.Random.Range(0, maxPassiveSkills);
            while (rand1 == rand2)
            {
                rand2 = UnityEngine.Random.Range(0, maxPassiveSkills);
            }
            retList.Add(availablePassiveSkills[rand1]);
            retList.Add(availablePassiveSkills[rand2]);
        }
        else
        {
            int rand1 = UnityEngine.Random.Range(0, 9);
            int rand2 = UnityEngine.Random.Range(0, 9);
            while (rand1 == rand2)
            {
                rand2 = UnityEngine.Random.Range(0, 9);
            }
            retList.Add(availablePassiveSkills[rand1]);
            retList.Add(availablePassiveSkills[rand2]);
        }
        return retList;
    }
    public static int skillchoice;//ui 버튼으로 입력받아서 int형식으로 저장
    public static void SkillLevelUP()
    {
        //0레벨이면 리스트에 등록, 1레벨 이상이면 레벨업만
        if (skillchoice < 2)
        {
            LevelUpSkill(skillchoices[skillchoice]);//고른 스킬이 액티브일때 고른 스킬과 활성화된 스킬 리스트를 매개로 하는 함수 호출
        }
        else
        {
            LevelUpPassiveSkill(passivechoices[skillchoice - 2]);//고른 스킬이 패시브일때 고른 스킬과 활성화된 스킬 리스트를 매개로 하는 함수 호출
        }
        foreach (Skill active in activeSkills)
        {
            Debug.Log("레벨업된 액티브 스킬 이름" + active.skillName);//지금까지 나온 모든 액티브 스킬의 이름을 콘솔창에 띄움
            Debug.Log("레벨업된 액티브 스킬 레벨" + active.level);
        }
        foreach (PassiveSkill passive in passiveSkills)
        {
            Debug.Log("레벨업된 패시브 스킬 이름" + passive.skillName);//지금까지 나온 모든 패지브 스킬의 이름을 콘솔창에 띄움
            Debug.Log("레벨업된 패시브 스킬 레벨" + passive.level);
        }
        skillchoices.Clear();//랜덤으로 두 개씩 뽑은 스킬 리스트들 초기화.
        passivechoices.Clear();
    }
    static bool cantfind = true;//지금까지 고른 적 없는 스킬일 때 true
    private static void LevelUpSkill(Skill skill)
    {
        cantfind = true;//일단 true로 초기화
        foreach (Skill active in activeSkills)
        {
            if (active == skill)//지금까지 고른 스킬리스트에 레벨업할 스킬이 있다면 실행.
            {
                active.LevelUp();//스킬 레벨업(고른 적 있는 리스트의)
                cantfind = false;//고른적 있다고 바꿔버림.
                break;
            }
        }
        if (cantfind)//이전 if문이 발동되지 않음(리스트에 없음)
        {
            activeSkills.Add(skill);//리스트에 없다는 이야기이므로 일단 레벨업
            foreach (Skill active in activeSkills)//아까 Add한 스킬 찾아서 레벨업.
            {
                if (active == skill)
                {
                    active.LevelUp();
                    cantfind = false;
                    break;
                }
            }
        }
    }
    private static void LevelUpPassiveSkill(PassiveSkill skill)
    {
        cantfind = true;
        foreach (PassiveSkill passive in passiveSkills)
        {
            if (passive == skill)
            {
                passive.LevelUp();
                cantfind = false;
                break;
            }
        }
        if (cantfind)
        {
            passiveSkills.Add(skill);
            foreach (PassiveSkill passive in passiveSkills)
            {
                if (passive == skill)
                {
                    passive.LevelUp();
                    cantfind = false;
                    break;
                }
            }
        }
    }
}