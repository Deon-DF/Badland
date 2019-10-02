using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public int coordX = 0;
    public int coordY = 0;

    void OnMouseUp() {
        Tile clickedTile = Game.instance.terrain_tiles[coordX, coordY];
        if (Game.instance.selected_unit)
        {        
            Debug.Log ("Trying to move '" +
                    Game.instance.selected_unit.GetComponent<UnitScript>().unit_data.unit_name +
                    "' to '" +
                    clickedTile.name +
                    "' [" + coordX + ":" + coordY + "] " +
                    " , Walkable:" + clickedTile.isWalkable);
            Pathfinding.instance.GenerateUnitPathTo(Game.instance.selected_unit, this);
            //game.instance.MoveUnitToTile(game.instance.selected_unit, this);
        }
    }
}
