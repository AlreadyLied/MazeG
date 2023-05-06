using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEX : MonoBehaviour
{
    public static SceneManagerEX instance;

    private void Awake()
    {
        instance = this;
    }

    public void ChangeToMainGameScene()
    {
        SceneManager.LoadScene("MainGameScene");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ChangeToRanccatScene()
    {
        SceneManager.LoadSceneAsync("RanccatTestScene");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ChangeToMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ChangeToClearScene()
    {
        SceneManager.LoadScene("ClearScene");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        SceneManager.UnloadSceneAsync("RanccatTestScene");
    }
}
