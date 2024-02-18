using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Transition transi;
    public void PlayGame()
    {
        transi.Out(1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToMenu()
    {
        transi.Out(0);
    }
}
