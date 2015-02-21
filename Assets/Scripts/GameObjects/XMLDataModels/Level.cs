using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level
{
    public int m_number { get; set; } //the number of this level
    public string m_name { get; set; }
    public List<Contour> m_contours { get; set; }
    public List<Shape> m_initialShapes { get; set; }
    public List<string> m_actionButtonsTags { get; set; }

    public Level()
    {
        m_contours = new List<Contour>();
        m_initialShapes = new List<Shape>();
        m_actionButtonsTags = new List<string>();
    }
}
