using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoadManager
{
    public static void SaveCurState(GameManager gameManager, out string savedataPath)
    {
        // 데이터 패스 설정
        savedataPath = Application.persistentDataPath + "/test.sav"; // 임시

        // 저장
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(savedataPath, FileMode.Create);

        GameSavedata data = new GameSavedata(gameManager);

        bf.Serialize(stream, data);
        stream.Close();
    }

    public static GameSavedata LoadFromSave(out string savedataPath)
    {
        // 데이터 패스 설정
        savedataPath = Application.persistentDataPath + "/test.sav";

        // 불러오기
        if (File.Exists(savedataPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(savedataPath, FileMode.Open);

            GameSavedata data = bf.Deserialize(stream) as GameSavedata;

            stream.Close();
            return data;
        }
        else
        {
            return null;
        }
    }
}

[Serializable]
public class GameSavedata
{
    public int playerGold;
    public int playerPopularity;

    public GameSavedata(GameManager input)
    {
        playerGold = input.playerGold;
        playerPopularity = input.playerPopularity;
    }
}
