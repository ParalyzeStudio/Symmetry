using UnityEngine;
using System.Collections.Generic;

public class GameScene : GUIScene
{
    public const float GRID_Z_VALUE = -10.0f;
    public const float CONTOURS_Z_VALUE = -20.0f;
    public const float SHAPES_Z_VALUE = -30.0f;

    //public GameObject m_gridAnchorSelectedPfb;

    public override void Show(bool bAnimated, float fDelay = 0.0f)
    {
        base.Show(bAnimated, fDelay);

        GameObjectAnimator sceneAnimator = this.gameObject.GetComponent<GameObjectAnimator>();
        sceneAnimator.OnOpacityChanged(1);

        TextMeshAnimator debugTextMesh = this.gameObject.GetComponentInChildren<TextMeshAnimator>();
        debugTextMesh.OnOpacityChanged(0);
        debugTextMesh.FadeFromTo(0, 1, 0.5f, fDelay);

        //ShowGrid(fDelay);
        //ShowGUI(fDelay);
        //ShowContours(fDelay);
        //ShowShapes(fDelay);
    }

    public override void Dismiss(float fDuration, float fDelay = 0.0f)
    {
        base.Dismiss(fDuration, fDelay);
    }

    /**
     * We build the grid of anchors that is displayed on the screen and that will help the player positionning shapes and axis...
     * Two anchors are separated from each other of a distance of m_gridSpacing that can be set in the editor
     * **/
    private void ShowGrid(float fDelay)
    {
        GameObject grid = GameObject.FindGameObjectWithTag("Grid");
        grid.transform.position = new Vector3(0, 0, GRID_Z_VALUE);
        grid.GetComponent<GridBuilder>().Build();

        ///*** DEBUG TMP ***/
        ///GridBuilder gridBuilder = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridBuilder>();
        ////List<GameObject> anchors = gridBuilder.GetAnchorsConstrainedBySymmetryType(new Vector2(8, 9), Symmetrizer.SymmetryType.SYMMETRY_AXIS_VERTICAL);
        ////List<GameObject> anchors = gridBuilder.GetAnchorsConstrainedBySymmetryType(new Vector2(8, 9), Symmetrizer.SymmetryType.SYMMETRY_AXIS_HORIZONTAL);
        ////List<GameObject> anchors = gridBuilder.GetAnchorsConstrainedBySymmetryType(new Vector2(8, 9), Symmetrizer.SymmetryType.SYMMETRY_AXES_STRAIGHT);
        //List<GameObject> anchors = gridBuilder.GetAnchorsConstrainedBySymmetryType(new Vector2(19, 1), Symmetrizer.SymmetryType.SYMMETRY_AXES_ALL);
        //foreach (GameObject anchor in anchors)
        //{
        //    Vector2 gridPos = gridBuilder.GetGridCoordinatesFromWorldCoordinates(anchor.transform.position);
        //    Vector3 selectedAnchorPosition = GeometryUtils.BuildVector3FromVector2(anchor.transform.position, -10);
        //    Instantiate(m_gridAnchorSelectedPfb, selectedAnchorPosition, Quaternion.identity);
        //}
        ///*** DEBUG TMP ***/
    }

    /**
     * GUI elements such as pause button, help button, retry button and other stuff
     * **/
    private void ShowGUI(float fDelay)
    {
        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        GameObject gameHUDObject = GameObject.FindGameObjectWithTag("GameHUD");
        GameHUD gameHUD = gameHUDObject.GetComponent<GameHUD>();
        gameHUD.BuildForLevel(levelManager.m_currentLevel.m_number);
    }

    /**
     * Here we build the contour of the shape the player has to reproduce
     * **/
    private void ShowContours(float fDelay)
    {
        GameObject contours = GameObject.FindGameObjectWithTag("Contours");
        contours.transform.position = new Vector3(0, 0, CONTOURS_Z_VALUE);
        contours.GetComponent<ContoursBuilder>().Build();
    }

    /**
     * We set the shapes the player initally starts with
     * **/
    private void ShowShapes(float fDelay)
    {
        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        GameObject shapesObject = GameObject.FindGameObjectWithTag("Shapes");
        ShapeBuilder shapeBuilder = shapesObject.GetComponent<ShapeBuilder>();
        List<Shape> initialShapes = levelManager.m_currentLevel.m_initialShapes;
        for (int iShapeIndex = 0; iShapeIndex != initialShapes.Count; iShapeIndex++)
        {
            Shape shape = initialShapes[iShapeIndex];

            //First triangulate the shape
            shape.Triangulate();

            shapeBuilder.CreateFromShapeData(shape);
        }

        shapesObject.transform.position = new Vector3(0, 0, SHAPES_Z_VALUE);
    }
}
