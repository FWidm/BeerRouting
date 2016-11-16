using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;
using System.Text;


public class PathScript : MonoBehaviour
{
    public bool debugging = false;

    public float yOffsetPathCostDisplay = .35f;
    //offsets for the text
    public float offsetX = 0, offsetY = 0;
    // Represents the router game object where the player is currently located.
    public GameObject from;
    // Represents the router game object to which the player is heading.
    public GameObject to;

    // The path costs for this path.
    public int pathCosts;

    public bool Disabled
    {
        get;
        set;
    }


    public bool PathHighlight
    {
        get;
        set;
    }
    public bool lastPathHL = false;
    // Indicates whether the path has already been discovered or not.
    private bool isDiscovered;

    // The script for the text object which displays the path costs on the screen.
    private TextScript textScript;

    // A reference to the MovementManager.
    private MovementManagerInterface movementManager;

    private LevelController levelController;
    private GameObject pathCostDisplay;
    private DisplayPathHovers displayPathHovers;

    private SoundManager soundManager;

    // Use this for initialization
    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        isDiscovered = false;
        HideLines();

        // Get the TextScript of this path from the child game object.
        //		var child = this.gameObject.transform.GetChild (1).gameObject;
        //		textScript = child.GetComponent<TextScript> ();
        //		textScript.SetPathText ("");
        displayDestinationRouter();

        movementManager = GameObject.FindGameObjectWithTag("MovementManager").GetComponent<MovementManagerInterface>();

        // Init current level controller.
        levelController = LevelController.GetCurrentLevelController();
        GameObject goDisplayPathHovers = GameObject.FindGameObjectWithTag("ButtonHighlight");
        if (goDisplayPathHovers != null)
            displayPathHovers = goDisplayPathHovers.GetComponent<DisplayPathHovers>();
        if (Application.platform != RuntimePlatform.Android)
        {
            displayPathHovers = null;
        }
    }


    public void Update()
    {
        LineRenderer lr = GetComponentInChildren<LineRenderer>(true);
        StringBuilder sb = new StringBuilder();

        if (PathHighlight != lastPathHL)
        {
            foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>(true))
            {
                if (renderer.name.Equals("PathHighlight"))
                {
                    renderer.enabled = PathHighlight;
                    sb.Append(renderer.transform.parent.transform.parent.name + ", ");
                }
            }
            lastPathHL = PathHighlight;

        }

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (lr != null)
            {
                lr.transform.gameObject.SetActive(true);
                sb.Remove(0, sb.Length);
                foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>(true))
                {
                    if (renderer.name.Equals("PathHighlight"))
                    {
                        renderer.enabled = true;
                        sb.Append(renderer.transform.parent.transform.parent.name + ", ");
                    }
                }
                lastPathHL = true;

            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            HideLines();
            foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>(true))
            {
                if (renderer.name.Equals("PathHighlight"))
                    renderer.enabled = false;

            }
            lastPathHL = false;

        }



        return;

    }

    /// <summary>
    /// Returns the path costs of the path.
    /// </summary>
    /// <returns></returns>
    public int GetPathCosts()
    {
        return pathCosts;
    }

    /// <summary>
    /// Sets the IsDiscovered value.
    /// </summary>
    /// <param name="discovered">True or false.</param>
    public void SetDiscovered(bool discovered)
    {
        isDiscovered = discovered;
    }

    /// <summary>
    /// Checks whether the path is already discoverd by the player.
    /// </summary>
    /// <returns>True, if the path is discovered, false otherwise.</returns>
    public bool IsDiscovered()
    {
        return isDiscovered;
    }

    /// <summary>
    /// Displays the path costsof the path.
    /// </summary>
    public void DisplayPathCosts()
    {
        if (debugging)
            Debug.Log("Display path cost.");
        //		textScript.SetPathText (pathCosts.ToString ());
        if (pathCostDisplay == null)
        {
            DisplayPathCostObject();
        }
    }

    public void DisplayPathCostObject()
    {
        // TODO create game object.
        pathCostDisplay = new GameObject("PathCostSpriteText", typeof(SpriteFontRenderer));
        SpriteFontRenderer sfr = pathCostDisplay.GetComponent<SpriteFontRenderer>();
        sfr.resName = "Numbers2";
        sfr.displayText = "" + pathCosts;
        sfr.sortingLayerName = "Paths";
        sfr.scaleAxis = 0.4f;
        if (debugging)
            Debug.Log("Displaying for: " + name + " - costs=" + pathCosts);
        pathCostDisplay.transform.parent = gameObject.transform;
        pathCostDisplay.transform.position = new Vector3(transform.GetChild(0).position.x - .10f, transform.GetChild(0).position.y + yOffsetPathCostDisplay, transform.GetChild(0).position.z);
    }

    public void ResetPathcost()
    {
        if (pathCostDisplay != null)
        {
            Destroy(pathCostDisplay);
        }
    }

    /// <summary>
    /// Displays the destination router on top of the beer bottle.
    /// </summary>
    public void displayDestinationRouter()
    {
        SpriteRenderer spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();

        // Get the Text Mesh. The TextMesh is contained in the first child of the router game object.
        TextMesh textField = null;
        foreach (TextMesh m in GetComponentsInChildren<TextMesh>())
        {
            if (m.name == "PathDestinationText")
                textField = m;
        }

        if (textField != null)
        {
            textField.transform.localScale = new Vector3(0.1f, 0.1f, 1.0f);
            // Set the rendering order and the sorting order of the TextMesh.
            MeshRenderer textFieldRenderer = textField.transform.GetComponent<MeshRenderer>();
            textFieldRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
            textFieldRenderer.sortingOrder = spriteRenderer.sortingOrder + 2;     // Set sorting order +2 compared to the sprite renderer.

            // Set the position and text of the TextMesh.
            Vector3 routerPosition = transform.position;
            routerPosition.x = routerPosition.x + offsetX;
            routerPosition.y = routerPosition.y + offsetY;
            textField.transform.position = routerPosition;

            RouterScript toScript = to.GetComponent<RouterScript>();

            textField.text = toScript.routerName;
        }

    }

    private void OnMouseUp()
    {
        // Check, if mouse input is enabled.
        if (levelController.IsGameInputEnabled())
        {
            movementManager.PerformMoveOnPath(this);
            levelController.OnPathClicked(this);
            if (soundManager != null)
                soundManager.PlaySound(SoundManager.SoundType.BottleClick);
            else
                Debug.Log("PathScript - No SoundManager found.");
        }
    }

    public void HideLines()
    {
        LineRenderer lr = GetComponentInChildren<LineRenderer>(true);
        if (lr != null)
        {
            lr.transform.gameObject.SetActive(false);
            // DestroyImmediate(lr.transform.gameObject);
        }
    }
}
