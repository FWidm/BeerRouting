using UnityEngine;
using System.Collections;
using System;


public class CameraPan : MonoBehaviour
{
    CameraFollowPlayer cameraScript;
    public bool zoomIn = false, zoomOut = false, active = false;
    public float camHeight, camMax = 100, camMin = 1;

    public bool debugging = true;

    //fancy camera panning
    private float dampTime = 1f;
    private float maxSpeed = 7f;

    // Approximate time for the camera to refocus.
    private Vector3 moveVelocity;
    private Vector3 pos;
    private bool resetOnce = false;
    private float camHeighSpeed = .1f;
    private float maxZoomOutHeight = 100.0f;
    private BackgroundDimensions bgDim;

    // Determines the current gradient of the zoom out speed function.
    private float zoomOutModifier = 0.5f;

    // Use this for initialization
    void Start()
    {
        cameraScript = GetComponentInChildren<CameraFollowPlayer>();
        Debug.Log("CameraPan>>" + cameraScript);
        camHeight = cameraScript.cameraHeight;
        Debug.Log("CameraPan>> camHeight instantiated " + camHeight);
        Bounds b = cameraScript.OrthographicBounds();
        bgDim = cameraScript.GetBackroundDim();
    }



    void Update()
    {
        Bounds b = cameraScript.OrthographicBounds();

        if (Input.GetKeyUp(KeyCode.RightShift))
        {
            ZoomIn(new Vector3(30.5f, 25, 0), .1f, 3, 100.0f);
        }
        if (Input.GetKeyUp(KeyCode.RightCommand))
        {
            ZoomOut(bgDim.GetPivot(), .1f);
        }

        bool arrivedAtPosition = Vector3.Distance(transform.position, pos) < 1.0f;
        if (active)
        {
            if (pos != null && !arrivedAtPosition)
            {
                transform.position = Vector3.SmoothDamp(transform.position, pos, ref moveVelocity, dampTime, maxSpeed);
                // Debug.Log("Zoomout? height=" + camHeight + " Max=" + camMax);
                if (zoomIn && camHeight < camMax && camHeight <= maxZoomOutHeight)
                {
                    // Smooth the zoom out process using an exponential function.
                    // Goal was to start with a fast zooming and reduce the speed rapidly over time.
                    float zoomOutParameter = 1.0f / Mathf.Pow(1.7f, zoomOutModifier);
                    zoomOutParameter = zoomOutParameter * camHeighSpeed;
                    zoomOutModifier = zoomOutModifier + 0.10f;
                    camHeight += zoomOutParameter;
                }
            }

            if (arrivedAtPosition && zoomOut)
            {
                camHeight += camHeighSpeed;
//                Debug.Log("CameraPan>>" + cameraScript + " -> " + camHeight);
            }
            if (arrivedAtPosition && zoomIn)
            {
                camHeight -= camHeighSpeed;
//                Debug.Log("CameraPan>>" + cameraScript + " -> " + camHeight);

            }
            Camera.main.orthographicSize = camHeight;
            Camera.main.orthographicSize = Mathf.Clamp(camHeight, cameraScript.cameraHeightMin, camMax);
            // b.extends<---(bg) ------- b.center
            if (arrivedAtPosition && zoomOut && bgDim.GetBackgroundStart().x > b.center.x - b.extents.x
                && bgDim.GetBackgroundStart().y > b.center.y - b.extents.y)
            {
                Debug.Log("Finished zooming out!");
                active = false;
            }
            if (zoomIn && arrivedAtPosition && camHeight <= camMin)
            {
                Debug.Log("Finished zooming in!");
                active = false;
            }
        }
        else if (resetOnce)
        {
            Invoke("StopZoom", 3);
            resetOnce = false;
        }

    }

    /// <summary>
    /// Moves and zooms into the specified position.
    /// </summary>
    /// <param name="pos">Zoom in.</param>
    /// <param name="camHeighSpeed">Speed for the zoom in.</param>
    /// <param name="camMin">camera maximum zoom in value.</param>
    /// <param name="maxZoomHeight">Max zoom height.</param>
    public void ZoomIn(Vector3 pos, float camHeighSpeed, int camMin, float maxZoomHeight)
    {
        if (!active)
        {
            cameraScript.EnableMovement(false);
            cameraScript.enabled = false;
            zoomOut = false;
            zoomIn = true;
            camHeight = cameraScript.cameraHeight;   
            active = true;
            resetOnce = true;

            // start movement.
            this.pos = new Vector3(pos.x, pos.y, Camera.main.transform.position.z);
            zoomOutModifier = -1.0f;
            maxZoomOutHeight = maxZoomHeight;

            this.camHeighSpeed = camHeighSpeed;

            if (debugging)
            {
                // DrawDebugImage(pos, "test");
                Debug.Log(("Zooming in on position=" + pos));
            }
                
            if (camMin > 0)
                this.camMin = camMin;
        }

    }

    /// <summary>
    /// Zooms out from the given position with the given camera speed.
    /// </summary>
    /// <param name="pos">Position to move to.</param>
    /// <param name="camHeighSpeed">Speed for the zoom out.</param>
    public void ZoomOut(Vector3 pos, float camHeighSpeed)
    {
        if (!active)
        {
            cameraScript.EnableMovement(false);
            cameraScript.enabled = false;
            zoomOut = true;
            zoomIn = false;
            camHeight = cameraScript.cameraHeight;
            active = true;
            resetOnce = true;
            this.camHeighSpeed = camHeighSpeed;
            this.pos = new Vector3(pos.x, pos.y, Camera.main.transform.position.z);
            if (debugging)
            {
                Debug.Log(("Zooming out on position=" + pos));
            }
                
        }


    }

    /// <summary>
    /// Draws a debug image on the given position.
    /// </summary>
    /// <param name="pos">Position.</param>
    /// <param name="name">Resource name e.g. "test" -> Resources/test</param>.</param>
    void DrawDebugImage(Vector3 pos, String name)
    {
        GameObject start = new GameObject(name, typeof(SpriteRenderer));
        start.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("BeerPuddle");
        start.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
        start.GetComponent<SpriteRenderer>().sortingOrder = 11111;
        start.transform.position = pos;
        start.transform.name = name;
    }

    /// <summary>
    /// Stops the zoom.
    /// </summary>
    public void StopZoom()
    {
        
        Debug.Log("CameraPan>> height=" + camHeight + " max=" + 10 + "min=" + cameraScript.cameraHeightMin);
        zoomOut = false;
        zoomIn = false; 
        cameraScript.enabled = true;
        cameraScript.EnableMovement(true);
        active = false;
    }

}
