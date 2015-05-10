using UnityEngine;
using System.Collections.Generic;

public class TitleBuilder : MonoBehaviour
{
    public const float TITLE_Z_VALUE = -200.0f;

    public GameObject m_letterSegmentPfb;
    public Material m_segmentMaterial;
    private Material m_clonedMaterial;
    private TitleLetter[] m_letters;

    public void Build()
    {
        //Set the unique instance of material for all segments that are going to be rendered
        m_clonedMaterial = (Material) Instantiate(m_segmentMaterial);

        //Title is SYMMETRY
        m_letters = new TitleLetter[8];

        TitleLetter S = ParseAndBuildLetter('S');
        TitleLetter Y = ParseAndBuildLetter('Y');
        //TitleLetter M = ParseAndBuildLetter('M');
        //TitleLetter E = ParseAndBuildLetter('E');
        //TitleLetter T = ParseAndBuildLetter('T');
        //TitleLetter R = ParseAndBuildLetter('R');

        m_letters[0] = S;
        m_letters[1] = Y;
        m_letters[2] = new TitleLetter(S);
        m_letters[3] = new TitleLetter(S);
        m_letters[4] = new TitleLetter(S);
        m_letters[5] = new TitleLetter(S);
        m_letters[6] = new TitleLetter(S);
        m_letters[7] = new TitleLetter(Y);
    }

    public void Show(bool bAnimated)
    {
        MainMenu mainMenu = (MainMenu)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        GameObject titleHolder = new GameObject("Title");
        titleHolder.AddComponent<GameObjectAnimator>();
        titleHolder.transform.parent = mainMenu.transform;
        titleHolder.transform.localPosition = new Vector3(0, 349.0f, TITLE_Z_VALUE);

        float gapBetweenLetters = 151.0f;
        int lettersCount = m_letters.Length;

        for (int i = 0; i != lettersCount; i++)
        {
            float letterXPosition = (lettersCount % 2 == 0) ? -0.5f * gapBetweenLetters + (i + 1 - lettersCount / 2) * gapBetweenLetters
                                                            : (i - lettersCount / 2) * gapBetweenLetters;
            DrawLetter(bAnimated, m_letters[i], new Vector2(letterXPosition, 0), titleHolder);
        }
        //letterObject = DrawLetter(bAnimated, m_letters[1], new Vector2(394.0f, 0));
        //letterObject = DrawLetter(bAnimated, m_letters[2], new Vector2(394.0f, 0));
        //letterObject = DrawLetter(bAnimated, m_letters[3], new Vector2(394.0f, 0));
        //letterObject = DrawLetter(bAnimated, m_letters[4], new Vector2(394.0f, 0));
        //letterObject = DrawLetter(bAnimated, m_letters[5], new Vector2(394.0f, 0));
        //letterObject = DrawLetter(bAnimated, m_letters[6], new Vector2(394.0f, 0));
        //letterObject = DrawLetter(bAnimated, m_letters[7], new Vector2(394.0f, 0));
    }

    private TitleLetter ParseAndBuildLetter(char value)
    {
        float letterScale = 200.0f;

        string levelFilename = "Title/letter_" + value;
        Object levelObjectFile = Resources.Load(levelFilename);

        if (levelObjectFile == null)
            return null;

        TextAsset levelFile = (TextAsset)levelObjectFile;

        XMLParser xmlParser = new XMLParser();
        XMLNode rootNode = xmlParser.Parse(levelFile.text);

        XMLNode letterNode = rootNode.GetNode("letter>0");
        string letterValue = letterNode.GetValue("@value");

        TitleLetter letter = new TitleLetter();

        //Parse contour
        XMLNodeList contourPointsNodeList = letterNode.GetNodeList("contour>0>point");
        int iVertexIndex = 1;
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

            TitleLetterVertex letterVertex = new TitleLetterVertex(iVertexIndex, contourPointX, contourPointY);
            letterVertex.AddNeighbor((iVertexIndex == 1) ? contourPointsNodeList.Count : iVertexIndex - 1); //add previous point in contour as neighbor
            letterVertex.AddNeighbor((iVertexIndex == contourPointsNodeList.Count) ? 1 : iVertexIndex); //add next point in contour as neighbor
            letter.Add(letterVertex);
            iVertexIndex++;
        }

        //Parse inner points
        XMLNodeList innerPointsNodeList = letterNode.GetNodeList("innerPoints>0>point");
        foreach (XMLNode innerPointNode in innerPointsNodeList)
        {
            string strInnerPointX = innerPointNode.GetValue("@x");
            string strInnerPointY = innerPointNode.GetValue("@y");

            float innerPointX = float.Parse(strInnerPointX);
            float innerPointY = float.Parse(strInnerPointY);

            //transform the coordinates of the letter vertices so the origin is at the center of the letter (x et y coordinates are between 0 and 1000)
            innerPointX -= 500.0f;
            innerPointY = 1000 - innerPointY; //y-axis is reversed in photoshop
            innerPointY -= 500.0f;

            //normalize coordinates by dividing by 1000 and scale them to fit the correct letter size
            innerPointX *= (letterScale / 1000.0f);
            innerPointY *= (letterScale / 1000.0f);

            TitleLetterVertex letterVertex = new TitleLetterVertex(iVertexIndex, innerPointX, innerPointY);
            letter.Add(letterVertex);
            iVertexIndex++;
        }

        //Add missing neighbors
        XMLNodeList nodesList = letterNode.GetNodeList("network>0>node");
        foreach (XMLNode node in nodesList)
        {
            string strNodeIndex = node.GetValue("@index");

            int nodeIndex = int.Parse(strNodeIndex);

            TitleLetterVertex vertex = letter.GetVertexForIndex(nodeIndex);

            XMLNodeList neighborsNodeList = node.GetNodeList("neighbor");
            foreach (XMLNode neighborNode in neighborsNodeList)
            {
                string strNeighborIndex = neighborNode.GetValue("@index");

                //Add this vertex as a neighbor for current vertex
                int neighborIndex = int.Parse(strNeighborIndex);
                vertex.AddNeighbor(neighborIndex);

                //Add current vertex as a neighbor for this vertex
                TitleLetterVertex neighborVertex = letter.GetVertexForIndex(neighborIndex);
                neighborVertex.AddNeighbor(nodeIndex);
            }
        }

        return letter;
    }

    public void DrawLetter(bool bAnimated, TitleLetter letter, Vector2 position, GameObject titleHolder)
    {
        GameObject letterObject = new GameObject("letter_" + letter.m_value);
        letterObject.transform.parent = titleHolder.transform;
        letterObject.transform.localPosition = GeometryUtils.BuildVector3FromVector2(position, 0);

        for (int i = 0; i != letter.Count; i++)
        {
            TitleLetterVertex vertex = letter[i];
            List<int> neighborsIndices = vertex.m_neighbors;
            for (int iNeighborIdx = 0; iNeighborIdx != neighborsIndices.Count; iNeighborIdx++)
            {
                int neighborIndex = neighborsIndices[iNeighborIdx];
                if (!vertex.IsLinkedToNeighbor(neighborIndex))
                {
                    TitleLetterVertex neighborVertex = letter.GetVertexForIndex(neighborIndex);

                    vertex.m_linkedNeighbors.Add(neighborIndex);
                    neighborVertex.m_linkedNeighbors.Add(vertex.m_index);

                    GameObject clonedRoundedSegmentObject = (GameObject)Instantiate(m_letterSegmentPfb);
                    clonedRoundedSegmentObject.transform.parent = letterObject.transform;
                    SimplifiedRoundedSegment roundedSegment = clonedRoundedSegmentObject.GetComponent<SimplifiedRoundedSegment>();
                    roundedSegment.Build(vertex.m_position, neighborVertex.m_position, 1.5f, m_clonedMaterial, Color.white);
                }
            }
        }
    }
}

