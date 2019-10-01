using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class game : MonoBehaviour
{
    

#region global variables
    public static game instance;

    [Header("Map variables")]
    public GameObject map_tiles_root;
    public GameObject map_units_root;
    public string WorldName = "World Map";
    public int mapSizeX = 10;
    public int mapSizeY = 10;
    
    [Header("NonSerialized game lists/objects")]
    [System.NonSerialized]
    public GameObject selected_unit;
    [System.NonSerialized]
    public GameObject selected_tile;

    public Tile[,] terrain_tiles;
    List<Unit> units;
    Node[,] graph;

    [Header("Testing objects")]
    public GameObject sample_unit;
#endregion

#region Unit operations
    void CreateNewUnit(string type, Unit.UnitOwner owner, int x, int y, Unit.UnitType flag)
    {
        Unit newunit = new Unit();
        bool isGuidUnique = true;

        foreach (Unit unit in units)
        {
            if (unit.guid == newunit.guid) { isGuidUnique = false; }
        }

        if (isGuidUnique)
        {
            newunit.unit_name = type;
            newunit.coordX = x;
            newunit.coordY = y;
            Debug.Log("Created a new unit: " + newunit.unit_name + " (" + newunit.guid + ")");

            switch (flag)
            {
                case Unit.UnitType.flying:
                    newunit.flying = true;
                    break;
                case Unit.UnitType.flat:
                    newunit.flatMovementCost = true;
                    break;
                default:
                    break;
            }

            units.Add(newunit);
        } else { Debug.LogError("There's already a unit with this GUID created! Skipping!"); }
        
    }

    public void MoveUnitToTile(GameObject unit, TileScript tile)
    {
        if (terrain_tiles[tile.coordX, tile.coordY].isWalkable == true)
        {
            unit.GetComponent<UnitScript>().unit_data.coordX = tile.coordX;
            unit.GetComponent<UnitScript>().unit_data.coordY = tile.coordY;
            unit.transform.position = new Vector3 (tile.coordX, tile.coordY, 0);
        }        
    }


#endregion

#region Tile operations
    void SetTile(int x, int y, Tile.terrain type)
    {
        if (terrain_tiles != null)
        {
            if ((x >= 0) && (x <= mapSizeX - 1) && (y >= 0) && (y <= mapSizeY))
            {                
                terrain_tiles [x,y].SetTile(type);
            } else {
                Debug.LogError ("X/Y values for SetTile are outside of map bounds!");
                Debug.Log("x = " + x + ", y = " + y + ", mapsizeX = " + mapSizeX + ", mapsizeY = " + mapSizeY);
            }
        } else { Debug.LogError("The object terrain_tiles is not initialized!");}
    }
#endregion

#region Pathfinding

void GeneratePathfindingGraph()
{
    graph = new Node[mapSizeX, mapSizeY];

    // Initialize each node in the array
    for (int x = 0; x < mapSizeX; x++)
    {
        for (int y = 0; y < mapSizeY; y++)
        {
            graph[x,y] = new Node();
            graph[x,y].coordX = x;
            graph[x,y].coordY = y; 
        }
    }

    // 4-way connected tiles on map
    for (int x = 0; x < mapSizeX; x++)
    {
        // This doesn't take into account impassable tiles. TODO implement walkable/unwalkable tiles for land units, ignore for flying units!
        for (int y = 0; y < mapSizeY; y++)
        { 
            if (x > 0){
                graph[x,y].neighbours.Add(graph[(x-1), y]);
                //Debug.Log("Adding a left neighbour for " + x + ":" + y);
            }
            if (x < mapSizeX - 1){
                graph[x,y].neighbours.Add(graph[(x+1), y]);
                //Debug.Log("Adding a right neighbour for " + x + ":" + y);
            }
            if (y > 0)
            {
                graph[x,y].neighbours.Add(graph[x, (y-1)]);
                //Debug.Log("Adding a bottom neighbour for " + x + ":" + y);
            }
            if (y < mapSizeY - 1)
            {
                graph[x,y].neighbours.Add(graph[x, (y+1)]);
                //Debug.Log("Adding a top neighbour for " + x + ":" + y);
            }
        }
    }
}

float CostToEnterTile(Unit unit, int x, int y)
{
    float mcost = terrain_tiles[x,y].movementCost;
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
            float alt = distance[u] + u.DistanceTo(v) * CostToEnterTile(selected_unit.GetComponent<UnitScript>().unit_data,v.coordX, v.coordY);
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
    // Adding source node as the start of the path, TODO check if needs removal for the actual game
    current_path.Add(graph[unit.GetComponent<UnitScript>().unit_data.coordX, unit.GetComponent<UnitScript>().unit_data.coordY]);
    //Revert the 'reverse path' to have a correct path from the source to the target
    current_path.Reverse();

    unit.GetComponent<UnitScript>().unit_data.current_path  = current_path;
}
#endregion

#region Unit/map Generation
    void GenerateUnitData()
    {
        CreateNewUnit("Infantry", Unit.UnitOwner.player0, 0, 0, Unit.UnitType.land);
        CreateNewUnit("Helicopter", Unit.UnitOwner.player0, 2, 1, Unit.UnitType.flying);
    }
    void GenerateMapData()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                terrain_tiles [x,y] = new Tile(Tile.terrain.plains);
            }
        }

        SetTile(1, 3, Tile.terrain.badlands);
        SetTile(2, 3, Tile.terrain.badlands);

        SetTile(3, 3, Tile.terrain.peaks);
        SetTile(3, 4, Tile.terrain.peaks);
        SetTile(4, 3, Tile.terrain.peaks);
        SetTile(5, 3, Tile.terrain.peaks);
        SetTile(6, 3, Tile.terrain.peaks);
        SetTile(6, 4, Tile.terrain.peaks);
    }
    void GenerateMapVisuals()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                GameObject maptile_go = Instantiate (terrain_tiles[x,y].tilePrefab, new Vector3 (x , y, 0), Quaternion.identity);
                maptile_go.GetComponent<TileScript>().coordX = x;
                maptile_go.GetComponent<TileScript>().coordY = y;
                maptile_go.transform.parent = map_tiles_root.transform;
            }
        }
    }
    void GenerateUnitVisuals()
    {
        foreach(Unit unit in units)
        {
            GameObject spawned_unit = Instantiate (sample_unit, new Vector3 (unit.coordX, unit.coordY, 0), Quaternion.identity);
            spawned_unit.GetComponent<UnitScript>().unit_data = unit;
            spawned_unit.GetComponent<UnitScript>().unit_data.unit_go = spawned_unit;
            spawned_unit.transform.parent = map_units_root.transform;
        }
    }
#endregion

#region Unity life
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Debug.LogError("Another instance of 'game' class is already running!");
        }
    }

    void Start()
    {
        terrain_tiles = new Tile[mapSizeX,mapSizeY];
        units = new List<Unit>();

        GenerateUnitData();
        GenerateMapData();
        GenerateMapVisuals();
        GenerateUnitVisuals();

        GeneratePathfindingGraph();
    }
#endregion
}
