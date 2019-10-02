using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    

#region global variables
    public static Game instance;

    [Header("Map variables")]
    public GameObject map_tiles_root;
    public GameObject map_units_root;
    public GameObject map_ui_root;
    public string WorldName = "World Map";
    public int mapSizeX = 10;
    public int mapSizeY = 10;
    
    [Header("NonSerialized game lists/objects")]
    [System.NonSerialized]
    public GameObject selected_unit;
    [System.NonSerialized]
    public GameObject selected_tile;

    public Tile[,] terrain_tiles;
    public List<Unit> units;

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
        
        UI.instance.GenerateMapVisuals();
        UI.instance.GenerateUnitVisuals();

        Pathfinding.instance.GeneratePathfindingGraph();
    }
#endregion
}
