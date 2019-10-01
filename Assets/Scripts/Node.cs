using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node
{
    public List<Node> neighbours;
    public int coordX;
    public int coordY;
    public Node()
    {
        neighbours = new List<Node>();
    }

    // Distance between two nodes
    public float DistanceTo(Node node)
    {
        return Vector2.Distance(new Vector2(coordX,coordY), new Vector2(node.coordX, node.coordY));
    }
}
