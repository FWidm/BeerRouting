using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Security.Permissions;
using System.Collections.Generic;

public class Platformer_Movement : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed = 20;
    public float jumpheight = 5;
    private Animator animator;
    private bool blocked = false;
    private bool touchJump = false;
    //-1=left, 1=right, 0=nothing
    private float touchMove = 0;
    public float touchMovespeed = .7f;
    private bool jumpEnabled = false;
    private GameObject movingPlatform = null;
    private SoundManager soundManager;

    // Use this for initialization
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();
        soundManager = FindObjectOfType<SoundManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!blocked)
        {
            Vector2 velocity = rb.velocity;
            float movex = Input.GetAxis("Horizontal");
            velocity.x = movex * speed;

            if (velocity.x <= 1 && velocity.x >= 0)
            {
                velocity.x = touchMove * speed;
            }
            //Flip the player
            if (velocity.x < 0)
                transform.localScale = new Vector3(-.3f, .3f, .3f);
            else
                transform.localScale = new Vector3(.3f, .3f, .3f);
            //Jumping
            if (jumpEnabled && (Input.GetKey(KeyCode.Space) || touchJump))
            {
                velocity.y = jumpheight;
                animator.SetFloat("Speed", .2f);
                touchJump = false;
                animator.SetBool("Jump", true);
                soundManager.PlaySound(SoundManager.SoundType.PlayerJump);
            }

            if (Mathf.Abs(velocity.x) > 0)
            {
                // Start walking.
                animator.SetFloat("Speed", speed);
            }
            else
            {
                // Stop walking.
                animator.SetFloat("Speed", 0f);
            }

            // Moving platforms.
            if (movingPlatform != null)
            {
                MovingIsland movIsland = movingPlatform.GetComponent<MovingIsland>();
                if (movIsland != null)
                {
                    velocity.x += movIsland.GetVelocity().x;
                    if (movIsland.GetVelocity().y < 0)
                    {
                        velocity.y += movIsland.GetVelocity().y;
                    }
                }
            }

            rb.velocity = velocity;
            //        Debug.Log("Velocity=" + rb.velocity);
        }
        else
        {
            // Stop walking.
            animator.SetFloat("Speed", 0f);
            rb.velocity = new Vector2(0, 0);
        }

    }

    /// <summary>
    /// Allow jumping when we are on a platform, set the moving platform if we are on one.
    /// </summary>
    /// <param name="coll">Coll.</param>
    void OnCollisionEnter2D(Collision2D coll)
    {
        string collUpperCaseName = coll.collider.name.ToUpper();
        Debug.Log("Entered: " + coll.collider.name);
        if (collUpperCaseName.StartsWith("LANDSCAPE"))
        {
            jumpEnabled = true;
        }

        if (coll.collider.transform.parent.name.ToUpper().Contains("MOVINGISLAND"))
        {
            movingPlatform = coll.collider.transform.parent.gameObject;
        }
    }

    /// <summary>
    /// If we stay on a Platform, allow jumping if the player is standing on it, else pull him down.
    /// </summary>
    /// <param name="coll">Coll.</param>
    void OnCollisionStay2D(Collision2D coll)
    {
        string collUpperCaseName = coll.collider.name.ToUpper();
        // Debug.Log("Stayed: " + coll.collider.name);
        if (collUpperCaseName.StartsWith("LANDSCAPE"))
        {
            // If player is standing on the platform.
            // Debug.Log("Player transform y: " + transform.position.y + ", collider transform y: " + coll.collider.transform.position.y);
            if (transform.position.y >= coll.collider.transform.position.y)
            {
                jumpEnabled = true;
            }
            else
            {
                jumpEnabled = false;
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - 2);
            }
            	
        }
    }

    /// <summary>
    /// Disable jumping when we are leaving a platform
    /// </summary>
    /// <param name="coll">Coll.</param>
    void OnCollisionExit2D(Collision2D coll)
    {
        Debug.Log("Exit: " + coll.collider.name);
        string collUpperCaseName = coll.collider.name.ToUpper();

        if (collUpperCaseName.StartsWith("LANDSCAPE"))
        {
            jumpEnabled = false;
        }
        if (coll.collider.transform.parent.name.ToUpper().Contains("MOVINGISLAND"))
        {
            movingPlatform = null;
        }
    }

    /// <summary>
    /// set the touchJump flag to true if the button is pressed.
    /// </summary>
    public void Jump()
    {
        Debug.Log("JUMP BUTTON pressed!");
        touchJump = true;
    }

    /// <summary>
    /// Handles the MoveRight Touch button.
    /// </summary>
    public void MoveRight()
    {
        Debug.Log("Right BUTTON");

        touchMove = touchMovespeed;
    }

    /// <summary>
    /// Stops the movement.
    /// </summary>
    public void MoveStop()
    {
        touchMove = 0;
    }

    /// <summary>
    /// Handles the MoveLeft Touch button.
    /// </summary>
    public void MoveLeft()
    {
        Debug.Log("LEFT BUTTON");

        touchMove = -touchMovespeed;
    }

    /// <summary>
    /// Blocks the movement.
    /// </summary>
    /// <param name="blocked">If set to <c>true</c> blocked.</param>
    public void BlockMovement(bool blocked)
    {
        this.blocked = blocked;
        Debug.Log("Blocking input entabled? " + blocked);
    }

    /// <summary>
    /// Is triggered when the level is finished and 
    /// takes care of keeping the player save from moving objects.
    /// </summary>
    public void LevelFinished()
    {
        // Deactivate all colliders of the player.
        var colliders = GetComponents<Collider2D>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        // Deactivate the rigidbody.
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0.0f;
    }
}
