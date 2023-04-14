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
    }

    public void ChangeToMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void ChangeToClearScene()
    {
        SceneManager.LoadScene("ClearScene");
    }
}
