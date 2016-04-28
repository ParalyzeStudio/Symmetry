using UnityEngine;
using System.Collections;

public class OutlineSegment : TexturedSegment 
{
    public const float DEFAULT_OUTLINE_SEGMENT_THICKNESS = 8.0f;

    public void Build(Vector3 pointA, Vector3 pointB, Color tintColor, float thickness = DEFAULT_OUTLINE_SEGMENT_THICKNESS)
    {
        base.Build(pointA, pointB, thickness, null, tintColor, TextureWrapMode.Repeat); //material is already set in the prefab so pass null to the function
    }
}
