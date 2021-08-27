using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonBhvr : MonoBehaviour
{
    public GameObject target;
    [SerializeField] float secondsToWait;

    IEnumerator LoadScene(int levelIndex)
    {
        //Reference Fade In-Out comes here
        yield return new WaitForSeconds(secondsToWait);
        SceneManager.LoadScene(levelIndex);
    }

    public void ShowTarget()
    {
        target.SetActive(true);
    }

    public void LoadNewGame()
    {
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadGame()
    {

    }

    public void Unpause()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
