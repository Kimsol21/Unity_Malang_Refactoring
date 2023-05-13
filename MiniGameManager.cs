using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{

    #region Singleton
    //�̱��� ���� : ���α׷� ���� �ν��Ͻ��� �� �ϳ��� �����ϵ��� ����� ����.
    private static MiniGameManager instance; //private �����ڷ� ���� �ν��Ͻ��� �ϳ� ����.
    public static MiniGameManager MyInstance // �̿� �����ϱ� ���� ������Ƽ�� ����.
    {
        get
        {
            if (null==instance) //���� �ϳ����� �ν��Ͻ��� null�̶�� ä���ֱ�.
            {
                instance = FindObjectOfType<MiniGameManager>();
            }
            return instance; // �ν��Ͻ� ��ȯ.
        }
    }
    #endregion

    private Coroutine shopOpen = null; // ���� �����ϰ��� �������� �ڷ�ƾ���� ����.
    private WaitForSeconds WalkTime = new WaitForSeconds(2.0f); // �մ��� �ɾ��������� �ð��� 2�ʷ� ����.
    private int score = 0; // ������ 0���� �ʱ�ȭ.
    private bool isRunning = true; // ������ ���������� true�� �ʱ�ȭ.

    [SerializeField] private GameObject CostomerPrefab; // �մ� ������ ����.
    [SerializeField] private GameObject reaction_Success; //������ ����� ������Ʈ.
    [SerializeField] private GameObject reaction_Fail; //���н� ����� ������Ʈ.
    [SerializeField] private WorkTable workTable; // ���� ������ ���ϴ� å���� ������Ʈ�� ����.
    [SerializeField] private Text scoreText; // ������ ����� �ؽ�Ʈ ����.
    

    #region Property

    #endregion

    void Start()
    {
        if (shopOpen != null) // Ȥ�� ó�� �����Ҷ� �ڷ�ƾ�� null�� �ƴ϶�� 
            StopCoroutine(shopOpen); // �ڷ�ƾ�� ������Ų��. (�ٽ� �ʱ�ȭ�� ����)
        shopOpen = StartCoroutine(ShopOpen()); // �ڷ�ƾ�� �Ʒ� ������ IEnumerator ��ȯ���� �Լ��� �Ѱ��ش�. 
    }

    IEnumerator ShopOpen()
    {
        while (isRunning)
        {           
            GameObject newCostomer = Instantiate(CostomerPrefab); //�մ� �������� ���� �մ� �ν��Ͻ� ����.
            Costomer costomer = newCostomer.GetComponent<Costomer>(); // �մ��ν��Ͻ��� CostomerŬ������ �޾ƿ� ����.
            costomer.SetImage(); //�մ� �ܰ� ���� sprite�� ����.
            costomer.Walk(-1, -1.0f, 3.0f); // �մ� ����. (����, ��ǥ��ġ, �ӵ�)

            yield return WalkTime; //�մ��� �ɾ�ö����� ���. (�ش� �ð��� ������ ���� ��ٷȴٰ� �Լ� �ӽ� ���� �� CPU�� �ٸ��� �ϵ��� ���� �Ѵ�.)
 
            workTable.MakeOrderSheet(); //�ֹ��� ����.

            Timer.MyInstance.ResetTimer(); // Ÿ�̸� ����.
            CalculateTimeAcceleration(); //������ ���� �ð����� ���

            while (Timer.MyInstance.IsTimerStart) // Ÿ�̸� start�� true�϶�����.
            {                                 
                yield return new WaitForEndOfFrame(); //���������� ���� ������ ��ٸ� �� CPU �ٸ����� ����.
                //������� ���̽�ũ�� ���� ����

                if (WorkTable.MyInstance.isCostomerExit) // ���� �մ��� �����ǻ縦 �����ٸ�, 
                {
                    //������� ���̽�ũ�� ���� �Ұ�
                    SetCostomerReaction(); //���⼭ �ɾ�� ���� ���� �ѹ� �����ֱ�
                    yield return new WaitForSeconds(1.0f); // �ش� �ð���ŭ ��ٸ� �� �ڷ�ƾ �Լ� �ӽø���. 
                    UnActiveReaction();//���� ���ֱ�.

                    costomer.Walk(-1, -5.0f, 2.0f); //�մ� �̵���Ű��.
                    WorkTable.MyInstance.isCostomerExit = false; // �մ��� �������� Ȯ���ϴ� ������ false�� ����.
                    Destroy(newCostomer, 3.0f); // 3�� �� �մ� Destroy. 
                }
            }
                

            //  ��� ���ϸ� Ż��
            if(LifeManager.MyInstance.LifeCount <= 0)
            {              
                ShopClose(); //bool�� ����.
                GameOverDlg.MyInstance.Open(); // ���ӿ��� UI ����.
            }    

        }
        //Debug.Log("������ ��Ĩ�ϴ�.");
    }
    private void SetCostomerReaction() // �մ��� ���׼��� �������ִ� �Լ�.
    {
        WorkTable.MyInstance.RemoveOrderSheet(); // �ֹ����� �����.

        if (WorkTable.MyInstance.isTurnFailed) // ���� �ֹ����ۿ� �����ߴٸ� ���� ������ ���
            ActiveFailReaction();
        else // �����ߴٸ� ���� ���׼� ���.
            ActiveSuccessReaction();
    }

    public void SetScore(int m_score) // ������ ǥ�����ִ� �Լ�. ���ڷ� ���� ������ ���� ������ ���� UI�� ǥ�����ش�. 
    {
        score += m_score;
        scoreText.text = score.ToString();
    }
    public int GetScore() // ���� ������ �޾ƿ��� �Լ�.
    {
        return score;
    }

    public void ShopClose() // ���� ���ݴ� �Լ�.
    {
        isRunning = false;
    }
    private void CalculateTimeAcceleration() //������ ���� �ð����� ���
    {
        if (score < 100)
        {
            Timer.MyInstance.SetTimeAcceleration(1.0f); //1�ܰ� 
        }            
        else if (score >=100 && score <200)
        {
            Timer.MyInstance.SetTimeAcceleration(3.0f); //2�ܰ�
        }           
        else if (score >= 200 && score < 300)
        {
            Timer.MyInstance.SetTimeAcceleration(5.0f); //3�ܰ�
        }          
        else if (score >= 300 && score < 400)
        {
            Timer.MyInstance.SetTimeAcceleration(7.0f); //4�ܰ�
        }          
        else
        {
            Timer.MyInstance.SetTimeAcceleration(9.0f); //5�ܰ�
        }
    }
    private void ActiveSuccessReaction() // ���� ���׼� ȭ�鿡 ���
    {
        reaction_Success.SetActive(true);       
    }
    private void ActiveFailReaction() // ���� ���׼� ȭ�鿡 ���
    {
        reaction_Fail.SetActive(true);
    }
    private void UnActiveReaction() // ���׼ǳ������� �����.
    {
        reaction_Fail.SetActive(false);
        reaction_Success.SetActive(false);
    }
}
