using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    private static SaveLoadManager _instance;

    public static SaveLoadManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SaveLoadManager>();

                if(_instance == null)
                {
                    GameObject container = new GameObject("SaveLoadManager");
                    _instance = container.AddComponent<SaveLoadManager>();
                }
            }

            return _instance;
        }
    }
    string savedataPath;
    GameSavedata savedata;

    private void Start()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("이미 있음." + this.gameObject.GetInstanceID());
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // 현 상태 저장
    public void Save()
    {
        // 데이터 패스 설정
        savedataPath = Application.persistentDataPath + "/test.sav"; // 임시
        string sceneName = SceneManager.GetActiveScene().name; 

        // 저장
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(savedataPath, FileMode.Create);

        GameSavedata data = new GameSavedata(sceneName);

        bf.Serialize(stream, data);
        stream.Close();
    }

    public void Load()
    {
        LoadFromSave();
        LoadScene(savedata.sceneName);
        //InstantiateFromSave();
    }

    // 세이브 파일에서 로드
    public GameSavedata LoadFromSave()
    {
        // 데이터 패스 설정
        savedataPath = Application.persistentDataPath + "/test.sav"; // 임시

        // 불러오기
        if (File.Exists(savedataPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(savedataPath, FileMode.Open);

            savedata = bf.Deserialize(stream) as GameSavedata;

            stream.Close();
            return savedata;
        }
        else
        {
            return null;
        }
    }

    public void InstantiateFromSave()
    {
        if (savedata == null)
            return;

        // 세이브에서 받아서 결과값 대입
        if (savedata != null)
        {
            GameManager.Instance.LoadPlayerData(savedata);

            GameManager.Instance.LoadTileMap(savedata);
            GameManager.Instance.LoadTravelerList(savedata);

            StructureManager.Instance.LoadStructuresFromSave(savedata);

            TileMapGenerator.Instance.SetTileStructure(savedata);

            Debug.Log("불러오기 성공");

            savedata = null;
        }
        // 실패 메시지 출력
        else
        {
            Debug.Log("불러오기 실패");
        }
    }

    // Scene 로드
    public void LoadScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

[Serializable]
public class GameSavedata
{
    public string sceneName;

    public int playerGold;
    public int playerPopularity;
    public Vector3Data cameraPosition;
    public float cameraSize;

    public List<TravelerData> travelerDatas;
    public List<TileData> tileDatas;
    public List<StructureData> structureDatas;

    public GameSavedata(string sceneNameInput)
    {
        GameManager gameManager = GameManager.Instance;
        StructureManager structureManager = StructureManager.Instance;

        sceneName = sceneNameInput;
        playerGold = gameManager.playerGold;
        playerPopularity = gameManager.playerPopularity;

        cameraPosition = new Vector3Data(Camera.main.transform.position);
        cameraSize = Camera.main.orthographicSize;
        Camera.main.GetComponent<InputManager>().isLoading = true;

        travelerDatas = new List<TravelerData>();

        // travelers 저장.
        for (int i = 0; i < gameManager.travelersEnabled.Count; i++)
        {
            travelerDatas.Add(new TravelerData(gameManager.travelersEnabled[i]));
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
    public SuperState superState;
    public State state;
    public Vector3Data position;
//    public Vector3Data rotation;
    public int destinationStructure;

    // String으로 고쳐야 할 수 있음. 물어보기.
    // 혹은 레이어 번호를 같이 받아야할 수 있음.
    public int destinationTile;
    public int curTile;
    // 임시
    public int curTileForMove;

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
        superState = inputTraveler.GetSuperState();
        state = inputTraveler.curState;
        destinationTile = inputTraveler.GetDestinationTileSave();
        curTile = inputTraveler.GetCurTileSave();
        curTileForMove = inputTraveler.GetCurTileForMove().GetChildNum();
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

    public bool isRoad;
    public bool isStructed;
    public bool isNonTile;
    public bool isBuildable;
    public bool isHuntingArea;

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

        isRoad = inputTile.GetRoad();
        isStructed = inputTile.GetStructed();
        isNonTile = inputTile.GetNonTile();
        isBuildable = inputTile.GetBuildingArea();
        isHuntingArea = inputTile.GetHuntingArea();

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
    public Vector3Data position;
    // 마찬가지로 바꿔야할 수 있음.
    public int pointTile;
    public int[,] extent;
    public List<int> entranceList;
    public int entCount;
    public int sitInCount;

    public Queue<int> curUsingQueue;
    public Queue<float> elapsedTimeQueue;
    public Queue<int> curWaitingQueue;

    public string structureCategory;
    public int structureNumber;
    public int structureIndex;

    public StructureData(Structure input)
    {
        position = new Vector3Data(input.gameObject.transform.position);
        entranceList = new List<int>();
        curUsingQueue = new Queue<int>();
        elapsedTimeQueue = new Queue<float>();
        curWaitingQueue = new Queue<int>();

        pointTile = int.Parse(input.point.gameObject.name);
        extent = input.extent;
        for (int i = 0; i < input.entrance.Count; i++)
        {
            Debug.Log("i : " + i + " entrance count : " + input.entrance.Count);
            
            entranceList.Add(int.Parse(input.entrance[i].gameObject.name));
        }
        entCount = input.entCount;
        sitInCount = input.sitInCount;

        // Queue에 집어넣는 순서가 맞는지 모르겠음. 아니라면, Arr에 Reverse()해주면 됨.
        Traveler[] tempArr = input.GetCurUsingQueueAsArray();
        for (int i = 0; i < tempArr.Length; i++)
            curUsingQueue.Enqueue(tempArr[i].index);
        tempArr = input.GetCurWaitingQueueAsArray();
        for (int i = 0; i < tempArr.Length; i++)
            curWaitingQueue.Enqueue(tempArr[i].index);

        float timeNow = Time.fixedTime;
		float[] timeArr = input.GetElapsedTimeQueueAsArray();/*input.GetEnteredTimeQueueAsArray();*///TravelerTimer에 elapsedTime 만들었음
		for (int i = 0; i < timeArr.Length; i++)
			elapsedTimeQueue.Enqueue(timeArr[i]);//elapsedTimeQueue.Enqueue(timeNow - timeArr[i]);//elapsedTime 직접

        structureCategory = input.structureCategory;
        structureNumber = input.structureNumber;
        structureIndex = input.structureIndex;
    }
}