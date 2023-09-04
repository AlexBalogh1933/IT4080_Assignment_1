using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Action for when you click Play
    public void OnPlayButton ()
    {
        SceneManager.LoadScene(1);
    }

    // Action for when you click Quit
    public void OnQuitButton ()
    {
        Application.Quit();
    }
}
