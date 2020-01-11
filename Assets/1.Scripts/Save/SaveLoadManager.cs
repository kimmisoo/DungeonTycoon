using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoadManager
{
    public static void SaveCurState(List<GameObject> travler)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + "/player.sav", FileMode.Create);

        CurPlayingData data = new CurPlayingData(travler);

        bf.Serialize(stream, data);
        stream.Close();
    }

    public static CurPlayingData LoadCurState()
    {
        if (File.Exists(Application.persistentDataPath + "/player.sav"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + "/player.sav", FileMode.Open);

            CurPlayingData data = bf.Deserialize(stream) as CurPlayingData;

            stream.Close();
            return data;
        }
        else
        {
            Debug.Log("파일 없음");
            return null;
        }
    }
}

[Serializable]
public class CurPlayingData
{
    public List<GameObject> travelers;

    public CurPlayingData(List<GameObject> input)
    {
        travelers = input;
    }
}
