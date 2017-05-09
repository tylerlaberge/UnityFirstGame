using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public float speed_force;

    private string axis;

    void Start()
    {
        if (this.gameObject.tag == "PlayerOne")
        {
            this.axis = "Horizontal";
        }
        else if (this.gameObject.tag == "PlayerTwo")
        {
            this.axis = "HorizontalTwo";
        }
    }
    	
	void FixedUpdate () {
        float translation = Input.GetAxis(this.axis) * speed_force / 100;

        transform.Translate(translation, 0, 0);
	}
}
