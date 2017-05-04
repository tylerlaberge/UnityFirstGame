using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOScript : MonoBehaviour {

    public float speed_force;
    public Transform beam;

    private PlayerScript player;
    private bool playerAbducted = false;
    private bool beamActive = false;
    private bool movementLocked = false;

    private float maxBeamInterval = 9.0f;
    private float timeOfLastBeam = 0.0f;

	// Use this for initialization
	void Start () {
        this.player = FindObjectOfType<PlayerScript>();
        InvokeRepeating("MaybeRunBeam", 3f, 3f);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!this.playerAbducted && !this.movementLocked)
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

        if ((UnityEngine.Random.value > .5 && !this.beamActive) || ((Time.time - this.timeOfLastBeam) >= this.maxBeamInterval))
        {
            this.ClearInvokes();
            this.ActivateBeam();
            Invoke("DeactivateBeam", 3f);
        }
    }

    void ClearInvokes()
    {
        CancelInvoke("MoveRight");
        CancelInvoke("MoveLeft");
        CancelInvoke("DeactivateBeam");
        CancelInvoke("UnlockMovement");
        CancelInvoke("BeamEngine");
    }
    void ActivateBeam()
    {
        this.LockMovement();
        this.beamActive = true;
        StartCoroutine("BeamEngine");
    }

    void DeactivateBeam()
    {
        this.beamActive = false;
        this.timeOfLastBeam = Time.time;
        Invoke("UnlockMovement", 1f);
    }

    void LockMovement()
    {
        this.movementLocked = true;
    }

    void UnlockMovement()
    {
        this.movementLocked = false;
    }

    IEnumerator BeamEngine()
    {
        while(this.beamActive)
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

    public void Abduct(GameObject gameObject)
    {
        if (gameObject.tag == "Player" && !this.playerAbducted)
        {
            this.playerAbducted = true;
            StartCoroutine(Abductor(gameObject));
            Debug.Log("GAME OVER");
        }
        else
        {
            StartCoroutine(Abductor(gameObject));
        }
    }

    IEnumerator Abductor(GameObject other)
    {
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(other.transform.position, this.transform.position);
        while (other.transform.position != this.transform.position)
        {
            float distCovered = (Time.time - startTime) * 2.0f;
            float fracJourney = distCovered / journeyLength;
            other.transform.position = Vector3.Lerp(other.transform.position, this.transform.position, fracJourney);
            other.transform.localScale = Vector3.Lerp(
                other.transform.localScale,
                new Vector3(0, 0, this.transform.localScale.z),
                fracJourney
            );

            yield return null;
        }
    }
}
