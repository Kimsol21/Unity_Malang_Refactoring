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
        //position ����
        //�ɾ���� �ִϸ��̼� ���
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
        isWalking = true; // �ȴ����� true�� ����.
        direction = m_direction; // �ȴ� ���� ����.
        targetPosition = m_targetPosition; // ��ǥ ��������.
        moveSpeed = m_moveSpeed; // ���� �ӵ� ����.
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
            else //����
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
            else //����
            {
                isWalking = false;
            }               
        }       
    }
}
