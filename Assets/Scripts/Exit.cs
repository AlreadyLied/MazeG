using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    private void OnTriggerEnter(Collider coll)
    {
        SceneManager.LoadScene("ClearScene");
    }
}
