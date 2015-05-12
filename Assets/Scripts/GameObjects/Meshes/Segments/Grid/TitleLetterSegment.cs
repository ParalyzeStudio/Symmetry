using UnityEngine;

public class TitleLetterSegment : SimplifiedRoundedSegment
{
    public TitleLetterVertex m_vertexA { get; set; }
    public TitleLetterVertex m_vertexB { get; set; }

    public TitleLetterSegment()
    {

    }

    /**
     * Build the segment holding vertexA and vertexB
     * pointA and pointB are not necessarily equal to vertexA.m_position and vertexB.m_position at this point 
     * because the segment is possibly animated (i.e starts spreading)
     * **/
    public void Build(TitleLetterVertex vertexA, TitleLetterVertex vertexB, Vector2 pointA, Vector2 pointB, float thickness, Material material, Color tintColor)
    {
        m_vertexA = vertexA;
        m_vertexB = vertexB;
        Build(pointA, pointB, thickness, material, tintColor);
    }
}