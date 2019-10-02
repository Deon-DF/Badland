using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public enum UnitOwner {player0, player1, player2, player3, player4, neutral}
    public enum UnitType {land, flying, flat}
    public int coordX;
    public int coordY;
    public System.Guid guid;
    public string unit_name = "Unit 1";
    public UnitOwner owner = Unit.UnitOwner.player0;

    public bool flatMovementCost = false;
    public bool flying = false;

    public float movementMax = 2f;
    public float movementLeft = 2f;

    public GameObject unit_go;

    public List<Node> current_path;


    public Unit()
    {
        unit_name = "Test unit";
        owner = Unit.UnitOwner.player0;
        guid = System.Guid.NewGuid();
        coordX = 0;
        coordY = 0;
        unit_go = null;
    }
}
