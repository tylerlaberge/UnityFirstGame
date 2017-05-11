using UnityEngine;

public class TeleporterScript : MonoBehaviour {

    public Transform target;  // The target transform to teleport to.

    /*
     * Every frame make sure the target transform hasn't been destroyed.
     * 
     * If it has been destroyed then destroy this Teleporter and insert an invisible wall (Stopper prefab) in its place.
     */
    void Update()
    {
        // Target has been destroyed
        if (this.target == null)
        {
            // Create an invisible wall
            GameObject stopper = (GameObject) Resources.Load("prefabs/Stopper", typeof(GameObject));
            GameObject stopperInstance = Instantiate(stopper, this.transform.position, this.transform.rotation);
            stopperInstance.transform.localScale = this.transform.localScale;

            // Destroy this teleporter
            Destroy(this.gameObject);
        }
    }

    /*
     * Teleport players to this instances target location when they collide with this teleporter.
     */
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "PlayerOne" || collider.gameObject.tag == "PlayerTwo")
        {
            collider.gameObject.transform.position = target.position;
        }
    }
}
