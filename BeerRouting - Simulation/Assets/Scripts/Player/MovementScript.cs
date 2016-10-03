using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Pathfinding;

public class MovementScript : MonoBehaviour
{
    //--- Player Attributes
    
    // Parameter for player movement speed.
    public float speed = 2.0f;
    // Defines in which distance waypoints should be set by the Pathfinder.
    public float nextWaypointDistance = 1.0f;
    // Walk animation
    public Animator animator;

    // Indicates whether logging statements should be executed.
    public bool isLogEnabled = false;

    //--- Movement
    //Defines the target position for the player movement.
    private Vector3 targetPosition;
    private bool isPreLoadMove = false;

    //--- A* Pathfinding
    // The discovered path between two points.
    private Path path;
    // Seeker for pathfinding.
    private Seeker seeker;
    // The index of the current waypoint of the path.
    private int currentWaypoint = 0;

    //--- PuddleSpawning
    private bool spawnPuddles;
    private SpawnPuddles spawnPuddlesScript;
    public int puddleDifference = 10;
    private int puddleDiffCount = 0;

    private PlayerController playerController;

    // Event to notify about arrival at target position.
    UnityEvent e_arrivedAtTargetPoint;
    
    // Use this for initialization
    void Start()
    {
        Rigidbody2D myRigidBody = GetComponent<Rigidbody2D>();

        playerController = GetComponent<PlayerController>();

        // Load Event that is used to notify about the arrival at the target position.
        if (e_arrivedAtTargetPoint == null)
            e_arrivedAtTargetPoint = new UnityEvent();

        //Get the Ref to the PuddleSpawnScript.
        spawnPuddlesScript = GetComponent<SpawnPuddles>();

        // Get the reference to the Seeker.
        seeker = GetComponent<Seeker>();

        myRigidBody.freezeRotation = true;
    }

    /// <summary>
    /// Performs a move to the position indicated by the target game object without the walk animation.
    /// </summary>
    /// <param name="target">GameObject which is the target position of the pre load move.</param>
    public void PerformPreLoadMove(GameObject target)
    {
        isPreLoadMove = true;
        MovePlayer(target);
    }

    /// <summary>
    /// Controls the spawing of beer puddles during the player movement. Activate or deactivate the spawning.
    /// </summary>
    /// <param name="spawnPuddles">A bool value that indicates whether beer puddles should be spawned or not.</param>
    public void ControlPuddleSpawning(bool spawnPuddles)
    {
        this.spawnPuddles = spawnPuddles;
    }

    public void resetPuddles()
    {
        spawnPuddlesScript.destroyAll();
    }

    public void Update()
    {
//		if (Input.GetKeyDown (KeyCode.LeftControl)) {
//			speed = 6;
//		}	
//		if (Input.GetKeyUp (KeyCode.LeftControl)) {
//			speed = 2;
//		}
    }

    public void FixedUpdate()
    {
        //Debug.Log(">>> is Path null?" + (path == null));
        // No path, no movement.
        if (path == null)
        {
            return;
        }

        // Perform movement
        //Debug.Log("CurrWaypoint="+ currentWaypoint+"PathVectorPath.Count="+path.vectorPath.Count);
        if (currentWaypoint >= path.vectorPath.Count)
        {   // Check if we have reached target point.
            if (isLogEnabled)
                Debug.Log("Reached target point");

            // Set path to null to stop movement.
            path = null;
            // Deactivate movement animation.
            animator.SetFloat("Speed", 0);

            // Player has reached the target position. Fire the event
            e_arrivedAtTargetPoint.Invoke();
            
            return;
        }
        

        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;     // Direction vector.
        dir *= speed * Time.fixedDeltaTime;
        transform.Translate(dir);
        //spawn puddles on the way
        if (spawnPuddles)
        {
            if (puddleDiffCount % puddleDifference == 0)
            {
                spawnPuddlesScript.Spawn(transform.position);
            }
            puddleDiffCount++;
        }

        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            //Debug.Log(">>> path=" + path.vectorPath[currentWaypoint] + " ::: " + transform.position);
            currentWaypoint++;      // Go to the next waypoint.
        }
    }

    /// <summary>
    /// Sets the target position of the player object so that the player moves towards the game object
    /// which is passed as the parameter.
    /// </summary>
    /// <param name="to">The game object to which the player should be moved.</param>
    public void MovePlayer(GameObject to)
    {
        // Get the collider of the "to" GameObject.
        Collider2D coll = to.GetComponent<Collider2D>();
        if (coll != null)
        {
            // Calculate the closest points on the collider's border towards the current position of the player.
            Vector3 borderPoint = coll.bounds.ClosestPoint(transform.position);

            if (isLogEnabled)
                Debug.Log("Border at position: " + borderPoint);

            // Set the border point as the actual target instead of the exact position of the game object.
            targetPosition = borderPoint;
        }
        else
        {
            targetPosition = new Vector3(to.transform.position.x, to.transform.position.y, to.transform.position.z);
        }

        //targetPosition = new Vector3(to.transform.position.x, to.transform.position.y + playerHeight, to.transform.position.z);
        seeker.StartPath(transform.position, targetPosition, OnPathComplete);
        if (!isPreLoadMove)
        {
            if (isLogEnabled)
                Debug.Log("Set animator speed.");

            animator.SetFloat("Speed", speed);
        }
        else
        {
            isPreLoadMove = false;
        }

        int currDirection = (int)Mathf.Sign(targetPosition.x - transform.position.x);

        if (isLogEnabled)
            Debug.Log("Current Direction: " + currDirection);
        if (currDirection < 0 && transform.localScale.x >= 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (currDirection >= 0)
        {
            transform.localScale = new Vector3(transform.localScale.y, transform.localScale.y, transform.localScale.z);
        }
    }

    /// <summary>
    /// Called when the pathfinder has calculated the path between two points.
    /// </summary>
    /// <param name="p">The calculated path.</param>
    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            // Set the current waypoint to 0 since we have a new path.
            currentWaypoint = 0;
            //Debug.Log(">>> OnPathComplete path=" + path);
        }
        

    }

    /// <summary>
    /// Stops the jumping.
    /// </summary>
    public void StopJumping()
    {
        if (playerController != null)
            playerController.StopJump();
    }

    /// <summary>
    /// Aborts the current movement process.
    /// </summary>
    public void AbortCurrentMovementProcess()
    {
        if (path != null)
        {
            path = null;

            if (e_arrivedAtTargetPoint != null)
            {
                e_arrivedAtTargetPoint.Invoke();
            }
        }
    }

    /// <summary>
    /// This method can be used to subscribe to the event which will notify its subscribers 
    /// when the player has reached his target point in a movement process.
    /// The method expects a callback function which takes no parameters and does not return any value.
    /// If the event fires, the specified callback function will be called.
    /// </summary>
    /// <param name="callbackFunc">The callback function that will be called when the event fires.</param>
    public void SubscribeToTargetPositionArrivedEvent(UnityAction callbackFunc)
    {
        if (e_arrivedAtTargetPoint != null)
        {
            e_arrivedAtTargetPoint.AddListener(callbackFunc);
        }
    }

    /// <summary>
    /// This method can be used to unsubscribe from the event which will notify its subscribers
    /// when the player has reached his target point in a movement process.
    /// The caller will no longer be notified when the event fires.
    /// </summary>
    /// <param name="callbackFunc">The callback function that should be removed from the subscriber list.</param>
    public void UnsubscribeFromTargetPositionArrivedEvent(UnityAction callbackFunc)
    {
        if (e_arrivedAtTargetPoint != null)
        {
            e_arrivedAtTargetPoint.RemoveListener(callbackFunc);
        }
    }

    //public void SetNextNode(PathScript destination)
    //{
    //    if (isLogEnabled)
    //        Debug.Log("Destination " + destination.name);

    //    // Check if player is allowed to move.
    //    if (levelController.IsGameInputEnabled() && destination != null && path == null)
    //    {
            
    //        ////check if we use the dijkstra manager or not.
    //        //if (isDijkstra)
    //        //{
    //        //    CheckDijkstra(destination);
    //        //}
    //    }
    //}
   
    //public void CheckGreedy(PathScript destination)
    //{

    //}
}
