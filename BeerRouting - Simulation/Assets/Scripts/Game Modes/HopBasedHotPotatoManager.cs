using UnityEngine;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;

public class HopBasedHotPotatoManager : GameManager, GameManagerInterface
{
    /// <summary>
    /// The optimal path from source to destination for the current run.
    /// </summary>
    private List<GameObject> optimalPath;

    void Awake()
    {
        base.Initialize();
    }

    #region GameManagerInterface implementation

    void GameManagerInterface.Start(GameTuple startAndEndPoint)
    {
        if (isLogEnabled)
            Debug.Log("Source: " + startAndEndPoint.source.name + " , Destination: " + startAndEndPoint.destination.name);

        base.recreateGraphRepresentation();

        // Find paths from start to end point.
        optimalPath = base.findOptimalHopCountPath(startAndEndPoint.source, startAndEndPoint.destination);

        base.InitializeRun(startAndEndPoint);
    }

    GameStatus GameManagerInterface.IsValidHop(PathScript path)
    {
        //neither is active --> hop is impossible.
        if (path.from.gameObject != activeRouter.gameObject && path.to.gameObject != activeRouter.gameObject)
        {
            gameStatus = GameStatus.ForbiddenHop;
            return  gameStatus;
        }

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

        // Check win condition.
        if (hopTarget.gameObject == currentRun.destination)
        {
            gameStatus = GameStatus.RunFinished;
            return  gameStatus;
        }

        gameStatus = GameStatus.ValidHop;
        return GameStatus.ValidHop;
    }

    void GameManagerInterface.PerformHop(PathScript path)
    {
        GameObject hopTarget = null;

        //to=active --> goto from
        if (path.to.gameObject == activeRouter.gameObject)
        {
            hopTarget = path.from.gameObject;
        }
        else   //from=active --> goto to
            if (path.from.gameObject == activeRouter.gameObject)
        {
            hopTarget = path.to.gameObject;
        }

        // Update the active router.
        activeRouter = hopTarget;

        currentPath.Add(hopTarget.GetComponent<RouterScript>());

        if (isLogEnabled)
        {
            Debug.Log("Acitve router is now: " + activeRouter);
        }
    }

    void GameManagerInterface.PerformWrongHop(PathScript path)
    {
        // Do nothing.
        if (isLogEnabled)
        {
            Debug.Log("In PerformWrongHop method.");
        }
    }

    bool GameManagerInterface.PerformErrorRecoveryHop(PathScript path)
    {
        // Do nothing.
        if (isLogEnabled)
        {
            Debug.Log("In PerformErrorRecoveryHop method.");
        }

        return true;
    }

    public RouterScript GetCurrentPlayerPosition()
    {
        return activeRouter.GetComponent<RouterScript>();
    }

    void GameManagerInterface.SetCurrentPlayerPosition(RouterScript playerPos)
    {
        activeRouter = playerPos.gameObject;
    }

    RouterScript[] GameManagerInterface.GetAllRouterScripts()
    {
        return listOfRouterScripts;
    }

    PathScript[] GameManagerInterface.GetAllPathScripts()
    {
        return listOfPathScripts;
    }

    GameStatus GameManagerInterface.getGameStatus()
    {
        return gameStatus;
    }

    #endregion

    /// <summary>
    /// Gets the length of the optimal path.
    /// </summary>
    /// <returns>The length of the optimal path.</returns>
    public int GetOptimalPathLength()
    {
        return optimalPath.Count;
    }

    /// <summary>
    /// Gets the length of the path performed by the player in this run.
    /// </summary>
    /// <returns>The path length.</returns>
    public int GetCurrentPlayerPathLength()
    {
        return currentPath.Count;
    }

    /// <summary>
    /// Gets the path between the provided nodes.
    /// </summary>
    /// <returns>The path between the nodes.</returns>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    public PathScript GetPathBetweenNodes(RouterScript from, RouterScript to)
    {
        PathScript path = graphRepresentation2[
                              from.GetRouterIndex(),
                              to.GetRouterIndex()
                          ];

        if (path == null)
        {
            path = graphRepresentation2[
                to.GetRouterIndex(),
                from.GetRouterIndex()
            ];
        }

        return path;
    }


}
