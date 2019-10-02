using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UI : MonoBehaviour
{
    
#region global variables
    public static UI instance = null;

    [Header("UI")]
    public GameObject pathline;
    
    [Header("NonSerialized Game lists/objects")]
    [System.NonSerialized]
    public List<GameObject> pathlines;

#endregion
    public void DrawPath(UnitScript unit)
    {
        int currentNode = 0;
        // Delete all GO for pathfinding lines Gameobjects
        DeletePathLines();

        while (currentNode < unit.unit_data.current_path.Count - 1)
        {
            int x1 = unit.unit_data.current_path[currentNode].coordX;
            int y1 = unit.unit_data.current_path[currentNode].coordY;
            int x2 = unit.unit_data.current_path[currentNode+1].coordX;
            int y2 = unit.unit_data.current_path[currentNode+1].coordY;

            // Spawning actual Gameobjects for pathing.
            GameObject newpathline = Instantiate(pathline, new Vector3(x1,y1,-0.6f), Quaternion.identity);
            newpathline.transform.parent = Game.instance.map_ui_root.transform;

            // Rotate the line accordingly
            if (y2 > y1)
            {
                newpathline.transform.Rotate(-90, 90, 0, Space.Self);
            } else if (y2 < y1) {
                newpathline.transform.Rotate(90, 90, 0, Space.Self);
            } else if (x2 > x1) {
                newpathline.transform.Rotate(0, 90, 0, Space.Self);
            } else {
                newpathline.transform.Rotate(180, 90, 0, Space.Self);
            }
            
            pathlines.Add(newpathline);

            /* // Drawing debug lines for pathfinding
            Vector3 start = new Vector3 (unit_data.current_path[currentNode].coordX, unit_data.current_path[currentNode].coordY, -0.6f);
            Vector3 end = new Vector3 (unit_data.current_path[currentNode + 1].coordX, unit_data.current_path[currentNode + 1].coordY, -0.6f);
            Debug.DrawLine(start, end);*/
            currentNode++;
        }
    }

    public void DeletePathLines()
    {
        foreach (GameObject line in pathlines)
        {
            Destroy(line);
        }
    }
    public void GenerateMapVisuals()
    {
        for (int x = 0; x < Game.instance.mapSizeX; x++)
        {
            for (int y = 0; y < Game.instance.mapSizeY; y++)
            {
                GameObject maptile_go = Instantiate (Game.instance.terrain_tiles[x,y].tilePrefab, new Vector3 (x , y, 0), Quaternion.identity);
                maptile_go.GetComponent<TileScript>().coordX = x;
                maptile_go.GetComponent<TileScript>().coordY = y;
                maptile_go.transform.parent = Game.instance.map_tiles_root.transform;
            }
        }
    }

    public void GenerateUnitVisuals()
    {
        foreach(Unit unit in Game.instance.units)
        {
            GameObject spawned_unit = Instantiate (Game.instance.sample_unit, new Vector3 (unit.coordX, unit.coordY, 0), Quaternion.identity);
            spawned_unit.GetComponent<UnitScript>().unit_data = unit;
            spawned_unit.GetComponent<UnitScript>().unit_data.unit_go = spawned_unit;
            spawned_unit.transform.parent = Game.instance.map_units_root.transform;
        }
    }

#region Unity life
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Debug.LogError("Another instance of 'UI' class is already running!");
        }
    }

    void Start()
    {
        pathlines = new List<GameObject>();
    }
#endregion
}