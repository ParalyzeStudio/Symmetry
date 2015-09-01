using UnityEngine;
using System;

/**
 * Class to render a rounded segment using a circle texture.
 * Less vertices than segment with pure triangulated rounded endpoints
 * **/
public class SimplifiedRoundedSegment : TexturedSegment
{
    /**
     * Renders the segment with rounded endpoints
     * **/
    protected override void RenderInternal(bool bUpdateVertices = true, bool bUpdateIndices = true, bool bUpdateUVs = true)
    {
        //First set the position of the segment and define mesh coordinates for pointA and pointB
        Vector3 segmentPosition = 0.5f * (m_pointA + m_pointB);
        this.gameObject.transform.localPosition = segmentPosition;

        //Calculate the distance between pointA and pointB to determine their mesh coordinates
        m_length = (m_pointB - m_pointA).magnitude;
        Vector3 localPointA = new Vector3(-0.5f * m_length, 0, 0);
        Vector3 localPointB = new Vector3(0.5f * m_length, 0, 0);

        //Then find the angle between pointA and pointB and apply rotation to the segment object
        m_angle = Mathf.Atan2(m_pointB.y - m_pointA.y, m_pointB.x - m_pointA.x);
        this.gameObject.transform.rotation = Quaternion.AngleAxis(m_angle * Mathf.Rad2Deg, Vector3.forward);

        //Build the actual mesh if it doesnt exist
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh roundedSegmentMesh = meshFilter.sharedMesh;
        if (roundedSegmentMesh == null)
        {
            roundedSegmentMesh = new Mesh();
            roundedSegmentMesh.name = "RoundedSegmentMesh";

            //Set the mesh to the MeshFilter component
            meshFilter.sharedMesh = roundedSegmentMesh;
        }

        if (bUpdateVertices)
        {
            int numVertices = 8;
            Vector3[] meshVertices = new Vector3[numVertices];

            //Build vertices
            float halfThickness = 0.5f * m_thickness;

            //left vertices
            meshVertices[0] = localPointA - new Vector3(0, halfThickness, 0);
            meshVertices[1] = localPointA + new Vector3(0, halfThickness, 0);
            meshVertices[4] = localPointA + halfThickness * new Vector3(-1, -1, 0);
            meshVertices[5] = localPointA + halfThickness * new Vector3(-1, 1, 0);

            //right vertices
            meshVertices[2] = localPointB - new Vector3(0, halfThickness, 0);
            meshVertices[3] = localPointB + new Vector3(0, halfThickness, 0);
            meshVertices[6] = localPointA + halfThickness * new Vector3(1, -1, 0);
            meshVertices[7] = localPointA + halfThickness * new Vector3(1, 1, 0);            

            roundedSegmentMesh.vertices = meshVertices;

            //build normals
            Vector3[] normals = new Vector3[numVertices];
            for (int i = 0; i != numVertices; i++)
            {
                normals[i] = Vector3.forward;
            }
            roundedSegmentMesh.normals = normals;

            //build triangles
            if (bUpdateIndices)
            {
                int numTriangles = numVertices - 2;
                int[] meshIndices = new int[3 * numTriangles];

                meshIndices[0] = 0;
                meshIndices[1] = 1;
                meshIndices[2] = 3;
                meshIndices[3] = 0;
                meshIndices[4] = 3;
                meshIndices[5] = 2;
                meshIndices[6] = 1;
                meshIndices[7] = 4;
                meshIndices[8] = 5;
                meshIndices[9] = 0;
                meshIndices[10] = 4;
                meshIndices[11] = 1;
                meshIndices[12] = 2;
                meshIndices[13] = 3;
                meshIndices[14] = 7;
                meshIndices[15] = 2;
                meshIndices[16] = 7;
                meshIndices[17] = 6;

                roundedSegmentMesh.triangles = meshIndices;
            }           
        }

        if (bUpdateUVs)
        {
            UpdateUVs();
        }
    }

    public void Build(Vector2 pointA, Vector2 pointB, float thickness, Material material, Color tintColor, bool isGridSegment)
    {
        InitBasicVariables(pointA, pointB, thickness, material, isGridSegment);

        RenderInternal();

        TexturedSegmentAnimator segmentAnimator = this.GetComponent<TexturedSegmentAnimator>();
        segmentAnimator.SetColor(tintColor);
    }

    protected override void UpdateUVs()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;

        Vector2[] meshUVs = new Vector2[mesh.vertices.Length];

        meshUVs[0] = new Vector2(0.5f, 0);
        meshUVs[1] = new Vector2(0.5f, 1);
        meshUVs[2] = new Vector2(0.5f, 0);
        meshUVs[3] = new Vector2(0.5f, 1);
        meshUVs[4] = new Vector2(0, 0);
        meshUVs[5] = new Vector2(0, 1);
        meshUVs[6] = new Vector2(1, 0);
        meshUVs[7] = new Vector2(1, 1);

        mesh.uv = meshUVs;
    }

    public override void SetPointA(Vector2 pointB, bool bGridPoint = false, bool bRenderSegment = true)
    {
        m_pointB = pointB;

        if (bGridPoint)
            TransformPointsFromGridCoordinatesToWorldCoordinates(true, false);

        if (bRenderSegment)
            RenderInternal(true, false, false);
    }

    public override void SetPointB(Vector2 pointB, bool bGridPoint = false, bool bRenderSegment = true)
    {
        m_pointB = pointB;

        if (bGridPoint)
            TransformPointsFromGridCoordinatesToWorldCoordinates(false, true);

        if (bRenderSegment)
            RenderInternal(true, false, false);
    }

    public override void SetThickness(float thickness)
    {
        m_thickness = thickness;
        RenderInternal(true, false, false);
    }
}
