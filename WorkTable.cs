using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class WorkTable : MonoBehaviour
{
    #region Singleton
    private static WorkTable instance;
    public static WorkTable MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<WorkTable>();
            }
            return instance;
        }
    }
    #endregion

    public enum IceCreamType { CUP, CORN, LEMON, APPLE, ORANGE, BLUE };

    [SerializeField] private GameObject talkBox; //말풍선
    [SerializeField] private GameObject myIceCream_Parents; // 내가 만든 아이스크림
    [SerializeField] private GameObject orderedIceCream_Parents; // 손님이 주문한 아이스크림
    [SerializeField] private GameObject[] IceCreamPrefabs; //소환할 재료 prefab들
    [SerializeField] private Vector2 SpawnPoint; //내 아이스크림 스폰 지점

    public bool isTurnFailed = false;
    public bool isCostomerExit = false;

    // 주문서 리스트
    private List<IceCreamType> OrderSheetList = new List<IceCreamType>();

    // 내 아이스크림 리스트
    private List<IceCreamType> MyIceCreamList = new List<IceCreamType>();

    public void MakeOrderSheet() //주문서 제작
    {
        if (orderedIceCream_Parents.transform.childCount > 0)//주문서 초기화
            RemoveOrderSheet();//주문서 제작에 앞서 기존 주문서 remove
        
        int FlavorNum = Random.Range(1, 6); //아이스크림 맛을 몇층 쌓을지 결정.
        OrderSheetList.Add((IceCreamType)Random.Range(0, 2)); //Base 결정.
        for(int i = 0; i<FlavorNum; i++)
            OrderSheetList.Add((IceCreamType)Random.Range(2, 6)); //Flavor 결정.

        ShowOrderSheet(); //주문내역 화면에 보여주기.
    }
    private void ShowOrderSheet() //주문서 보여주기
    {
        talkBox.SetActive(true);//말풍선 보여주기.
        foreach (IceCreamType element in OrderSheetList) // enum으로 이루어진 주문서 뜯어서,
            Instantiate(IceCreamPrefabs[(int)element], orderedIceCream_Parents.transform); // 아이스크림 프리팹배열 안의 원하는 프리팹을 아이스크림이 놓여야 할 자리에 인스턴스로 생성하기.
    }
    public void RemoveOrderSheet() //주문서 삭제하기
    {
        talkBox.SetActive(false);//말풍선 숨기기.
        for(int i = 0; i<OrderSheetList.Count; i++) // 주문서의 요소 갯수만큼 
            DestroyImmediate(orderedIceCream_Parents.transform.GetChild(0).gameObject); // 아이스크림 Destroy.
        OrderSheetList.Clear(); // 주문서 리스트도 초기화시키기.
    }
    public void MakeMyIceCream(int enumKey) // 유저의 아이스크림 제작
    {
        if (!IsRightRules(enumKey) || Timer.MyInstance.IsTimerStart == false) //규칙에 어긋나거나, 타이머가 멈춰있으면 생성 불가능.
            return;
        GameObject temp = Instantiate(IceCreamPrefabs[enumKey], myIceCream_Parents.transform); // 아이스크림 프리팹배열에서 함수 인자로 받은 키를 이용해 인스턴스 생성
        MyIceCreamList.Add((IceCreamType)enumKey); //내 아이스크림 리스트에 추가

        if (SpawnPoint == Vector2.zero)//초기값이라면(콘or 컵)
        {
            SpawnPoint = temp.transform.position; //스폰위치 저장
            float height = ((IceCreamType)enumKey == IceCreamType.CORN) ? 165.0f : 115.0f; //콘일 경우 높이 반영
            SpawnPoint.y += height; //높이만큼 증가시키기
            return;
        }
        temp.transform.position = SpawnPoint; //높이 적용 
        SpawnPoint.y += 110; //높이만큼 증가시키기

        if (IsSameIceCream()) // 성공이라면 제출 안해도 자동 제출.
        {
            SubmitMyIceCream(); 
        }
    }
    private bool IsRightRules(int enumKey) // 아이스크림 제작 규칙에 어긋나진 않는지 검사하기 ( 첫번째가 아이스크림이면 안됌, 두번째가 콘이나 컵받침이면 안됌 ) 
    {
        //프리팹 배열 인덱스값을 enum으로 변환.
        IceCreamType iceCreamType = (IceCreamType)enumKey;

        if (MyIceCreamList.Count >6) //최대높이인데 더쌓으려할때
        {
            MessageManager.MyInstance.OpenMessangeBox("아이스크림은 최대 6개까지 쌓을 수 있습니다.");
            return false;
        }
        if(IsBottom()) //밑바닥인데 
        {
            if (!IsBaseElement(iceCreamType)) //아이스크림쌓으려할때
            {
                MessageManager.MyInstance.OpenMessangeBox("반드시 콘 or 컵을 선택해야 합니다.");
                return false;
            }               
        }
        else //받침 이미 쌓았는데
        {
            if (IsBaseElement(iceCreamType)) //받침 또 쌓으려할때
            {
                MessageManager.MyInstance.OpenMessangeBox("콘, 컵은 바닥에만 쌓을 수 있습니다.");
                return false;
            }            
        }
        return true;
    }
    private bool IsBottom() // 맨 밑바닥인가
    {
        return (MyIceCreamList.Count == 0) ? true : false;
    }
    private bool IsBaseElement(IceCreamType iceCreamType) //받침종류인가
    {
        return (iceCreamType == IceCreamType.CORN || iceCreamType == IceCreamType.CUP) ? true : false;
    }

    public void RemoveMyIceCream() // 내 아이스크림 삭제하기
    {
        SpawnPoint = Vector2.zero; // 스폰 포인트 원위치.

        for (int i = 0; i < MyIceCreamList.Count; i++) // 리스트사이즈만큼 인스턴스들 다 제거.
        {
            DestroyImmediate(myIceCream_Parents.transform.GetChild(0).gameObject);         
        }
        MyIceCreamList.Clear(); // 아이스크림 리스트 초기화.
    }
    public void SubmitMyIceCream() // 아이스크림 제출하기
    {
        if (MyIceCreamList.Count == 0) // 빈 아이스크림은 제출할 수 없음.
            return;

        Timer.MyInstance.SetTimerFalse(); //타이머 멈춰주기

        SpawnPoint = Vector2.zero; //내 아이스크림 스폰위치 초기화 시켜주기.

        CheckSuccess();//제출한 아이스크림이 주문서와 동일한지 확인.

        InitIceCream(); //아이스크림 모두 지워주기

        isCostomerExit = true; //손님 내보내기

    }


    private void CheckSuccess() //성공 or 실패 처리하기
    {
        if (IsSameIceCream()) //성공
        {
            MiniGameManager.MyInstance.SetScore(MyIceCreamList.Count*10); //점수주기
            Timer.MyInstance.PlusTimeCost(MyIceCreamList.Count * 3); // 시간 늘려주기.
            WorkTable.MyInstance.isTurnFailed = false; // 실패가 아님을 저장.
        }
        else //실패
        {
            Timer.MyInstance.TurnFail();
        }
    }

    public bool IsSameIceCream() // 주문서와 내 아이스크림 비교하기
    {
        if (OrderSheetList.SequenceEqual(MyIceCreamList))
            return true;
        else
            return false;
    }
    public void InitIceCream() //아이스크림 리스트 전체 초기화 및 삭제
    {
        RemoveOrderSheet();
        RemoveMyIceCream(); 

    }
}
