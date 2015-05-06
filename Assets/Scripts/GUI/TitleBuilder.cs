using UnityEngine;

public class TitleBuilder : MonoBehaviour
{
    public GameObject m_letterSegmentPfb;
    public Material m_segmentMaterial;
    private Material m_clonedMaterial;

    public void Build()
    {
        //Set the unique instance of material for all segments that are going to be rendered
         m_clonedMaterial = (Material) Instantiate(m_segmentMaterial);

        BuildLetter('S');
    }

    private void BuildLetter(char value)
    {
        float letterScale = 300.0f;

        GameObject letterObject = new GameObject("letter");
        letterObject.transform.localPosition = new Vector3(0, 0, -200);

        string levelFilename = "Title/letter_" + value;
        Object levelObjectFile = Resources.Load(levelFilename);

        TextAsset levelFile = (TextAsset)levelObjectFile;

        XMLParser xmlParser = new XMLParser();
        XMLNode rootNode = xmlParser.Parse(levelFile.text);

        XMLNode letterNode = rootNode.GetNode("letter>0");
        string letterValue = letterNode.GetValue("@value");

        XMLNodeList contourPointsNodeList = letterNode.GetNodeList("contour>0>point");
        Vector2[] contourPoints = new Vector2[contourPointsNodeList.Count];
        int iPointIdx = 0;
        foreach (XMLNode contourPointNode in contourPointsNodeList)
        {
            string strContourPointX = contourPointNode.GetValue("@x");
            string strContourPointY = contourPointNode.GetValue("@y");

            float contourPointX = float.Parse(strContourPointX);
            float contourPointY = float.Parse(strContourPointY);

            //transform the coordinates of the letter vertices so the origin is at the center of the letter (x et y coordinates are between 0 and 1000)
            contourPointX -= 500.0f;
            contourPointY = 1000 - contourPointY; //y-axis is reversed in photoshop
            contourPointY -= 500.0f;

            //normalize coordinates by dividing by 1000 and scale them to fit the correct letter size
            contourPointX *= (letterScale / 1000.0f);
            contourPointY *= (letterScale / 1000.0f);

            contourPoints[iPointIdx] = new Vector2(contourPointX, contourPointY);
            iPointIdx++;
        }

        for (int i = 0; i != contourPoints.Length; i++)
        {
            Vector2 pointA = contourPoints[i];
            Vector2 pointB = (i == contourPoints.Length - 1) ? contourPoints[0] : contourPoints[i + 1];

            GameObject clonedRoundedSegmentObject = (GameObject)Instantiate(m_letterSegmentPfb);
            clonedRoundedSegmentObject.transform.parent = letterObject.transform;

            Segment roundedSegment = clonedRoundedSegmentObject.GetComponent<Segment>();
            roundedSegment.Build(pointA, pointB, 3.0f, Color.black);

            MeshRenderer meshRenderer = clonedRoundedSegmentObject.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = m_clonedMaterial;
        }
    }
}

