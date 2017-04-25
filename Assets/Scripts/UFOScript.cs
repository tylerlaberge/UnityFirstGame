using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOScript : MonoBehaviour {

    public float speed_force;

    public Transform beam;

    private PlayerScript player;
    private bool beamActive = false;

	// Use this for initialization
	void Start () {
        this.player = FindObjectOfType<PlayerScript>();
        InvokeRepeating("MaybeRunBeam", 3f, 6f);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!this.beamActive)
        {
            if (player.transform.position.x > this.transform.position.x)
            {
                Invoke("MoveRight", 0.5f);
            }
            else if (player.transform.position.x < this.transform.position.x)
            {
                Invoke("MoveLeft", 0.5f);
            }
        }
	}

    void MaybeRunBeam()
    {
        if (Random.value > .25)
        {
            this.ActivateBeam();
            Invoke("DeactivateBeam", 3f);
        }
    }

    void ActivateBeam()
    {
        this.beamActive = true;
        StartCoroutine("BeamEngine");
    }

    void DeactivateBeam()
    {
        this.beamActive = false;
        StopCoroutine("BeamEngine");
    }

    IEnumerator BeamEngine()
    {
        while(true)
        {
            Instantiate(beam, this.transform.position, this.transform.rotation, this.transform);
            yield return new WaitForSeconds(0.05f);
        }
    }

    void MoveLeft()
    {
        transform.Translate(new Vector3(-speed_force / 100, 0, 0));
    }

    void MoveRight ()
    {
        transform.Translate(new Vector3(speed_force / 100, 0, 0));
    }
}
