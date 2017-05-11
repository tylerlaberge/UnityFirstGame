using UnityEngine;

public class BeamScript : MonoBehaviour {

    public float range;  // The range of this beam in the y-axis
    public float width;  // The max width of this beam.
    public float speed;  // The speed of this beam

    private Vector3 startPosition;
    private float startTime;

    void Start()
    {
        this.startTime = Time.time;
        this.startPosition = this.transform.position;
    }

    /*
     * Every frame linearly interpolate this instances position and scale
     * towards its max range and width. At the end this instance is destroyed.
     */
    void Update () {
        if (this.transform.position.y - .1 <= this.startPosition.y - this.range)
        {
            Object.Destroy(this.gameObject);
        }
        else
        {
            float distCovered = (Time.time - this.startTime) * this.speed;
            float fracJourney = distCovered / this.range;
            this.transform.localScale = Vector3.Lerp(
                this.transform.localScale, 
                new Vector3(this.width, this.transform.localScale.y, this.transform.localScale.z), 
                fracJourney
            );
            this.transform.position = Vector3.Lerp(
                this.transform.position, 
                new Vector3(this.transform.position.x, this.startPosition.y - this.range, this.transform.position.z), 
                fracJourney
            );
        }
    }

    /*
     * Ask the UFO this beam is a part of to abduct any object this beam collides with.
     */
    void OnTriggerEnter(Collider collider)
    {
        this.getUFO().GetComponent<UFOScript>().Abduct(collider.gameObject);
    }

    /*
     * Returns the parent of this object. 
     * 
     * This object should always be a child of UFO prefab or problems will occur.
     */
    public GameObject getUFO()
    {
        return this.transform.parent.gameObject;
    }
}
