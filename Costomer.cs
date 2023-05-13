using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Costomer : MonoBehaviour
{
    private float curPosition;
    private float targetPosition;
    private float moveSpeed;
    private int direction;
    private bool isWalking = false;
    [SerializeField] private List<Sprite> sprites = new List<Sprite>();


    void Start()
    {
        curPosition = transform.position.x;
        //position 조정
        //걸어오는 애니메이션 재생
    }

    void Update()
    {
        if(isWalking)
        {
            Moving();
        }
            
    }

    public void SetImage()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Count)];
    }

    public void Walk(int m_direction, float m_targetPosition, float m_moveSpeed)
    {
        isWalking = true; // 걷는중을 true로 설정.
        direction = m_direction; // 걷는 방향 설정.
        targetPosition = m_targetPosition; // 목표 지점설정.
        moveSpeed = m_moveSpeed; // 걸을 속도 지정.
    }
    private void Moving()
    {
        if(direction == 1)
        {
            if (targetPosition >= transform.position.x)
            {
                curPosition += Time.deltaTime * direction * moveSpeed;
                transform.position = new Vector3(curPosition, transform.position.y, 0);
            }
            else //도착
            {
                isWalking = false;
            }               
        }
        else
        {
            if (targetPosition <= transform.position.x)
            {
                curPosition += Time.deltaTime * direction * moveSpeed;
                transform.position = new Vector3(curPosition, transform.position.y, 0);
            }
            else //도착
            {
                isWalking = false;
            }               
        }       
    }
}
