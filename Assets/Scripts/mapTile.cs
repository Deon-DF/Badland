using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapTile : MonoBehaviour
{
    public int coordX = 0;
    public int coordY = 0;

    void OnMouseUp() {
        TileType clickedTile = game.instance.terrain_tiles[coordX, coordY];
        Debug.Log ("[" + coordX + ":" + coordY + "] " + clickedTile.name + " , Walkable:" + clickedTile.isWalkable);
        if (clickedTile.isWalkable == true)
        {
            game.instance.current_unit.transform.position = new Vector3 (coordX, coordY, 0);
        }
    }
}
