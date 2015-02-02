using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level
{
    public List<Contour> m_contours { get; set; }
    public List<Shape> m_initialShapes { get; set; }

    public Level()
    {
        m_contours = new List<Contour>();
        m_initialShapes = new List<Shape>();
    }
}
