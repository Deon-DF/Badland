using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class game : MonoBehaviour
{

    public static game instance;

    [Header("Map variables")]
    public GameObject map_tiles_root;
    public GameObject map_units_root;
    public string WorldName = "World Map";
    public TileType[,] terrain_tiles;

    public int mapSizeX = 10;
    public int mapSizeY = 10;

    [Header("Testing objects")]
    public GameObject current_unit;

    void GenerateMapData()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                terrain_tiles [x,y] = new TileType(TileType.terrain.plains);
            }
        }

        SetTile(3, 3, TileType.terrain.peaks);
        SetTile(3, 4, TileType.terrain.peaks);
        SetTile(4, 3, TileType.terrain.peaks);
        SetTile(5, 3, TileType.terrain.peaks);
        SetTile(6, 3, TileType.terrain.peaks);
        SetTile(6, 4, TileType.terrain.peaks);
    }

    void SetTile(int x, int y, TileType.terrain type)
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

    void GenerateMapVisuals()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                GameObject maptile = Instantiate (terrain_tiles[x,y].tilePrefab, new Vector3 (x , y, 0), Quaternion.identity);
                maptile.GetComponent<mapTile>().coordX = x;
                maptile.GetComponent<mapTile>().coordY = y;
                maptile.transform.parent = map_tiles_root.transform;
            }
        }
    }

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
        terrain_tiles = new TileType[mapSizeX,mapSizeY];
        GenerateMapData();
        GenerateMapVisuals();
    }

}
