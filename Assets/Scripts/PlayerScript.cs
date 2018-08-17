using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerState { Roam, Escape };

public class PlayerScript : MonoBehaviour {
    public float walkSpeed = 5.0f;
    public CameraScript cameraScript;
    public int lives = 5;
    public GameObject enemy;
    bool invulnerable = false;
    public CanvasScript canvasScript;
    public SoundtrackControllerScript soundScript;

    public PlayerState currentState;
    public EnvironmentType currentEnvironment = EnvironmentType.Forest;

    // Initialize
    void Start () {
        // Lock the cursor to game window and set the initial state to roaming
        Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
        currentState = PlayerState.Roam;
    }
	
	// Update is called once per frame
	void Update () {
        // Equate the y rotation of the camera and player
        transform.rotation = Quaternion.Euler(cameraScript.pitch, cameraScript.yaw, 0.0f);

        // Move the player left and right with horizontal input
        transform.Translate(Input.GetAxis("Horizontal") * walkSpeed * Time.deltaTime, 0.0f, 0.0f);

        // Move the player forwards and backwards with vertical input
        if (Input.GetAxis("Vertical") < 0)
        {
            transform.Translate(-transform.forward * walkSpeed * Time.deltaTime, Space.World);
        }
        else if (Input.GetAxis("Vertical") > 0)
        {
            transform.Translate(transform.forward * walkSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetButtonDown("Jump") && Physics.Raycast(transform.position, Vector3.down, 1.1f))
        {
            transform.GetComponent<Rigidbody>().AddForce(0.0f, 200.0f, 0.0f);
        }

        if (Input.GetButtonDown("Cancel"))
        {
            SceneManager.LoadScene("Menu");
        }
    }

    private void OnCollisionStay(Collision col)
    {
        if (!invulnerable && col.gameObject.tag == "enemy")
        {
            if (lives > 0)
            {
                lives--;
                canvasScript.lives[lives].SetActive(false);
                invulnerable = true;
                soundScript.PlayHitSound();
                Invoke("endCooldown", 1.0f);
                //print("health at " + lives);
            }
            if (lives == 0)
            {
                print("player died");
                SceneManager.LoadScene("Menu");
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "forest" && currentEnvironment != EnvironmentType.Forest)
        {
            print("entering forest");
            currentEnvironment = EnvironmentType.Forest;
        }
        else if (col.gameObject.tag == "clearing" && currentEnvironment != EnvironmentType.Clearing)
        {
            print("entering clearing");
            currentEnvironment = EnvironmentType.Clearing;
        }
        else if (col.gameObject.tag == "warehouse" && currentEnvironment != EnvironmentType.Warehouse)
        {
            print("entering warehouse");
            currentEnvironment = EnvironmentType.Warehouse;
        }
    }

    void endCooldown()
    {
        invulnerable = false;
    }
}