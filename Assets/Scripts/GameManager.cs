using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int maxNumberOfShots = 3;
    [SerializeField] private float secondsToWaitBeforeDeathCheck = 3f;
    [SerializeField] private GameObject restartScreenObject;
    [SerializeField] private SlingshotHandler slingshotHandler;
    [SerializeField] private Image nextLevelImage;


    private int usedNumberOfShots;

    private IconHandler iconHandler;

    private List<Piggie> piggies = new List<Piggie>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        iconHandler = FindObjectOfType<IconHandler>();

        Piggie[] array_piggies = FindObjectsOfType<Piggie>();
        for (int i = 0; i < array_piggies.Length; i++)
        {
            piggies.Add(array_piggies[i]);
        }

        nextLevelImage.enabled = false;
    }

    public void UseShot()
    {
        usedNumberOfShots++;
        iconHandler.UseShot(usedNumberOfShots);

        CheckForLastShot();
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

    public void CheckForLastShot()
    {
        if (usedNumberOfShots == maxNumberOfShots)
        {
            StartCoroutine(CheckAfterWaitTime());
        }
    }

    private IEnumerator CheckAfterWaitTime()
    {
        yield return new WaitForSeconds(secondsToWaitBeforeDeathCheck);

        if (piggies.Count == 0)
        {
            WinGame();
        }
        else
        {
            RestartGame();
        }
    }

    public void RemovePiggie(Piggie piggie)
    {
        piggies.Remove(piggie);
        CheckForAllDeadPiggies();
    }

    private void CheckForAllDeadPiggies()
    {
        if (piggies.Count == 0)
        {
            WinGame();
        }
    }

    #region Win/Lose

    private void WinGame()
    {
        restartScreenObject.SetActive(true);
        slingshotHandler.enabled = false;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int maxLevels = SceneManager.sceneCountInBuildSettings;
        if (currentSceneIndex + 1 < maxLevels)
        {
            nextLevelImage.enabled = true;
        }
    }

    public void RestartGame()
    {
        DOTween.Clear(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    #endregion
}
