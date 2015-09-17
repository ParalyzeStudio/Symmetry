using UnityEngine;
using System.Collections;

public class AxisSegment : ColorSegment
{
    public void Build(Vector2 pointA, Vector2 pointB, float thickness, Material material, Color color)
    {
        base.Build(pointA, pointB, thickness, material, color, 0);
    }
}
