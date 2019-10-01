using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public int coordX = 0;
    public int coordY = 0;

    void OnMouseUp() {
        Tile clickedTile = game.instance.terrain_tiles[coordX, coordY];
        if (game.instance.selected_unit)
        {        
            Debug.Log ("Trying to move '" +
                    game.instance.selected_unit.GetComponent<UnitScript>().unit_data.unit_name +
                    "' to '" +
                    clickedTile.name +
                    "' [" + coordX + ":" + coordY + "] " +
                    " , Walkable:" + clickedTile.isWalkable);
            game.instance.GenerateUnitPathTo(game.instance.selected_unit, this);
            //game.instance.MoveUnitToTile(game.instance.selected_unit, this);
        }
    }
}
