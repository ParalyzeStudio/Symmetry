using UnityEngine;
using System.Collections.Generic;

/**
 * Represents a group of symmetries the player has stacked
 * **/
//public class SymmetryGroup : List<Axis>
//{
//    /**
//     * Calculates the symmetric element of every axis in this group.
//     * Add every symmetrized axis to the group
//     * **/
//    public void SymmetrizeByAxis(Symmetrizer symmetrizer)
//    {
//        int elementsCount = this.Count; //store the count once so we do not loop over new elements that are added at the end of this list
//        for (int i = 0; i != elementsCount; i++)
//        {
//            //Debug.Log(">>>>Calculating symmetric axis of");
//            //Debug.Log(" A:" + this[i].m_pointA + " B:" + this[i].m_pointB);
//            //Debug.Log("about");
//            //Debug.Log(" A:" + symmetrizer.m_axis.m_pointA + " B:" + symmetrizer.m_axis.m_pointB);
//            Axis symmetrizedAxis = symmetrizer.CalculateSymmetricAxisByPoint(this[i]);
//            if (symmetrizedAxis != null)
//            {
//                symmetrizedAxis.CreateStrip(false);
//                Add(symmetrizedAxis);
//            }
//        }
//    }
//}

/**
 * Class that holds symmetries the player has stacked. Each element of the stack is a reference 
 * to a group of symmetries that may grow when the player unstack some parent group of symmetries
 * Use a List to make this stack as we want to be able to traverse the stack and modify elements
 * **/
//public class SymmetryStack
//{
//    private List<SymmetryGroup> m_stack;
//    private GameScene m_gameScene;

//    public SymmetryStack()
//    {
//        m_stack = new List<SymmetryGroup>();
//    }

//    /**
//     * Add a symmetry to the stack
//     * **/
//    public void StackSymmetry(Axis axis)
//    {
//        SymmetryGroup symmetryGroup = new SymmetryGroup();

//        symmetryGroup.Add(axis);
//        m_stack.Add(symmetryGroup);
//    }

//    /**
//     * Unstack the last entered element
//     * **/
//    public void UnstackSymmetryGroup()
//    {
//        //pop the last element
//        SymmetryGroup symmetryGroup = m_stack[m_stack.Count - 1];
//        m_stack.Remove(symmetryGroup);

//        //Apply symmetry process for every axis in the popped group
//        for (int i = 0; i != symmetryGroup.Count; i++)
//        {
//            Symmetrizer symmetrizer = symmetryGroup[i].GetComponent<Symmetrizer>();

//            QueuedThreadedJobsManager threadedJobsManager = GetGameScene().GetQueuedThreadedJobsManager();
//            threadedJobsManager.AddJob(new ThreadedJob
//                                            (
//                                                new ThreadedJob.ThreadFunction(symmetrizer.SymmetrizeShapes),
//                                                null,
//                                                new ThreadedJob.ThreadFunction(symmetrizer.OnSymmetrizingShapesDone)
//                                            )
//                                      );


//            //Apply symmetry about this axis on all symmetry groups remaining in the stack (i.e with a group number less than the popped group)
//            for (int j = 0; j != m_stack.Count; j++)
//            {
//                SymmetryGroup stackedSymmetryGroup = m_stack[j];
//                stackedSymmetryGroup.SymmetrizeByAxis(symmetrizer);
//            }        
//        }        
//    }

//    private GameScene GetGameScene()
//    {
//        if (m_gameScene == null)
//            m_gameScene = (GameScene)(GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>().m_currentScene);

//        return m_gameScene;
//    }
//}