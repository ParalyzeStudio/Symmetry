﻿using UnityEngine;
using System.Collections.Generic;

/**
 * A structure that represents a closed path of GridPoint data types
 * **/
public class Contour : List<GridPoint>
{
    public Contour()
        : base()
    {

    }

    public Contour(IEnumerable<GridPoint> collection)
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
            A += this[p].X * this[q].Y - this[q].X * this[p].Y;
        }
        return A * 0.5f;
    }

    /**
     * Calculate the position of the barycentre of this contour
     * **/
    //public GridPoint GetBarycentre()
    //{
    //    GridPoint barycentre = GridPoint.zero;
    //    for (int i = 0; i != this.Count; i++)
    //    {
    //        barycentre += this[i];
    //    }

    //    barycentre /= this.Count;

    //    return barycentre;
    //}

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

                GridPoint contourVertex = contour[i];

                int farthestEqualVertexIndex = -1;
                for (int j = i + 1; j != contour.Count; j++)
                {
                    GridPoint contourTestVertex = contour[j]; //the vertex to be test against contourVertex for equality

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

                    if (splitContour.Count > 2)
                        splitContours.Add(splitContour);

                    //replace the contour with the sub contour
                    contour = contour.SubContour(i, farthestEqualVertexIndex - i);

                    break; //break the for loop and continue on the while loop
                }
            }
            if (!bRepeatedVertices) //no repeated vertices in this contour, add it to split contours and break the while loop
            {
                if (contour.Count > 2)
                    splitContours.Add(contour);
                break;
            }
        }

        return splitContours;
    }

    /**
     *  Remove possible duplicate vertices in this contour
     * **/
    public void RemoveDuplicateVertices()
    {
        Contour uniqueVerticesContour = new Contour();
        List<int> duplicatesIndices = new List<int>();

        for (int i = 0; i != this.Count; i++)
        {
            if (!uniqueVerticesContour.HasPoint(this[i]))
                uniqueVerticesContour.Add(this[i]);
            else
                duplicatesIndices.Add(i);
        }

        for (int i = 0; i != duplicatesIndices.Count; i++)
        {
            this.Remove(this[duplicatesIndices[i] - i]);
        }
    }

    /**
     * If 3 points (or more) belong to the same segment remove the inner points
     * **/
    public void RemoveAlignedVertices()
    {
        for (int i = 0; i != this.Count; i++)
        {
            GridPoint point1 = this[i];
            GridPoint point2 = (i == this.Count - 1) ? this[0] : this[i + 1];
            GridPoint point3;
            if (i == this.Count - 2)
                point3 = this[0];
            else if (i == this.Count - 1)
                point3 = this[1];
            else
                point3 = this[i + 2];

            float determinant = MathUtils.Determinant(point1, point2, point3);
            if (Mathf.Abs(determinant) == 0)
            {
                this.Remove(point2);
                i--;
            }
        }
    }

    /**
     * When one or more vertex are shared between multiple edges we can split this contour into several
     * **/
    public List<Contour> SplitIntoSimpleContours()
    {
        List<Contour> splitContours = new List<Contour>();

        Contour contourToSplit = new Contour(this); //make a copy of this contour to work on

        //First thing to do is to test if one or more contour points is shared by another edge
        for (int i = 0; i != this.Count; i++)
        {
            GridPoint contourPoint = this[i];
            for (int j = 0; j != this.Count; j++)
            {
                if (i != j)
                {
                    GridPoint edgePoint1 = contourToSplit[j];
                    GridPoint edgePoint2 = contourToSplit[j < contourToSplit.Count - 1 ? j + 1 : 0];
                    GridEdge edge = new GridEdge(edgePoint1, edgePoint2);

                    if (edge.ContainsPoint(contourPoint, false))
                    {
                        contourToSplit.Insert(j + 1, contourPoint);
                    }
                }
            }
        }

        bool bRepeatedVertices = false;
        while (contourToSplit.Count > 0)
        {
            for (int i = 0; i != contourToSplit.Count; i++)
            {
                bRepeatedVertices = false;

                GridPoint contourPoint = contourToSplit[i];

                int farthestEqualVertexIndex = -1;
                for (int j = i + 1; j != contourToSplit.Count; j++)
                {
                    GridPoint contourTestVertex = contourToSplit[j]; //the vertex to be test against contourVertex for equality

                    if (contourTestVertex.Equals(contourPoint))
                        farthestEqualVertexIndex = j;
                }

                if (farthestEqualVertexIndex >= 0) //we found the same vertex at a different index
                {
                    bRepeatedVertices = true;

                    //extract the first split contour
                    Contour splitContour = new Contour();
                    splitContour.Capacity = contourToSplit.Count - farthestEqualVertexIndex + i;
                    for (int k = farthestEqualVertexIndex; k != contourToSplit.Count; k++)
                    {
                        splitContour.Add(contourToSplit[k]);
                    }
                    for (int k = 0; k != i; k++)
                    {
                        splitContour.Add(contourToSplit[k]);
                    }

                    if (splitContour.Count > 2)
                        splitContours.Add(splitContour);

                    //replace the contour with the sub contour
                    contourToSplit = new Contour(contourToSplit.GetRange(i, farthestEqualVertexIndex - i));

                    break; //break the for loop and continue on the while loop
                }
            }
            if (!bRepeatedVertices) //no repeated vertices in this contour, add it to split contours and break the while loop
            {
                if (contourToSplit.Count > 2)
                    splitContours.Add(contourToSplit);
                break;
            }
        }

        return splitContours;
    }

    /**
     * Multiply every point in that contour by a constant
     * **/
    public void Scale(float scaleConstant)
    {
        for (int i = 0; i != this.Count; i++)
        {
            this[i] *= scaleConstant;
        }
    }

    /**
     * Translate every point in that contour
     * **/
    public void Translate(GridPoint translation)
    {
        for (int i = 0; i != this.Count; i++)
        {
            this[i] += translation;
        }
    }

    /**
     * Returns the contour vertex with the minimal x-coordinate along the parameter 'axis'
     * **/
    //public Vector2 GetLeftMostPointAlongAxis(Vector2 axisDirection)
    //{
    //    Quaternion rotation = Quaternion.FromToRotation(GeometryUtils.BuildVector3FromVector2(axisDirection, 0), new Vector3(1,0,0));

    //    //create a rotated copy of the contour
    //    int leftMostVertexIndex = 0; //the index of the last left most vertex found
    //    float minX = float.MaxValue; //the current min x-coordinate found
    //    for (int i = 0; i != this.Count; i++)
    //    {
    //        Vector2 rotatedVertex = rotation * this[i];
    //        if (rotatedVertex.x < minX)
    //        {
    //            minX = rotatedVertex.x;
    //            leftMostVertexIndex = i;
    //        }
    //    }

    //    return this[leftMostVertexIndex];
    //}

    /**
     * Test if the contour has the 'point' parameter in it
     * **/
    public bool ContainsPoint(GridPoint point)
    {
        for (int i = 0; i != Count; i++)
        {
            GridPoint contourPoint1 = this[i];
            GridPoint contourPoint2 = this[(i == Count - 1) ? 0 : i + 1];
            GridEdge edge = new GridEdge(contourPoint1, contourPoint2);

            if (edge.ContainsPoint(point))
                return true;
        }

        return false;
    }

    /**
     * Test if one this contour vertices is equal to the 'point' parameter
     * **/
    public bool HasPoint(GridPoint point)
    {
        for (int i = 0; i != Count; i++)
        {
            if (this[i] == point)
                return true;
        }

        return false;
    }

    /**
     * Insert a point into this contour
     * **/
    public void InsertPoint(GridPoint point)
    {
        if (!ContainsPoint(point)) //if point is not on the contour we cannot insert it
            return;

        //First check if the point is equal to one of the contour vertices
        if (HasPoint(point)) //do nothing on the contour, just return
            return; 

        //Point is stricly on one of the contour edges
        for (int i = 0; i != Count; i++)
        {
            GridPoint contourPoint1 = this[i];
            GridPoint contourPoint2 = this[(i == Count - 1) ? 0 : i + 1];
            GridEdge edge = new GridEdge(contourPoint1, contourPoint2);

            if (edge.ContainsPoint(point))
            {
                int index = i + 1; //insert point between contourPoint1 and contourPoint2
                this.Insert(index, point);
                return;
            }
        }
    }

    /**
     * Test if the contour passed as parameter is the same as this contour
     * Offer the possibility to check if they have the same points despite an opposite winding order
     * **/
    public bool EqualsContour(Contour contour, bool bSameWindingOrder = true)
    {
        if (this.Count != contour.Count) //not the same number of elements in those two contours
            return false;

        //Find the same point as the one with index 0 in this contour and define an offset
        int offset = -1;
        for (int i = 0; i != contour.Count; i++)
        {
            if (MathUtils.AreVec2PointsEqual(contour[i], this[0]))
                offset = i;
        }

        if (offset < 0)
            return false;

        //now check if other points are equal
        for (int i = 0; i != this.Count; i++)
        {
            GridPoint contour1Point = this[i];
            GridPoint contour2Point;
            if (bSameWindingOrder)
            {
                contour2Point = contour[(i + offset) > contour.Count - 1 ? offset - (contour.Count - i) : i + offset];
                if (contour1Point != contour2Point)
                    return false;
            }
            else
            {
                //TODO treat the case where we can test also the opposite winding number
            }
        }

        return true;
    }
    
    /**
    * Tell if one of the contour edge intersects the edge passed as parameter
    **/
    public bool IntersectsEdge(GridEdge edge)
    {
        for (int i = 0; i != this.Count; i++)
        {
            GridEdge contourEdge = new GridEdge(this[i], this[(i == this.Count - 1) ? 0 : i + 1]);

            if (contourEdge.IntersectsEdge(edge) > 0)
                return true;
        }

        return false;
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