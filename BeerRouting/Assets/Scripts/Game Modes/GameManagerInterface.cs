using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public interface GameManagerInterface
{
    // void Start(GameTuple startAndEndPoint);
    void Start(GameTuple startAndEndPoint);

    GameStatus IsValidHop(PathScript path);

    /// <summary>
    /// Performs the hop along the path if it is a valid move.
    /// </summary>
    /// <param name="path">Path.</param>
    void PerformHop(PathScript path);

    /// <summary>
    /// Performs the hop along the path if it is not a valid move.
    /// </summary>
    /// <param name="path">Path.</param>
    void PerformWrongHop(PathScript path);

    /// <summary>
    /// Performs an error recovery hop.
    /// </summary>
    /// <param name="path">Path.</param>
    /// <returns>True, if the error has been recovered, false otherwise.</returns>
    bool PerformErrorRecoveryHop(PathScript path);

    /// <summary>
    /// Gets the current player position.
    /// </summary>
    /// <returns>The current player position.</returns>
    RouterScript GetCurrentPlayerPosition();

    /// <summary>
    /// Sets the current player position.
    /// </summary>
    /// <param name="playerPos">Player position.</param>
    void SetCurrentPlayerPosition(RouterScript playerPos);

    /// <summary>
    /// Gets all router scripts.
    /// </summary>
    /// <returns>The router scripts.</returns>
    RouterScript[] GetAllRouterScripts();

    /// <summary>
    /// Gets all path scripts.
    /// </summary>
    /// <returns>All path scripts.</returns>
    PathScript[] GetAllPathScripts();

    /// <summary>
    /// Gets the game status.
    /// </summary>
    /// <returns>The game status.</returns>
    GameStatus getGameStatus();
}

