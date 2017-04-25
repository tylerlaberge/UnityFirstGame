using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public float speed_force;
    private bool abducted = false;
	
	void FixedUpdate () {
        if (!this.abducted)
        {
            float translation = Input.GetAxis("Horizontal") * speed_force / 100;
            transform.Translate(translation, 0, 0);
        }
	}

    void OnTriggerEnter(Collider collider)
    {
        if (!this.abducted && collider.gameObject.tag == "Beam")
        {
            this.abducted = true;
            Transform UFO = collider.gameObject.GetComponent<BeamScript>().getUFO();
            UFO.gameObject.GetComponent<UFOScript>().Abduct(this.transform);
        }
    }
}
