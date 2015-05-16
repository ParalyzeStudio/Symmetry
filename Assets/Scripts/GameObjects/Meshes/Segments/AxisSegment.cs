using UnityEngine;
using System.Collections;

public class AxisSegment : ColorSegment
{
    public void Build(Vector2 gridPointA, Vector2 gridPointB, float thickness, Material material, Color color)
    {
        base.Build(gridPointA, gridPointB, thickness, material, color, true, 0);
    }
}
