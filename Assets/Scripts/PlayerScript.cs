using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public float speed;

    private string axis;

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
    	
	void FixedUpdate () {
        float translation = Input.GetAxis(this.axis) * speed / 100;

        transform.Translate(translation, 0, 0);
	}
}
