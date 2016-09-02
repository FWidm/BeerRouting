using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{

    // Indicates whether logging statements should be executed.
    public bool isLogEnabled = false;

    protected GameObject[] listOfNodes;
    protected GameObject[] listOfEdges;

    protected RouterScript[] listOfRouterScripts;
    protected PathScript[] listOfPathScripts;
    // The graph, i.e. the network topology represented as a multidimensional array.
    //private int[,] graphRepresentation;
    protected PathScript[,] graphRepresentation2;
    // The router that is currently handled in the dijkstra algorithm.
    protected RouterScript currentWorkingRouter;

    protected PriorityQueue<RouterScript> prioQueue;


    //  private List<RouterScript> referencePath;
    protected List<RouterScript> currentPath;

    protected GameTuple currentRun;

    // The router on which the player is currently located.
    protected GameObject activeRouter;
    // The neighbours of the currently active router.
    protected List<RouterScript> neighboursOfActiveRouter;

    protected bool errorRecovery;
    protected RouterScript lastValidRouter;

    protected GameStatus gameStatus;

	
    protected void Initialize()
    {
        if (isLogEnabled)
            Debug.Log("Awake the GameManager.");

        // Extract scripts from nodes and edges
        listOfNodes = GameObject.FindGameObjectsWithTag("Node");
        listOfEdges = GameObject.FindGameObjectsWithTag("Path");

        // Initialize graph datastructure.
        //graphRepresentation = new int[listOfNodes.Length,listOfNodes.Length];  // Nodes x Nodes
        graphRepresentation2 = new PathScript[listOfNodes.Length, listOfNodes.Length]; // Nodex x Nodes

        // Keep track of the predecessor router for each router on the shortest path.
        //predecessorRouter = new Dictionary<RouterScript, RouterScript> ();

        if (isLogEnabled)
            Debug.Log("Start extracting scripts in GameManager.");

        // listOfRouterScripts = new RouterScript[listOfNodes.Length];
        // listOfPathScripts = new PathScript[listOfEdges.Length];
       
        // Extract scripts from nodes and edges
        listOfNodes = GameObject.FindGameObjectsWithTag("Node");
        listOfEdges = GameObject.FindGameObjectsWithTag("Path");

        // Initialize graph datastructure.
        //graphRepresentation = new int[listOfNodes.Length,listOfNodes.Length];  // Nodes x Nodes
        graphRepresentation2 = new PathScript[listOfNodes.Length, listOfNodes.Length]; // Nodex x Nodes


        listOfRouterScripts = new RouterScript[listOfNodes.Length];
        listOfPathScripts = new PathScript[listOfEdges.Length];
        for (int i = 0; i < listOfNodes.Length; i++)
        {
            listOfRouterScripts[i] = listOfNodes[i].GetComponent<RouterScript>();
        }
        for (int i = 0; i < listOfEdges.Length; i++)
        {
            listOfPathScripts[i] = listOfEdges[i].GetComponent<PathScript>();
        }

        // Check router names.
        checkIntegrityOfRouterNames();

        if (isLogEnabled)
        {
            Debug.Log("Finished extracting scripts in GameManager.");
            Debug.Log("Finished Awake method of the GameManager.");
        }

        if (isLogEnabled)
            Debug.Log("Start initializing graph datastructure.");

        // Fill graph representation array with information taken from the nodes and edges
        for (int i = 0; i < listOfPathScripts.Length; i++)
        {
            RouterScript from = listOfPathScripts[i].from.GetComponent<RouterScript>();
            RouterScript to = listOfPathScripts[i].to.GetComponent<RouterScript>();

            graphRepresentation2[from.GetRouterIndex(), to.GetRouterIndex()] = listOfPathScripts[i];
        }

        if (isLogEnabled)
            Debug.Log("Finished initializing graph datastructure.");
        
    }

    /// <summary>
    /// Checks the integrity of router names.
    /// </summary>
    private void checkIntegrityOfRouterNames()
    {
        List<string> routerNames = new List<string>();
        for (int i = 0; i < listOfRouterScripts.Length; i++)
        {
            Assert.IsTrue(!routerNames.Contains(listOfRouterScripts[i].GetRouterName()),
                "Duplicate router name. Duplicate router name is " + listOfRouterScripts[i].GetRouterName() +
                " in game object " + listOfRouterScripts[i]);
            routerNames.Add(listOfRouterScripts[i].GetRouterName());
        }
    }

    /// <summary>
    /// Recreates the graph representation. Takes blocked routers into account.
    /// </summary>
    public void recreateGraphRepresentation()
    {
        graphRepresentation2 = new PathScript[listOfNodes.Length, listOfNodes.Length]; // Nodex x Nodes

        // Fill graph representation array with information taken from the nodes and edges.
        // Take blocked routers into the account.
        for (int i = 0; i < listOfPathScripts.Length; i++)
        {
            RouterScript from = listOfPathScripts[i].from.GetComponent<RouterScript>();
            RouterScript to = listOfPathScripts[i].to.GetComponent<RouterScript>();

            if (!from.Disabled && !to.Disabled)
            {
                // Include only active paths.
                graphRepresentation2[from.GetRouterIndex(), to.GetRouterIndex()] = listOfPathScripts[i];
            }
            else
            {
                listOfPathScripts[i].Disabled = true;
            }
        }
    }

    /// <summary>
    /// Initializes the basic parameters for the new run.
    /// </summary>
    /// <param name="startAndEndPoint">Start and end point of the run.</param>
    protected void InitializeRun(GameTuple startAndEndPoint)
    {
        currentRun = startAndEndPoint;
        currentPath = new List<RouterScript>();
        currentPath.Add(startAndEndPoint.source.GetComponent<RouterScript>());

        activeRouter = startAndEndPoint.source;
        if (isLogEnabled)
            Debug.Log("The current player position is: " + activeRouter);
    }

    /// <summary>
    /// Equivalent to neighborPaths in the GreedyManager, returns all children/neighbors of this router.
    /// </summary>
    /// <returns>The neighbors/children of the current working router</returns>
    /// <param name="current">Current Working router</param>
    public List<RouterScript> ExpandNode(RouterScript current)
    {
        List<RouterScript> neighbors = new List<RouterScript>();
        List<PathScript> neighborPaths = new List<PathScript>();

        int currentRouterIndex = current.GetRouterIndex();
        //Extract paths from graph representation.
        for (int i = 0; i < graphRepresentation2.GetLength(1); i++)
        {
            if (graphRepresentation2[currentRouterIndex, i] != null)
            {
                neighborPaths.Add(graphRepresentation2[currentRouterIndex, i]);
            }
        }

        foreach (PathScript p in neighborPaths)
        {
            RouterScript from = p.from.GetComponent<RouterScript>();
            RouterScript to = p.to.GetComponent<RouterScript>();
            if (current == from)
            {
                neighbors.Add(to);
            }
        }

        return neighbors;
    }

    /// <summary>
    /// Removes the router from the priority queue.
    /// </summary>
    /// <param name="routerScript">Router script.</param>
    protected void RemoveRouterFromPrioQueue(RouterScript routerScript)
    {
        if (prioQueue.IsContained(routerScript))
        {
            if (prioQueue.Peek() == routerScript)
            {
                // It is the first element, so pull it from the queue.
                prioQueue.PullHighest();
            }
            else
            {
                // Another router with the same distance is currently in front of the target router. 
                // Remove those routers before the target router, but put them back into the queue afterwards.
                List<RouterScript> routerCache = new List<RouterScript>();
                while (prioQueue.Count() > 0)
                {
                    if (prioQueue.Peek() != routerScript)
                    {
                        routerCache.Add(prioQueue.PullHighest());
                    }
                    else
                    {
                        // Found the target router. Remove it from the queue and abort the loop.
                        prioQueue.PullHighest();
                        break;
                    }
                }

                // Put cached routers back into priority queue.
                for (int i = 0; i < routerCache.Count; i++)
                {
                    prioQueue.Enqueue(routerCache[i]);
                }
            }
        }
    }

    /// <summary>
    /// Finds the optimal between the source and the destination router using the metric hop count.
    /// </summary>
    /// <returns>The optimal path as a list.</returns>
    /// <param name="source">Source.</param>
    /// <param name="destination">Destination.</param>
    protected List<GameObject> findOptimalHopCountPath(GameObject source, GameObject destination)
    {
        if (isLogEnabled)
            Debug.Log("Finding best path!");

        prioQueue = new PriorityQueue<RouterScript>();
        RouterScript current = source.GetComponent<RouterScript>();
        current.SetPriority(0);

        Dictionary<RouterScript, RouterScript> parentRelationship = new Dictionary<RouterScript, RouterScript>();

        List<RouterScript> closed = new List<RouterScript>();
        List<RouterScript> children = new List<RouterScript>();
        if (isLogEnabled)
            Debug.Log("Starting findOptimalHopCountPath from " + current.name + " to " + destination.name);

        while (current.gameObject != destination)
        {
            if (!closed.Contains(current)) // Don't need to check this node again.
            {
                children = ExpandNode(current);
                foreach (RouterScript tmp in children)
                {
                    // Check whether the router has already been handled. If this is the case,
                    // we don't need to add them to the priority queue again.
                    if (closed.Contains(tmp))
                        continue;

                    // Increase the priority. One more hop.
                    tmp.SetPriority(current.GetPriority() + 1);

                    // Store parent (predecessor router of current router) for child.
                    // Only do this if no router has already been defined as the predecessor router.
                    if (parentRelationship.ContainsKey(tmp))
                    {
                        // do nothing.
                        if (isLogEnabled)
                            Debug.Log("There is already a predecessor for the router " + tmp.name + "defined.");
                    }
                    else
                    {
                        parentRelationship.Add(tmp, current);

                        if (isLogEnabled)
                            Debug.Log("Setting predecessor for router: " + tmp.name + ", the predecessor is: " + current.name);
                    }

                    if (isLogEnabled)
                        Debug.Log("Adding to prioQueue: router: " + tmp.name + " | with priority: " + tmp.GetPriority());

                    prioQueue.Enqueue(tmp);
                }

                // Mark router as handled.
                closed.Add(current);
            }

            if (isLogEnabled)
                Debug.Log("Handling of router: " + current.name + " is done. Taking the next one.");

            // Continue with node that has the lowest priority at that time.
            current = prioQueue.PullHighest();
        }


        // Calculate optimal path.
        List<GameObject> optimalPath = new List<GameObject>();
        optimalPath.Insert(0, current.gameObject);

        while (parentRelationship[current].gameObject != source)
        {
            current = parentRelationship[current];
            optimalPath.Insert(0, current.gameObject);
        }
        optimalPath.Insert(0, source);

        if (isLogEnabled)
        {
            string logString = "Optimal Path: ";
            foreach (GameObject go in optimalPath)
            {
                logString += go.GetComponent<RouterScript>().GetRouterName() + " -> ";
            }
            Debug.Log(logString.Substring(0, logString.Length - 3));
        }

        return optimalPath;
    }


}
