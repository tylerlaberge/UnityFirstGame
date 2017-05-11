using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public float speed;  // The speed of this player.

    private string axis;  // The name of the input axis associated with this player.

    /*
     * Initialize axis field based on if this is a player one or player two prefab.
     */
    void Start()
    {
        if (this.gameObject.tag == "PlayerOne")
        {
            this.axis = "Horizontal";
        }
        else if (this.gameObject.tag == "PlayerTwo")
        {
            this.axis = "HorizontalTwo";
        }
    }
    
    /*
     * Every fixed update just move in the direction the player input is indicating.
     */
	void FixedUpdate () {
        float translation = Input.GetAxis(this.axis) * speed / 100;

        transform.Translate(translation, 0, 0);
	}
}
