using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public CanvasGroup GameUIcanvasGroup;
    public GameObject pauseMenu;
    public Text pauseMenuText;

    string pauseText = "PAUSE";
    string gameOverText = "GAEM OVER";

    bool isPause;
    bool IsPause {

        get { return isPause;  }

        set
        {
            if (value)
            {
                GameUIcanvasGroup.alpha = 0.5f;
                Time.timeScale = 0f;
                pauseMenu.SetActive (true);
            }
            else
            {
                GameUIcanvasGroup.alpha = 1f;
                Time.timeScale = 1f;
                pauseMenu.SetActive (false);
            }

            isPause = value;
        }
    }

    public void GameOver ()
    {
        pauseMenuText.text = gameOverText;
        Pause ();
    }

    private void Start ()
    {
        IsPause = false;
    }

    public void OnClickPauseButton ()
    {
        Pause ();
    }

    void Pause ()
    {
        if (IsPause)
        {
            IsPause = false;
        }
        else
        {
            IsPause = true;
        }
    }

    public void OnClickMainButton ()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene ("Main");
    }

    public void OnClickQuitButton ()
    {
        Time.timeScale = 1f;
        Application.Quit ();
    }

    public void OnClickRestartButton ()
    {
        Time.timeScale = 1f;
        IsPause = false;
        pauseMenuText.text = pauseText;
        GameManager.Instance.NewGame ();
    }
}
