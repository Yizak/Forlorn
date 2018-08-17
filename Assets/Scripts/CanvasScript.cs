using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScript : MonoBehaviour {
    public GameObject ControlsMenuHint;
    public GameObject ControlsMenu;
    bool showControls = false;
    public GameObject[] lives;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!showControls)
            {
                ControlsMenuHint.SetActive(false);
                ControlsMenu.SetActive(true);
                showControls = true;
            }
            else
            {
                ControlsMenuHint.SetActive(true);
                ControlsMenu.SetActive(false);
                showControls = false;
            }
        }
    }
}
