using UnityEngine;

public class ShapeBuilder : MonoBehaviour
{
    public GameObject m_shapePfb; //prefab to instantiate a shape

    /**
     * Build a shape game object from shape data (contour/triangles, color) and adds it to the ShapesHolder
     * **/
    public GameObject CreateFromShapeData(Shape shapeData)
    {
        //Create two new shapes from the left and right triangles
        ShapesHolder shapesHolder = this.gameObject.GetComponent<ShapesHolder>();

        GameObject clonedShapeObject = (GameObject)Instantiate(m_shapePfb);
        ShapeRenderer shapeRenderer = clonedShapeObject.GetComponent<ShapeRenderer>();
        shapeRenderer.m_shape = shapeData;
        shapeRenderer.Render(null, ShapeRenderer.RenderFaces.DOUBLE_SIDED, false);

        clonedShapeObject.transform.parent = this.gameObject.transform;
        clonedShapeObject.transform.localPosition = Vector3.zero;

        shapesHolder.AddShapeObject(clonedShapeObject);

        return clonedShapeObject;
    }
}

