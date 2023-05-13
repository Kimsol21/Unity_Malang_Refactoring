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
        if (isTimerStart) //타이머 작동중인가? true이면 시간 카운트해주기.
            CountTime();     
    }

    private void CountTime() //시간 카운트
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

    public void SetMaxTimeCost(float m_time) //타이머 최대값 설정.
    {
        timeCost = m_time; //timeCost 변수에 매개변수로 받은 값 대입. 
        TimeSlider.maxValue = timeCost; //슬라이더 최대값에 timeCost 대입.
        TimeSlider.value = TimeSlider.maxValue;
    }
    public void PlusTimeCost(float m_time) //시간 늘리기
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
    public void SetTimerTrue() //타이머 재생
    {
        isTimerStart = true;
    }
    public void SetTimerFalse() //타이머 멈추기
    {
        isTimerStart = false;
    }

    public void ResetTimer() //타이머 리셋
    {
        timeCost = TimeSlider.maxValue;
        TimeSlider.value = timeCost;
        isTimerStart = true;
    }

    public void SetTimeAcceleration(float m_timeAcceleration) //시간 가속도 설정
    {
        timeAcceleration = m_timeAcceleration;
    }

    public void TurnFail()
    {
        WorkTable.MyInstance.isTurnFailed = true; // 작업대의 실패 변수를 true로 설정.
        SetTimerFalse(); // 타이머 멈추기.
        LifeManager.MyInstance.DecreaseLifeCount(); // 목숨 하나 감소시키기.
    }

}
