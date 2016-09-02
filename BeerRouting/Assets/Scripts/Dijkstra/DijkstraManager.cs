using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class DijkstraManager : MonoBehaviour
{

    // Indicates whether the player can walk a path in both directions.
    public bool bidirectional = true;

    // Indicates whether logging statements should be executed.
    public bool isLogEnabled = false;

    /// <summary>
    /// List of all dijkstra statuses in one scene.
    /// </summary>
    private List<DijkstraMove> listOfMoves = new List<DijkstraMove>();

    private GameObject[] listOfNodes;
    private GameObject[] listOfEdges;

    private RouterScript[] listOfRouterScripts;
    private PathScript[] listOfPathScripts;

    // The graph, i.e. the network topology represented as a multidimensional array.
    //private int[,] graphRepresentation;
    private PathScript[,] graphRepresentation2;

    // A dictionary which stores the predecessor router on the shortest path for a given router.
    private Dictionary<RouterScript, RouterScript> predecessorRouter;

    // The router on which the user is currently located.
    private RouterScript routerScriptCurrentPlayerPosition;

    // If player has performed an invalid action, the field contains the last valid router position.
    private RouterScript lastValidRouter;

    // The router that is currently handled in the dijkstra algorithm.
    private RouterScript currentWorkingRouter;

    // Indicates if the player has performed an invalid action and is now in error recovery mode.
    private bool errorRecovery;

    // The queue contains routers which haven't been handled by the algorithm yet.
    private PriorityQueue<RouterScript> priorityQueue;

    // Use this for initialization.
    void Start()
    {
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

    // Called on script awake.
    void Awake()
    {
        if (isLogEnabled)
            Debug.Log("Awake the DijkstraManager.");

        // Extract scripts from nodes and edges
        listOfNodes = GameObject.FindGameObjectsWithTag("Node");
        listOfEdges = GameObject.FindGameObjectsWithTag("Path");

        // Initialize graph datastructure.
        //graphRepresentation = new int[listOfNodes.Length,listOfNodes.Length];  // Nodes x Nodes
        graphRepresentation2 = new PathScript[listOfNodes.Length, listOfNodes.Length]; // Nodex x Nodes

        // Keep track of the predecessor router for each router on the shortest path.
        predecessorRouter = new Dictionary<RouterScript, RouterScript>();

        if (isLogEnabled)
            Debug.Log("Start extracting scripts in DijkstraManager.");

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

        checkIntegrityOfRouterNames();

        if (isLogEnabled)
        {
            Debug.Log("Finished extracting scripts in DijkstraManager.");
            Debug.Log("Finished Awake method of the DijkstraManager.");
        }            
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
    /// Starts the Dijkstra algorithm. Performs the required initialization and fills the priority queue.
    /// Sets the player's position to the starting router.
    /// </summary>
    /// <param name="startingRouter">The starting router of the algorithm.</param>
    public void StartDijkstraAlgorithm(RouterScript startingRouter)
    {
        if (isLogEnabled)
            Debug.Log("StartDijkstraAlgorithm() called.");

        // Create the priority queue.
        priorityQueue = new PriorityQueue<RouterScript>();

        // Initialize Dijkstra algorithm by storing nodes in priority queue.
        for (int i = 0; i < listOfRouterScripts.Length; i++)
        {
            RouterScript router = listOfRouterScripts[i];
            if (router == startingRouter)
            {
                if (isLogEnabled)
                    Debug.Log("router instance id: " + router.GetInstanceID() + " and starting router instance id " + startingRouter.GetInstanceID());

                // Set the current shortest path of the starting router to 0.
                router.SetPriority(0);
                predecessorRouter[router] = router; // The predecessor of the starting router is the router itself.

                // Set this router as the current player position and as the current working router.
                routerScriptCurrentPlayerPosition = router;
                currentWorkingRouter = router;
            }
            else
            {
                // For all other routers, set the current distance to the max int value.
                router.SetPriority(int.MaxValue);

                // Add the router to the priority queue.
                priorityQueue.Enqueue(router);
            }
        }

        if (isLogEnabled)
            Debug.Log("Finished StartDijkstraAlgorithm().");
    }

    /// <summary>
    /// Checks whether the attempted hop by the player is a valid hop which fulfills the conditions of the dijkstra algorithm.
    /// First, it is checked whether the path is connecting the router on which the player is currently located and an adjacent router.
    /// If not, it is an invalid hop. Second, it is checked whether the path is already discovered. If not, it is a valid hop, but a 
    /// hop in discovery mode. If the path it is not discovered and there are still undiscovered paths connected to the router on which the player is
    /// located, it is an invalid hop due to undiscovered neighbors.  Third, if the target router of this path has already been handled, the user
    /// can move back to this router, so its a valid hop. Finally, if non of the cases mentioned before has applied, the player wants to perform an 
    /// actual movement towards an router which should be handled by the dijkstra algorithm next. It is checked whether the target router determined by
    /// the path is actually a router that the dijkstra algorithm would handle next. If it is, it is a valid hop, otherwise it is an invalid hop concerning
    /// the dijkstra algorithm.
    /// </summary>
    /// <param name="path">The path between the two routers which should be passed in this hop.</param>
    /// <returns>The result of the dijkstra path check.</returns>
    public DijkstraStatus IsValidHop(PathScript path)
    {
        RouterScript from;
        RouterScript to;

        DijkstraMove dijkstraMove = new DijkstraMove();

        // Get path script.
        //PathScript currentPath = graphRepresentation2[from.GetRouterIndex(), to.GetRouterIndex()];
        PathScript currentPath = path;
        if (currentPath == null)
        {
            // Invalid hop, no path found between these routers.
            return DijkstraStatus.HOP_UNREACHABLE;
        }

        // Check on which of the routers the player is located.
        if (path.from.GetComponent<RouterScript>() == routerScriptCurrentPlayerPosition)
        {
            to = path.to.GetComponent<RouterScript>();
            from = path.from.GetComponent<RouterScript>();
        }
        else if (bidirectional && path.to.GetComponent<RouterScript>() == routerScriptCurrentPlayerPosition)
        {
            to = path.from.GetComponent<RouterScript>();
            from = path.to.GetComponent<RouterScript>();
        }
        else
        {
            dijkstraMove.Source = routerScriptCurrentPlayerPosition;
            dijkstraMove.Destination = currentPath.from.GetComponent<RouterScript>();
            dijkstraMove.Status = DijkstraStatus.HOP_UNREACHABLE;
            listOfMoves.Add(dijkstraMove);

            // Invalid hop if player is not located on one of these routers.
            return DijkstraStatus.HOP_UNREACHABLE;
        }

        // Store the routers in the DijkstraMove object.
        dijkstraMove.Source = from;
        dijkstraMove.Destination = to;


        // Check if player is in error recovery mode.
        if (errorRecovery)
        {
            dijkstraMove.Status = DijkstraStatus.ERROR_RECOVERY;
            listOfMoves.Add(dijkstraMove);

            return DijkstraStatus.ERROR_RECOVERY;
        }

        // Check whether the path is already discovered.
        if (!currentPath.IsDiscovered())
        {
            dijkstraMove.Status = DijkstraStatus.VALID_HOP_DISCOVERY;
            listOfMoves.Add(dijkstraMove);

            // It is a valid hop. It is a path discovery move.
            return DijkstraStatus.VALID_HOP_DISCOVERY;
        }

        // Check if all paths to the adjacent routers have already been discovered.
        for (int i = 0; i < graphRepresentation2.GetLength(1); i++)
        {
            PathScript pathToNeighbor = graphRepresentation2[from.GetRouterIndex(), i];
            if (pathToNeighbor != null && !pathToNeighbor.IsDiscovered())
            {
                dijkstraMove.Status = DijkstraStatus.UNDISCOVERED_PATHS;
                listOfMoves.Add(dijkstraMove);

                // Not a valid move. The player needs to perform path discovery on all paths of this router first.
                return DijkstraStatus.UNDISCOVERED_PATHS;
            }
        }

        // Check if the router has already been handled by the dijkstra algorithm. 
        // If the router has already been handled, the player can perform a hop to this router.
        // Perform this check after the 'all paths discovered' check to make sure the player has handled the current working router completely before moving back to a already handled router. 
        if (!priorityQueue.IsContained(to))
        {
            dijkstraMove.Status = DijkstraStatus.NOP;
            listOfMoves.Add(dijkstraMove);

            // Valid hop.
            return DijkstraStatus.NOP;
        }

        // Check if the hop conforms to the hop which will be performed by the Dijkstra algorithm.
        // To do this, get the next hop from the priority queue, i.e. the router with the currently shortest distance.
        if (priorityQueue.Peek() == to)
        {
            dijkstraMove.Status = DijkstraStatus.VALID_HOP;
            listOfMoves.Add(dijkstraMove);

            // All conditions are met. It is a valid move.
            return DijkstraStatus.VALID_HOP;
        }
        else
        {
            // It is still possible that there is another router which has the same distance and is thus a valid next hop.
            bool isValidHop = false;
            // To check this, we need to remove the elements before this router from the queue and later put them back in.
            RouterScript headOfQueue = priorityQueue.PullHighest();
            // Check the following routers. Stop if they have a higher priority than the original head.
            List<RouterScript> nextRouterCandidates = new List<RouterScript>();
            while (priorityQueue.Count() > 0 && priorityQueue.Peek().GetPriority() == headOfQueue.GetPriority())
            {
                RouterScript candidateRouter = priorityQueue.PullHighest();
                nextRouterCandidates.Add(candidateRouter);
                if (candidateRouter == to)
                {
                    isValidHop = true;
                    break;
                }
            }

            // Store the candidate routers and the original headOfQueue back into the priority queue.
            priorityQueue.Enqueue(headOfQueue);
            for (int i = 0; i < nextRouterCandidates.Count; i++)
            {
                priorityQueue.Enqueue(nextRouterCandidates[i]);
            }

            // Break if it isn't a valid hop.
            if (!isValidHop)
            {
                dijkstraMove.Status = DijkstraStatus.WRONG_HOP;
                listOfMoves.Add(dijkstraMove);

                return DijkstraStatus.WRONG_HOP;
            }
        }

        dijkstraMove.Status = DijkstraStatus.VALID_HOP;
        listOfMoves.Add(dijkstraMove);

        // All conditions are met. It is a valid move.
        return DijkstraStatus.VALID_HOP;
    }

    /// <summary>
    /// Performs the path discovery on the specified path. The path costs of this path 
    /// are determined and it is checked whether the target router can be reached with 
    /// less path costs using this path. If the target router can be reached with less
    /// path costs via this path, the distance (priority) of the router is adjusted and
    /// the previous router on this path stored in the predecessorRouter data structure.
    /// Finally, the path is set to discovered.
    /// </summary>
    /// <param name="path">The path for which the path discovery is performed.</param>
    public void PerformPathDiscovery(PathScript path)
    {
        RouterScript from = path.from.GetComponent<RouterScript>();
        RouterScript to = path.to.GetComponent<RouterScript>();

        RouterScript previousRouter = null;
        RouterScript discoveredRouter = null;

        // Check which router will be discovered.
        if (from == routerScriptCurrentPlayerPosition)
        {
            // Player discovers 'to' router.
            discoveredRouter = to;
            previousRouter = routerScriptCurrentPlayerPosition;
        }
        else if (bidirectional && to == routerScriptCurrentPlayerPosition)
        {
            // Player discovers 'from' router.
            discoveredRouter = from;
            previousRouter = routerScriptCurrentPlayerPosition;
        }

        // Update the current distance if we have found a shorter path to the discovered router.
        // This only needs to be done when the discovered router has not already been handled by the dijkstra algorithm.
        if (priorityQueue.IsContained(discoveredRouter))
        {
            // Path costs to the discovered router are the path cost to the previous router + the path costs of this path.
            int pathCost = previousRouter.GetPriority() + graphRepresentation2[previousRouter.GetRouterIndex(), discoveredRouter.GetRouterIndex()].GetPathCosts();
            // Are the new path costs lower than the currently stored lowest path costs.
            if (pathCost < discoveredRouter.GetPriority())
            {
                // Update the path costs of the router.
                priorityQueue.DecreasePriority(discoveredRouter, pathCost);
                // Set the new predecessor for this router.
                predecessorRouter[discoveredRouter] = previousRouter;
            }
        }

        // Set path to discovered.
        graphRepresentation2[previousRouter.GetRouterIndex(), discoveredRouter.GetRouterIndex()].SetDiscovered(true);
        //graphRepresentation2[previousRouter.GetRouterIndex(), discoveredRouter.GetRouterIndex()].DisplayPathCosts();

        // If there is a path back with the same costs, discover this path as well.
        PathScript backPath = graphRepresentation2[discoveredRouter.GetRouterIndex(), previousRouter.GetRouterIndex()];
        if (backPath != null && backPath.GetPathCosts() == path.GetPathCosts())
        {
            backPath.SetDiscovered(true);
            //backPath.DisplayPathCosts();
        }

        // Test: Output the current priority queue.
        // Debug.Log(priorityQueue.ToString());
    }

    /// <summary>
    /// Performs a hop along the path to the target router. The target router will be the next
    /// router that is handled by the dijkstra algorithm. The target router is removed from the 
    /// priority queue which marks it as processed. For this router no shorter path can be found. 
    /// </summary>
    /// <param name="path"> The path on which the hop is performed. The path determines the previous router
    ///                     and the target router.</param>
    public void PerformHop(PathScript path)
    {
        // Update the current player position.
        UpdateCurrentPlayerPosition(path);

        // Update the current working router.
        currentWorkingRouter = routerScriptCurrentPlayerPosition;

        // The chosen router is now getting processed by the dijkstra algorithm and cannot get a shorter path afterwards.
        // Take it out of the priority queue.
        if (priorityQueue.IsContained(routerScriptCurrentPlayerPosition))
        {
            if (priorityQueue.Peek() == routerScriptCurrentPlayerPosition)
            {
                // It is the first element, so pull it from the queue.
                priorityQueue.PullHighest();
            }
            else
            {
                // Another router with the same distance is currently in front of the target router. 
                // Remove those routers before the target router, but put them back into the queue afterwards.
                List<RouterScript> routerCache = new List<RouterScript>();
                while (priorityQueue.Count() > 0)
                {
                    if (priorityQueue.Peek() != routerScriptCurrentPlayerPosition)
                    {
                        routerCache.Add(priorityQueue.PullHighest());
                    }
                    else
                    {
                        // Found the target router. Remove it from the queue and abort the loop.
                        priorityQueue.PullHighest();
                        break;
                    }
                }

                // Put cached routers back into priority queue.
                for (int i = 0; i < routerCache.Count; i++)
                {
                    priorityQueue.Enqueue(routerCache[i]);
                }
            }
        }
    }

    /// <summary>
    /// Performs a hop which is considered invalid due to the dijkstra algorithm.
    /// Starts the error recovery which lasts until the player returns to the last valid hop.
    /// </summary>
    /// <param name="path">The path along which the player performs the hop.</param>
    public void PerformWrongHop(PathScript path)
    {
        if (isLogEnabled)
            Debug.Log("Started error recovery mode.");

        // Start the error recovery.
        errorRecovery = true;
        // Store the last valid router to that the player needs to return.
        // Last valid position is the router on which the player is located before the wrong hop.
        lastValidRouter = routerScriptCurrentPlayerPosition;

        // Update the current player position.
        UpdateCurrentPlayerPosition(path);
    }

    /// <summary>
    /// Performs a hop within error recovery mode. It is checked whether this hop
    /// brings the player back to the last valid router. If the player returns to the
    /// last valid router, the error recovery mode will be disabled. Otherwise,
    /// the player remains in error recovery mode.
    /// </summary>
    /// <param name="path">The path along which the player performs the hop.</param>
    /// <returns>The current status of the error recovery.</returns>
    public ErrorRecoveryStatus PerformErrorRecoveryHop(PathScript path)
    {
        RouterScript from = path.from.GetComponent<RouterScript>();
        RouterScript to = path.to.GetComponent<RouterScript>();

        // Check if player returns to the last valid hop.
        if (lastValidRouter == to || (bidirectional && lastValidRouter == from))
        {
            lastValidRouter = null;
            // Error recovery finished.
            errorRecovery = false;

            if (isLogEnabled)
                Debug.Log("Left error recovery mode.");
        }

        // Update the current player position according to the performed hop.
        UpdateCurrentPlayerPosition(path);

        // Check if still in error recovery mode.
        if (errorRecovery)
        {
            return ErrorRecoveryStatus.ERROR_NOT_RECOVERED;
        }
        return ErrorRecoveryStatus.ERROR_RECOVERED;
    }

    public bool IsErrorRecovery()
    {
        return errorRecovery;
    }

    /// <summary>
    /// A helper method that updates the current player position depending on the specified
    /// path. The method takes into account whether bidirectional movements are allowed.
    /// </summary>
    /// <param name="path">The path on which the player performs the hop.</param>
    private void UpdateCurrentPlayerPosition(PathScript path)
    {
        RouterScript from = path.from.GetComponent<RouterScript>();
        RouterScript to = path.to.GetComponent<RouterScript>();

        // Check to which router the player moves.
        if (from == routerScriptCurrentPlayerPosition)
        {
            // Player performs hop to router "to".
            routerScriptCurrentPlayerPosition = to;
        }
        else if (bidirectional && to == routerScriptCurrentPlayerPosition)
        {
            // Player performs hop to router "from".
            routerScriptCurrentPlayerPosition = from;
        }
    }

    /// <summary>
    /// Returns the GameObject of the router on which the player is currently located.
    /// This router represents the current working router of the dijkstra algorithm.
    /// </summary>
    /// <returns>The GameObject of the router object on which the player is located.</returns>
    public GameObject GetCurrentPlayerPosition()
    {
        return routerScriptCurrentPlayerPosition.gameObject;
    }

    /// <summary>
    /// Returns the RouterScript object of the router that is currently handled by the dijkstra algorithm.
    /// </summary>
    /// <returns>The RouterScript of the current working router.</returns>
    public RouterScript GetCurrentWorkingRouter()
    {
        return currentWorkingRouter;
    }

    /// <summary>
    /// Returns the script of the router which is a candidate for the next hop regarding the dijkstra algorithm.
    /// </summary>
    /// <returns> A RouterScript instance.</returns>
    public RouterScript GetNextRouterSuggestion()
    {
        return priorityQueue.Peek();
    }

    /// <summary>
    /// Checks whether the current working router is completely handled, i.e. all paths from 
    /// or to this router have been discovered by the player.
    /// </summary>
    /// <returns>Returns true, if all paths from or to the current working router are discovered. Returns false, if there 
    ///     are still undiscovered paths.</returns>
    public bool IsCurrentWorkingRouterHandledCompletely()
    {
        bool handledCompletely = true;

        for (int i = 0; i < graphRepresentation2.GetLength(1); i++)
        {
            PathScript path = graphRepresentation2[routerScriptCurrentPlayerPosition.GetRouterIndex(), i];  // extract a path from or to the current working router.
            if (path != null)
            {
                if (!path.IsDiscovered())
                {
                    handledCompletely = false;  // There is still an undiscovered path, so the working router isn't handled completely so far.
                }
            }
        }

        return handledCompletely;
    }

    /// <summary>
    /// Returns the DijkstraMove object of the current move of the player. 
    /// </summary>
    /// <returns>The DijkstraMove object of the current move.</returns>
    public DijkstraMove GetCurrentMove()
    {
        if (listOfMoves.Count > 0)
        {
            return listOfMoves[listOfMoves.Count - 1];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Returns the DijkstraMove object of the previous move of the player. 
    /// </summary>
    /// <returns>The DijkstraMove object of the previous move.</returns>
    public DijkstraMove GetPreviousMove()
    {
        if (listOfMoves.Count > 1)
        {
            return listOfMoves[listOfMoves.Count - 2];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Returns a list of all moves that the player has performed.
    /// </summary>
    /// <returns>A list of DijkstraMove objects.</returns>
    public List<DijkstraMove> GetAllMoves()
    {
        return listOfMoves;
    }

    /// <summary>
    /// Searches all routers in the priority queue which currently have the shortest distance and
    /// are a suitable candidate for the next working router in the Dijkstra algorithm.
    /// </summary>
    /// <returns>A string which contains the names of the candidate routers.</returns>
    public string GetNextRouterSuggestionAsString()
    {
        if (priorityQueue.Count() == 0)
        {
            return string.Empty;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        RouterScript headOfQueue = priorityQueue.PullHighest();
        sb.Append(headOfQueue.GetRouterName() + ", ");

        // Check if the next element in the priority queue also has the same distance.
        if (headOfQueue.GetCurrentDistance() == priorityQueue.Peek().GetCurrentDistance())
        {
            // Retrieve all routers that have the same current distance.
            List<RouterScript> routerCache = new List<RouterScript>();
            while (headOfQueue.GetCurrentDistance() == priorityQueue.Peek().GetCurrentDistance())
            {
                sb.Append(priorityQueue.Peek().GetRouterName() + ", ");
                routerCache.Add(priorityQueue.PullHighest());
            }

            // Put the routers back in.
            foreach (RouterScript router in routerCache)
            {
                priorityQueue.Enqueue(router);
            }
        }

        // Remove the last comma.
        sb.Remove(sb.Length - 2, 2);

        // Put head of priority queue back.
        priorityQueue.Enqueue(headOfQueue);

        return sb.ToString();
    }

    /// <summary>
    /// Returns the amount of undiscovered routers.
    /// </summary>
    /// <returns>The amount of undiscovered routers.</returns>
    public int GetAmountOfUndiscoveredRouters()
    {
        return priorityQueue.Count();
    }

    /// <summary>
    /// Returns the currently known shortest path from the starting point to each router and the corresponding predecessor router on this shortest path as a string.
    /// </summary>
    /// <returns>The information about the shortest path as a string.</returns>
    public string GetShortestPathsListAsString()
    {
        System.Text.StringBuilder s = new System.Text.StringBuilder();
        s.Append("Routingtabelle" + System.Environment.NewLine);

        // Sort according to the current priority of the router.
        listOfRouterScripts = sortRouterArrayByPriority(listOfRouterScripts);

        for (int i = 0; i < listOfRouterScripts.Length; i++)
        {
            if (predecessorRouter.ContainsKey(listOfRouterScripts[i]))
            {
                s.Append("Tisch " + listOfRouterScripts[i].GetRouterName() +
                    ": (" + listOfRouterScripts[i].GetPriority() + "," + predecessorRouter[listOfRouterScripts[i]].GetRouterName() + ")" + System.Environment.NewLine);
            }
            else
            {
                // Router has no applied shortest path so far.
                s.Append("Tisch " + listOfRouterScripts[i].GetRouterName() + ": (inf,-) " + System.Environment.NewLine);
            }
        }

        return s.ToString();
    }


    /// <summary>
    /// Returns an array of RouterScript objects that are stored in the graph which is currently
    /// managed by the DijkstraManager.
    /// </summary>
    /// <returns>An array of RouterScripts.</returns>
    public RouterScript[] GetRoutersInGraph()
    {
        if (listOfRouterScripts != null)
        {
            return listOfRouterScripts;
        }
        return null;
    }

    /// <summary>
    /// Returns the inverse path which corresponds to specified path. The inverse path is the
    /// path which links the same routers, but from another direction.
    /// </summary>
    /// <param name="path">The path for which the inverse path should be extracted.</param>
    /// <returns>The PathScript of the inverse path.</returns>
    public PathScript GetInversePath(PathScript path)
    {
        PathScript option1 = graphRepresentation2[path.to.GetComponent<RouterScript>().GetRouterIndex(), path.from.GetComponent<RouterScript>().GetRouterIndex()];
        PathScript option2 = graphRepresentation2[path.from.GetComponent<RouterScript>().GetRouterIndex(), path.to.GetComponent<RouterScript>().GetRouterIndex()];

        if (path == option1)
        {
            return option2;
        }
        else
        {
            return option1;
        }
    }

    /// <summary>
    /// Returns the predecessor router of the target router on the shortest path from the starting
    /// router to the target router.
    /// </summary>
    /// <param name="targetRouter">The router whose predecessor on the shortest path should be extracted.</param>
    /// <returns>The RouterScript instance from the predecessor router or null if there is no valid predecessor for the target router.</returns>
    public RouterScript GetPredecessorRouterOnShortestPath(RouterScript targetRouter)
    {
        // Check if router has a valid predecessor.
        if (predecessorRouter.ContainsKey(targetRouter))
        {
            return predecessorRouter[targetRouter];
        }
        return null;
    }

    /// <summary>
    /// Indicates whether the specified router is already marked as handled in the current dijkstra run.
    /// </summary>
    /// <param name="router">The RouterScript instance of the router that should be checked.</param>
    /// <returns>Returns true, if the router is already handled, false otherwise.</returns>
    public bool IsRouterAlreadyHandled(RouterScript router)
    {
        if (router != null && priorityQueue != null)
        {
            if (priorityQueue.IsContained(router))
            {
                // Not yet handled as it is still in the priority queue.
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// A helper method that sorts the entries of the array of RouterScript instances according
    /// to the current priority of their corresponding router. The entries are 
    /// sorted ascendingly.
    /// </summary>
    /// <param name="routers">The array of RouterScript instances that should be sorted.</param>
    /// <returns>A sorted version of the array that was passed as a parameter.</returns>
    private RouterScript[] sortRouterArrayByPriority(RouterScript[] routers)
    {
        bool isSorted = false;
        while (!isSorted)
        {
            // Assume it is sorted.
            isSorted = true;

            // Check if all elements are sorted.
            for (int i = 0; i < routers.Length - 1; i++)
            {
                // Sort by priority ascendingly.
                if (routers[i].GetPriority() > routers[i + 1].GetPriority())
                {
                    // Not sorted yet.
                    isSorted = false;

                    // Swap elements.
                    RouterScript tmp = routers[i];
                    routers[i] = routers[i + 1];
                    routers[i + 1] = tmp;
                }
            }
        }

        return routers;
    }

    /// <summary>
    /// Get all the paths that are connected to the current working router 
    /// and lead to the neighbor router.
    /// </summary>
    /// <returns>Returns a list of PathScripts. The list contains the PathScripts of paths that are
    ///     connected to the current working router.<returns>
    public List<PathScript> GetNeighborPathsOfCurrentRouter()
    {
        List<PathScript> neighborPaths = new List<PathScript>();

        int currentRouterIndex = routerScriptCurrentPlayerPosition.GetRouterIndex();
        //Extract paths from graph representation.
        for (int i = 0; i < graphRepresentation2.GetLength(1); i++)
        {
            if (graphRepresentation2[currentRouterIndex, i] != null)
            {
                neighborPaths.Add(graphRepresentation2[currentRouterIndex, i]);
            }
        }

        return neighborPaths;
    }
}

// ********************* Priority Queue Helper Class *******************************************************************************************************************

/// <summary>
/// This class represents a priority queue. The priority queue allows to access the element with the highest priority
/// in an efficient way. Internally the priority queue uses a binary heap to manage the elements.
/// </summary>
/// <typeparam name="T"> The type of the objects which should be stored within the queue.</typeparam>
public class PriorityQueue<T> where T : System.IComparable<T>, Priority
{
    // Internal data structure of the queue is a list.
    private List<T> data;

    // Keeps track of the indices which are currently assigned to certain RouterScripts in the priority queue.
    // This enables fast lookups of elements in the queue.
    Dictionary<T, int> indexMap;

    // Constructor
    public PriorityQueue()
    {
        this.data = new List<T>();
        indexMap = new Dictionary<T, int>();
    }

    /// <summary>
    /// Stores an element in the priority queue. The priority queue is internally managed as a binary heap
    /// and the element is added to fulfill the heap conditions.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void Enqueue(T item)
    {
        // Add the item at the end of the list.
        data.Add(item);

        // Start with index of the inserted element.
        int index = data.Count - 1;
        indexMap[item] = index;     // store the index of this element in the indexMap.
        while (index > 0)
        {
            // Determine index of the parent elment.
            int parentIndex = (index - 1) / 2;
            // Check heap conditions.
            if (data[index].CompareTo(data[parentIndex]) >= 0)
            {
                // Child index has equal or higher priority than parent which fulfills the heap conditions.
                break;
            }

            // Heap conditions not fulfilled. Swap child and parent element.
            swapElements(index, parentIndex);
                        
            // Set index for next iteration.
            index = parentIndex;
        } 
    }

    /// <summary>
    /// Retrieves the element with the highest priority from the queue and remove it from the queue.
    /// </summary>
    /// <returns>The element with the highest priority.</returns>
    public T PullHighest()
    {
        // Check if heap is empty.
        if (data.Count == 0)
        {
            return default(T);
        }

        // Save front element.
        T tmp = data[0];

        // Set new front element by moving the last element.
        int lastIndex = data.Count - 1;
        indexMap[data[lastIndex]] = 0;  // Last element will get new index 0.
        data[0] = data[lastIndex];
        data.RemoveAt(lastIndex);
        lastIndex = lastIndex - 1;
        
        int currentIndex = 0;   // The element which needs to be checked is now at the beginning of the list.
        while (true)
        {
            int indexLeftChild = 2 * currentIndex + 1;
            int indexRightChild = 2 * currentIndex + 2;

            // First check left child.
            if (indexLeftChild > lastIndex)
            {
                // No child elements, you can stop here.
                break;
            }
            else
            {
                // Take left child by default.
                int indexChild = indexLeftChild;
                // Check if the right child exists and has a lower priority than the right child.
                if (indexRightChild <= lastIndex && data[indexRightChild].CompareTo(data[indexLeftChild]) < 0)
                {
                    // If right is smaller, we set the child index to the right child.
                    indexChild = indexRightChild;
                }

                // Check heap condition.
                if (data[currentIndex].CompareTo(data[indexChild]) <= 0)
                {
                    // Current element has smaller priority than its childern, i.e. heap condition satisfied.
                    break;
                }
                else
                {
                    // Heap condition violated. Swap elements
                    swapElements(currentIndex, indexChild);

                    // Set the index for the next iteration.
                    currentIndex = indexChild;
                }
            }
        }

        // delete the front element from the index map 
        indexMap.Remove(tmp);

        return tmp;
    }

    /// <summary>
    /// Returns the first element of the priority queue without removing it from the queue.
    /// </summary>
    /// <returns>The first element of the queue. Returns null if the queue is empty.</returns>
    public T Peek()
    {
        if (data.Count >= 1)
        {
            T fronItem = data[0];
            return fronItem;
        }
        return default(T);
    }

    /// <summary>
    /// Returns the amount of elements which are stored in the queue.
    /// </summary>
    /// <returns>The amount of elements in the queue.</returns>
    public int Count()
    {
        return data.Count;
    }

    /// <summary>
    /// Decreases the priority of the specified element in the priority queue and rearranges the queue if necessary.
    /// </summary>
    /// <param name="element">The element that should receive a new priority value.</param>
    /// <param name="newPriority">The new priority value.</param>
    public void DecreasePriority(T element, int newPriority)
    {
        if (element.GetPriority() < newPriority)
        {
            Debug.LogError("The priority of the element can only be decreased, but the new priority value is higher than the current one.");
            return;
        }

        // Retrieve the index of the element in the queue.
        int elementIndex = -1;
        bool successful = indexMap.TryGetValue(element, out elementIndex);
        if (!successful)
        {
            Debug.LogError("Couldn't find the element in the indexMap");
            return;
        }

        // Set the new priority of the element.
        data[elementIndex].SetPriority(newPriority);

        // Check and recondition the priority queue.
        while (elementIndex > 0)
        {
            int parentIndex = (elementIndex - 1) / 2;
            if (data[elementIndex].CompareTo(data[parentIndex]) < 0)        // If child element has smaller priority than parent element.
            {
                swapElements(elementIndex, parentIndex);
                elementIndex = parentIndex;                                 // Continues with the parent element's index in the next round.
            }
            else
            {
                // Conditions are fulfilled.
                break;
            }
        }
    }

    /// <summary>
    /// Checks whether a certain element is contained in the priority queue.
    /// </summary>
    /// <param name="element">The element which should be checked.</param>
    /// <returns>True, if the element is contained in the queue, false otherwise.</returns>
    public bool IsContained(T element)
    {
        bool contains = indexMap.ContainsKey(element);
        return contains;
    }

    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < data.Count; ++i)
            s += data[i].ToString() + " ";
        s += "count = " + data.Count + "\n";
        return s;
    }

    /// <summary>
    /// Swaps the elements which are located at the given indices. 
    /// </summary>
    /// <param name="indexElementA">The index of the first element that should be swapped.</param>
    /// <param name="indexElementB">The index of the second element that should be swapped.</param>
    private void swapElements(int indexElementA, int indexElementB)
    {
        indexMap[data[indexElementA]] = indexElementB;    // Update the index in the indexMap, the element A at indexA gets the new index of the element B
        indexMap[data[indexElementB]] = indexElementA;    // and the element B gets the index of element A.
        T tmp = data[indexElementA];
        data[indexElementA] = data[indexElementB];
        data[indexElementB] = tmp;
    }
}

/// <summary>
/// Interface which guarantees that a element of the priortiy queue provides methods for the retrieval and the setting of the priority value.
/// </summary>
public interface Priority
{
    void SetPriority(int priority);

    int GetPriority();
}

/// <summary>
/// The status code specifies the result of the check. 
///                          Possible status codes are:
///                             VALID_HOP  -> valid hop
///                             VALID_HOP_DISCOVERY  -> valid hop in discovery mode
///                             HOP_UNREACHABLE -> invalid hop
///                             WRONG_HOP -> invalid hop concerning the dijkstra algorithm
///                             UNDISCOVERED_PATHS -> invalid hop due to undiscovered paths to neighbor routers
///                             ERROR_RECOVERY -> player has performed a wrong action and is now in error recovery mode
///                             NOP -> No operation. The player performs a valid hop, but the hop leads to an already handled router and
///                                     thus the dijkstra algorithm does not need to perform any action.
/// </summary>
public enum DijkstraStatus
{
    VALID_HOP,
    VALID_HOP_DISCOVERY,
    HOP_UNREACHABLE,
    WRONG_HOP,
    UNDISCOVERED_PATHS,
    ERROR_RECOVERY,
    NOP
}

/// <summary>
/// The error recovery status indicates whether the player has left the error recovery mode with a hop performed within the error recovery mode.
/// If the status says ERROR_RECOVERED, then the player has performed the hop back to the last valid router and leaves the error recovery mode.
/// However, if the status is ERROR_NOT_RECOVERED, then the player has performed another invalid hop and remains in error recovery mode.
/// </summary>
public enum ErrorRecoveryStatus
{
    ERROR_NOT_RECOVERED,
    ERROR_RECOVERED
}
