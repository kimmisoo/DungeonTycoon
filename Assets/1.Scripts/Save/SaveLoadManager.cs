using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoadManager
{
    // 현 상태 저장
    public static void SaveCurState(out string savedataPath)
    {
        // 데이터 패스 설정
        savedataPath = Application.persistentDataPath + "/test.sav"; // 임시

        // 저장
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(savedataPath, FileMode.Create);

        GameSavedata data = new GameSavedata();

        bf.Serialize(stream, data);
        stream.Close();
    }

    // 세이브 파일에서 로드
    public static GameSavedata LoadFromSave(out string savedataPath)
    {
        // 데이터 패스 설정
        savedataPath = Application.persistentDataPath + "/test.sav"; // 임시

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

    public GameSavedata()
    {
        GameManager gameManager = GameManager.Instance;

        playerGold = gameManager.playerGold;
        playerPopularity = gameManager.playerPopularity;
    }
}

[Serializable]
public class TravelerData
{
}
