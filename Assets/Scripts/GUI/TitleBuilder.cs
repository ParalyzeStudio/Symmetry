using UnityEngine;
using System.Collections.Generic;

public class TitleBuilder : MonoBehaviour
{
    public const float DEFAULT_LETTER_SEGMENT_THICKNESS = 1.5f;

    public Material m_titleLetterSegmentMaterial;
    public GameObject m_titleLetterPfb;

    private TitleLetter[] m_letters;

    //variables to handle the drawing of letters with some delay
    private bool m_drawingLettersWithDelay;
    private float m_drawLettersDelay;
    private float m_drawLettersElapsedTime;

    public void Build()
    {
        //Set the unique instance of material for all segments that are going to be rendered
        Material clonedMaterial = (Material)Instantiate(m_titleLetterSegmentMaterial);

        //Title is SYMMETRY
        int lettersCount = 8;
        m_letters = new TitleLetter[lettersCount];

        TitleLetter S = ParseAndBuildLetter('S', clonedMaterial);
        TitleLetter Y = ParseAndBuildLetter('Y', clonedMaterial);
        TitleLetter M = ParseAndBuildLetter('M', clonedMaterial);
        TitleLetter E = ParseAndBuildLetter('E', clonedMaterial);
        TitleLetter T = ParseAndBuildLetter('T', clonedMaterial);
        TitleLetter R = ParseAndBuildLetter('R', clonedMaterial);

        m_letters[0] = S;
        m_letters[1] = Y;
        m_letters[2] = M;
        m_letters[3] = Instantiate(M); //create a clone of M
        m_letters[4] = E;
        m_letters[5] = T;
        m_letters[6] = R;
        m_letters[7] = Instantiate(Y); //create a clone of Y

        for (int i = 0; i != lettersCount; i++)
        {
            m_letters[i].gameObject.transform.parent = this.gameObject.transform;
        }
    }

    public void Show(bool bAnimated, float fDelay)
    {
        if (bAnimated && fDelay > 0)
        {
            m_drawingLettersWithDelay = true;
            m_drawLettersDelay = fDelay;
            return;
        }

        MainMenu mainMenu = (MainMenu)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;

        float gapBetweenLetters = 40.0f;
        int lettersCount = m_letters.Length;

        //calculate first the width of the whole title
        float titleWidth = 0;
        titleWidth += (lettersCount - 1) * gapBetweenLetters;
        for (int i = 0; i != lettersCount; i++)
        {
            titleWidth += m_letters[i].m_width;
        }

        //Then draw letters at correct position
        float previousLetterXPosition = 0;
        for (int i = 0; i != lettersCount; i++)
        {
            TitleLetter letter = m_letters[i];
            float previousLetterWidth = (i == 0) ? 0 : m_letters[i - 1].m_width;
            float letterXPosition = previousLetterXPosition + 0.5f * previousLetterWidth + 0.5f * letter.m_width + gapBetweenLetters;

            previousLetterXPosition = letterXPosition;

            letterXPosition -= 0.5f * titleWidth;
            DrawLetter(bAnimated, letter, new Vector2(letterXPosition, 0));           
        }
    }

    private TitleLetter ParseAndBuildLetter(char value, Material material)
    {
        float letterScale = 250.0f;

        string levelFilename = "Title/letter_" + value;
        Object levelObjectFile = Resources.Load(levelFilename);

        if (levelObjectFile == null)
            return null;

        TextAsset levelFile = (TextAsset)levelObjectFile;

        XMLParser xmlParser = new XMLParser();
        XMLNode rootNode = xmlParser.Parse(levelFile.text);

        XMLNode letterNode = rootNode.GetNode("letter>0");
        string strLetterValue = letterNode.GetValue("@value");
        string strLetterWidth = letterNode.GetValue("@width");

        char letterValue = strLetterValue.ToCharArray()[0];
        float letterWidth = float.Parse(strLetterWidth) * (letterScale / 1000.0f); ;

        GameObject letterObject = (GameObject)Instantiate(m_titleLetterPfb);
        letterObject.transform.parent = this.gameObject.transform;
        TitleLetter letter = letterObject.GetComponent<TitleLetter>();
        letter.Init(letterValue, letterWidth, material);

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
            letterVertex.m_parentLetter = letter;

            letter.m_vertices.Add(letterVertex);            
            iVertexIndex++;
        }

        //Parse inner points
        XMLNodeList innerPointsNodeList = letterNode.GetNodeList("innerPoints>0>point");
        if (innerPointsNodeList != null)
        {
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
                letterVertex.m_parentLetter = letter;

                letter.m_vertices.Add(letterVertex);
                iVertexIndex++;
            }
        }

        //Add missing neighbors
        XMLNodeList nodesList = letterNode.GetNodeList("network>0>node");
        if (nodesList != null)
        {
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
        }

        //parse 'spread' nodes
        XMLNodeList spreadVerticesNodeList = letterNode.GetNodeList("spread>0>vertex");
        if (spreadVerticesNodeList != null)
        {
            foreach (XMLNode spreadVertexNode in spreadVerticesNodeList)
            {
                string strVertexIndex = spreadVertexNode.GetValue("@index");
                int vertexIndex = int.Parse(strVertexIndex);

                letter.m_spreadVertices.Add(vertexIndex);
            }
        }

        return letter;
    }

    public void DrawLetter(bool bAnimated, TitleLetter letter, Vector2 position)
    {
        letter.gameObject.transform.localPosition = GeometryUtils.BuildVector3FromVector2(position, 0);

        if (bAnimated)
        {
            List<int> spreadVertices = letter.m_spreadVertices;
            for (int iVertexIdx = 0; iVertexIdx != spreadVertices.Count; iVertexIdx++)
            {
                TitleLetterVertex spreadVertex = letter.GetVertexForIndex(spreadVertices[iVertexIdx]);
                spreadVertex.SpreadToNeighbors();
            }
        }
        else //simply draw all segments at once
        {
            for (int i = 0; i != letter.m_vertices.Count; i++)
            {
                TitleLetterVertex vertex = letter.m_vertices[i];
                List<int> neighborsIndices = vertex.m_neighbors;
                for (int iNeighborIdx = 0; iNeighborIdx != neighborsIndices.Count; iNeighborIdx++)
                {
                    int neighborIndex = neighborsIndices[iNeighborIdx];
                    if (!vertex.IsLinkedToNeighbor(neighborIndex))
                    {
                        TitleLetterVertex neighborVertex = letter.GetVertexForIndex(neighborIndex);
                        vertex.LinkToNeighbor(neighborIndex);
                        letter.BuildSegmentBetweenVertices(vertex, neighborVertex, neighborVertex.m_position);
                    }
                }
            }
        }
    }

    protected virtual void Update()
    {
        if (m_drawingLettersWithDelay)
        {
            float dt = Time.deltaTime;
        
            bool inDelay = (m_drawLettersElapsedTime < m_drawLettersDelay);
            m_drawLettersElapsedTime += dt;
            if (m_drawLettersElapsedTime >= m_drawLettersDelay) //delay is passed
            {
                Show(true, 0);
                m_drawingLettersWithDelay = false;
            }
        }
    }
}

