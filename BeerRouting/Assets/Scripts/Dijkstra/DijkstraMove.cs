using UnityEngine;
using System.Collections;

public class DijkstraMove {

    public RouterScript Source { get; set; }
    public RouterScript Destination { get; set; }

    public DijkstraStatus Status { get; set; }
}
