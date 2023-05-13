using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    #region Singleton
    private static LifeManager instance;
    public static LifeManager MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LifeManager>();
            }
            return instance;
        }
    }
    #endregion

    private int maxLifeCount = 3;
    private int lifeCount;

    #region Property
    public int LifeCount { get => lifeCount; }
    #endregion

    private void Start()
    {
        lifeCount = maxLifeCount;
    }

    public void DecreaseLifeCount()
    {
        lifeCount = Mathf.Clamp(lifeCount - 1, 0, maxLifeCount);

        RemoveLifeImage();
    }

    private void RemoveLifeImage()
    {
        this.transform.GetChild(lifeCount).gameObject.SetActive(false);
    }
}
