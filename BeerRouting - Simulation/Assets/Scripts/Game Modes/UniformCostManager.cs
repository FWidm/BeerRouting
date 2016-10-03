using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UniformCostManager : GameManager, GameManagerInterface
{
    void Awake()
    {
        base.Initialize();   
    }

    // Use this for initialization
    void Start()
    {

    }

    /// <summary>
    /// Determines whether this router instance is already handled in the current run.
    /// </summary>
    /// <returns><c>true</c> if this router instance is already handled in the current run; otherwise, <c>false</c>.</returns>
    /// <param name="router">The router instance.</param>
    public bool IsRouterAlreadyHandled(RouterScript router)
    {
        // The routers in current path have already been visited and no shorter path
        // can be found for this routers.
        if (currentPath != null && !currentPath.Contains(router))
        {
            return false;
        }
        return true;
    }

    #region GameManagerInterface implementation

    void GameManagerInterface.Start(GameTuple startAndEndPoint)
    {
        recreateGraphRepresentation();

        base.InitializeRun(startAndEndPoint);

        RouterScript currentRouter = activeRouter.GetComponent<RouterScript>();
        currentRouter.SetPriority(0);

        neighboursOfActiveRouter = ExpandNode(currentRouter);
        prioQueue = new PriorityQueue<RouterScript>();

        for (int i = 0; i < neighboursOfActiveRouter.Count; i++)
        {
            PathScript pathToNeighbor = graphRepresentation2[currentRouter.GetRouterIndex(), neighboursOfActiveRouter[i].GetRouterIndex()];
            neighboursOfActiveRouter[i].SetPriority(pathToNeighbor.GetPathCosts());

            prioQueue.Enqueue(neighboursOfActiveRouter[i]);

            if (isLogEnabled)
                Debug.Log(string.Format("Added router {0} to prio queue with path costs {1}.",
                        neighboursOfActiveRouter[i],
                        pathToNeighbor.GetPathCosts()));
        }
    }

    GameStatus GameManagerInterface.IsValidHop(PathScript path)
    {
        GameObject hopTarget = null;
        if (isLogEnabled)
            Debug.Log("The current player position is: " + activeRouter + ", The path.to is: " + path.to + ", the path.from is: " + path.from);

        if (isLogEnabled)
            Debug.Log("Active Router: " + activeRouter.GetComponent<RouterScript>().GetRouterName() +
                " | queue: " + prioQueue.ToString());

        //neither is active --> hop is impossible.
        if (path.from.gameObject != activeRouter.gameObject && path.to.gameObject != activeRouter.gameObject)
        {
            gameStatus = GameStatus.ForbiddenHop;
            return  gameStatus;
        }

        //to=active --> goto from
        if (path.to.gameObject == activeRouter.gameObject)
        {
            hopTarget = path.from.gameObject;
        }
        else           //from=active --> goto to
            if (path.from.gameObject == activeRouter.gameObject)
        {
            hopTarget = path.to.gameObject;
        }

        if (errorRecovery)
        {
            gameStatus = GameStatus.ErrorRecoveryHop;
            return GameStatus.ErrorRecoveryHop;
        }

        //check win condition
        if (hopTarget.gameObject == currentRun.destination
            && IsValidUniformCostHop(hopTarget.GetComponent<RouterScript>()))
        {
            gameStatus = GameStatus.RunFinished;
            return  gameStatus;
        }

        if (IsValidUniformCostHop(hopTarget.GetComponent<RouterScript>()))
        {
            // Valid hop.
            gameStatus = GameStatus.ValidHop;
            return GameStatus.ValidHop;
        }

        gameStatus = GameStatus.InvalidHop;
        return gameStatus;
    }

    /// <summary>
    /// Determines whether this hop is a valid uniform cost hop for the specified targetRouter.
    /// </summary>
    /// <returns><c>true</c> if this instance is a valid uniform cost hop for the specified targetRouter; otherwise, <c>false</c>.</returns>
    /// <param name="targetRouter">Target router.</param>
    private bool IsValidUniformCostHop(RouterScript targetRouter)
    {
        if (!prioQueue.IsContained(targetRouter))
        {
            // Valid hop.
            return true;
        }

        // Is the target the first element in the priority queue?
        if (prioQueue.Peek() == targetRouter)
        {
            return true;
        }
        else
        {
            // Target is not the first element in the priority Queue.
            // But it is still a valid hop if there are more elements with
            // the same costs.

            bool isValidHop = false;
            // To check this, we need to remove the elements before this router from the queue and later put them back in.
            RouterScript headOfQueue = prioQueue.PullHighest();
            // Check the following routers. Stop if they have a higher priority than the original head.
            List<RouterScript> nextRouterCandidates = new List<RouterScript>();
            while (prioQueue.Count() > 0 && prioQueue.Peek().GetPriority() == headOfQueue.GetPriority())
            {
                RouterScript candidateRouter = prioQueue.PullHighest();
                nextRouterCandidates.Add(candidateRouter);
                if (candidateRouter == targetRouter)
                {
                    isValidHop = true;
                    break;
                }
            }

            // Store the candidate routers and the original headOfQueue back into the priority queue.
            prioQueue.Enqueue(headOfQueue);
            for (int i = 0; i < nextRouterCandidates.Count; i++)
            {
                prioQueue.Enqueue(nextRouterCandidates[i]);
            }

            if (!isValidHop)
            {
                return false;
            }
        }

        return true;
    }

    void GameManagerInterface.PerformHop(PathScript path)
    {
        GameObject hopTarget = null;

        //to=active --> goto from
        if (path.to.gameObject == activeRouter.gameObject)
        {
            hopTarget = path.from.gameObject;
        }
        else           //from=active --> goto to
            if (path.from.gameObject == activeRouter.gameObject)
        {
            hopTarget = path.to.gameObject;
        }

        // Update the active router and the neighbours of the active router.
        activeRouter = hopTarget;
        currentPath.Add(hopTarget.GetComponent<RouterScript>());
        neighboursOfActiveRouter = ExpandNode(activeRouter.GetComponent<RouterScript>());


        // Remove the hop target from the priortiy queue.
        RemoveRouterFromPrioQueue(hopTarget.GetComponent<RouterScript>());

        for (int i = 0; i < neighboursOfActiveRouter.Count; i++)
        {
            RouterScript neighborRouter = neighboursOfActiveRouter[i].GetComponent<RouterScript>();

            // Path costs to the target router are the path cost to the currently active router + the path costs of this path.
            int pathCost = activeRouter.GetComponent<RouterScript>().GetPriority() +
                           graphRepresentation2[
                               activeRouter.GetComponent<RouterScript>().GetRouterIndex(),
                               neighborRouter.GetRouterIndex()
                           ].GetPathCosts();

            if (currentPath.Contains(neighborRouter))
            {
                continue;
            }

            if (prioQueue.IsContained(neighborRouter))
            {
                if (pathCost < neighborRouter.GetPriority())
                {
                    // Update the path costs of the router.
                    prioQueue.DecreasePriority(neighborRouter, pathCost);

                    if (isLogEnabled)
                    {
                        Debug.Log(string.Format("Updated path costs of router {0}, new path costs are {1}.",
                                neighborRouter.GetRouterName(), neighborRouter.GetPriority()));
                    }
                }

            }
            else
            {
                neighborRouter.SetPriority(pathCost);

                if (isLogEnabled)
                {
                    Debug.Log(string.Format("Inserted neighbor into prio queue. Neighbor is {0}, path costs are {1}.",
                            neighborRouter.GetRouterName(), pathCost));
                }

                // Insert neighbor into priority queue.
                prioQueue.Enqueue(neighborRouter);
            }
        }

        if (isLogEnabled)
        {
            Debug.Log("Prio Queue after hop: " + prioQueue.ToString());   
        }
    }

    void GameManagerInterface.PerformWrongHop(PathScript path)
    {
        if (isLogEnabled)
            Debug.Log("Started error recovery mode.");

        // Start the error recovery.
        errorRecovery = true;
        // Store the last valid router to that the player needs to return.
        // Last valid position is the router on which the player is located before the wrong hop.
        lastValidRouter = activeRouter.GetComponent<RouterScript>();

        GameObject hopTarget = null;

        //to=active --> goto from
        if (path.to.gameObject == activeRouter.gameObject)
        {
            hopTarget = path.from.gameObject;
        }
        else           //from=active --> goto to
            if (path.from.gameObject == activeRouter.gameObject)
        {
            hopTarget = path.to.gameObject;
        }

        activeRouter = hopTarget;
    }

    bool GameManagerInterface.PerformErrorRecoveryHop(PathScript path)
    {
        GameObject hopTarget = null;

        //to=active --> goto from
        if (path.to.gameObject == activeRouter.gameObject)
        {
            hopTarget = path.from.gameObject;
        }
        else           //from=active --> goto to
            if (path.from.gameObject == activeRouter.gameObject)
        {
            hopTarget = path.to.gameObject;
        }

        if (hopTarget.gameObject == lastValidRouter.gameObject)
        {
            errorRecovery = false;
            activeRouter = hopTarget;
            return true;
        }

        activeRouter = hopTarget;
        return false;
    }

    RouterScript GameManagerInterface.GetCurrentPlayerPosition()
    {
        return activeRouter.GetComponent<RouterScript>();
    }

    void GameManagerInterface.SetCurrentPlayerPosition(RouterScript playerPos)
    {
        activeRouter = playerPos.gameObject;
    }

    public RouterScript[] GetAllRouterScripts()
    {
        return listOfRouterScripts;
    }

    public PathScript[] GetAllPathScripts()
    {
        return listOfPathScripts;
    }

    GameStatus GameManagerInterface.getGameStatus()
    {
        return gameStatus;
    }

    #endregion
}
