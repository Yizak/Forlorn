using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour {
    public Transform playerTransform;
    float enemySpeed = 1.0f;
    float roamSpeed = 1.0f;
    float chaseSpeed = 4.35f;
    bool roam = true;
    float distance = 100.0f;
    Vector3 target;

	// Use this for initialization
	void Start () {
        InvokeRepeating("Roaming", 5, 5);
	}
	
	// Update is called once per frame
	void Update () {
        distance = Mathf.Abs((playerTransform.position - transform.position).magnitude);

        if (Random.Range(0,100) == 0)
        {
            // randomly turn renderer off for a fraction of a second
            FlickerOn();
        }

        if (distance > 80.0f)
        {
            if (!roam) roam = true;
            transform.position = Vector3.MoveTowards(transform.position, target, enemySpeed * Time.deltaTime);
        }
        else
        {
            if (roam) roam = false;

            if (distance > 16.0f && enemySpeed == chaseSpeed)
            {
                enemySpeed = roamSpeed;
            }
            else if (enemySpeed == roamSpeed)
            {
                enemySpeed = chaseSpeed;
            }

            if (distance > 16.0f)
            {
                target = playerTransform.position;
                target.y += 6.0f;

                transform.position = Vector3.MoveTowards(transform.position, target, enemySpeed * Time.deltaTime);
            }
            else
            {
                target = playerTransform.position;
                target.y += 0.5f;

                transform.position = Vector3.MoveTowards(transform.position, target, enemySpeed * Time.deltaTime);
            }

            transform.LookAt(playerTransform);
        }
    }

    void Roaming()
    {
        if (roam)
        {
            //print("monster is changing course");
            target = transform.position;
            target.x += Random.Range(1.0f, 30.0f) * 10.0f * ((Random.Range(1, 3) * 2) - 3);
            target.z += Random.Range(1.0f, 30.0f) * 10.0f * ((Random.Range(1, 3) * 2) - 3);
            //print("target x: " + target.x);
            //print("target y: " + target.x);

            if (Random.Range(0, 10) == 0)
            {
                print("enemy is jumping to new position");

                Cloth cloth = transform.GetChild(0).GetComponent<Cloth>();
                Cloth veil = transform.GetChild(1).GetComponent<Cloth>();
                cloth.worldAccelerationScale = 0.0f;
                cloth.worldVelocityScale = 0.0f;
                veil.worldAccelerationScale = 0.0f;
                veil.worldVelocityScale = 0.0f;

                do
                {
                    transform.position = new Vector3(Random.Range(0.0f, 500.0f), 22.0f, Random.Range(0.0f, 500.0f));
                    distance = Mathf.Abs((playerTransform.position - transform.position).magnitude);
                } while (distance < 80.0f || distance > 120.0f);

                cloth.worldAccelerationScale = 0.5f;
                cloth.worldVelocityScale = 0.5f;
                veil.worldAccelerationScale = 0.5f;
                veil.worldVelocityScale = 0.5f;
            }
        }
    }

    void FlickerOn()
    {
        transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled = false;
        transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = false;
        Invoke("FlickerOff", 0.01f);
    }

    void FlickerOff()
    {
        transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled = true;
        transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = true;
        if (Random.Range(0, 3) == 0) Invoke("FlickerOn", 0.01f);
    }
}
