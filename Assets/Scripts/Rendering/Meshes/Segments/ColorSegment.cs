using UnityEngine;

public class ColorSegment : Segment
{
    protected Color m_color; //the color of the segment

    protected virtual void RenderInternal(bool bUpdateVertices = true, bool bUpdateIndices = true, bool bUpdateColor = true)
    {
        base.RenderInternal(bUpdateVertices, bUpdateIndices);

        if (bUpdateColor)
        {
            Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

            int colorsLength = mesh.vertices.Length;
            Color[] colors = new Color[colorsLength];
            for (int i = 0; i != colorsLength; i++)
            {
                colors[i] = m_color;
            }

            mesh.colors = colors;
        }
    }

    public virtual void Build(Vector2 pointA, Vector2 pointB, float thickness, Material material, Color color, bool bGridPoints, int numSegmentsPerHalfCircle = DEFAULT_NUM_SEGMENTS_PER_HALF_CIRCLE)
    {
        m_color = color;
        base.InitBasicVariables(pointA, pointB, thickness, material, bGridPoints, numSegmentsPerHalfCircle);
        RenderInternal(); //builds the mesh
    }

    /**
     * Set new color for the segment
     * **/
    public virtual void SetColor(Color color)
    {
        RenderInternal(false, false, true);
    }
}

