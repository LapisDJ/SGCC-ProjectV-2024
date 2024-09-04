using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;


public class MiddleBossActivator : PlayerController
{
    public GameObject middleBoss;   // 중간보스 오브젝트                                    // tag 사용해서 자동으로 불러오기
    public UnityEngine.Transform player;    // 플레이어 위치                                // tag 사용해서 자동으로 불러오기
    private bool isRestore = false; // 신호타워 복구 여부
    private Vector3 finVector = new Vector3(1.5f, -2f, 0);  // 마지막 미션 이동 위치
    private float hp_prev;
    private float hp_cur;                                                              // Player_Controller.cs에서 public ( 아직 미구현 )
    private bool isBossAppear = false;  // 중간보스 등장 여부
    private float interactionTime = 0f;                                                     // Player_Controller.cs에서 public
    private Vector3 interactPlayerPosition;                                                 // Player_Controller.cs에서 public
    private float requiredInteractionTime = 10.0f;                                          // Player_Controller.cs에서 public
    private bool isBossDead = false;    // 중간보스가 처치되었는지 여부

    private new void Start()
    {
        GameObject player = GameObject.Find("Player"); // Player 오브젝트 찾기
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.questPosition = new Vector3(1.5f, 35f, 0);
            }
        }
        questPosition = new Vector3(1.5f, 35f, 0);
        player_T = GameObject.Find("Player").transform;
        middleBoss.SetActive(false);    // Map2 입장 시 중간보스 ( 해킹된 안드로이드 ) 비활성화 
        isInteractionStarted = false;   // 상호작용 미션 완료 여부
        Debug.Log("퀘스트2을 시작합니다!");
        Debug.Log("미션 1 : 경로를 따라가서 중간보스를 처치하세요");
    }


    private void Update()
    {
        GameObject player = GameObject.Find("Player"); // Player 오브젝트 찾기
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                bool isWithinXRange = player_T.position.x >= -0.5f && player_T.position.x <= 2.5f;  // 중간보스 소환 지점 X좌표 범위에 있는지
                bool isWithinYRange = player_T.position.y >= 34f && player_T.position.y <= 36f;     // 중간보스 소환 지점 y좌표 범위에 있는지

                if (isWithinXRange && isWithinYRange && !isBossAppear)  // 중간보스 소환 범위에 있으며 보스가 아직 소환되지 않은 경우 새로운 미션으로 보스를 소환해서 처치하는 미션을 시작
                {
                    Debug.Log("중간보스가 소환되었습니다!!");
                    middleBoss.SetActive(true); // 중간보스 소환
                    isBossAppear = true;    // 중간보스 소환여부 True로 변환 : 중간보스는 1회만 출현
                }

                if (middleBoss != null && Input.GetKeyDown(KeyCode.O))  // 임시로 퀘스트 클리어 위해 만든 치트키 42~45 Lines는 삭제 예정
                {
                    Destroy(middleBoss); // 보스 파괴
                }

                if (!isBossDead && middleBoss == null)
                {
                    Debug.Log("중간보스를 처치하였습니다!");
                    playerController.questPosition = new Vector3(1.5f, 18f, 0);
                    Debug.Log("신호타워를 복구해 주세요!");
                    isBossDead = true;
                }


                if (middleBoss == null && Vector3.Distance(player_T.position, transform.position) <= 3f && !isRestore)    // 중간보스가 처치되고 신호타워와 상호작용할 조건을 만족할 경우 다음 미션 진행
                {
                    if (!isInteractionStarted && Input.GetKey(KeyCode.F))   // F키를 누르면 상호작용 시작
                    {
                        interactionTime = 0.1f; // 상호작용 시작하면 상호작용 시간 초기화
                        Debug.Log("고장난 신호 타워를 복구합니다");
                        interactPlayerPosition = player_T.position;   // 상호작용 시작시 플레이어 위치를 기록
                        playerController.isInteractionStarted = true;
                        isInteractionStarted = true;
                    }

                    if (interactionTime > 0f)
                    {
                        interactionTime += Time.deltaTime;
                    }

                    if (interactionTime >= requiredInteractionTime)
                    {
                        Debug.Log("신호 타워 복구 완료");
                        isRestore = true;
                        playerController.questPosition = finVector;
                        playerController.isInteractionStarted = false;
                        Debug.Log("출발 지점으로 돌아가세요");
                    }
                    else if (isInteractionStarted && Input.GetKey(KeyCode.G))  // 구출 중단 조건
                    {
                        Debug.Log("신호 타워 복구 중단");
                        playerController.isInteractionStarted = false;
                        isInteractionStarted = false;
                        interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
                    }
                    else if (hp_cur < hp_prev)
                    {
                        // 상호작용이 중단되었을 때
                        Debug.Log("신호 타워 복구 실패");
                        playerController.isInteractionStarted = false;
                        isInteractionStarted = false;
                        interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
                    }
                }

                if (isRestore && (Vector3.Distance(finVector, player_T.position)) < 3f)
                {
                    Debug.Log("퀘스트2 클리어!");
                    QuestManager.instance.CompleteQuest();
                }
            }
        }
        hp_prev = hp_cur;
    }
}