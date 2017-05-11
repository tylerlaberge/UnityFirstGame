using System;
using System.Collections;
using UnityEngine;

public class UFOScript : MonoBehaviour {

    public float speed;  // The speed of this UFO.
    public Transform beam;  // A beam prefab for this UFO to use.
    public float maxPlayerDistance;  // The max distance possible between this UFO and the target player

    private PlayerScript[] players;

    private bool playerAbducted = false;
    private bool beamActive = false;
    private bool movementLocked = false;

    private float minBeamInterval = 4.0f;  // Beam never runs more than once every 4 seconds.
    private float maxBeamInterval = 8.0f;  // Beam always runs at least once every 8 seconds.
    private float timeOfLastBeam = 0.0f;
    private float timeofLastTargetChange = 0.0f;  // The time when this UFO last changed the target player it follows
    private int targetPlayerIndex = 0;  // The index of the target player to follow.


	void Start () {
        this.players = FindObjectsOfType<PlayerScript>();
        InvokeRepeating("MaybeRunBeam", this.minBeamInterval, this.minBeamInterval);  // Every 4 seconds potentially run the beam.
    }
	
	void FixedUpdate () {
        if (!this.playerAbducted && !this.movementLocked)
        {
            // Choose a player and follow it.
            PlayerScript targetPlayer = this.ChooseTarget();
            this.FollowTarget(targetPlayer);
        }
	}

    /*
     * Choose the player to follow. 
     * 
     * A player is always followed for 5 seconds and then another player is chosen as the target.
     */
    PlayerScript ChooseTarget()
    {
        if ((Time.time - this.timeofLastTargetChange) > 5)
        {
            if (this.targetPlayerIndex < this.players.Length - 1)
            {
                this.targetPlayerIndex++;
            }
            else
            {
                this.targetPlayerIndex = 0;
            }
            this.timeofLastTargetChange = Time.time;
        }

        return this.players[this.targetPlayerIndex];
    }

    /*
     * Follow the target Player. 
     * 
     * Generally moves left and right above players location,
     * but will teleport to the target player if the player is far away.
     */
    void FollowTarget(PlayerScript target)
    {
        // If player is far away, teleport to the player.
        if (Math.Abs(target.transform.position.x - this.transform.position.x) > this.maxPlayerDistance)
        {
            this.transform.position = new Vector3(target.transform.position.x, this.transform.position.y, this.transform.position.z);
        }

        //If player is on the right, move right.
        if (target.transform.position.x > this.transform.position.x)
        {
            Invoke("MoveRight", 0.25f);
        }

        //If player is on the left, move left.
        else if (target.transform.position.x < this.transform.position.x)
        {
            Invoke("MoveLeft", 0.25f);
        }
    }

    /*
     * Move to the left on the x-axis.
     * 
     * Distance moved is based on this instances speed.
     */
    void MoveLeft()
    {
        transform.Translate(new Vector3(-speed / 100, 0, 0));
    }

    /*
     * Move to the right on the x-axis.
     * 
     * Distance moved is based on this instances speed.
     */
    void MoveRight()
    {
        transform.Translate(new Vector3(speed / 100, 0, 0));
    }

    /*
     * Lock this instances movement.
     */
    void LockMovement()
    {
        this.movementLocked = true;
    }

    /*
     * Unlock this instances movement.
     */
    void UnlockMovement()
    {
        this.movementLocked = false;
    }

    /*
     * Potentially run the beam.
     * 
     * There is a 25% chance the beam will run on any given call.
     * however if the time since the last beam activation is greater than this instances maxBeamInterval 
     * the beam is guarenteed to run.
     */
    void MaybeRunBeam()
    {
        if (((UnityEngine.Random.value > .75 && !this.beamActive) || ((Time.time - this.timeOfLastBeam) >= this.maxBeamInterval)))
        {
            this.ClearInvokes();  // Clear any old invokes
            this.ActivateBeam();
            Invoke("DeactivateBeam", 2f);  // Deactivate the beam after 2 seconds.
            Invoke("CreateStopper", 2f);   // Insert a 'Stopper' prefab to prevent movement over abducted flooring.
        }
    }

    /*
     * Activate the beam. 
     * 
     * Movement is locked while the beam is running.
     */
    void ActivateBeam()
    {
        this.LockMovement();
        this.beamActive = true;
        StartCoroutine("BeamEngine");
    }

    /*
     * Deactivate the beam.
     * 
     * Movement will be restored on deactivation.
     */
    void DeactivateBeam()
    {
        this.beamActive = false;
        this.timeOfLastBeam = Time.time;
        Invoke("UnlockMovement", 1f);
    }

    /*
     * Abduct a GameObject into this UFO.
     * 
     * Sucks the game object up towards this instances transform
     * and scales the object down along the way. At the end of the abduction
     * the game object will be destroyed.
     */
    public void Abduct(GameObject gameObject)
    {
        //  If player one or two is being abducted. End game condition.
        if (!this.playerAbducted && (gameObject.tag == "PlayerOne" || gameObject.tag == "PlayerTwo"))
        {
            this.playerAbducted = true;
            StartCoroutine(Abductor(gameObject));

            if (gameObject.tag == "PlayerOne")
            {
                Debug.Log("Player Two Wins!");
            }
            else
            {
                Debug.Log("Player One Wins!");
            }

            CancelInvoke("MaybeRunBeam");
        }
        else
        {
            StartCoroutine(Abductor(gameObject));
        }
    }

    /*
     * A coroutine responsible for actually creating the beam.
     * 
     * Continually instanstiates Beam prefabs while the beam is active,
     */
    IEnumerator BeamEngine()
    {
        while(this.beamActive)
        {
            Instantiate(beam, this.transform.position, this.transform.rotation, this.transform);
            yield return new WaitForSeconds(0.05f);
        }
    }

    /*
     * A coroutine responsible for actually abducting a game object.
     */
    IEnumerator Abductor(GameObject other)
    {
        float startTime = Time.time;

        // Distance of abduction
        float journeyLength = Vector3.Distance(other.transform.position, this.transform.position);

        // This loop just linearly interpolates the abducted objects position and scale until it has reached this UFO.
        //
        // Check for null because its highly possible for the same object to be requested to be abducted more than once,
        // and at the end of an abduction the object is destroyed so subsequent calls need to be careful.
        //
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

    /*
     * Create a Stopper prefab below this UFO instance at the beams max range.
     * 
     * This is for replacing the abducted floor with an invisible wall that players can't pass through.
     */
    void CreateStopper()
    {
        GameObject stopper = (GameObject)Resources.Load("prefabs/Stopper", typeof(GameObject));

        // Instantiate stopper at where the beam would hit if it were on.
        GameObject stopperInstance = Instantiate(
            stopper,
            new Vector3(
                this.transform.position.x,
                this.transform.position.y - this.beam.gameObject.GetComponent<BeamScript>().range,
                this.transform.position.z
            ),
            this.transform.rotation
        );

        // Scale the stopper so that it creates a sufficient wall.
        stopperInstance.transform.localScale = new Vector3(
            this.transform.localScale.x * 1.1f, 
            this.transform.localScale.y + this.beam.gameObject.GetComponent<BeamScript>().range / 2, 
            this.transform.localScale.z
        );
    }

    /*
     * Clear any invokes on the following methods.
     * 
     * - MoveRight
     * - MoveLeft
     * - DeactivateBeam
     * - UnlockMovement
     * - BeamEngine
     * 
     * Used to clear up any stale invokes that shouldnt be relevent anymore when the beam is activated.
     */
    void ClearInvokes()
    {
        CancelInvoke("MoveRight");
        CancelInvoke("MoveLeft");
        CancelInvoke("DeactivateBeam");
        CancelInvoke("UnlockMovement");
        CancelInvoke("BeamEngine");
    }
}
