using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEX : MonoBehaviour
{
    public static SceneManagerEX instance;
    public void DebugChangeToMainGameScene()
    {
        SceneManager.LoadScene("RanccatTestScene");
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
