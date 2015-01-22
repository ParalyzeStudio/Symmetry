using UnityEngine;
using System.Collections;

public enum SymmetryType
{
    SYMMETRY_AXIS,
    SYMMETRY_POINT,
    SUBTRACTION_AXIS,
    SUBTRACTION_POINT
};

public class Symmetrizer : MonoBehaviour 
{
    public SymmetryType m_type;

    public void Symmetrize()
    {
        switch (m_type)
        {
            case SymmetryType.SYMMETRY_AXIS:
                SymmetrizeByAxis();
                break;
            case SymmetryType.SYMMETRY_POINT:
                SymmetrizeByPoint();
                break;
            case SymmetryType.SUBTRACTION_AXIS:
                SymmetrizeSubtractionByAxis();
                break;
            case SymmetryType.SUBTRACTION_POINT:
                SymmetrizeSubtractionByPoint();
                break;
            default:
                break;
        }
    }

    public void SymmetrizeByAxis()
    {

    }

    public void SymmetrizeByPoint()
    {

    }

    public void SymmetrizeSubtractionByAxis()
    {

    }

    public void SymmetrizeSubtractionByPoint()
    {

    }
}
