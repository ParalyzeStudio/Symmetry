using UnityEngine;

public class ColorSegment : Segment
{
    protected Color[] m_meshColors;
    protected bool m_meshColorsDirty;

    protected virtual void RenderInternal(bool bUpdateVertices = true, bool bUpdateIndices = true, bool bUpdateColor = true)
    {
        base.RenderInternal(bUpdateVertices, bUpdateIndices);

        if (bUpdateColor)
        {
            Color color = GetComponent<ColorSegmentAnimator>().m_color;

            int colorsLength = m_meshVertices.Length;
            if (m_meshColors == null)
                m_meshColors = new Color[colorsLength];
            for (int i = 0; i != colorsLength; i++)
            {
                m_meshColors[i] = color;
            }

            m_meshColorsDirty = true;
        }
    }

    public virtual void Build(Vector3 pointA, Vector3 pointB, float thickness, Material material, Color color, int numSegmentsPerHalfCircle = DEFAULT_NUM_SEGMENTS_PER_HALF_CIRCLE)
    {
        base.InitBasicVariables(pointA, pointB, thickness, material, numSegmentsPerHalfCircle);
        RenderInternal(); //builds the mesh
        GetComponent<ColorSegmentAnimator>().SetColor(color);
    }

    /**
     * Set new color for the segment
     * **/
    public virtual void SetColor(Color color)
    {
        RenderInternal(false, false, true);
    }

    public override void Update()
    {
        base.Update();
        if (m_segmentMesh != null)
        {
            if (m_meshColorsDirty)
            {
                m_segmentMesh.colors = m_meshColors;
                m_meshColorsDirty = false;
            }
        }
    }
}

