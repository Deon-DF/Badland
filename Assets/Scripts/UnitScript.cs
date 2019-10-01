using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitScript : MonoBehaviour
{
    public Unit unit_data;

    void OnMouseUp() {
        game.instance.selected_unit = this.unit_data.unit_go;

        Debug.Log ("Selected unit: " + unit_data.unit_name + " [" + unit_data.coordX + ":" + unit_data.coordY + "], Owner: " + unit_data.owner + ", Guid: " + unit_data.guid);
    }

    void Update()
    {
        if (game.instance.selected_unit != null && game.instance.selected_unit.GetComponent<UnitScript>().unit_data == this.unit_data)
        {
            // Drawing debug lines for pathfinding
            if (unit_data != null)
            {
                if (unit_data.current_path != null)
                {
                    int currentNode = 0;

                    while (currentNode < unit_data.current_path.Count - 1)
                    {
                        Vector3 start = new Vector3 (unit_data.current_path[currentNode].coordX, unit_data.current_path[currentNode].coordY, -0.6f);
                        Vector3 end = new Vector3 (unit_data.current_path[currentNode + 1].coordX, unit_data.current_path[currentNode + 1].coordY, -0.6f);
                        Debug.DrawLine(start, end);
                        currentNode++;
                    }
                }
            }
        }
    }
}
