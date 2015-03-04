using UnityEngine;
using System.Collections.Generic;

public class ShapeBuilder : MonoBehaviour
{
    public GameObject m_shapePfb; //prefab to instantiate a shape

    /**
     * Build a shape game object from shape data (contour/triangles, color)
     * **/
    public GameObject CreateFromShapeData(Shape shapeData)
    {
        ShapesHolder shapesHolder = this.gameObject.GetComponent<ShapesHolder>();

        GameObject clonedShapeObject = (GameObject)Instantiate(m_shapePfb);
        ShapeRenderer shapeRenderer = clonedShapeObject.GetComponent<ShapeRenderer>();
        shapeRenderer.m_shape = shapeData;
        shapeRenderer.Render(null, ShapeRenderer.RenderFaces.DOUBLE_SIDED, false);

        clonedShapeObject.transform.parent = this.gameObject.transform;
        clonedShapeObject.transform.localPosition = Vector3.zero;

        return clonedShapeObject;
    }

    /**
     * Build a shape game object from a contour and a color
     * **/
    public GameObject CreateFromContourAndColor(List<Vector2> contour, Color color)
    {
        //Create two new shapes from the left and right triangles
        ShapesHolder shapesHolder = this.gameObject.GetComponent<ShapesHolder>();

        GameObject clonedShapeObject = (GameObject)Instantiate(m_shapePfb);
        ShapeRenderer shapeRenderer = clonedShapeObject.GetComponent<ShapeRenderer>();
        Shape shapeData = new Shape();
        shapeData.m_contour = contour;
        shapeData.m_color = color;
        shapeData.Triangulate();
        shapeRenderer.m_shape = shapeData;
        shapeRenderer.Render(null, ShapeRenderer.RenderFaces.DOUBLE_SIDED, false);

        clonedShapeObject.transform.parent = this.gameObject.transform;
        clonedShapeObject.transform.localPosition = Vector3.zero;

        return clonedShapeObject;
    }
}

