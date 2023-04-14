using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    private void OnTriggerEnter(Collider coll)
    {
        SceneManagerEX.instance.ChangeToClearScene();
    }
}
