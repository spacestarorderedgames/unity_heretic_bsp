using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BspTools.v38.Types {
	[Serializable]
	public class Edge
    {
	    public int vert1;
	    public int vert2;

	    public Edge(ushort vert1, ushort vert2)
	    {
		    this.vert1 = (int)vert1;
		    this.vert2 = (int)vert2;
	    }

	    public override string ToString()
	    {
		    return "Vert1: " + vert1.ToString() + " Vert2: " + vert2.ToString() + "\r\n";
	    }
    }
}