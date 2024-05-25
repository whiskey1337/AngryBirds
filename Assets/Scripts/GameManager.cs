using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int maxNumberOfShots = 3;

    private int usedNumberOfShots;

    private IconHandler iconHandler;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        iconHandler = FindObjectOfType<IconHandler>();
    }

    public void UseShot()
    {
        usedNumberOfShots++;
        iconHandler.UseShot(usedNumberOfShots);
    }

    public bool HasEnoughShots()
    {
        if (usedNumberOfShots < maxNumberOfShots)
        {
            return true;
        } 
        else
        {
            return false;
        }
    }
}
