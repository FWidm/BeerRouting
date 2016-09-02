using UnityEngine;
using System.Collections;

public class CloudController : MonoBehaviour
{
    public bool randomScale = true;
    public bool randomFlip = true;
    private float velocity;
    private int direction;

    // Use this for initialization
    void Start()
    {
        velocity = Random.Range(0.001f, 0.003f);
        direction = Random.Range(-1, 1);
        if (direction == 0)
        {
            direction = 1;
        }
        if (randomScale)
        {
            float scale = Random.Range(0.5f, 1f);
            transform.localScale = new Vector3(scale, scale, 1);
        }
        if (randomFlip)
        {
            bool flip = false;
            if(Random.Range(0,2) == 0)
            {
                flip = true;
            }
            foreach (Transform childTransform in transform)
            {
                SpriteRenderer spr = GetComponent<SpriteRenderer>();
                if(spr != null)
                spr.flipX = flip;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x + (velocity * direction), transform.position.y, transform.position.z);
    }
}
