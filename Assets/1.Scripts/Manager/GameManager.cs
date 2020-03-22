#define DEBUG_SAVELOAD

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using SimpleJSON;
using UnityEngine.Events;


public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;

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
    public List<GameObject> adventurersEnabled;
    public List<GameObject> specialAdventurers;
    public List<GameObject> inactiveTravelers;
    public List<GameObject> adventurersDisabled;

    public Queue<GameObject> advEnterQ;

    public int corporateNum = 1;
    public List<float> popular;
    #endregion
    public JSONNode items;
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

    // 사냥터 정보
    int huntingAreaCount = 0;
    // 사냥터 개방에 따라 필요해질 데이터 저장
    List<ProgressInformation> progressInformations;

    JSONNode advStatData;

    private int CurAdvMaxPerHuntingArea
    {
        get
        {
            int total = 0;
            for (int i = 0; i <= HuntingAreaManager.Instance.ActiveIndex; i++)
            {
                total += progressInformations[i].guestCapacity;
            }

            return total / ((HuntingAreaManager.Instance.ActiveIndex + 1) / 2); // Int연산인데 버려지는 건? 버려지면 나머지는 traveler에서 보충해도 되긴함.
        }
    }


    int mapEntranceCount = 0;
    public int vertexCount = 0;
    WaitForSeconds wait;
    List<TileForMove> mapEntrance = new List<TileForMove>();

    WaitForSeconds countLogWait;
    #endregion

    #region Initialization
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

        // Scene 이름 받기
        sceneName = SceneManager.GetActiveScene().name;

        // 아이템 목록 로드
        TextAsset itemText = Resources.Load<TextAsset>("Items/items");
        items = JSON.Parse(itemText.ToString());

        // Adventurer 데이터 로드(레벨에 따른 스탯 등)
        TextAsset advStatTxt = Resources.Load<TextAsset>("Characters/adventurerstat");
        advStatData = JSON.Parse(advStatTxt.ToString());

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
        adventurersEnabled = new List<GameObject>();
        specialAdventurers = new List<GameObject>();
        inactiveTravelers = new List<GameObject>();
        adventurersDisabled = new List<GameObject>();

        advEnterQ = new Queue<GameObject>();

        progressInformations = new List<ProgressInformation>();

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
            travelers[i].GetComponent<Traveler>().index = i;
            // Debug.Log("character instantiate - " + i);
        }

        StartCoroutine(TEnter());

        for (int i = 0; i < adventurer_Max; i++)
        {
            GameObject go = (GameObject)Resources.Load("CharacterPrefabs/Adventurer_test");

            // 생성만 해놓고 비활성화
            go.SetActive(false);

            // List에 추가
            adventurersDisabled.Add(Instantiate(go));
            go.transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);
            adventurersDisabled[i].transform.parent = GameObject.FindGameObjectWithTag("Characters").transform;
            adventurersDisabled[i].GetComponent<Adventurer>().index = i;
            // Debug.Log("character instantiate - " + i);
        }
        StartCoroutine(AdvEnter());

        //StartCoroutine(GCcall());
        for (int i = 0; i < corporateNum; i++)
        {
            popular.Add(0);
        }

        StartCoroutine(LoadIfNeeded());
    }

    IEnumerator LoadIfNeeded()
    {
        yield return null;

        SaveLoadManager.Instance.InstantiateFromSave();
    }
    #endregion

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

    IEnumerator AdvEnter()
    {
        GameObject temp;
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 10.0f)); // 임시 수치
            if (advEnterQ.Count > 0)
            {
                temp = advEnterQ.Dequeue();
                // 어떻게 disabled에서 찾나? 역시 Dict?
            }
        }
        
    }

    private void GenAndEnqueueSingleAdventurer(int minLevel, int maxLevel)
    {
        GameObject temp = adventurersDisabled[adventurersDisabled.Count - 1];
        Adventurer tempAdventurer = temp.GetComponent<Adventurer>();
        adventurersDisabled.RemoveAt(adventurersDisabled.Count - 1);

        int advLevel = Random.Range(minLevel, maxLevel+1);

        BattleStat tempBattleStat = new BattleStat();

        tempBattleStat.Level = advLevel;
        tempBattleStat.BaseHealthMax = advStatData[advLevel-1]["health"].AsFloat;
        tempBattleStat.BaseDefence = advStatData[advLevel - 1]["defense"].AsFloat;
        tempBattleStat.BaseAvoid = advStatData[advLevel - 1]["avoid"].AsFloat;
        tempBattleStat.BaseAttack = advStatData[advLevel - 1]["attack"].AsFloat;
        tempBattleStat.BaseAttackSpeed = advStatData[advLevel - 1]["attackspeeds"].AsFloat;
        tempBattleStat.BaseCriticalChance = advStatData[advLevel - 1]["criticalchance"].AsFloat;
        tempBattleStat.BaseCriticalDamage = advStatData[advLevel - 1]["criticalattack"].AsFloat;
        tempBattleStat.BasePenetrationFixed = advStatData[advLevel - 1]["penetration"].AsFloat;
        tempBattleStat.BaseMoveSpeed = advStatData[advLevel - 1]["movespeed"].AsFloat;
        tempBattleStat.BaseRange = advStatData[advLevel - 1]["attackrange"].AsInt;

        advEnterQ.Enqueue(temp);
    }

    private void ResetAdvStatistics()
    {
        for (int i = 0; i <= HuntingAreaManager.Instance.ActiveIndex; i++)
        {
            progressInformations[i].curAdvNum = 0;
        }
    }

    // 레벨대 별 모험가 수 통계를 새로 냄.
    private void CompileAdvStatistics()
    {
        ResetAdvStatistics();

        for (int i = 0; i < adventurersEnabled.Count; i++)
        {
            for (int j = 0; j < progressInformations.Count; j++)
            {
                if (adventurersEnabled[i].GetComponent<Adventurer>().Level <= progressInformations[j].maxLevel
                    && adventurersEnabled[i].GetComponent<Adventurer>().Level >= progressInformations[j].minLevel)
                    progressInformations[j].curAdvNum++;
            }
        }
    }

    private void GenerateAdventurers(int needed)
    {
        //List<ProgressInformation> tempList = progressInformations.CopyTo()
        CompileAdvStatistics();

        List<ProgressInformation> tempArr = new List<ProgressInformation>(progressInformations);

        tempArr.Sort(CompareByCurHuntingAreaAsc);
        int totalAdv = CurAdvMaxPerHuntingArea * (HuntingAreaManager.Instance.ActiveIndex + 1);
        tempArr.Add(new ProgressInformation(totalAdv));

        int generated = 0;

        for(int i = 0; i < tempArr.Count-1; i++)
        {
            while ( tempArr[i].curAdvNum < tempArr[i + 1].curAdvNum )
            {
                for (int j = 0; j <= i; j++)
                {
                    GenAndEnqueueSingleAdventurer(tempArr[j].minLevel, tempArr[j].maxLevel);
                    generated++;

                    if (generated >= needed)
                        return;
                }
            }
        }
    }

    private int CompareByCurHuntingAreaAsc(ProgressInformation info1, ProgressInformation info2)
    {
        return info1.curAdvNum - info2.curAdvNum;
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
#if DEBUG
            Debug.Log(x + "   " + y);
#endif
            mapEntrance.Add(layer.GetTileForMove(x * 2, y * 2));
            mapEntrance.Add(layer.GetTileForMove((x * 2) + 1, y * 2));
            mapEntrance.Add(layer.GetTileForMove(x * 2, (y * 2) + 1));
            mapEntrance.Add(layer.GetTileForMove((x * 2) + 1, (y * 2) + 1));
        }

        // 사냥터 관련 정보 저장
        huntingAreaCount = aData["scene"][sceneNumber]["huntingAreaCount"].AsInt;
        for (int i = 0; i < huntingAreaCount; i++)
        {
            progressInformations.Add(new ProgressInformation());
            progressInformations[i].huntingAreaIndex = aData["scene"][sceneNumber]["progressInformation"]["huntingAreaIndex"].AsInt; // 이거 지워도 될듯.
            progressInformations[i].guestCapacity = aData["scene"][sceneNumber]["progressInformation"]["guestCapacity"].AsInt;
            progressInformations[i].minLevel = aData["scene"][sceneNumber]["progressInformation"]["minLevel"].AsInt;
            progressInformations[i].maxLevel = aData["scene"][sceneNumber]["progressInformation"]["maxLevel"].AsInt;
        }

        traveler_Max = 0;
        adventurer_Max = 1;
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
    public void Save()
    {
        SaveLoadManager.Instance.Save();
#if DEBUG
        Debug.Log("저장 성공");
#endif
    }

    // 세이브 파일에서 Load
    public void Load()
    {
        SaveLoadManager.Instance.Load();
    }

    public void LoadPlayerData(GameSavedata savedata)
    {
        playerGold = savedata.playerGold;
        playerPopularity = savedata.playerPopularity;

        Camera.main.transform.position = new Vector3(savedata.cameraPosition.x, savedata.cameraPosition.y, savedata.cameraPosition.z);
        Camera.main.orthographicSize = savedata.cameraSize;
    }

    // 일단 완성
    public void LoadTravelerList(GameSavedata savedata)
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

        // Active 관련 요소는 이야기해보고 결정.
        for (int i = 0; i < savedata.travelerDatas.Count; i++)
        {
            newObject = (GameObject)Resources.Load("CharacterPrefabs/Traveler_test");
            newObject.SetActive(false);

            inputTravelerData = savedata.travelerDatas[i];


            // List에 추가
            travelers.Add(Instantiate(newObject));
            travelers[i].SetActive(false);

            newTraveler = travelers[i].GetComponent<Traveler>();

            travelers[i].transform.parent = GameObject.FindGameObjectWithTag("Characters").transform;
#if DEBUG_SAVELOAD
            Debug.Log("character instantiate - " + i);
#endif

            // 세이브 데이터에서 대입.

            newTraveler.index = inputTravelerData.index;

            if (inputTravelerData.isActive)
            {
                newTraveler.SetDestinationTileLoad(inputTravelerData.destinationTile);
                newTraveler.SetCurTileLoad(inputTravelerData.curTile);
                newTraveler.SetCurTileForMoveLoad(inputTravelerData.curTileForMove);

                travelers[i].transform.position = new Vector3(inputTravelerData.position.x, inputTravelerData.position.y, inputTravelerData.position.z);
                newTraveler.StopAllCoroutines(); // 일단 문제 해결.

                travelers[i].SetActive(true);

                newTraveler.SetSuperState(inputTravelerData.superState);
                if (inputTravelerData.state == State.MovingToDestination)
                {
                    newTraveler.curState = State.PathFinding;
                }
                else
                {
                    newTraveler.curState = inputTravelerData.state;
                }
#if DEBUG_SAVELOAD
                Debug.Log("DestTile:" + inputTravelerData.destinationTile);
#endif
            }

            // TEnter 관련은 아직 저장안함
            // 현 상태에서, 모험가 입장도중 저장하면 다른 모험가가 들어오지 않을 가능성 있음.
            // 어디까지 활성화했나 보고 그 뒤에꺼 이어가면 문제 X.
        }
    }

    public void LoadTileMap(GameSavedata savedata)
    {
        // 일단 Layer 없이 구현.
        tmg.ClearTileMap();
        tileMap = tmg.GenerateMapFromSave("TileMap/" + sceneName, savedata);
    }

    private void SetTileStructure(GameSavedata savedata)
    {
        TileMapGenerator.Instance.SetTileStructure(savedata);
    }

    public void LoadStructures(GameSavedata savedata)
    {
        StructureManager.Instance.LoadStructuresFromSave(savedata);
    }

    public GameObject GetTileLayer()
    {
        return tileMap.transform.GetChild(0).gameObject;
    }
    #endregion

    #region BattleImpl
    // 디버깅용 사냥터 생성 코드
    public void DebugHuntingArea()
    {
        HuntingAreaManager.Instance.ConstructHuntingArea(0, 0, GetTileLayer().transform.GetChild(1185).gameObject);
    }
    #endregion
}
