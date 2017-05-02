using UnityEngine;

public class BeamScript : MonoBehaviour {

    public float range;
    public float width;
    public float speed;

    private Vector3 startPosition;
    private float startTime;

    void Start()
    {
        this.startTime = Time.time;
        this.startPosition = this.transform.position;
    }
	// Update is called once per frame
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

    public GameObject getUFO()
    {
        return this.transform.parent.gameObject;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Floor")
        {
            if (Mathf.Abs(this.transform.position.x - collider.transform.position.x) <= this.width)
            {
                this.getUFO().GetComponent<UFOScript>().Abduct(collider.gameObject);
            }
        }
    }
}
