using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverDlg : MonoBehaviour
{
    #region Singleton
    private static GameOverDlg instance;
    public static GameOverDlg MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameOverDlg>();
            }
            return instance;
        }
    }
    #endregion

    [SerializeField] private GameObject visible;
    [SerializeField] private Text scoreText;

    public void Open()
    {
        SetScore();
        visible.SetActive(true);
    }

    private void Close()
    {
        visible.SetActive(false);
    }

    private void SetScore()
    {
        scoreText.text = MiniGameManager.MyInstance.GetScore().ToString();
    }

    public void QuitMinigame()
    {
        Close();
        //æ¿ ¿Ãµø, ∫∏ªÛ
    }    
}
