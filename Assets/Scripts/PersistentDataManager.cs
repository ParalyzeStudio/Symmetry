using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PersistentDataManager : MonoBehaviour
{
    public const string MUSIC_ON = "music_on";
    public const string SOUND_ON = "sound_on";

    public void SetMusicStatus(bool bMusicOn)
    {
        PlayerPrefs.SetInt(MUSIC_ON, bMusicOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool IsMusicOn()
    {
        int musicStatus = PlayerPrefs.GetInt(MUSIC_ON, 1);
        return (musicStatus == 1);
    }

    public void SetSoundActive(bool bSoundOn)
    {
        PlayerPrefs.SetInt(SOUND_ON, bSoundOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool IsSoundOn()
    {
        int soundStatus = PlayerPrefs.GetInt(SOUND_ON, 1);
        return (soundStatus == 1);
    }



    public void SetLevelDone(int iLevelNumber)
    {
        LevelData levelData = LoadLevelDataFromFile(iLevelNumber);
        if (!levelData.m_done)
        {
            levelData.m_done = true;
            SaveLevelDataToFile(levelData);
        }
    }

    /**
     * Creates all data that will hold information per level and fill it with default values
     * **/
    public void CreateAndInitAllLevelData()
    {
        int totalLevelCount = LevelManager.CHAPTERS_COUNT * LevelManager.LEVELS_PER_CHAPTER;

        //try to open every level data file, each time the file is not found create it with default level data
        for (int iLevelNumber = 1; iLevelNumber != totalLevelCount + 1; iLevelNumber++)
        {
            try
            {
                FileStream fs = File.Open(Application.persistentDataPath + "/level_" + iLevelNumber + ".dat", FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (FileNotFoundException)
            {
                Debug.Log("CREATING LEVEL DATA FILE " + iLevelNumber);
                LevelData defaultLevelData = new LevelData(iLevelNumber);
                SaveLevelDataToFile(defaultLevelData);
            }
        }
    }

    /**
     * Resets all data holding information per level to default values
     * **/
    public void ResetAllLevelData()
    {
        int totalLevelCount = LevelManager.CHAPTERS_COUNT * LevelManager.LEVELS_PER_CHAPTER;
        for (int i = 0; i != totalLevelCount; i++)
        {
            int iAbsoluteLevelNumber = i + 1;
            LevelData defaultLevelData = new LevelData(iAbsoluteLevelNumber);
            SaveLevelDataToFile(defaultLevelData);
        }
    }

    /**
     * Saves level data to the associated file
     * **/
    private bool SaveLevelDataToFile(LevelData levelData)
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream fs = null;
        try
        {
            fs = File.Open(Application.persistentDataPath + "/level_" + levelData.m_absNumber + ".dat", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        }
        catch (Exception)
        {
            return false; //failed to open or create the file
        }

        bf.Serialize(fs, levelData);
        fs.Close();

        return true;
    }

    /**
     * Loads data from file holding information on level with absolute number passed as parameter
     * **/
    private LevelData LoadLevelDataFromFile(int iAbsoluteLevelNumber)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = null;
        LevelData levelData = null;
        try
        {
            fs = File.Open(Application.persistentDataPath + "/level_" + iAbsoluteLevelNumber + ".dat", FileMode.Open, FileAccess.Read, FileShare.None);
        }
        catch (FileNotFoundException)
        {
            levelData = new LevelData(iAbsoluteLevelNumber);
            fs = File.Create(Application.persistentDataPath + "/level_" + iAbsoluteLevelNumber + ".dat");
            fs.Close();
            return levelData;
        }
        levelData = (LevelData)bf.Deserialize(fs);
        fs.Close();
        return levelData;
    }
}

[Serializable]
public class LevelData
{
    public int m_absNumber; //the absolute number of this level
    public bool m_done; //has this level been done successfully

    //TODO save hints and other stuff

    public LevelData(int iLevelNumber)
    {
        m_absNumber = iLevelNumber;
        m_done = false;
    }

    public LevelData(int iLevelNumber, bool bDone)
    {
        m_absNumber = iLevelNumber;
        m_done = bDone;
    }
}

