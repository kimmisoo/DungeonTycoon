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
    public List<StructureData> structureDatas;

    public GameSavedata()
    {
        GameManager gameManager = GameManager.Instance;
        StructureManager structureManager = StructureManager.Instance;

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

        // 건물 저장.
        structureDatas = new List<StructureData>();

        for(int i = 0; i< structureManager.structures.Count; i++)
        {
            structureDatas.Add(new StructureData(structureManager.structures[i]));
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

    // String으로 고쳐야 할 수 있음. 물어보기.
    // 혹은 레이어 번호를 같이 받아야할 수 있음.
    public int destinationTile;
    public int curTile;

    // inactive List가 있으니 거기 넣을건지 이야기해봐야함.
    public bool isActive;

    // 로드용(참조용) 인덱스
    public int index;

    // 어떤 프리팹을 로드하는지는 아직 안 적어놨음.

    public TravelerData(GameObject input)
    {
        Traveler inputTraveler = input.GetComponent<Traveler>();
        isActive = input.activeSelf;
        position = new Vector3Data(input.transform.position);
//        rotation = new Vector3Data(input.transform.rotation.eulerAngles);
        //destinationStructure = destinationStructure;
        state = inputTraveler.curState;
        destinationTile = inputTraveler.GetDestinationTileSave();
        curTile = inputTraveler.GetCurTileSave();
        index = inputTraveler.index;
    }
}

[Serializable]
public class TileData
{
    public Vector3Data position;
    public int x, y;
    public string tileName; // 이름.
    public int prefabInfo;
    public int layerNum;

    public bool isPassable;
    public bool isStructed;
    public bool isNonTile;
    public bool isBuildable;
    public bool isMonsterArea;

    public int structureIndex;

    public TileData(GameObject input)
    {
        Tile inputTile = input.GetComponent<Tile>();
        position = new Vector3Data(input.transform.position);
        x = inputTile.x;
        y = inputTile.y;
        tileName = input.name;
        prefabInfo = inputTile.prefabInfo;
        layerNum = inputTile.GetLayerNum();

        isPassable = inputTile.GetPassable();
        isStructed = inputTile.GetStructed();
        isNonTile = inputTile.GetNonTile();
        isBuildable = inputTile.GetBuildable();
        isMonsterArea = inputTile.GetMonsterArea();

        Structure tileStructure = inputTile.GetStructure();
        if (tileStructure != null)
            structureIndex = tileStructure.structureIndex;
        else
            structureIndex = -1;
    }
}

[Serializable]
public class StructureData
{
    // 마찬가지로 바꿔야할 수 있음.
    public int pointTile;
    public int[,] extent;
    public List<int> entranceList;
    public int entCount;
    public int sitInCount;

    public Queue<int> curUsingQueue;
    public Queue<int> curWaitingQueue;

    public string structureCategory;
    public int structureNumber;

    public StructureData(Structure input)
    {
        pointTile = int.Parse(input.point.name);
        extent = input.extent;
        for (int i = 0; i < input.entrance.Count; i++)
            entranceList.Add(int.Parse(input.entrance[i].name));
        entCount = input.entCount;
        sitInCount = input.sitInCount;

        Traveler[] tempArr = input.GetCurUsingQueAsArray();
        for (int i = 0; i < tempArr.Length; i++)
            curUsingQueue.Enqueue(tempArr[i].index);
        tempArr = input.GetCurWatingQueAsArray();
        for (int i = 0; i < tempArr.Length; i++)
            curWaitingQueue.Enqueue(tempArr[i].index);
    }
}
