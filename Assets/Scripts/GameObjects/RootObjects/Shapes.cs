using UnityEngine;
using System.Collections.Generic;

public class Shapes : MonoBehaviour
{
    public GameObject m_shapePfb; //prefab to instantiate a shape
    public List<GameObject> m_shapesObj { get; set; } //list of children shapes

    public void Awake()
    {
        m_shapesObj = new List<GameObject>();
    }

    /**
     * Build a shape game object from shape data (contour/triangles, color)
     * **/
    public GameObject CreateShapeObjectFromData(Shape shapeData)
    {
        GameObject clonedShapeObject = (GameObject)Instantiate(m_shapePfb);
        ShapeRenderer shapeRenderer = clonedShapeObject.GetComponent<ShapeRenderer>();
        shapeRenderer.m_shape = shapeData;
        shapeRenderer.Render(null, ShapeRenderer.RenderFaces.DOUBLE_SIDED, false);

        clonedShapeObject.transform.parent = this.gameObject.transform;
        clonedShapeObject.transform.localPosition = Vector3.zero;

        AddShapeObject(clonedShapeObject);

        return clonedShapeObject;
    }

    /**
     * Build a shape game object from a contour and a color
     * **/
    public GameObject CreateShapeObjectFromContourAndColor(List<Vector2> contour, Color color)
    {
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

        AddShapeObject(clonedShapeObject);

        return clonedShapeObject;
    }

    public void AddShapeObject(GameObject shape)
    {
        m_shapesObj.Add(shape);
    }

    public void RemoveShapeObject(GameObject shape)
    {
        for (int shapeIndex = 0; shapeIndex != m_shapesObj.Count; shapeIndex++)
        {
            if (m_shapesObj[shapeIndex] == shape)
            {
                m_shapesObj.Remove(shape);
                return;
            }
        }
    }

    public void DestroyShape(GameObject shape)
    {
        Destroy(shape);
    }

    public void ClearShapeObjects()
    {
        m_shapesObj.Clear();
    }
}
