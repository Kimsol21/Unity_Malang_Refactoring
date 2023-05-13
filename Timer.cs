using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    #region Singleton
    private static Timer instance;
    public static Timer MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Timer>();
            }
            return instance;
        }
    }
    #endregion

    [SerializeField] private Slider TimeSlider;
  
    private bool isTimerStart = false;
    private float timeCost;
    private float timeAcceleration = 1.0f;

    #region Property
    public bool IsTimerStart { get => isTimerStart; }
    #endregion

    private void Start()
    {
        SetMaxTimeCost(10.0f);
    }

    void Update()
    {
        if (isTimerStart) //Ÿ�̸� �۵����ΰ�? true�̸� �ð� ī��Ʈ���ֱ�.
            CountTime();     
    }

    private void CountTime() //�ð� ī��Ʈ
    {
        if (timeCost > 0)
        {
            timeCost -= Time.deltaTime * timeAcceleration;
            TimeSlider.value = timeCost;
        }
        else
        {
            WorkTable.MyInstance.isCostomerExit = true;
            TurnFail();
        }
    }

    public void SetMaxTimeCost(float m_time) //Ÿ�̸� �ִ밪 ����.
    {
        timeCost = m_time; //timeCost ������ �Ű������� ���� �� ����. 
        TimeSlider.maxValue = timeCost; //�����̴� �ִ밪�� timeCost ����.
        TimeSlider.value = TimeSlider.maxValue;
    }
    public void PlusTimeCost(float m_time) //�ð� �ø���
    {
        if (TimeSlider.maxValue < timeCost + m_time)
            SetTimeCost(TimeSlider.maxValue);
        else
            SetTimeCost(timeCost += m_time);
    }
  
    private void SetTimeCost(float m_time)
    {
        timeCost = m_time;
        TimeSlider.value = timeCost;
    }
    public void SetTimerTrue() //Ÿ�̸� ���
    {
        isTimerStart = true;
    }
    public void SetTimerFalse() //Ÿ�̸� ���߱�
    {
        isTimerStart = false;
    }

    public void ResetTimer() //Ÿ�̸� ����
    {
        timeCost = TimeSlider.maxValue;
        TimeSlider.value = timeCost;
        isTimerStart = true;
    }

    public void SetTimeAcceleration(float m_timeAcceleration) //�ð� ���ӵ� ����
    {
        timeAcceleration = m_timeAcceleration;
    }

    public void TurnFail()
    {
        WorkTable.MyInstance.isTurnFailed = true; // �۾����� ���� ������ true�� ����.
        SetTimerFalse(); // Ÿ�̸� ���߱�.
        LifeManager.MyInstance.DecreaseLifeCount(); // ��� �ϳ� ���ҽ�Ű��.
    }

}
