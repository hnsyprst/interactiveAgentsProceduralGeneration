using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPLine
{
    public LineOrientation Orientation;
    public Vector2 Coordinates;

    public SPLine(LineOrientation _Orientation, Vector2 _Coordinates)
    {
        Orientation = _Orientation;
        Coordinates = _Coordinates;
    }
}

public enum LineOrientation
{
    Horizontal = 0,
    Vertical = 1
}
