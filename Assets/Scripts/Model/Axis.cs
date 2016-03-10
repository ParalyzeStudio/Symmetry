using UnityEngine;

public class Axis
{
    public AxisRenderer m_parentRenderer { get; set; }

    public GridPoint m_pointA { get; set; }
    public GridPoint m_pointB { get; set; }    

    //Type of the axis
    public enum AxisType
    {
        STATIC_PENDING, //axis is waiting for the player to unstack this symmetry
        STATIC_DONE, //player has finished drawing the axis and symmetry has been done
        DYNAMIC_UNSNAPPED, //player is currently drawing the axis but it is not snapped to a grid anchor
        DYNAMIC_SNAPPED,  //same but this time axis is snapped
        HINT //axis is displayed when the player has requested some help
    }

    public AxisType m_type { get; set; }

    public enum AxisSymmetryType
    {
        SYMMETRY_AXES_ONE_SIDE,
        SYMMETRY_AXES_TWO_SIDES
    }

    public AxisSymmetryType m_symmetryType { get; set; }

    public Axis(GridPoint pointA, GridPoint pointB, AxisType type, AxisSymmetryType symmetryType)
    {
        m_pointA = pointA;
        m_pointB = pointB;
        m_type = type;
        m_symmetryType = symmetryType;
    }

    /**
     * Set the symmetry type for this axis based on the currently selected action button ID
     * **/
    public static AxisSymmetryType GetSymmetryTypeFromActionButtonID(GUIButton.GUIButtonID buttonID)
    {
        if (buttonID == GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_TWO_SIDES)
            return AxisSymmetryType.SYMMETRY_AXES_TWO_SIDES;
        else if (buttonID == GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_ONE_SIDE)
            return AxisSymmetryType.SYMMETRY_AXES_ONE_SIDE;
        else //default
            return AxisSymmetryType.SYMMETRY_AXES_TWO_SIDES;
    }

    /**
    * Return the coordinates of the middle of the axis
    * **/
    public GridPoint GetCenter()
    {
        return 0.5f * (m_pointA + m_pointB);
    }

    /**
    * Return the world coordinates of the middle of this axis
    * **/
    public Vector2 GetWorldCenter()
    {
        Grid grid = m_parentRenderer.GetGameScene().m_grid;
        Vector2 endpoint1WorldPosition = grid.GetPointWorldCoordinatesFromGridCoordinates(m_pointA);
        Vector2 endpoint2WorldPosition = grid.GetPointWorldCoordinatesFromGridCoordinates(m_pointB);
        Vector2 axisWorldCenter = 0.5f * (endpoint1WorldPosition + endpoint2WorldPosition);
        return axisWorldCenter;
    }

    /**
    * Return the direction of this axis without normalizing it
    * **/
    public GridPoint GetDirection()
    {
        return m_pointB - m_pointA;
    }

    /**
     * Return the normal of this axis without normalizing it
     * Can choose between cw or ccw order
     * **/
    public GridPoint GetNormal(bool bClockwiseOrder = true)
    {
        GridPoint axisDirection = GetDirection();
        if (bClockwiseOrder)
            return new GridPoint(axisDirection.Y, -axisDirection.X, false);
        else
            return new GridPoint(-axisDirection.Y, axisDirection.X, false);
    }
}
