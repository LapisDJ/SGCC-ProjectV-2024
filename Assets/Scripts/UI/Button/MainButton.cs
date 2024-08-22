using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainButton : MonoBehaviour
{
    //main scene
    public void OnClickNewgame()
    {
        LoadingSceneController.Loadscene("Character choose");
    }
    public void OnClickBringup()
    {
        SceneManager.LoadScene("?");
    }
    public void OnClickSettings()
    {
        SceneManager.LoadScene("Settings");
    }
    public void OnClickQuit()
    {
        Application.Quit();
    }
    //character choose scene
    int characternum;
    public void FirstCharacterButton()
    {
        characternum = 1;
    }
    public void Confirmbutton()
    {
        SceneManager.LoadScene("Prototype");
    }
    public void Previousbutton()
    {
        SceneManager.LoadScene("Main Menu");
    }
    //finish scene
    [SerializeField] GameObject recheckwindow;
    void Start()
    {
        recheckwindow.SetActive(false);
    }
    public void GotoMainmenuButton()
    {
        recheckwindow.SetActive(true);
    }
    public void Recheckyes()
    {
        LoadingSceneController.Loadscene("Main Menu");
    }
    public void Recheckno()
    {
        recheckwindow.SetActive(false);
    }
    public void Retry()
    {
        LoadingSceneController.Loadscene("Prototype");
    }
    public void Nextchapter()
    {
        LoadingSceneController.Loadscene("Prototype");
    }
}