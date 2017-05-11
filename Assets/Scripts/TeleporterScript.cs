using UnityEngine;

public class TeleporterScript : MonoBehaviour {

    public Transform target;

    void Update()
    {
        if (this.target == null)
        {
            GameObject stopper = (GameObject) Resources.Load("prefabs/Stopper", typeof(GameObject));
            GameObject stopperInstance = Instantiate(stopper, this.transform.position, this.transform.rotation);
            stopperInstance.transform.localScale = this.transform.localScale;     
            Destroy(this.gameObject);
        }
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "PlayerOne" || collider.gameObject.tag == "PlayerTwo")
        {
            collider.gameObject.transform.position = target.position;
        }
    }
}
