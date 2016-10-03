using UnityEngine;
using System.Collections;

public class MovingIsland : MonoBehaviour
{
    public bool moveHorizontal = true;
    public float speed = 0.04f;
    public float moveArea = 3f;
    public int direction = 1;

    private float posXStart;
    private float posYStart;
	private Vector2 velocity;

    // Use this for initialization
    void Start()
    {
        posXStart = transform.position.x;
        posYStart = transform.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		Vector2 previousXYPos = new Vector2 (transform.position.x, transform.position.y);
        
		if (moveHorizontal)
        {
            transform.position = new Vector3(transform.position.x + (speed * direction), transform.position.y, transform.position.z);
            if (Mathf.Abs(posXStart - transform.position.x) > moveArea)
            {
                direction *= -1;
            }
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + (speed * direction), transform.position.z);
            if (Mathf.Abs(posYStart - transform.position.y) > moveArea)
            {
                direction *= -1;
            }
        }

		Vector2 newXYPos = new Vector2 (transform.position.x, transform.position.y);

		// Calculate velocity.
		velocity = (newXYPos - previousXYPos) / Time.deltaTime;
    }

	/// <summary>
	/// Gets the velocity.
	/// </summary>
	/// <returns>The velocity.</returns>
	public Vector2 GetVelocity()
	{
		return velocity;
	}
}
