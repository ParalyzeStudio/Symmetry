using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level
{
    public List<Contour> m_contours { get; set; }
    public List<Shape> m_shapes { get; set; }

    public Level()
    {
        m_contours = new List<Contour>();
        m_shapes = new List<Shape>();
    }
}
