using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitScript : MonoBehaviour
{
    public Unit unit_data;

    void OnMouseUp() {
        Game.instance.selected_unit = this.unit_data.unit_go;

        if (unit_data.current_path == null)
        {
            UI.instance.DeletePathLines();
        }

        Debug.Log ("Selected unit: " + unit_data.unit_name + " [" + unit_data.coordX + ":" + unit_data.coordY + "], Owner: " + unit_data.owner + ", Guid: " + unit_data.guid);
    }

    void Update()
    {
        if (Game.instance.selected_unit != null && Game.instance.selected_unit.GetComponent<UnitScript>().unit_data == this.unit_data)
        {
            // Drawing debug lines for pathfinding
            if (unit_data != null)
            {
                if (unit_data.current_path != null)
                {
                    UI.instance.DrawPath(this);
                }
            }
        }
    }
}
