using UnityEngine;
using System.Collections.Generic;

public class TitleLetter : MonoBehaviour
{
    public struct TitleTriangle
    {
        public TitleTriangle(int column, int line, bool pointingToRight)
        {
            m_column = column;
            m_line = line;
            m_pointingToRight = pointingToRight;
        }

        public int m_column;
        public int m_line;
        public bool m_pointingToRight; //is the triangle pointing to left or right
    }

    private GameObject[] m_triangles;
    private GameObject[] m_shadowTriangles;
    public GameObject m_circleMeshPfb;

    public GameObject m_foregroundTrianglesHolder { get; set; }
    public GameObject m_shadowTrianglesHolder { get; set; }

    private BackgroundTrianglesRenderer m_backgroundRenderer;

    /**
     * Init the letter by setting its data, material color and eventually offset.
     * offset is expressed in triangle units
     * **/
    public void Init(TitleTriangle[] m_trianglesData, Material material, Material shadowMaterial, Vector2 offset)
    {
        m_triangles = new GameObject[m_trianglesData.Length];
        m_shadowTriangles = new GameObject[m_trianglesData.Length];

        m_foregroundTrianglesHolder = new GameObject("Foreground");
        m_foregroundTrianglesHolder.transform.parent = this.transform;
        m_shadowTrianglesHolder = new GameObject("Shadow");
        m_shadowTrianglesHolder.transform.parent = this.transform;
        m_foregroundTrianglesHolder.AddComponent<GameObjectAnimator>();
        m_shadowTrianglesHolder.AddComponent<GameObjectAnimator>();

        for (int i = 0; i != m_trianglesData.Length; i++)
        {
            GameObject triangleObject = (GameObject)Instantiate(m_circleMeshPfb);
            triangleObject.name = "TitleTriangle";

            CircleMesh triangleMesh = triangleObject.GetComponent<CircleMesh>();
            triangleMesh.Init(material);

            CircleMeshAnimator triangleMeshAnimator = triangleObject.GetComponent<CircleMeshAnimator>();
            triangleMeshAnimator.SetParentTransform(m_foregroundTrianglesHolder.transform);
            triangleMeshAnimator.SetNumSegments(3, false);
            triangleMeshAnimator.SetInnerRadius(0, false);
            triangleMeshAnimator.SetOuterRadius(0.92f * 2 / 3.0f * GetBackgroundRenderer().m_triangleHeight);

            //Set correct position for triangle
            Vector3 trianglePosition;
            
            if (m_trianglesData[i].m_column < 0)
                trianglePosition = new Vector3((m_trianglesData[i].m_column + offset.x + 0.5f) * GetBackgroundRenderer().m_triangleHeight,
                                               (m_trianglesData[i].m_line + offset.y) * 0.5f * GetBackgroundRenderer().m_triangleEdgeLength,
                                               0);
            else
                trianglePosition = new Vector3((m_trianglesData[i].m_column + offset.x - 0.5f) * GetBackgroundRenderer().m_triangleHeight,
                                               (m_trianglesData[i].m_line + offset.y) * 0.5f * GetBackgroundRenderer().m_triangleEdgeLength,
                                               0);
            if (m_trianglesData[i].m_pointingToRight)
                trianglePosition -= new Vector3(1 / 6.0f * GetBackgroundRenderer().m_triangleHeight, 0, 0);
            else
                trianglePosition += new Vector3(1 / 6.0f * GetBackgroundRenderer().m_triangleHeight, 0, 0);

            triangleMeshAnimator.SetPosition(trianglePosition);

            //Set correct orientation for triangle
            float angle = m_trianglesData[i].m_pointingToRight ? -90 : 90;
            triangleMeshAnimator.SetRotationAxis(Vector3.forward);
            triangleMeshAnimator.SetRotationAngle(angle);

            m_triangles[i] = triangleObject;

            //set shadow
            GameObject shadowTriangleObject = (GameObject)Instantiate(m_circleMeshPfb);
            shadowTriangleObject.name = "ShadowTriangle";

            CircleMesh shadowTriangleMesh = shadowTriangleObject.GetComponent<CircleMesh>();
            shadowTriangleMesh.Init(shadowMaterial);

            CircleMeshAnimator shadowTriangleMeshAnimator = shadowTriangleObject.GetComponent<CircleMeshAnimator>();
            shadowTriangleMeshAnimator.SetParentTransform(m_shadowTrianglesHolder.transform);
            shadowTriangleMeshAnimator.SetNumSegments(3, false);
            shadowTriangleMeshAnimator.SetInnerRadius(0, false);
            shadowTriangleMeshAnimator.SetOuterRadius(2 / 3.0f * GetBackgroundRenderer().m_triangleHeight);
            Vector3 shadowTrianglePosition = trianglePosition + new Vector3(-20, 20, 0);
            shadowTrianglePosition.z = trianglePosition.z + 1;
            shadowTriangleMeshAnimator.SetPosition(shadowTrianglePosition);

            //Set correct orientation for triangle
            angle = m_trianglesData[i].m_pointingToRight ? -90 : 90;
            shadowTriangleMeshAnimator.SetRotationAxis(Vector3.forward);
            shadowTriangleMeshAnimator.SetRotationAngle(angle);

            m_shadowTriangles[i] = shadowTriangleObject;
        }        
    }

    /**
     * Set color on the triangles that form this letter foreground
     * **/
    public void SetForegroundColor(Color color)
    {
        m_foregroundTrianglesHolder.GetComponent<GameObjectAnimator>().SetColor(color);
        for (int i = 0; i != m_triangles.Length; i++)
        {
            m_triangles[i].GetComponent<GameObjectAnimator>().SetColor(color);
        }
    }

    /**
     * Set color on the triangles that form this letter shadow
     * **/
    public void SetShadowColor(Color color)
    {
        m_shadowTrianglesHolder.GetComponent<GameObjectAnimator>().SetColor(color);
        for (int i = 0; i != m_shadowTriangles.Length; i++)
        {
            m_shadowTriangles[i].GetComponent<GameObjectAnimator>().SetColor(color);
        }
    }

    /**
     * Set opacity on the triangles that form this letter foreground
     * **/
    public void SetForegroundOpacity(float opacity)
    {
        m_foregroundTrianglesHolder.GetComponent<GameObjectAnimator>().SetOpacity(opacity);
    }

    /**
     * Set opacity on the triangles that form this letter shadow
     * **/
    public void SetShadowOpacity(float opacity)
    {
        m_shadowTrianglesHolder.GetComponent<GameObjectAnimator>().SetOpacity(opacity);
    }

    private BackgroundTrianglesRenderer GetBackgroundRenderer()
    {
        if (m_backgroundRenderer == null)
            m_backgroundRenderer = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundTrianglesRenderer>();
        return m_backgroundRenderer;
    }
}
