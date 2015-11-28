﻿using UnityEngine;

/**
 * A data structure to manage points held by data (outlines, shapes, axes...) inside the grid
 * It approximates decimal values to int values (we do not need long value type as we won't use more than 1 or 2 significant figures for precision) 
 * **/
public struct GridPoint
{
    public const int DEFAULT_SCALE_PRECISION = 1000; //set the default scale value which is usually a power of 10 (a bigger value means bigger precision on data)

    private int m_x;
    public int X
    {
        get
        {
            return m_x;
        }
        set
        {
            m_x = value;
        }
    }
    private int m_y;
    public int Y
    {
        get
        {
            return m_y;
        }
        set
        {
            m_y = value;
        }
    }

    public static GridPoint zero
    {
        get
        {
            return new GridPoint(0, 0);
        }
    }

    public static GridPoint operator -(GridPoint a) { return new GridPoint(-a.X, -a.Y); }
    public static GridPoint operator -(GridPoint a, GridPoint b) { return new GridPoint(a.X - b.X, a.Y - b.Y); }
    public static bool operator !=(GridPoint lhs, GridPoint rhs) { return !lhs.Equals(rhs); }
    public static GridPoint operator *(float d, GridPoint a) { return new GridPoint(Mathf.RoundToInt(d * a.X), Mathf.RoundToInt(d * a.Y)); }
    public static GridPoint operator *(GridPoint a, float d) { return new GridPoint(Mathf.RoundToInt(a.X * d), Mathf.RoundToInt(a.Y * d)); }
    public static GridPoint operator /(GridPoint a, float d) { return new GridPoint(Mathf.RoundToInt(a.X / d), Mathf.RoundToInt(a.Y / d)); }
    public static GridPoint operator +(GridPoint a, GridPoint b) { return new GridPoint(a.X + b.X, a.Y + b.Y); }
    public static bool operator ==(GridPoint lhs, GridPoint rhs) { return lhs.Equals(rhs); }
    public static implicit operator Vector2(GridPoint a) { return new Vector2(a.X, a.Y); }
    public static implicit operator ClipperLib.IntPoint(GridPoint a) { return new ClipperLib.IntPoint(a.X, a.Y); }

    public override bool Equals(object obj) 
    {
        if (!(obj is GridPoint))
            return false;

        return Equals((GridPoint)obj);
    }

    public bool Equals(GridPoint other)
    {
        if (X != other.X)
            return false;

        return Y == other.Y;
    }         

    public override int GetHashCode() { return X ^ Y; }

    public float magnitude
    {
        get
        {
            return Mathf.Sqrt(X * X + Y * Y);
        }
    }

    public long sqrMagnitude
    {
        get
        {
            return X * X + Y * Y;
        }
    }

    //We can scale this vector (by an integer value) for more precision when performing math operations on it
    public int m_scale { get; set; }

    public GridPoint(int x, int y) : this()
    {
        m_x = x;
        m_y = y;
    }

    /**
     * Scales the point
     * **/
    public void Scale(int scale = DEFAULT_SCALE_PRECISION)
    {
        //Apply the new scale to the (column, line) GridPoint
        m_scale = scale;
        X *= scale;
        Y *= scale;
    }

    /**
     * Downscale this gridpoint to have its original (column, line) position
     * These are float values so we return a Vector2
     * **/
    public Vector2 GetAsColumnLineVector()
    {
        return new Vector2(X / (float)m_scale, Y / (float)m_scale);
    }

    public override string ToString()
    {
        return "(" + X + "," + Y + ")";
    }
}
