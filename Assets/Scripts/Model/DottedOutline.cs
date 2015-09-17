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
}