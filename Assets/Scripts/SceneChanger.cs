using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneChanger : MonoBehaviour
{
    public void OnSumbit()
    {
        LoadScene();
    }

    public void OnCancel()
    {
        LoadScene();
    }

    void LoadScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
