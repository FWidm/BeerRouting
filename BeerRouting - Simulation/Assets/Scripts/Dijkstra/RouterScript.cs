using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

//this compiler flag enables unity to execute the update method everytime something changes
[ExecuteInEditMode]
public class RouterScript : MonoBehaviour, System.IComparable<RouterScript>, Priority
{
    public bool debugging = false;

    public bool Disabled
    {
        get;
        set;
    }

    public int layerOffsetHeuristicVal = 0;
    public float yOffset = -0.3f;
    public float xOffset = -0.1f;
    public float xOffsetHeuristicValue = 0.25f;
    public float yOffsetHeuristicValue = 0f;

    public float textScaling = 0.5f;
    // The name of the router.
    public string routerName;
    // Specifies whether this router should be treated like an autonomous system.
    public bool isAutonomousSystem = false;

    // The index which determines the position of the router in the multidimensional array that represents the graph in the DijkstraManager.
    private int routerIndex;

    // The currently shortest distance to the starting point for this router. This is the priority value used in the priority queue.
    private int currentDistance;

    //private GameObject routerNameDisplay;

    #region GreedyRelatedFields

    private GameObject greedyHeuristicDisplay;
    private int greedyHeuristicValue;

    #endregion GreedyRelatedFields

    // Temporary reference to the MovementScript instance during highlighting process.
    //private MovementScript _movementScript;


    // Use this for initialization
    void Start()
    {
        // Display the router's name on top of the router on the screen.
        displayRouterName(debugging);


        foreach (Transform child in transform)
        {
            if (child.name == "RouterHighlight" && GameObject.FindObjectOfType<LevelProperties>().gameType == LevelProperties.GameType.Dijkstra)
            {
                child.gameObject.SetActive(true);
                Animator anim = child.GetComponent<Animator>();
                anim.enabled = false;
                child.gameObject.SetActive(false);
            }
        }
    }
#if UNITY_EDITOR
    void Update()
    {
        if (EditorApplication.isPlaying)
            return;
        displayRouterName(false);
    }
#endif

    void Awake()
    {
        // if(debugging)Debug.Log("Setting the current distance in the router " + this.routerName + " in the Awake() method to the MaxValue of int.");
        Assert.IsTrue(routerName.Length == 1, "Router name must contain only 1 character.");

        currentDistance = int.MaxValue;
        routerName = routerName.ToUpper();
        generateID(routerName[0]);
    }

    //// Displays the router's name on the screen on top of the router sprite.
    //void OnGUI()
    //{
    //    Vector3 getPixelPos = Camera.main.WorldToScreenPoint(transform.position);
    //    getPixelPos.y = Screen.height - getPixelPos.y;
    //    GUI.Label(new Rect(getPixelPos.x, getPixelPos.y, 50.0f, 50.0f), routerName);
    //}

    /// <summary>
    /// Returns the current distance from the starting router to this router.
    /// </summary>
    /// <returns>The current distance.</returns>
    public int GetCurrentDistance()
    {
        return currentDistance;
    }

    /// <summary>
    /// Sets the current distance to a new value.
    /// </summary>
    /// <param name="currentDistance">The new distance value.</param>
    public void SetCurrentDistance(int currentDistance)
    {
        this.currentDistance = currentDistance;
    }

    /// <summary>
    /// Returns the name of the router.
    /// </summary>
    /// <returns>The router name.</returns>
    public string GetRouterName()
    {
        return routerName;
    }

    /// <summary>
    /// Returns the index of the router.
    /// </summary>
    /// <returns></returns>
    public int GetRouterIndex()
    {
        return routerIndex;
    }

    /// <summary>
    /// Compares this instance of the RouterScript to another instance of the RouterScript. It is
    /// checked which of these instances has a lower current distance value. If the current distance of
    /// the instance on which the method is called is less than those of the instance passed as a parameter, the
    /// method returns -1. If it is the other way round, the method returns 1. If both have the same current distance,
    /// the method returns 0.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(RouterScript other)
    {
        if (this.currentDistance < other.GetCurrentDistance())
        {
            return -1;
        }
        else if (this.currentDistance > other.GetCurrentDistance())
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }



    /// <summary>
    /// Sets the priority parameter for this instance of RouterScript. The priority parameter is relevant 
    /// if an instance should be kept within a priority queue. Here the priority is the current distance value.
    /// </summary>
    /// <param name="priority">The new priority value.</param>
    public void SetPriority(int priority)
    {
        // if(debugging)Debug.Log("Setting the priority of router " + this.routerName + " to: " + priority);
        this.currentDistance = priority;
    }

    /// <summary>
    /// Retrieve the current priority of this instance. Here the priority is the current distance value of this router.
    /// </summary>
    /// <returns>The current priortiy value.</returns>
    public int GetPriority()
    {
        return this.currentDistance;
    }

    /// <summary>
    /// Sets the value for the greedy heuristic for this router.
    /// </summary>
    /// <param name="greedyHeuristicValue"></param>
    public void SetGreedyHeuristicValue(int greedyHeuristicValue)
    {
        this.greedyHeuristicValue = greedyHeuristicValue;
    }

    /// <summary>
    /// Get the value for the greedy heuristic that is assigned to this router instance.
    /// </summary>
    /// <returns></returns>
    public int GetGreedyHeuristicValue()
    {
        return this.greedyHeuristicValue;
    }

    /// <summary>
    /// Displays the router's name on top of the router sprite.
    /// </summary>
    private void displayRouterName(bool debugmsg)
    {
        //if (isAutonomousSystem)
        // Don't display router name for autonomous systems.
        //    return;             

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        // Get the Text Mesh. The TextMesh is contained in the first child of the router game object.
        var childGameObject = transform.GetChild(0).gameObject;
        TextMesh textField = childGameObject.GetComponent<TextMesh>();
        textField.transform.localScale = new Vector3(0.1f, 0.1f, 1.0f);

        // Set the rendering order and the sorting order of the TextMesh.
        MeshRenderer textFieldRenderer = childGameObject.GetComponent<MeshRenderer>();
        textFieldRenderer.sortingLayerID = renderer.sortingLayerID;
        if (debugmsg)
            Debug.Log(name + " Renderer=" + renderer.sortingOrder + " TextOrder=" + textFieldRenderer.sortingOrder);
        textFieldRenderer.sortingOrder = renderer.sortingOrder + 1;     // Set sorting order +1 compared to the sprite renderer.

        // Set the position and text of the TextMesh.
        Vector3 routerPosition = transform.position;
        routerPosition.x = routerPosition.x + xOffset;
        routerPosition.y = routerPosition.y + yOffset;
        textField.transform.position = routerPosition;

        textField.text = routerName;


        //        routerNameDisplay = new GameObject("RouternameDisplay game object", typeof(SpriteFontRenderer));
        //        SpriteFontRenderer sfr = routerNameDisplay.GetComponent<SpriteFontRenderer>();
        //        sfr.layerOffset = layerOffsetHeuristicVal;
        //        sfr.resName = "Alphabet";
        //        sfr.sortingLayerName = "Routers";
        //        sfr.displayText = routerName;
        //        sfr.scaleAxis = textScaling;
        //        if (debugging)
        //            Debug.Log("Displaying for: " + name + " - costs=" + greedyHeuristicValue);
        //        routerNameDisplay.transform.parent = gameObject.transform;
        //        routerNameDisplay.transform.position = new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, transform.position.z);
    }

    /// <summary>
    /// Resets the text field of the router.
    /// </summary>
    public void ResetRouterText()
    {
        var childGameObject = transform.GetChild(0).gameObject;
        TextMesh textField = childGameObject.GetComponent<TextMesh>();
        textField.text = string.Empty;

        displayRouterName(debugging);
    }

    /// <summary>
    /// Highlights the router and thus marks is as finished in terms of the Dijkstra algorithm.
    /// </summary>
    public void HighlightRouter()
    {
        if (debugging)
            Debug.Log("Highlight router called for router: " + routerName);

        foreach (Transform child in transform)
        {
            if (child.name == "RouterHighlight")
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Disables the highlight on the router object.
    /// </summary>
    public void DisableHighlight()
    {
        foreach (Transform child in transform)
        {
            if (child.name == "RouterHighlight")
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Displays the greedy heuristic for the given router. The greedy heuristic is the
    /// distance from this router to the target router.
    /// </summary>
    public void DisplayGreedyHeuristic()
    {
        if (greedyHeuristicDisplay != null)
        {
            ResetGreedyHeuristicDisplay();
        }

        var childGameObject = transform.GetChild(0).gameObject;
        TextMesh textField = childGameObject.GetComponent<TextMesh>();

        textField.text += " - " + greedyHeuristicValue;
        //Debug.Log("heuristic val of <"+name+"> = "+greedyHeuristicValue+" textfield? "+textField.text);

        // TODO create game object.
        //        greedyHeuristicDisplay = new GameObject("greedyHeuristicDisplay game object", typeof(SpriteFontRenderer));
        //        SpriteFontRenderer sfr = greedyHeuristicDisplay.GetComponent<SpriteFontRenderer>();
        //        sfr.layerOffset = layerOffsetHeuristicVal;
        //        sfr.resName = "Alphabet";
        //        sfr.sortingLayerName = "Routers";
        //        sfr.displayText = "" + this.greedyHeuristicValue;
        //        sfr.scaleAxis = textScaling;
        //        if (debugging)
        //            Debug.Log("Displaying for: " + name + " - costs=" + greedyHeuristicValue);
        //        greedyHeuristicDisplay.transform.parent = gameObject.transform;
        //        greedyHeuristicDisplay.transform.position = new Vector3(transform.position.x + xOffsetHeuristicValue + xOffset, transform.position.y + yOffset + yOffsetHeuristicValue, transform.position.z);
    }

    public void ResetGreedyHeuristicDisplay()
    {
        if (greedyHeuristicDisplay != null)
        {
            Destroy(greedyHeuristicDisplay);
        }
    }

    public override string ToString()
    {
        string s = string.Format("Router Name: {0}, current distance: {2}", routerName, routerIndex, currentDistance);
        return s;
    }

    /// <summary>
    /// Generates an index for the router based on the provided
    /// character (which is usually the router name).
    /// </summary>
    /// <param name="c">The character from which the index is derived.</param>
    public void generateID(char c)
    {
        int offsetA = 65;
        routerIndex = (int)c - offsetA;
    }
}
