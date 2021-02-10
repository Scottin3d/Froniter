using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoreLoadScript : MonoBehaviour
{
    void Awake()
    {
        if (SceneManager.GetSceneByName("FrontierCore").isLoaded == false) {
            SceneManager.LoadScene("FrontierCore", LoadSceneMode.Additive);
        } else {
            SceneManager.UnloadSceneAsync("FrontierCore");
        }

    }
}
