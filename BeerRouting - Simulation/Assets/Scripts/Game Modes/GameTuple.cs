using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameTuple
{
    public GameObject source;
    public GameObject destination;

    public GameTuple()
    {

    }

    public GameTuple(GameObject source, GameObject destination)
    {
        this.source = source;
        this.destination = destination;
    }
}


