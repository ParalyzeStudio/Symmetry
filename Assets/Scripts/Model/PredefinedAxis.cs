using UnityEngine;

public class PredefinedAxis
{
    //Predefined axes
    public const string PREDEFINED_AXIS_HORIZONTAL = "PREDEFINED_AXIS_HORIZONTAL";
    public const string PREDEFINED_AXIS_VERTICAL = "PREDEFINED_AXIS_VERTICAL";
    public const string PREDEFINED_AXIS_DIAGONAL_LEFT = "PREDEFINED_AXIS_DIAGONAL_LEFT";
    public const string PREDEFINED_AXIS_DIAGONAL_RIGHT = "PREDEFINED_AXIS_DIAGONAL_RIGHT";

    public string m_strID { get; set; }
    public int m_length { get; set; }

    public PredefinedAxis(string ID, int length)
    {
        m_strID = ID;
        m_length = length;
    }
}
