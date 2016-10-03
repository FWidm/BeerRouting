using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class CameraFollowPlayer : MonoBehaviour
{
    public bool debug = false;
    public float KeyMovement = 0.2f;
    public float dragSpeed = 1f;
    public float cameraHeightMax_ = 4, cameraHeightMin = 2, cameraMultiplier = 1f;

    private float cameraHeightMax = 8;
    //vectors we need to know about
    public GameObject followingPos;
    private Vector3 currentPosition;
    private Vector3 dragOrigin;

    public float cameraHeight;
    //general info for the camera
    private LevelController levelController;
    private bool isDragging = false;
    private bool isFocusPlayer;
    private Vector3 backgroundStart, backgroundEnd;
    //fancy camera panning
    public float dampTime = 0.4f;
    // Approximate time for the camera to refocus.
    private Vector3 moveVelocity;
    // Reference velocity for the smooth damping of the position.
    private bool enableMovement = true;

    private float distanceOld = 0;
    private BackgroundDimensions bgDim;

    #if UNITY_ANDROID
    void Awake()
    {
        cameraMultiplier = .1f;
    }
    #endif

    // Use this for initialization
    void Start()
    {
        levelController = LevelController.GetCurrentLevelController();
        currentPosition = new Vector3(followingPos.transform.position.x, followingPos.transform.position.y, -10);
        transform.position = currentPosition;

        bgDim = GetBackgroundDimensionSecure();

        if (debug)
            Debug.Log("CAM>>" + bgDim);

        Assert.IsTrue(bgDim != null, "BG DIMENSIONS NOT IN LEVEL!");

        backgroundStart = bgDim.GetBackgroundStart();
        backgroundEnd = bgDim.GetBackgroundEnd();
        cameraHeight = 4f;
    }

    /// <summary>
    /// Returns the instance of BackgroundDimensions for the currently active level.
    /// </summary>
    /// <returns>An instance of BackgroundDimensions.</returns>
    private BackgroundDimensions GetBackgroundDimensionSecure()
    {
        GameObject[] bg = GameObject.FindGameObjectsWithTag("BackgroundBounds");
        if (debug)
        {
            Debug.Log("CAM>>" + bg + " length=" + bg.Length);
        }
        BackgroundDimensions bgDim = null;
        foreach (var item in bg)
        {
            if (debug)
                Debug.Log(item.gameObject.name);

            if (bgDim == null)
            {
                bgDim = item.GetComponent<BackgroundDimensions>();

                if (bgDim != null)
                    Debug.Log("Found CameraBounds: CAM>>" + bgDim);
            }
        }
        return bgDim;
    }

    // Update is called once per frame
    void Update()
    {
        Bounds cameraBounds = OrthographicBounds();
        Vector3 newPos = currentPosition;
        bool isCameraManipulationEnabled = !levelController.IsPlayerWalking();

        // Trigger a smooth transition if we switch from professor to the player.
        bool smoothTransition = true;

        if (!isCameraManipulationEnabled)
        {
            isFocusPlayer = true;
        }
        if (debug)
            Debug.Log("isCameraManipulationEnabled: " + isCameraManipulationEnabled + " enableMovement: " + enableMovement);
        if (isCameraManipulationEnabled && enableMovement)
        {
            //Key based movement
            if (!isDragging)
            {
                if (Input.GetKey(KeyCode.D))
                {
                    newPos += new Vector3(KeyMovement, 0, 0);
                    isFocusPlayer = false;
                    smoothTransition = true;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    newPos += new Vector3(-KeyMovement, 0, 0);
                    isFocusPlayer = false;
                    smoothTransition = true;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    newPos += new Vector3(0, -KeyMovement, 0);
                    isFocusPlayer = false;
                    smoothTransition = true;
                }
                if (Input.GetKey(KeyCode.W))
                {
                    newPos += new Vector3(0, KeyMovement, 0);
                    isFocusPlayer = false;
                    smoothTransition = true;
                }
            }
            //drag and drop
            if (Input.GetMouseButtonDown(0))
            {
                dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (debug)
                    Debug.Log("Started drag! pos=" + Camera.main.ScreenToWorldPoint(Input.mousePosition));
                isDragging = true;
                isFocusPlayer = false;
            }
            if (Input.GetMouseButton(0) && isDragging)
            {
                Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragOrigin;
                if (debug)
                    Debug.Log("Drag the Mouse! difference between origin and now=" + diff);
                newPos = currentPosition - diff;
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (debug)
                    Debug.Log("Stop drag! pos=" + Camera.main.ScreenToWorldPoint(Input.mousePosition));
                isDragging = false;
            }
            //If move is legal, then set it
            if (!IsXAxisLegal(cameraBounds.extents, newPos))
            {
                newPos.x = currentPosition.x;

            }
            if (!IsYAxisLegal(cameraBounds.extents, newPos))
            {
                newPos.y = currentPosition.y;
            }
        }

        if (isDragging)
        {
            smoothTransition = false;
        }

        if (!isCameraManipulationEnabled && isFocusPlayer)
        {
            newPos = new Vector3(followingPos.transform.position.x, followingPos.transform.position.y, -10);
            isDragging = false;
        }


        checkCameraZoom();
        //dampen the transition if we sweitch from free camera to the fixed one on 

        if (smoothTransition)
            transform.position = Vector3.SmoothDamp(transform.position, newPos, ref moveVelocity, dampTime);
        else
            transform.position = newPos;
        currentPosition = newPos;
    }

    private void checkCameraZoom()
    {
        if (Input.touchCount >= 2)
        {
            Vector2 touch0, touch1;
            float distance;
            touch0 = Input.GetTouch(0).position;
            touch1 = Input.GetTouch(1).position;
            distance = Vector2.Distance(touch0, touch1);
            if (distanceOld <= distance)
            {
                //zoom out
                Debug.Log("Zoom Out (old=" + distanceOld + " new=" + distance + ")");
                cameraHeight -= cameraMultiplier;
            }
            if (distanceOld >= distance)
            {
                //zoom in
                Debug.Log("Zoom in (old=" + distanceOld + " new=" + distance + ")");
                cameraHeight += cameraMultiplier;
            }
            Debug.Log("Touchpoints: first=" + Input.GetTouch(0) + " second=" + Input.GetTouch(1));
            distanceOld = distance;
        }
        else
        {
            cameraHeight -= cameraMultiplier * Input.GetAxis("Mouse ScrollWheel");
        }
        //Camera height
        Camera.main.orthographicSize = Mathf.Clamp(cameraHeight, cameraHeightMin, cameraHeightMax);
        if (cameraHeight > cameraHeightMax)
            cameraHeight = cameraHeightMax;
        if (cameraHeight < cameraHeightMin)
            cameraHeight = cameraHeightMin;
    }

    /// <summary>
    /// Calculates the current bounds of the camera.
    /// </summary>
    /// <returns>Bounds of the current camera object.</returns>
    public Bounds OrthographicBounds()
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = Camera.main.orthographicSize * 2;
        Bounds bounds = new Bounds(
                            GetComponent<Camera>().transform.position,
                            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }

    /// <summary>
    /// Determines whether the X axis of the newPosition is legal.
    /// </summary>
    /// <returns><c>true</c> if the x axis is contained in the borders, otherwise <c>false</c>.</returns>
    /// <param name="camBoundsExtents">Cam bounds extents.</param>
    /// <param name="newPos">New position.</param>
    public bool IsXAxisLegal(Vector3 camBoundsExtents, Vector3 newPos)
    {
        Vector3 diff = currentPosition - newPos;
        bool xLegal = false;

        // Cases need to be checked due to the so called "Zoom problem".
        // Case1: x position of the newPos is within the borders of the game.
        if ((newPos.x - camBoundsExtents.x) > backgroundStart.x && (newPos.x + camBoundsExtents.x) < backgroundEnd.x)
        {
            xLegal = true;
        }
        // Case2: x position of the newpos is left of the vertical borders of the game. However, we are moving towards the right boundary,
        // so its legal.
        else if ((newPos.x - camBoundsExtents.x) <= backgroundStart.x && diff.x < 0)
        {
            xLegal = true;
        }
        // Case3: Same as case 2 only for the right boundary.
        else if ((newPos.x + camBoundsExtents.x) >= backgroundEnd.x && diff.x > 0)
        {
            xLegal = true;
        }

        return xLegal;
    }

    /// <summary>
    /// Determines whether the Y axis of the newPosition is legal.
    /// </summary>
    /// <returns><c>true</c> if the y axis is contained in the borders otherwise, <c>false</c>.</returns>
    /// <param name="halfSize">camBounds Extents</param>
    /// <param name="newPos">New position.</param>
    public bool IsYAxisLegal(Vector3 camBoundsExtents, Vector3 newPos)
    {
        Vector3 diff = currentPosition - newPos;
        bool yLegal = false;

        // Cases need to be checked due to the so called "Zoom problem".
        // Case1: The y position of the newPos is within the borders of the game.
        if ((newPos.y - camBoundsExtents.y) > backgroundStart.y && (newPos.y + camBoundsExtents.y) < backgroundEnd.y)
        {
            yLegal = true;
        }
        // Case2: y position of the newpos is bottom of the horizontal borders of the game. However, we are moving towards the bottom boundary,
        // so its legal.
        else if ((newPos.y - camBoundsExtents.y) <= backgroundStart.y && diff.y < 0)
        {
            yLegal = true;
        }
        // Case3: Same as case 2 only for the top boundary.
        else if ((newPos.y + camBoundsExtents.y) >= backgroundEnd.y && diff.y > 0)
        {
            yLegal = true;
        }
        return yLegal;
    }

    //    public bool IsMoveLegal(Vector3 halfSize, Vector3 newPos)
    //    {
    //        Vector3 diff = currentPosition - newPos;
    //        bool xLegal = false;
    //        bool yLegal = false;
    //
    //        if ((newPos.x - halfSize.x) > backgroundStart.x)
    //        {
    //            xLegal = true;
    //        }
    //        else if (diff.x < 0)
    //        {
    //            xLegal = true;
    //        }
    //
    //        if ((newPos.x + halfSize.x) < backgroundEnd.x)
    //        {
    //            xLegal = true;
    //        }
    //        else if (diff.x > 0)
    //        {
    //            xLegal = true;
    //        }
    //
    //        if ((newPos.y - halfSize.y) > backgroundStart.y)
    //        {
    //            yLegal = true;
    //        }
    //        else if (diff.y < 0)
    //        {
    //            yLegal = true;
    //        }
    //
    //        if ((newPos.y + halfSize.y) < backgroundEnd.y)
    //        {
    //            yLegal = true;
    //        }
    //        else if (diff.y > 0)
    //        {
    //            yLegal = true;
    //        }
    //
    //
    //        if (debug)
    //            Debug.Log(" Is legal: X? " + xLegal + " Y? " + yLegal);
    //
    //        return xLegal && yLegal;
    //    }
    /// <summary>
    /// Enables the movement.
    /// </summary>
    /// <param name="enabled">If set to <c>true</c> enabled.</param>
    public void EnableMovement(bool enabled)
    {
        enableMovement = enabled;
    }

    /// <summary>
    /// Gets the backround dimenesions.
    /// </summary>
    /// <returns>The backround dimensions</returns>
    public BackgroundDimensions GetBackroundDim()
    {
        return bgDim;
    }
}