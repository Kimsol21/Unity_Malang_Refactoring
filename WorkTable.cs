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

    [SerializeField] private GameObject talkBox; //��ǳ��
    [SerializeField] private GameObject myIceCream_Parents; // ���� ���� ���̽�ũ��
    [SerializeField] private GameObject orderedIceCream_Parents; // �մ��� �ֹ��� ���̽�ũ��
    [SerializeField] private GameObject[] IceCreamPrefabs; //��ȯ�� ��� prefab��
    [SerializeField] private Vector2 SpawnPoint; //�� ���̽�ũ�� ���� ����

    public bool isTurnFailed = false;
    public bool isCostomerExit = false;

    // �ֹ��� ����Ʈ
    private List<IceCreamType> OrderSheetList = new List<IceCreamType>();

    // �� ���̽�ũ�� ����Ʈ
    private List<IceCreamType> MyIceCreamList = new List<IceCreamType>();

    public void MakeOrderSheet() //�ֹ��� ����
    {
        if (orderedIceCream_Parents.transform.childCount > 0)//�ֹ��� �ʱ�ȭ
            RemoveOrderSheet();//�ֹ��� ���ۿ� �ռ� ���� �ֹ��� remove
        
        int FlavorNum = Random.Range(1, 6); //���̽�ũ�� ���� ���� ������ ����.
        OrderSheetList.Add((IceCreamType)Random.Range(0, 2)); //Base ����.
        for(int i = 0; i<FlavorNum; i++)
            OrderSheetList.Add((IceCreamType)Random.Range(2, 6)); //Flavor ����.

        ShowOrderSheet(); //�ֹ����� ȭ�鿡 �����ֱ�.
    }
    private void ShowOrderSheet() //�ֹ��� �����ֱ�
    {
        talkBox.SetActive(true);//��ǳ�� �����ֱ�.
        foreach (IceCreamType element in OrderSheetList) // enum���� �̷���� �ֹ��� ��,
            Instantiate(IceCreamPrefabs[(int)element], orderedIceCream_Parents.transform); // ���̽�ũ�� �����չ迭 ���� ���ϴ� �������� ���̽�ũ���� ������ �� �ڸ��� �ν��Ͻ��� �����ϱ�.
    }
    public void RemoveOrderSheet() //�ֹ��� �����ϱ�
    {
        talkBox.SetActive(false);//��ǳ�� �����.
        for(int i = 0; i<OrderSheetList.Count; i++) // �ֹ����� ��� ������ŭ 
            DestroyImmediate(orderedIceCream_Parents.transform.GetChild(0).gameObject); // ���̽�ũ�� Destroy.
        OrderSheetList.Clear(); // �ֹ��� ����Ʈ�� �ʱ�ȭ��Ű��.
    }
    public void MakeMyIceCream(int enumKey) // ������ ���̽�ũ�� ����
    {
        if (!IsRightRules(enumKey) || Timer.MyInstance.IsTimerStart == false) //��Ģ�� ��߳��ų�, Ÿ�̸Ӱ� ���������� ���� �Ұ���.
            return;
        GameObject temp = Instantiate(IceCreamPrefabs[enumKey], myIceCream_Parents.transform); // ���̽�ũ�� �����չ迭���� �Լ� ���ڷ� ���� Ű�� �̿��� �ν��Ͻ� ����
        MyIceCreamList.Add((IceCreamType)enumKey); //�� ���̽�ũ�� ����Ʈ�� �߰�

        if (SpawnPoint == Vector2.zero)//�ʱⰪ�̶��(��or ��)
        {
            SpawnPoint = temp.transform.position; //������ġ ����
            float height = ((IceCreamType)enumKey == IceCreamType.CORN) ? 165.0f : 115.0f; //���� ��� ���� �ݿ�
            SpawnPoint.y += height; //���̸�ŭ ������Ű��
            return;
        }
        temp.transform.position = SpawnPoint; //���� ���� 
        SpawnPoint.y += 110; //���̸�ŭ ������Ű��

        if (IsSameIceCream()) // �����̶�� ���� ���ص� �ڵ� ����.
        {
            SubmitMyIceCream(); 
        }
    }
    private bool IsRightRules(int enumKey) // ���̽�ũ�� ���� ��Ģ�� ��߳��� �ʴ��� �˻��ϱ� ( ù��°�� ���̽�ũ���̸� �ȉ�, �ι�°�� ���̳� �Ź�ħ�̸� �ȉ� ) 
    {
        //������ �迭 �ε������� enum���� ��ȯ.
        IceCreamType iceCreamType = (IceCreamType)enumKey;

        if (MyIceCreamList.Count >6) //�ִ�����ε� ���������Ҷ�
        {
            MessageManager.MyInstance.OpenMessangeBox("���̽�ũ���� �ִ� 6������ ���� �� �ֽ��ϴ�.");
            return false;
        }
        if(IsBottom()) //�عٴ��ε� 
        {
            if (!IsBaseElement(iceCreamType)) //���̽�ũ���������Ҷ�
            {
                MessageManager.MyInstance.OpenMessangeBox("�ݵ�� �� or ���� �����ؾ� �մϴ�.");
                return false;
            }               
        }
        else //��ħ �̹� �׾Ҵµ�
        {
            if (IsBaseElement(iceCreamType)) //��ħ �� �������Ҷ�
            {
                MessageManager.MyInstance.OpenMessangeBox("��, ���� �ٴڿ��� ���� �� �ֽ��ϴ�.");
                return false;
            }            
        }
        return true;
    }
    private bool IsBottom() // �� �عٴ��ΰ�
    {
        return (MyIceCreamList.Count == 0) ? true : false;
    }
    private bool IsBaseElement(IceCreamType iceCreamType) //��ħ�����ΰ�
    {
        return (iceCreamType == IceCreamType.CORN || iceCreamType == IceCreamType.CUP) ? true : false;
    }

    public void RemoveMyIceCream() // �� ���̽�ũ�� �����ϱ�
    {
        SpawnPoint = Vector2.zero; // ���� ����Ʈ ����ġ.

        for (int i = 0; i < MyIceCreamList.Count; i++) // ����Ʈ�����ŭ �ν��Ͻ��� �� ����.
        {
            DestroyImmediate(myIceCream_Parents.transform.GetChild(0).gameObject);         
        }
        MyIceCreamList.Clear(); // ���̽�ũ�� ����Ʈ �ʱ�ȭ.
    }
    public void SubmitMyIceCream() // ���̽�ũ�� �����ϱ�
    {
        if (MyIceCreamList.Count == 0) // �� ���̽�ũ���� ������ �� ����.
            return;

        Timer.MyInstance.SetTimerFalse(); //Ÿ�̸� �����ֱ�

        SpawnPoint = Vector2.zero; //�� ���̽�ũ�� ������ġ �ʱ�ȭ �����ֱ�.

        CheckSuccess();//������ ���̽�ũ���� �ֹ����� �������� Ȯ��.

        InitIceCream(); //���̽�ũ�� ��� �����ֱ�

        isCostomerExit = true; //�մ� ��������

    }


    private void CheckSuccess() //���� or ���� ó���ϱ�
    {
        if (IsSameIceCream()) //����
        {
            MiniGameManager.MyInstance.SetScore(MyIceCreamList.Count*10); //�����ֱ�
            Timer.MyInstance.PlusTimeCost(MyIceCreamList.Count * 3); // �ð� �÷��ֱ�.
            WorkTable.MyInstance.isTurnFailed = false; // ���а� �ƴ��� ����.
        }
        else //����
        {
            Timer.MyInstance.TurnFail();
        }
    }

    public bool IsSameIceCream() // �ֹ����� �� ���̽�ũ�� ���ϱ�
    {
        if (OrderSheetList.SequenceEqual(MyIceCreamList))
            return true;
        else
            return false;
    }
    public void InitIceCream() //���̽�ũ�� ����Ʈ ��ü �ʱ�ȭ �� ����
    {
        RemoveOrderSheet();
        RemoveMyIceCream(); 

    }
}
