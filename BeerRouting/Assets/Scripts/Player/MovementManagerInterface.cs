using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


interface MovementManagerInterface
{
    /// <summary>
    /// Perform a move along the path. The actual implementation
    /// of this method depends on the current game mode, e.g. Dijkstra 
    /// or Greedy.
    /// </summary>
    /// <param name="path">The path on which a player move should be performed.</param>
    void PerformMoveOnPath(PathScript path);
}

