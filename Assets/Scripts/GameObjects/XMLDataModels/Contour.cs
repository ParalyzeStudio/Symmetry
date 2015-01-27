﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Contour
{
    public List<Vector2> m_points { get; set; } //the list of points in this contour in GridAnchor coordinates (line, column)
    public float m_area { get; set; }

    public Contour()
    {
        m_points = new List<Vector2>();
        m_area = 0;
    }
}
