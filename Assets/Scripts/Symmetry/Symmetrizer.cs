#define REMOVE_THREADS_DEBUG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Symmetrizer : MonoBehaviour
{
    //public enum SymmetryType
    //{
    //    NONE = 0,
    //    SYMMETRY_AXES_TWO_SIDES,
    //    SYMMETRY_AXES_ONE_SIDE, //both straight and diagonal axes
    //    SYMMETRY_POINT,
    //};

    //public SymmetryType m_symmetryType { get; set; }

    protected GameScene m_gameScene;
    protected ClippingManager m_clippingManager;

    public virtual void Init()
    {
        GameObject gameControllerObject = (GameObject)GameObject.FindGameObjectWithTag("GameController");
        m_gameScene = (GameScene)gameControllerObject.GetComponent<SceneManager>().m_currentScene;
        m_clippingManager = gameControllerObject.GetComponent<ClippingManager>();
    }

    public void Symmetrize()
    {
#if (REMOVE_THREADS_DEBUG)
        PerformSymmetry();

        OnSymmetryDone();
#else
        QueuedThreadedJobsManager threadedJobsManager = m_gameScene.GetQueuedThreadedJobsManager();

        threadedJobsManager.AddJob(new ThreadedJob
                                       (
                                           new ThreadedJob.ThreadFunction(PerformSymmetry),
                                           null,
                                           new ThreadedJob.ThreadFunction(OnSymmetryDone)
                                       )
                                 );
#endif
    }

    protected virtual void PerformSymmetry()
    {

    }

    /**
     * Return a Shape that is symmetric of the parameter 'shapeToSymmetrize' about this symmetrizer
     * **/
    public virtual Shape CalculateSymmetricShape(Shape shapeToSymmetrize)
    {
        return null;
    }

    /**
     * Return the symmetric edge of the parameter 'edge' about this symmetrizer
     * **/
    public virtual GridEdge CalculateSymmetricEdge(GridEdge edge)
    {
        return null;
    }

    /**
     * Return the point symmetric of the parameter 'point' about this symmetrizer
     * **/
    public virtual GridPoint CalculateSymmetricPoint(GridPoint point)
    {
        return GridPoint.zero;
    }

    /**
     * Callback when symmetry has been done
     * **/
    public virtual void OnSymmetryDone()
    {

    }

    /**
    * Call this function to render shapes obtained from the process of symmetrization.
    **/
    protected void RenderSymmetrizedShapes(List<Shape> shapes)
    {
        if (shapes != null)
        {
            for (int i = 0; i != shapes.Count; i++)
            {
                Shapes shapesHolder = m_gameScene.m_shapesHolder;

                shapes[i].Triangulate();
                GameObject shapeObject = shapesHolder.CreateShapeObjectFromData(shapes[i], false);
                ShapeMesh shapeMesh = shapeObject.GetComponent<ShapeMesh>();
                shapeMesh.DrawSelectionContour(false);
                shapeMesh.ReleaseSelectionContour();
                shapes[i].FinalizeClippingOperations();
            }
        }
    }

    /**
     * Set the symmetry type for this axis based on the currently selected action button ID
     * **/
    ////public static SymmetryType GetSymmetryTypeFromActionButtonID(GUIButton.GUIButtonID buttonID)
    ////{
    ////    if (buttonID == GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_TWO_SIDES)
    ////        return SymmetryType.SYMMETRY_AXES_TWO_SIDES;
    ////    else if (buttonID == GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_ONE_SIDE)
    ////        return SymmetryType.SYMMETRY_AXES_ONE_SIDE;
    ////    else if (buttonID == GUIButton.GUIButtonID.ID_POINT_SYMMETRY)
    ////        return SymmetryType.SYMMETRY_POINT;
    ////    else
    ////        return SymmetryType.NONE;
    ////}
}