using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    // Start is called before the first frame update
    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void InstructionsMenu()
    {
        SceneManager.LoadScene("InstructionsMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
