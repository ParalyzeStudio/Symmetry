using UnityEngine;
using System.Collections;

public class OutlineSegment : TexturedSegment 
{
    public const float DEFAULT_OUTLINE_SEGMENT_THICKNESS = 8.0f;

    public void Build(Vector2 gridPointA, Vector2 gridPointB, Color tintColor, float thickness = DEFAULT_OUTLINE_SEGMENT_THICKNESS)
    {
        base.Build(gridPointA, gridPointB, thickness, null, tintColor, true, TextureWrapMode.Repeat); //material is already set in the prefab so pass null to the function
        //base.Build(gridPointA, gridPointB, thickness, null, Color.black, true, 0); //material is already set in the prefab so pass null to the function
    }
}
