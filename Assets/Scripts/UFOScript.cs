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
        InvokeRepeating("MaybeRunBeam", 2f, 2f);
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
        if (Random.value > .5 && !this.beamActive)
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

    public void Abduct(Transform transform)
    {
        StartCoroutine(Abductor(transform));
    }

    IEnumerator Abductor(Transform other_transform)
    {
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(other_transform.position, this.transform.position);
        while (other_transform.position != this.transform.position)
        {
            float distCovered = (Time.time - startTime) * 2.0f;
            float fracJourney = distCovered / journeyLength;
            other_transform.position = Vector3.Lerp(other_transform.position, this.transform.position, fracJourney);
            other_transform.localScale = Vector3.Lerp(
                other_transform.localScale,
                new Vector3(0, 0, this.transform.localScale.z),
                fracJourney
            );

            yield return null;
        }
        
        Debug.Log("GAME OVER");
    }
}
