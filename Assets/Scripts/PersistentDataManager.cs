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

    private void SaveLevelDataToFile(LevelData levelData)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = File.Open(Application.persistentDataPath + "/level_" + levelData.m_number + ".dat", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);

        bf.Serialize(fs, levelData);
        fs.Close();
    }

    private LevelData LoadLevelDataFromFile(int iLevelNumber)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = File.Open(Application.persistentDataPath + "/level_" + iLevelNumber + ".dat", FileMode.Open, FileAccess.Read, FileShare.None);
        return (LevelData) bf.Deserialize(fs);
    }
}

[Serializable]
public class LevelData
{
    public int m_number; //the absolute number of this level
    public bool m_done; //has this level been done successfully

    //TODO save hints and other stuff
}

