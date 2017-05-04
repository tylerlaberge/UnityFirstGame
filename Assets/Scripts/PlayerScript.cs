using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public float speed_force;
    	
	void FixedUpdate () {
        float translation = Input.GetAxis("Horizontal") * speed_force / 100;

        transform.Translate(translation, 0, 0);
	}
}
