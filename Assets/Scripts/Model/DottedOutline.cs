using UnityEngine;
using System.Collections.Generic;

public class DottedOutline : GridTriangulable	
{
    public DottedOutline()
        : base()
    {

    }

    public DottedOutline(Contour contour)
        : base(contour)
    {

    }

    public DottedOutline(Contour contour, List<Contour> holes)
        : base(contour, holes)
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
        Shape outlineShape = new Shape(this.m_contour, this.m_holes);
        return outlineShape.OverlapsShape(shape, bEnsureNonNullIntersection);
    }
}