using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using SimpleJSON;


public class GameManager : MonoBehaviour
{
    public static GameManager _instance = null;

    #region PlayerStats
    #region 세이브!
    public int playerGold = 0;
    public int playerPopularity = 0;
    #endregion
    #endregion

    #region Tiles
    public TileMapGenerator tmg;

    #region 세이브!
    GameObject tileMap;
    #endregion

    bool isUI = false;
    bool isConstructing = false;
    public int enteringTimeMin = 1;
    public int enteringTimeMax = 5;
    public int enteringMax = 100;

    #endregion

    #region Characters
    #region 세이브!
    public List<GameObject> travelers;
    public List<GameObject> adventurers;
    public List<GameObject> specialAdventurers;
    public List<GameObject> inactiveTravelers;
    public List<GameObject> inactiveAdventurers;

    public int corporateNum = 1;
    public List<float> popular;
    #endregion

    public JSONNode items;
    private object lockObject = new object();
    private object lockObject2 = new object();
    #endregion

    #region SceneDatas
    string sceneName;
    JSONNode sceneData;

    int traveler_Max = 100;
    int adventurer_Max = 0;
    int specialAdventurer_Max = 0;
    int drink_Max = 0;
    int food_Max = 0;
    int lodge_Max = 0;
    int equipment_Max = 0;
    int tour_Max = 0;
    int convenience_Max = 0;
    int fun_Max = 0;
    int santuary_Max = 0;
    int rescue_Max = 0;
    int complete_Popularity = 0;

    int mapEntranceCount = 0;
    public int vertexCount = 0;
    WaitForSeconds wait;
    List<TileForMove> mapEntrance = new List<TileForMove>();

    WaitForSeconds countLogWait;
    #endregion

    public static GameManager Instance
    {
        // 싱글톤 사용
        get
        {
            if (_instance == null)
                Debug.Log("gameManager find Error!");
            return _instance;
        }
    }

    // Use this for initialization
    void Awake()
    {
        _instance = this;

        // 로드용. Scene 바뀌어도 이 오브젝트 유지함.
        DontDestroyOnLoad(this.gameObject);

        // Scene 이름 받기
        sceneName = SceneManager.GetActiveScene().name;

        // 아이템 목록 로드
        TextAsset itemText = Resources.Load<TextAsset>("Items/items");
        items = JSON.Parse(itemText.ToString());

        wait = new WaitForSeconds(0.11f);
        countLogWait = new WaitForSeconds(3.0f);
    }

    void Start()
    {
        // Scene 데이터 로드
        TextAsset sceneTxt = Resources.Load<TextAsset>("SceneData/scenedata");

        SetMap();
        sceneData = JSON.Parse(sceneTxt.ToString());
        setSceneData(sceneData);

        // Scene 정보 세팅 
        travelers = new List<GameObject>();
        adventurers = new List<GameObject>();
        specialAdventurers = new List<GameObject>();
        inactiveTravelers = new List<GameObject>();
        inactiveAdventurers = new List<GameObject>();

        // Scene별로 미리 정의된 관광객의 최대 수에 따라 생성
        for (int i = 0; i < traveler_Max; i++)
        {
            GameObject go = (GameObject)Resources.Load("CharacterPrefabs/Traveler_test");

            // 생성만 해놓고 비활성화
            go.SetActive(false);

            // List에 추가
            travelers.Add(Instantiate(go));
            go.transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);
            travelers[i].transform.parent = GameObject.FindGameObjectWithTag("Characters").transform;
//            Debug.Log("character instantiate - " + i);
        }

        StartCoroutine(TEnter());

        //StartCoroutine(GCcall());
        for (int i = 0; i < corporateNum; i++)
        {
            popular.Add(0);
        }
    }

    // 모험가 입장 코루틴
    // 이거에 i 값을 던져주면 활성화 문제는 해결됨.
    IEnumerator TEnter()
    {
        for (int i = 0; i < traveler_Max; i++)
        {
            //yield return wait;
            yield return null;
            travelers[i].SetActive(true);
        }
    }

    // 인기도 추가
    public void AddPop(int who, float amount)
    {
        popular[who] += amount;
    }

    // 타일맵 생성
    private void SetMap()
    {
        tileMap = tmg.GenerateMap("TileMap/" + sceneName);
    }

    // 타일맵 Get
    public TileMap GetMap()
    {
        return tileMap.GetComponent<TileMap>();
    }

    // 시간 배속 설정
    public void SetTimeScale(float ts)
    {
        Time.timeScale = ts;
    }

    // 골드 추가
    public void AddGold(int amount)
    {
        playerGold += amount;
    }

    // 플레이어 골드 Get
    public int GetPlayerGold()
    {
        return playerGold;
    }

    // Scene 데이터 설정
    public void setSceneData(JSONNode aData)
    {
        int sceneNumber = int.Parse(SceneManager.GetActiveScene().name);

#if DEBUG
        Debug.Log("Scenedata 읽어오기 디버그 시작");
        Debug.Log("현재 씬 : " + sceneNumber);
        Debug.Log(aData["scene"]);
        Debug.Log(aData["scene"][sceneNumber]);
        Debug.Log(aData["scene"][sceneNumber]["traveler_Max"].AsInt);
#endif

        traveler_Max = aData["scene"][sceneNumber]["traveler_Max"].AsInt;
        adventurer_Max = aData["scene"][sceneNumber]["adventurer_Max"].AsInt;
        complete_Popularity = aData["scene"][sceneNumber]["complete_Popularity"].AsInt;
        drink_Max = aData["scene"][sceneNumber]["buildable"]["drink"].AsInt;
        food_Max = aData["scene"][sceneNumber]["buildable"]["food"].AsInt;
        lodge_Max = aData["scene"][sceneNumber]["buildable"]["lodge"].AsInt;
        equipment_Max = aData["scene"][sceneNumber]["buildable"]["equipment"].AsInt;
        tour_Max = aData["scene"][sceneNumber]["buildable"]["tour"].AsInt;
        convenience_Max = aData["scene"][sceneNumber]["buildable"]["convenience"].AsInt;
        fun_Max = aData["scene"][sceneNumber]["buildable"]["fun"].AsInt;
        santuary_Max = aData["scene"][sceneNumber]["buildable"]["santuary"].AsInt;
        rescue_Max = aData["scene"][sceneNumber]["buildable"]["rescue"].AsInt;

        mapEntranceCount = aData["scene"][sceneNumber]["mapEntranceCount"].AsInt;
        TileLayer layer = tileMap.GetComponent<TileMap>().GetLayer(0).GetComponent<TileLayer>();
        int x = 0;
        int y = 0;
        for (int i = 0; i < mapEntranceCount; i++)
        {
            x = aData["scene"][sceneNumber]["mapEntrance"][i]["x"].AsInt;
            y = aData["scene"][sceneNumber]["mapEntrance"][i]["y"].AsInt;

            Debug.Log(x + "   " + y);
            mapEntrance.Add(layer.GetTileForMove(x * 2, y * 2));
            mapEntrance.Add(layer.GetTileForMove((x * 2) + 1, y * 2));
            mapEntrance.Add(layer.GetTileForMove(x * 2, (y * 2) + 1));
            mapEntrance.Add(layer.GetTileForMove((x * 2) + 1, (y * 2) + 1));
        }

        //traveler_Max = 1;
        //adventurer_Max = 1;
        //specialAdventurer_Max = 800;

    }

    // 입구 랜덤으로 찾기
    public Tile GetRandomEntrance()
    {
        int rand = Random.Range(0, mapEntranceCount);

        return mapEntrance[rand].GetParent();
    }

    // 뭔지 모르겠음
    public string getCurrentDateForSave()
    {
        return "";
    }

    // 게임 종료
    public void QuitGame()
    {
        Application.Quit();
    }

    // Scene 로드
    public void LoadScene(int sceneNum)
    {
        Application.LoadLevel(sceneNum);
    }
    public void LoadScene(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }

    // 모험가 스탯 리셋
    public Stat ResetTravelerStat(Traveler owner)
    {

        Stat stat = owner.stat;

        //owner Type 별로 race, job, name, wealth, gender 부여
        //stat 생성하여 owner.Init
        return null;
    }

    #region Save Load
    // 현재 상황 Save
    public void Save(int slot)
    {
        string savedataPath;

        SaveLoadManager.SaveCurState(out savedataPath);
        Debug.Log(savedataPath + " - 저장 성공");
    }

    // 세이브 파일에서 Load
    public void Load(int slot)
    {
        string savedataPath;
        // 세이브파일 읽기
        GameSavedata savedata = SaveLoadManager.LoadFromSave(out savedataPath);

        //        LoadScene(savedata.sceneName);
        // 세이브에서 받아서 결과값 대입
        if (savedata != null)
        {
            Debug.Log(savedataPath + " - 불러오기 성공");

            LoadPlayerData(savedata);
            LoadTravelerList(savedata);
        }
        // 실패 메시지 출력
        else
        {
            Debug.Log(savedataPath + " - 불러오기 실패");
        }
    }

    private void LoadPlayerData(GameSavedata savedata)
    {
        playerGold = savedata.playerGold;
        playerPopularity = savedata.playerPopularity;
    }

    // 일단 완성
    private void LoadTravelerList(GameSavedata savedata)
    {
        GameObject newObject;
        TravelerData inputTravelerData;
        Traveler newTraveler;
        GameObject tileMapLayer;

        // 할당 해제. 다른 Scene일 수도 있으니.
        foreach (GameObject traveler in travelers)
        {
            traveler.GetComponent<Traveler>().StopAllCoroutines();
            Destroy(traveler);
        }
        travelers.Clear();

        // Active 관련 요소는 이야기해보고 결정. 현재는 빠져있음.
        for (int i = 0; i < savedata.travelerDatas.Count; i++)
        {
            newObject = (GameObject)Resources.Load("CharacterPrefabs/Traveler_test");
            newObject.SetActive(false);

            inputTravelerData = savedata.travelerDatas[i];
            
            
            // List에 추가
            travelers.Add(Instantiate(newObject));

            newTraveler = travelers[i].GetComponent<Traveler>();

            travelers[i].transform.parent = GameObject.FindGameObjectWithTag("Characters").transform;
            Debug.Log("character instantiate - " + i);

            // 세이브 데이터에서 대입.
            travelers[i].SetActive(inputTravelerData.isActive);

            if (travelers[i].activeSelf)
            {
                travelers[i].transform.position = new Vector3(inputTravelerData.position.x, inputTravelerData.position.y, inputTravelerData.position.z);
                Debug.Log(i + " Active? " + travelers[i].activeSelf);
                newTraveler.curState = inputTravelerData.state;
                newTraveler.SetDestinationTileLoad(inputTravelerData.destinationTile);
                newTraveler.SetCurTileLoad(inputTravelerData.curTile);
            }

            // TEnter 관련은 아직 저장안함
            // 현 상태에서, 모험가 입장도중 저장하면 다른 모험가가 들어오지 않을 가능성 있음.
            // 어디까지 활성화했나 보고 그 뒤에꺼 이어가면 문제 X.
        }
    }

    private void LoadTileMap(GameSavedata savedata)
    {
        // Destroy 한 다음 타일맵 제네레이터로 넘겨주는게 나아보임.
    }

    public GameObject GetTileLayer()
    {
        return tileMap.transform.GetChild(0).gameObject;
    }
    #endregion
}
