using UnityEngine;
using System.Collections.Generic;
using System;

public class GreedyManager : GameManager, GameManagerInterface
{
    // Called on startup.
    void Awake()
    {
        base.Initialize();
    }

    void Start()
    {
        
    }

    /// <summary>
    /// Finds the greedy path between source and destination.
    /// </summary>
    /// <param name="source">Source.</param>
    /// <param name="destination">Destination.</param>
    private List<RouterScript> FindPath(RouterScript source, RouterScript destination)
    {
        if (isLogEnabled)
            Debug.Log("Finding best greedy path!");
        prioQueue = new PriorityQueue<RouterScript>();
        RouterScript current = source;
        List<RouterScript> closed = new List<RouterScript>();
        List<RouterScript> children = new List<RouterScript>();
        if (isLogEnabled)
            Debug.Log("Starting Greedy from " + current.name + " to " + destination.name);
        UpdateGreedyHeuristics(source, destination);

        while (current != destination)
        {
            if (!closed.Contains(current))
            {
                children = ExpandNode(current);
                foreach (RouterScript tmp in children)
                {
                    if (isLogEnabled)
                        Debug.Log("Current Child: " + tmp.name + " | heuristic: " + tmp.GetGreedyHeuristicValue());
                    tmp.SetPriority(tmp.GetGreedyHeuristicValue());
                    prioQueue.Enqueue(tmp);
                }

                closed.Add(current);
            }
            if (isLogEnabled)
                Debug.Log("Finished with: " + current.name);
            //Check if more than one solution exists.
            current = prioQueue.PullHighest();
        }

        string path = "";
        foreach (RouterScript tmp in closed)
        {
            path += tmp.name + "|" + CalcStraightLineHeuristic(tmp, destination)
            + " >> ";
        }
        path += current.name + "|0";
        if (isLogEnabled)
            Debug.Log("Path=" + path);

        closed.Add(destination);
        return closed;
    }

    /// <summary>
    /// Determines whether this instance is valid greedy hop for the specified hopTarget.
    /// </summary>
    /// <returns><c>true</c> if this instance is valid greedy hop for the specified hopTarget; otherwise, <c>false</c>.</returns>
    /// <param name="hopTarget">Hop target.</param>
    private bool IsValidGreedyHop(RouterScript hopTarget)
    {
        foreach (RouterScript router in neighboursOfActiveRouter)
        {
            if (hopTarget.gameObject == router.gameObject)
            {
                continue;
            }

            if (currentPath.Contains(router))
            {
                continue;
            }
				
            if (router.GetGreedyHeuristicValue() < hopTarget.GetGreedyHeuristicValue())
            {
                return false;
            }				
        }
        return true;
    }

    private void UpdateGreedyHeuristics(RouterScript source, RouterScript destination)
    {
        foreach (RouterScript tmp in listOfRouterScripts)
        {
            int heuristic = CalcStraightLineHeuristic(tmp, destination);
            tmp.SetGreedyHeuristicValue(heuristic);
        }
    }

    /// <summary>
    /// Calculates the straight line heuristic.
    /// </summary>
    /// <returns>The straight line heuristic.</returns>
    /// <param name="source">Source.</param>
    /// <param name="destination">Destination.</param>
    private int CalcStraightLineHeuristic(RouterScript source, RouterScript destination)
    {
        return (int)Vector3.Distance(source.transform.position, destination.transform.position);
    }

    //	/// <summary>
    //	/// Checks whether the hop is valid in the context of the greedy algorithm and performs
    //	/// actions depending depending on the result of the check. The result of the validity
    //	/// check is also returned as a status so that the MovementManager can take actions
    //	/// correspondingly.
    //	/// </summary>
    //	/// <param name="path">The path on which the hop should be performed.</param>
    //	/// <returns>The status which indicates the result of the hop attempt.</returns>
    //	public GameStatus PerformHop (PathScript path)
    //	{
    //		GameObject hopTarget = null;
    //
    //		if (isLogEnabled)
    //			Debug.Log ("The current player position is: " + activeRouter + ", The path.to is: " + path.to + ", the path.from is: " + path.from);
    //
    //		//neither is active --> hop is impossible.
    //		if (path.from.gameObject != activeRouter.gameObject && path.to.gameObject != activeRouter.gameObject) {
    //			gameStatus = GameStatus.ForbiddenHop;
    //			return  gameStatus;
    //		}
    //
    //		//to=active --> goto from
    //		if (path.to.gameObject == activeRouter.gameObject) {
    //			hopTarget = path.from.gameObject;
    //		} else
    //            //from=active --> goto to
    //            if (path.from.gameObject == activeRouter.gameObject) {
    //			hopTarget = path.to.gameObject;
    //		}
    //
    //		//check win condition
    //		if (hopTarget.gameObject == referencePath [referencePath.Count - 1].gameObject) {
    //			currentPath.Add (hopTarget.GetComponent<RouterScript> ());
    //			gameStatus = GameStatus.RunFinished;
    //			return  gameStatus;
    //		}
    //
    //		if (isLogEnabled)
    //			Debug.Log ("is valid hop? hopTarget=" + hopTarget.gameObject + " should be " + referencePath [indexReferencePath].gameObject);
    //
    //		if (hopTarget.gameObject == referencePath [indexReferencePath].gameObject) {
    //			// Valid hop.
    //			indexReferencePath++;
    //			currentPath.Add (hopTarget.GetComponent<RouterScript> ());
    //			return GameStatus.ValidHop;
    //		} else {
    //			if (currentPath.Contains (hopTarget.GetComponent<RouterScript> ())) {
    //				currentPath.Add (hopTarget.GetComponent<RouterScript> ());
    //				gameStatus = GameStatus.ErrorRecoveryHop;
    //				return  gameStatus;
    //			}
    //		}
    //
    //		// Invalid hop.
    //		currentPath.Add (hopTarget.GetComponent<RouterScript> ());
    //		gameStatus = GameStatus.InvalidHop;
    //		return  gameStatus;
    //	}

    public RouterScript GetCurrentPlayerPosition()
    {
        return activeRouter.GetComponent<RouterScript>();
    }

    public void SetCurrentPlayerPosition(RouterScript playerPos)
    {
        activeRouter = playerPos.gameObject;
    }

    /// <summary>
    /// Returns an array of all RouterScript instances of routers that are involved in the current run.
    /// </summary>
    /// <returns>An array of RouterScript instances.</returns>
    public RouterScript[] GetAllRouterScripts()
    {
        return listOfRouterScripts;
    }

    /// <summary>
    /// Starts a new run of the greedy algorithm. The reference path 
    /// for the given tuple of start and end router is calculated.
    /// </summary>
    /// <param name="startAndEndPoint">The start and end router for this run.</param>
    public void Start(GameTuple startAndEndPoint)
    {
        base.recreateGraphRepresentation();

        base.InitializeRun(startAndEndPoint);

        UpdateGreedyHeuristics(
            startAndEndPoint.source.GetComponent<RouterScript>(),
            startAndEndPoint.destination.GetComponent<RouterScript>());
        
        neighboursOfActiveRouter = ExpandNode(activeRouter.GetComponent<RouterScript>());       
    }

    public GameStatus getGameStatus()
    {
        return gameStatus;
    }

    public PathScript[] GetAllPathScripts()
    {
        return listOfPathScripts;
    }

    /// <summary>
    /// Determines whether this move is a valid hop on the specified path.
    /// </summary>
    /// <returns><c>true</c> if this move is valid hop on the specified path; otherwise, <c>false</c>.</returns>
    /// <param name="path">Path.</param>
    public GameStatus IsValidHop(PathScript path)
    {
        GameObject hopTarget = null;
        if (isLogEnabled)
            Debug.Log("The current player position is: " + activeRouter + ", The path.to is: " + path.to + ", the path.from is: " + path.from);
		
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
        if (hopTarget.gameObject == currentRun.destination)
        {
            gameStatus = GameStatus.RunFinished;
            return  gameStatus;
        }

        if (IsValidGreedyHop(hopTarget.GetComponent<RouterScript>()))
        {
            // Valid hop.
            gameStatus = GameStatus.ValidHop;
            return GameStatus.ValidHop;
        }
				
        // Invalid hop.
        gameStatus = GameStatus.InvalidHop;
        return  gameStatus;
    }


    public void PerformHop(PathScript path)
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
        neighboursOfActiveRouter = ExpandNode(activeRouter.GetComponent<RouterScript>());

        currentPath.Add(hopTarget.GetComponent<RouterScript>());

        if (isLogEnabled)
        {
            Debug.Log("Acitve router is now: " + activeRouter);
        }
    }


    public void PerformWrongHop(PathScript path)
    {
        GameObject hopTarget = null;

        //to=active --> goto from
        if (path.to.gameObject == activeRouter.gameObject)
        {
            lastValidRouter = path.to.gameObject.GetComponent<RouterScript>();
            hopTarget = path.from.gameObject;
        }
        else           //from=active --> goto to
			if (path.from.gameObject == activeRouter.gameObject)
        {
            lastValidRouter = path.from.gameObject.GetComponent<RouterScript>();
            hopTarget = path.to.gameObject;
        }

        errorRecovery = true;

        activeRouter = hopTarget;
    }

    /// <summary>
    /// Performs an error recovery hop.
    /// </summary>
    /// <param name="path">Path.</param>
    /// <returns><c>true</c>, if error has been recovered, <c>false</c> otherwise.</returns>
    public bool PerformErrorRecoveryHop(PathScript path)
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
}
