using UnityEngine;
using System.Collections.Generic;

public class DottedOutline : GridTriangulable	
{
    public DottedOutline(bool gridPointMode)
        : base(gridPointMode)
    {

    }

    public DottedOutline(bool gridPointMode, Contour contour)
        : base(gridPointMode, contour)
    {

    }

    public DottedOutline(bool gridPointMode, Contour contour, List<Contour> holes)
        : base(gridPointMode, contour, holes)
    {

    }

    public DottedOutline(GridTriangulable other)
        : base(other)
    {

    }

    /**
     * Same as Shape.OverlapsShape but with an outline as first parameter
     * **/
    public bool OverlapsShape(Shape shape, bool bEnsureNonNullIntersection)
    {
        Shape outlineShape = new Shape(false, this.m_contour, this.m_holes);
        return outlineShape.OverlapsShape(shape, bEnsureNonNullIntersection);
    }
}