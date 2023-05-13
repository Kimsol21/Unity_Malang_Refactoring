using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    #region Singleton
    private static MessageManager instance;
    public static MessageManager MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MessageManager>();
            }
            return instance;
        }
    }
    #endregion

    [SerializeField] private MessageBox messageBox;

    public void OpenMessangeBox(string message)
    {
        messageBox.gameObject.SetActive(true);
        messageBox.SetMessage(message);
    }


}
