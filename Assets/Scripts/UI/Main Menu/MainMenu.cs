using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Camera _mainMenuCamera;

    private void Start()
    {

    }
    public void CloseGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("DiscoLevel", LoadSceneMode.Single);
    }

    public void StartCredits()
    {
        _mainMenuCamera.transform.Rotate(new Vector3(0, 180, 0));
    }

    public void LeaveCredits()
    {
        _mainMenuCamera.transform.Rotate(new Vector3(0, 180, 0));
    }
}
