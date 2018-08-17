using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        transform.rotation = new Quaternion(0.0f, Random.Range(0.0f, 360.0f), 0.0f, 0.0f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
