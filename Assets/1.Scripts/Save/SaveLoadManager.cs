using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

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
    public string sceneName;
    public int playerGold;
    public int playerPopularity;

    public List<TravelerData> travelerDatas;
    public List<TileData> tileDatas;

    public GameSavedata()
    {
        GameManager gameManager = GameManager.Instance;

        sceneName = SceneManager.GetActiveScene().name;
        playerGold = gameManager.playerGold;
        playerPopularity = gameManager.playerPopularity;

        travelerDatas = new List<TravelerData>();

        // travelers 저장.
        for (int i = 0; i < gameManager.travelers.Count; i++)
        {
            travelerDatas.Add(new TravelerData(gameManager.travelers[i]));
        }

        // 타일맵 저장.
        tileDatas = new List<TileData>();
        GameObject tileLayer = gameManager.GetTileLayer();

        int tileCount = tileLayer.transform.childCount;
        
        for (int i = 0; i < tileCount; i++)
        {
            tileDatas.Add(new TileData(tileLayer.transform.GetChild(i).gameObject));
        }
    }
}

[Serializable]
public class Vector3Data
{
    public float x, y, z;

    public Vector3Data(Vector3 input)
    {
        x = input.x;
        y = input.y;
        z = input.z;
    }
}


[Serializable]
public class TravelerData
{
    public State state;
    public Vector3Data position;
//    public Vector3Data rotation;
    public int destinationStructure;
    public int destinationTile;
    public int curTile;

    // inactive List가 있으니 거기 넣을건지 이야기해봐야함.
    public bool isActive;

    // 어떤 프리팹을 로드하는지는 아직 안 적어놨음.

    public TravelerData(GameObject input)
    {
        isActive = input.activeSelf;
        position = new Vector3Data(input.transform.position);
//        rotation = new Vector3Data(input.transform.rotation.eulerAngles);
        //destinationStructure = destinationStructure;
        state = input.GetComponent<Traveler>().curState;
        destinationTile = input.GetComponent<Traveler>().GetDestinationTileSave();
        curTile = input.GetComponent<Traveler>().GetCurTileSave();
    }
}

[Serializable]
public class TileData
{
    public Vector3Data position;
    public int x, y;
    public int tileNum; // 이름 겸 인덱스.
    public int prefabInfo;

    public TileData(GameObject input)
    {
        position = new Vector3Data(input.transform.position);
        x = input.GetComponent<Tile>().x;
        y = input.GetComponent<Tile>().y;
        tileNum = int.Parse(input.name);
        prefabInfo = input.GetComponent<Tile>().prefabInfo;
    }
}
