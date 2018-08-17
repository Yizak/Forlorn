using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public Button button;

    void Start()
    {
        button.onClick.AddListener(Press);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }
    }

    void Press()
    {
        print("button pressed");
        Cursor.visible = false;
        SceneManager.LoadScene("Game");
    }
}