using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOScript : MonoBehaviour {

    public float speed_force;
    public Transform beam;
    public Transform stopper;
    public float maxPlayerDistance;

    private PlayerScript[] players;
    private bool playerAbducted = false;
    private bool beamActive = false;
    private bool movementLocked = false;

    private float minBeamInterval = 4.0f;
    private float maxBeamInterval = 8.0f;
    private float timeOfLastBeam = 0.0f;

	void Start () {
        this.players = FindObjectsOfType<PlayerScript>();
        InvokeRepeating("MaybeRunBeam", this.minBeamInterval, this.minBeamInterval);
    }
	
	void FixedUpdate () {
        if (!this.playerAbducted && !this.movementLocked)
        {
            PlayerScript nearest_player = this.GetNearestPlayer();

            if (Math.Abs(nearest_player.transform.position.x - this.transform.position.x) > this.maxPlayerDistance)
            {
                this.transform.position = new Vector3(nearest_player.transform.position.x, this.transform.position.y, this.transform.position.z);
            }
            if (nearest_player.transform.position.x > this.transform.position.x)
            {
                Invoke("MoveRight", 0.25f);
            }
            else if (nearest_player.transform.position.x < this.transform.position.x)
            {
                Invoke("MoveLeft", 0.25f);
            }
        }
	}

    PlayerScript GetNearestPlayer()
    {
        PlayerScript closest_player = players[0];
        double min_distance = Double.PositiveInfinity; 
        foreach (PlayerScript player in this.players)
        {
            double distance = Math.Abs(player.transform.position.x - this.transform.position.x);
            if (distance < min_distance)
            {
                closest_player = player;
                min_distance = distance;
            }
        }

        return closest_player;
    }

    void MaybeRunBeam()
    {
        if (((UnityEngine.Random.value > .75 && !this.beamActive) || ((Time.time - this.timeOfLastBeam) >= this.maxBeamInterval)))
        {
            this.ClearInvokes();
            this.ActivateBeam();
            Invoke("DeactivateBeam", 2f);
            Invoke("CreateStopper", 2f);
        }
    }

    void CreateStopper()
    {
        Transform stopper_object = Instantiate(
            stopper,
            new Vector3(
                this.transform.position.x, 
                this.transform.position.y - this.beam.gameObject.GetComponent<BeamScript>().range, 
                this.transform.position.z
            ),
            this.transform.rotation
        );
        stopper_object.localScale = new Vector3(this.transform.localScale.x*1.1f, this.transform.localScale.y + this.beam.gameObject.GetComponent<BeamScript>().range/2, this.transform.localScale.z);
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
        if (!this.playerAbducted && (gameObject.tag == "PlayerOne" || gameObject.tag == "PlayerTwo"))
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
        while (other != null && other.transform.position != this.transform.position)
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
        Destroy(other);
    }
}
