using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{

    #region Singleton
    //싱글톤 패턴 : 프로그램 내에 인스턴스가 단 하나만 존재하도록 만드는 패턴.
    private static MiniGameManager instance; //private 지정자로 정적 인스턴스를 하나 만듦.
    public static MiniGameManager MyInstance // 이에 접근하기 위한 프로퍼티를 설정.
    {
        get
        {
            if (null==instance) //만약 하나뿐인 인스턴스가 null이라면 채워주기.
            {
                instance = FindObjectOfType<MiniGameManager>();
            }
            return instance; // 인스턴스 반환.
        }
    }
    #endregion

    private Coroutine shopOpen = null; // 가게 오픈하고의 과정으로 코루틴으로 선언.
    private WaitForSeconds WalkTime = new WaitForSeconds(2.0f); // 손님이 걸어오기까지의 시간을 2초로 설정.
    private int score = 0; // 점수를 0으로 초기화.
    private bool isRunning = true; // 게임이 실행중임을 true로 초기화.

    [SerializeField] private GameObject CostomerPrefab; // 손님 프리팹 선언.
    [SerializeField] private GameObject reaction_Success; //성공시 출력할 오브젝트.
    [SerializeField] private GameObject reaction_Fail; //실패시 출력할 오브젝트.
    [SerializeField] private WorkTable workTable; // 실제 유저가 일하는 책상을 오브젝트로 선언.
    [SerializeField] private Text scoreText; // 점수를 출력할 텍스트 선언.
    

    #region Property

    #endregion

    void Start()
    {
        if (shopOpen != null) // 혹시 처음 시작할때 코루틴이 null이 아니라면 
            StopCoroutine(shopOpen); // 코루틴을 중지시킨다. (다시 초기화를 위해)
        shopOpen = StartCoroutine(ShopOpen()); // 코루틴에 아래 정의한 IEnumerator 반환형의 함수를 넘겨준다. 
    }

    IEnumerator ShopOpen()
    {
        while (isRunning)
        {           
            GameObject newCostomer = Instantiate(CostomerPrefab); //손님 프리팹을 통해 손님 인스턴스 생성.
            Costomer costomer = newCostomer.GetComponent<Costomer>(); // 손님인스턴스의 Costomer클래스를 받아와 저장.
            costomer.SetImage(); //손님 외관 랜덤 sprite로 설정.
            costomer.Walk(-1, -1.0f, 3.0f); // 손님 입장. (방향, 목표위치, 속도)

            yield return WalkTime; //손님이 걸어올때까지 대기. (해당 시간이 지나갈 동안 기다렸다가 함수 임시 리턴 후 CPU가 다른일 하도록 위임 한다.)
 
            workTable.MakeOrderSheet(); //주문서 생성.

            Timer.MyInstance.ResetTimer(); // 타이머 리셋.
            CalculateTimeAcceleration(); //점수에 따른 시간가속 계산

            while (Timer.MyInstance.IsTimerStart) // 타이머 start가 true일때동안.
            {                                 
                yield return new WaitForEndOfFrame(); //한프레임이 끝날 때까지 기다린 후 CPU 다른곳에 위임.
                //여기부터 아이스크림 제작 가능

                if (WorkTable.MyInstance.isCostomerExit) // 만약 손님이 퇴장의사를 밝혔다면, 
                {
                    //여기부터 아이스크림 제작 불가
                    SetCostomerReaction(); //여기서 걸어가기 전에 반응 한번 보여주기
                    yield return new WaitForSeconds(1.0f); // 해당 시간만큼 기다린 후 코루틴 함수 임시리턴. 
                    UnActiveReaction();//반응 꺼주기.

                    costomer.Walk(-1, -5.0f, 2.0f); //손님 이동시키기.
                    WorkTable.MyInstance.isCostomerExit = false; // 손님이 나갔는지 확인하는 변수를 false로 설정.
                    Destroy(newCostomer, 3.0f); // 3초 후 손님 Destroy. 
                }
            }
                

            //  목숨 다하면 탈출
            if(LifeManager.MyInstance.LifeCount <= 0)
            {              
                ShopClose(); //bool값 설정.
                GameOverDlg.MyInstance.Open(); // 게임오버 UI 띄우기.
            }    

        }
        //Debug.Log("게임을 마칩니다.");
    }
    private void SetCostomerReaction() // 손님의 리액션을 설정해주는 함수.
    {
        WorkTable.MyInstance.RemoveOrderSheet(); // 주문지를 지운다.

        if (WorkTable.MyInstance.isTurnFailed) // 만약 주문제작에 실패했다면 실패 리엑션 출력
            ActiveFailReaction();
        else // 성공했다면 성공 리액션 출력.
            ActiveSuccessReaction();
    }

    public void SetScore(int m_score) // 점수를 표시해주는 함수. 인자로 받은 점수를 현재 점수에 더해 UI에 표현해준다. 
    {
        score += m_score;
        scoreText.text = score.ToString();
    }
    public int GetScore() // 현재 점수를 받아오는 함수.
    {
        return score;
    }

    public void ShopClose() // 가게 문닫는 함수.
    {
        isRunning = false;
    }
    private void CalculateTimeAcceleration() //점수에 따른 시간가속 계산
    {
        if (score < 100)
        {
            Timer.MyInstance.SetTimeAcceleration(1.0f); //1단계 
        }            
        else if (score >=100 && score <200)
        {
            Timer.MyInstance.SetTimeAcceleration(3.0f); //2단계
        }           
        else if (score >= 200 && score < 300)
        {
            Timer.MyInstance.SetTimeAcceleration(5.0f); //3단계
        }          
        else if (score >= 300 && score < 400)
        {
            Timer.MyInstance.SetTimeAcceleration(7.0f); //4단계
        }          
        else
        {
            Timer.MyInstance.SetTimeAcceleration(9.0f); //5단계
        }
    }
    private void ActiveSuccessReaction() // 성공 리액션 화면에 출력
    {
        reaction_Success.SetActive(true);       
    }
    private void ActiveFailReaction() // 실패 리액션 화면에 출력
    {
        reaction_Fail.SetActive(true);
    }
    private void UnActiveReaction() // 리액션끝났으면 지우기.
    {
        reaction_Fail.SetActive(false);
        reaction_Success.SetActive(false);
    }
}
