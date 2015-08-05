﻿using UnityEngine;
using System.Collections.Generic;

public class Contour : List<Vector2>
{
    public Contour()
        : base()
    {

    }

    public Contour(IEnumerable<Vector2> collection)
        : base(collection)
    {

    }

    public Contour(int capacity)
        : base(capacity)
    {

    }

    public Contour SubContour(int index, int count)
    {
        return new Contour(base.GetRange(index, count));
    }

    /**
     * Computes the area of this contour
     * Note that the area can be either positive or negative depending on the orientation of the contour
     * A positive area means a counter-clockwise orientation and a negative one a clockwise orientation
     * **/
    public float GetArea()
    {
        int n = this.Count;

        float A = 0.0f;

        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            A += this[p].x * this[q].y - this[q].x * this[p].y;
        }
        return A * 0.5f;
    }

    /**
     * Calculate the position of the barycentre of this contour
     * **/
    public Vector2 GetBarycentre()
    {
        Vector2 barycentre = Vector2.zero;
        for (int i = 0; i != this.Count; i++)
        {
            barycentre += this[i];
        }

        barycentre /= this.Count;

        return barycentre;
    }

    /**
     * Ensure that the contour has no vertices that repeat.
     * If at least 2 vertices repeat in that contour, split it into several contours
     * **/
    public List<Contour> Split()
    {
        List<Contour> splitContours = new List<Contour>();
        Contour contour = this;

        bool bRepeatedVertices = false;
        while (contour.Count > 0)
        {
            for (int i = 0; i != contour.Count; i++)
            {
                bRepeatedVertices = false;

                Vector2 contourVertex = contour[i];

                int farthestEqualVertexIndex = -1;
                for (int j = i + 1; j != contour.Count; j++)
                {
                    Vector2 contourTestVertex = contour[j]; //the vertex to be test against contourVertex for equality

                    if (contourTestVertex.Equals(contourVertex))
                        farthestEqualVertexIndex = j;
                }

                if (farthestEqualVertexIndex >= 0) //we found the same vertex at a different index
                {
                    bRepeatedVertices = true;

                    //extract the first split contour
                    Contour splitContour = new Contour();
                    splitContour.Capacity = this.Count - farthestEqualVertexIndex + i;
                    for (int k = farthestEqualVertexIndex; k != contour.Count; k++)
                    {
                        splitContour.Add(contour[k]);
                    }
                    for (int k = 0; k != i; k++)
                    {
                        splitContour.Add(contour[k]);
                    }

                    splitContours.Add(splitContour);

                    //replace the contour with the sub contour
                    contour = contour.SubContour(i, farthestEqualVertexIndex - i);

                    break; //break the for loop and continue on the while loop
                }
            }
            if (!bRepeatedVertices) //no repeated vertices in this contour, add it to split contours and break the while loop
            {
                splitContours.Add(contour);
                break;
            }
        }

        return splitContours;
    }

    /**
     * If 3 points (or more) belong to the same segment remove the inner points
     * **/
    public void RemoveAlignedVertices()
    {
        for (int i = 0; i != this.Count; i++)
        {
            Vector2 point1 = this[i];
            Vector2 point2 = (i == this.Count - 1) ? this[0] : this[i + 1];
            Vector2 point3;
            if (i == this.Count - 2)
                point3 = this[0];
            else if (i == this.Count - 1)
                point3 = this[1];
            else
                point3 = this[i + 2];

            float determinant = MathUtils.Determinant(point1, point2, point3, false);
            if (Mathf.Abs(determinant) < MathUtils.DEFAULT_EPSILON)
            {
                this.Remove(point2);
                i--;
            }
        }
    }

    /**
     * Multiply every point in that contour by a constant
     * **/
    public void ScalePoints(float scaleConstant)
    {
        for (int i = 0; i != this.Count; i++)
        {
            this[i] *= scaleConstant;
        }
    }
}

/**
 * Same as contour but with the possibility to set a color on each segment of it
 * **/
public class ColoredContour : Contour
{
    public Color[] m_segmentsColors { get; set; }

    /**
     * Set colors on the left and on the right of the vertex (following the order of vertices)
     * coloring both segments containing that vertex
     * **/
    public void SetColorForVertex(int iVertexIdx, Color leftColor, Color rightColor)
    {
        m_segmentsColors[(iVertexIdx > 0) ? (iVertexIdx - 1) : (m_segmentsColors.Length - 1)] = leftColor;
        m_segmentsColors[(iVertexIdx < m_segmentsColors.Length - 1) ? (iVertexIdx + 1) : 0] = rightColor;  
    }
}