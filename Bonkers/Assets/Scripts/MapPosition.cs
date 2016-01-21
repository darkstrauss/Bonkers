using UnityEngine;
using System.Collections;

public class MapPosition {

    public int xPos;
    public int yPos;

    public MapPosition(int x, int y, bool b)
    {
        xPos = x;
        yPos = y;
    }

    override public string ToString()
    {
        return xPos + ", " + yPos;
    }

    public static float EucludianDistance(MapPosition _start, MapPosition _goal)
    {
        return Mathf.Sqrt(Mathf.Pow(_goal.xPos - _start.xPos, 2) + Mathf.Pow(_goal.yPos - _start.yPos, 2));
    }
}
