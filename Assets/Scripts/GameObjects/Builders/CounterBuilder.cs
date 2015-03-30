using UnityEngine;

public class CounterBuilder : MonoBehaviour
{
    public void Build()
    {
        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        Level currentLevel = levelManager.m_currentLevel;

        int numberOfActions = 4; //TODO retrieve number of actions from currentLevel object
    }
}

