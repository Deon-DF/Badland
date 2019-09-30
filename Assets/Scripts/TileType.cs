using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileType
{
    public enum terrain { plains, grassland, badlands, peaks}
    
    public string name;
    public terrain terrain_type;
    public GameObject tilePrefab;

    public bool isWalkable = false;
    public float movementCost = 1.0f;

    public void SetTile(terrain type)
    {
        this.terrain_type = type;
       switch (type)
        {
            case terrain.plains:
                this.name = "Plains";
                this.tilePrefab = Resources.Load<GameObject>("Prefabs/Terrain/tile_plains");
                this.isWalkable = true;
                this.movementCost = 1.0f;
                break;
            case terrain.grassland:
                this.name = "Grassland";
                this.tilePrefab = Resources.Load<GameObject>("Prefabs/Terrain/tile_grassland");
                this.isWalkable = true;
                this.movementCost = 1.0f;
                break;
            case terrain.badlands:
                this.name = "Badlands";
                this.tilePrefab = Resources.Load<GameObject>("Prefabs/Terrain/tile_badlands");
                this.isWalkable = true;
                this.movementCost = 1.0f;
                break;
            case terrain.peaks:
                this.name = "Peaks";
                this.tilePrefab = Resources.Load<GameObject>("Prefabs/Terrain/tile_peaks");
                this.isWalkable = false;
                this.movementCost = 1.0f;
                break;
            default:
                this.name = "Error!";
                this.tilePrefab = Resources.Load<GameObject>("Prefabs/Terrain/tile_plains");
                this.isWalkable = true;
                this.movementCost = 1.0f;
                break;
        }
    }

    public TileType()
    {
        this.terrain_type = terrain.plains;
        this.name = "Plains";
        this.tilePrefab = Resources.Load<GameObject>("Prefabs/Terrain/tile_plains");
        this.isWalkable = true;
        this.movementCost = 1.0f;
    }

    public TileType(terrain _terrain)
    {
        this.terrain_type = _terrain;
        switch (_terrain)
        {
            case terrain.plains:
                this.name = "Plains";
                this.tilePrefab = Resources.Load<GameObject>("Prefabs/Terrain/tile_plains");
                this.isWalkable = true;
                this.movementCost = 1.0f;
                break;
            case terrain.grassland:
                this.name = "Grassland";
                this.tilePrefab = Resources.Load<GameObject>("Prefabs/Terrain/tile_grassland");
                this.isWalkable = true;
                this.movementCost = 1.0f;
                break;
            case terrain.badlands:
                this.name = "Badlands";
                this.tilePrefab = Resources.Load<GameObject>("Prefabs/Terrain/tile_badlands");
                this.isWalkable = true;
                this.movementCost = 1.0f;
                break;
            case terrain.peaks:
                this.name = "Peaks";
                this.tilePrefab = Resources.Load<GameObject>("Prefabs/Terrain/tile_peaks");
                this.isWalkable = false;
                this.movementCost = 1.0f;
                break;
            default:
                this.name = "Error!";
                this.tilePrefab = Resources.Load<GameObject>("Prefabs/Terrain/tile_plains");
                this.isWalkable = true;
                this.movementCost = 1.0f;
                break;
        }
    }
}
