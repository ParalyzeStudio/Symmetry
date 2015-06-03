using UnityEngine;
using System.Collections.Generic;

/**
 * Class to hold and animate background triangles in one single column
 * **/
public class BackgroundTriangleColumn : List<BackgroundTriangle>
{
    public int m_index { get; set; } //the index of this column in the global mesh

    public BackgroundTriangleColumn(int index) : base()
    {
        m_index = index;
    }
}

