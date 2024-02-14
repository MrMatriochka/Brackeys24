using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector2RangeAttribute : PropertyAttribute
{
    // Min/Max values for the X axis
    public readonly int MinX;
    public readonly int MaxX;
    // Min/Max values for the Y axis
    public readonly int MinY;
    public readonly int MaxY;

    public Vector2RangeAttribute(int iMinX, int iMaxX, int iMinY, int iMaxY)
    {
        MinX = iMinX;
        MaxX = iMaxX;
        MinY = iMinY;
        MaxY = iMaxY;
    }
}
