using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    #region Singleton
    private static MessageBox instance;
    public static MessageBox MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MessageBox>();
            }
            return instance;
        }
    }
    #endregion

    // Start is called before the first frame update
    void OnEnable()
    {
        Invoke("Close", 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Close()
    {
        this.gameObject.SetActive(false);
    }
    public void SetMessage(string message)
    {
        this.GetComponentInChildren<Text>().text = message;
    }
}
