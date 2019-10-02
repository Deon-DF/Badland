using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding instance = null;
    public Node[,] graph;


    public void GeneratePathfindingGraph()
    {
        graph = new Node[Game.instance.mapSizeX, Game.instance.mapSizeY];

        // Initialize each node in the array
        for (int x = 0; x < Game.instance.mapSizeX; x++)
        {
            for (int y = 0; y < Game.instance.mapSizeY; y++)
            {
                graph[x,y] = new Node();
                graph[x,y].coordX = x;
                graph[x,y].coordY = y; 
            }
        }

        // 4-way connected tiles on map
        for (int x = 0; x < Game.instance.mapSizeX; x++)
        {
            // This doesn't take into account impassable tiles. TODO implement walkable/unwalkable tiles for land units, ignore for flying units!
            for (int y = 0; y < Game.instance.mapSizeY; y++)
            { 
                if (x > 0){
                    graph[x,y].neighbours.Add(graph[(x-1), y]);
                    //Debug.Log("Adding a left neighbour for " + x + ":" + y);
                }
                if (x < Game.instance.mapSizeX - 1){
                    graph[x,y].neighbours.Add(graph[(x+1), y]);
                    //Debug.Log("Adding a right neighbour for " + x + ":" + y);
                }
                if (y > 0)
                {
                    graph[x,y].neighbours.Add(graph[x, (y-1)]);
                    //Debug.Log("Adding a bottom neighbour for " + x + ":" + y);
                }
                if (y < Game.instance.mapSizeY - 1)
                {
                    graph[x,y].neighbours.Add(graph[x, (y+1)]);
                    //Debug.Log("Adding a top neighbour for " + x + ":" + y);
                }
            }
        }
    }

    float CostToEnterTile(Unit unit, int x, int y)
    {
        float mcost = Game.instance.terrain_tiles[x,y].movementCost;
        if (mcost >= 0)
        {
            if (unit.flatMovementCost || unit.flying) {
                return 1.0f;
            } else return mcost;
        } else {
            if (unit.flying)
            {
                return 1.0f;
            } else return Mathf.Infinity;
        }
    }
    public void GenerateUnitPathTo(GameObject unit, TileScript tile)
    {
        List<Node> current_path = new List<Node>();
        // clear out the old Unit's path
        unit.GetComponent<UnitScript>().unit_data.current_path = null;

        int sourceX = unit.GetComponent<UnitScript>().unit_data.coordX;
        int sourceY = unit.GetComponent<UnitScript>().unit_data.coordY;
        int targetX = tile.coordX;
        int targetY = tile.coordY;
        
        Debug.Log("Generating a walkable path. Source - [" + sourceX + ":" + sourceY + "], target - ["  + targetX + ":" + targetY + "]");

        Dictionary<Node, float> distance = new Dictionary<Node, float>();
        Dictionary<Node, Node> previous = new Dictionary<Node, Node>();

        // The list of nodes we haven't checked yet
        List<Node> unvisited = new List<Node>();

        Node source = graph[sourceX, sourceY];
        Node target = graph[targetX, targetY];
        distance[source] = 0;
        previous[source] = null;

        // Initialize everything to have infinity distance since we didn't calculate anything else + some nodes may be unreachable from the source
        foreach (Node v in graph)
        {
            if (v != source)
            {
                distance[v] = Mathf.Infinity;
                previous[v] = null;
            }

            unvisited.Add(v);
            //Debug.Log("Adding the node [" + v.coordX + ":" + v.coordY + "] to the list of unvisited nodes.");
        }

        while (unvisited.Count > 0)
        {
            // Finding the node with minimum distance.
            Node u = null; // Unvisited node with the smallest distance
            foreach (Node possibleU in unvisited)
            {
                if (u == null || distance[possibleU] < distance[u])
                {
                    u = possibleU;
                    //Debug.Log("Found the node with the minimum movement cost: [" + u.coordX + ":" + u.coordY + "]");
                }
            }

            if (u == target) break; // Exit the while loop if the target is reached
            
            unvisited.Remove(u);
        
            foreach(Node v in u.neighbours)
            {
                // Here distance does not take into account movement costs! TODO implement movement costs between tiles
                float alt = distance[u] + u.DistanceTo(v) * CostToEnterTile(Game.instance.selected_unit.GetComponent<UnitScript>().unit_data,v.coordX, v.coordY);
                if (alt < distance[v])
                {
                    distance[v] = alt;
                    previous[v] = u;
                }
            }
            
        }

        // If we are here, either we found the shortest route, or there is no route
        if (previous[target] == null)
        {
            // No path to our target!
            Debug.LogWarning("No route from the source to the target tiles!");
            unit.GetComponent<UnitScript>().unit_data.current_path = null;
            UI.instance.DeletePathLines();
            return;
        }

        current_path = new List<Node>();
        Node current_node = target;

        // Step through the 'previous' chain to generate the reverse path
        while (previous[current_node] != null)
        {
            current_path.Add(current_node);
            current_node = previous[current_node];
        }
        // Adding source node as the start of the path, TODO check if needs removal for the actual Game
        current_path.Add(graph[unit.GetComponent<UnitScript>().unit_data.coordX, unit.GetComponent<UnitScript>().unit_data.coordY]);
        //Revert the 'reverse path' to have a correct path from the source to the target
        current_path.Reverse();

        unit.GetComponent<UnitScript>().unit_data.current_path  = current_path;
    }

#region Unity life
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Debug.LogError("Another instance of 'pathfinding' class is already running!");
        }
    }
#endregion
}