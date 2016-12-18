using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScene : MonoBehaviour {

    void Start()
    {
        string[] names = UnityEngine.Input.GetJoystickNames();
        for (int i = 0; i < names.Length; i++)
        {
            Debug.Log(names[i]);
        }

    }
    void Update()
    {

        if (Input.GetButtonDown("ResetButton"))
        {
            Debug.Log("Button PRESS!");
            SceneManager.LoadScene("MainScreen");
        }
    }
}
