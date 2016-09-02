using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Platformer_PenguinBehaviour : MonoBehaviour
{
    Rigidbody2D rb;
    Bounds colliderBounds;
    SpriteRenderer sr;
    public float turningDistance = .5f;
    public float speed = 0;

    bool left = true;
    private SoundManager soundManager;


    // Use this for initialization
    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        rb = GetComponent<Rigidbody2D>();
        if (speed == 0)
            speed = UnityEngine.Random.Range(1, 4);
    }

    void FixedUpdate()
    {

        if (colliderBounds != null)
        {
            Vector3 platformEnd = colliderBounds.center + colliderBounds.extents;
            Vector3 platformBegin = colliderBounds.center - colliderBounds.extents;

            // (P<-->End) <= Dist --> Turn left
            if (Math.Abs(transform.position.x - platformEnd.x) <= turningDistance)
            {
                left = true;
            }
            // (Begin<-->P) <= Dist <-- Turn right
            if (Math.Abs(transform.position.x - platformBegin.x) <= turningDistance)
            {
                left = false;
            }

            if (left)
            {
                rb.velocity = new Vector2(-speed, rb.velocity.y);
                transform.localScale = new Vector3(.5f, .5f, .5f);

            }
            else
            {
                rb.velocity = new Vector2(speed, rb.velocity.y);
                transform.localScale = new Vector3(-.5f, .5f, .5f);

            }

        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        Vector3 posDiff = coll.transform.position - this.transform.position;

        if (coll.collider.name.Contains("Player"))
        {
            soundManager.PlaySound(SoundManager.SoundType.Meow);
        }
        else
        {
            colliderBounds = coll.collider.gameObject.GetComponentInChildren<SpriteRenderer>().bounds;

        }
    }

    void OnCollisionExit2D(Collision2D coll)
    {
    }
}
