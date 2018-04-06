using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject mainMenuHolder;
    public GameObject optionsMenuHolder;

    public ItemDataParser itemDataParser;

    // 버튼 메소드
    public void Play ()
    {
        SceneManager.LoadScene ("Game");
    }

    public void Quit ()
    {
        Application.Quit ();
    }

    public void OptionsMenu ()
    {
        mainMenuHolder.SetActive (false);
        optionsMenuHolder.SetActive (true);
    }

    public void MainMenu ()
    {
        optionsMenuHolder.SetActive (false);
        mainMenuHolder.SetActive (true);
    }

    private void Start ()
    {
        if (GameObject.FindGameObjectWithTag("ItemDataBase") == null)
        {
            Instantiate (itemDataParser , Vector3.zero , Quaternion.identity);
        }
    }


}
