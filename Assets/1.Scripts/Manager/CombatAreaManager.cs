#define DEBUG_ADV

using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class CombatAreaManager : MonoBehaviour
{
    // 싱글톤
    static CombatAreaManager _instance;
    public static CombatAreaManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("CombatAreaManager is null");
                return null;
            }
            else
                return _instance;
        }
    }

    // 건설할 사냥터
    public GameObject constructing;

    // 사냥터들의 부모가 될 오브젝트
    public GameObject rootCombatAreaObject;

    // 사냥터 몇번까지 활성화됐는지.
    public int PublicHuntingAreaIndex { get; private set; }
    public int ConqueringHuntingAreaIndex { get { return PublicHuntingAreaIndex + 1; } }
    public int BossAreaIndex { get; private set; }


    // 맵 내에서 maxLevel이 가장 높은 사냥터의 maxLevel
    public int LevelUpperBound
    {
        get
        {
            return huntingAreas[PublicHuntingAreaIndex].LevelMax;
        }
    }

    // maxLevel이 가장 낮은 사냥터의 maxLevel
    public int LevelLowerBound
    {
        get
        {
            return huntingAreas[0].LevelMax - 9;
        }
    }

    // 사냥터 정보 읽어오기용
    public JSONNode huntingAreaJson;
    // 보스방 정보 읽어오기용
    public JSONNode bossAreaJson;
    // 몬스터 목록 읽어오기용
    public JSONNode monsterJson;

    #region 세이브!
    public List<HuntingArea> huntingAreas;
    public List<BossArea> bossAreas;
	#endregion

    void Awake()
    {
        _instance = this;
        LoadHuntingAreaData();
        LoadBossAreaData();
        LoadMonsterData();
        huntingAreas = new List<HuntingArea>();
        bossAreas = new List<BossArea>();

        PublicHuntingAreaIndex = -1;
        BossAreaIndex = 0;
    }

    void LoadHuntingAreaData()
    {
        TextAsset huntingAreaText = Resources.Load<TextAsset>("CombatArea/HuntingAreas");
        huntingAreaJson = JSON.Parse(huntingAreaText.text);
#if DEBUG_CREATE_HA
        Debug.Log("hunting Area1: " + huntingAreaText.text);
        Debug.Log("hunting Area2: " + huntingAreaJson);
        Debug.Log("hunting Area3: " + huntingAreaJson["stage1"][0]["name"]);
#endif
    }

    void LoadBossAreaData()
    {
        TextAsset bossAreaText = Resources.Load<TextAsset>("CombatArea/BossAreas");
        bossAreaJson = JSON.Parse(bossAreaText.text);
    }

    void LoadMonsterData()
    {
        TextAsset monsterText = Resources.Load<TextAsset>("Monsters/Monsters");
        monsterJson = JSON.Parse(monsterText.text);
#if DEBUG_CREATE_HA
        Debug.Log("monster1 : " + monsterText.text);
        Debug.Log("monster2 : " + monsterJson["Standard"][0]["level"]);
#endif
    }

    //public List<HuntingArea> GetHuntingAreas()
    //{
    //    return huntingAreas;
    //}

    //public List<BossArea> GetBossAreas()
    //{
    //    return bossAreas;
    //}

    /// <summary>
    /// 모험가에게 레벨에 적합한 사냥터를 반환해주는 메서드
    /// </summary>
    /// <param name="level">모험가의 레벨</param>
    /// <returns></returns>
    public HuntingArea FindHuntingAreaAdv(int level)
    {
        HuntingArea searchResult = null;

        //Debug.Log("HA IDX : " + PublicHuntingAreaIndex);
        // LevelMax만 검사함. 사냥터에 진입 못할 모험가는 애초에 생성을 안하는 방향으로.
        for (int i = 0; i <= PublicHuntingAreaIndex; i++)
        {
            if (level <= huntingAreas[i].LevelMax)
            {
                if (searchResult == null)
                    searchResult = huntingAreas[i];
                else if (searchResult.LevelMax >= huntingAreas[i].LevelMax)
                    searchResult = huntingAreas[i];
            }
        }

        return searchResult;
    }

    /// <summary>
    /// 일선 모험가에게 레벨에 적합한 사냥터를 반환해주는 메서드
    /// </summary>
    /// <param name="level">일선 모험가의 레벨</param>
    /// <returns></returns>
    public HuntingArea FindHuntingAreaSpAdv(int level)
    {
        HuntingArea searchResult = null;

        //Debug.Log("HA IDX : " + ConqueringHuntingAreaIndex);
        // LevelMax만 검사함. 사냥터에 진입 못할 모험가는 애초에 생성을 안하는 방향으로.
        for (int i = 0; i <= ConqueringHuntingAreaIndex; i++)
        {
            if (level <= huntingAreas[i].LevelMax)
            {
                if (searchResult == null)
                    searchResult = huntingAreas[i];
                else if (searchResult.LevelMax >= huntingAreas[i].LevelMax)
                    searchResult = huntingAreas[i];
            }
        }

        // 일선 모험가는 나가면 안되므로 가장 높은 곳으로 그냥 찍어줌.
        if (searchResult == null)
            searchResult = huntingAreas[ConqueringHuntingAreaIndex];

        return searchResult;
    }

    public BossArea FindBossArea()
    {
        return bossAreas[BossAreaIndex];
    }

    public void SetHuntingAreaPoint(Tile t)
    {
        constructing.GetComponent<HuntingArea>().point = t;
    }

    public void SetBossAreaPoint(Tile t)
    {
        constructing.GetComponent<BossArea>().point = t;
    }

    public void DestroyConstructing()
    {
        Destroy(constructing);
        constructing = null;
    }

    public void ClearHuntingAreaManager()
    {

        if (constructing != null)
            DestroyConstructing();

        huntingAreas.Clear();
        int childCount = rootCombatAreaObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
            Destroy(rootCombatAreaObject.transform.GetChild(i).gameObject);

        //structureJson 는 일단 손 안댐.
    }

    // 사냥터 건설
    public void ConstructHuntingArea(int areaNum, int areaIndex, GameObject pointTile)
    {
        #region InstantiateStructure()
        string stageNum = "stage" + SceneManager.GetActiveScene().name;
        int huntingAreaNum = areaNum;
        //Debug.Log("stageNum : " + stageNum + " areaNum : " + huntingAreaNum);
        //Debug.Log("디버그2: " + huntingAreaJson[stageNum][huntingAreaNum]["name"]);
        //Debug.Log("HuntingArea/HuntingAreaPrefabs/" + stageNum + "/" + huntingAreaNum);
        constructing = (GameObject)Instantiate(Resources.Load("CombatArea/HuntingAreaPrefabs/" + stageNum + "/" + huntingAreaNum.ToString()));

        constructing.transform.parent = rootCombatAreaObject.transform;

        //임시
        HuntingArea huntingArea = constructing.GetComponent<HuntingArea>();
        huntingArea.name = huntingAreaJson[stageNum][huntingAreaNum]["name"];

        int conquerCondition = huntingAreaJson[stageNum][huntingAreaNum]["conquerCondition"].AsInt;
        bool isBossArea = huntingAreaJson[stageNum][huntingAreaNum]["isBossArea"].AsBool; //서로 알 필요 없지않나?
        //Debug.Log(huntingAreaJson[stageNum][huntingAreaNum]["isBossRoom"].AsBool);
        int levelMax = huntingAreaJson[stageNum][huntingAreaNum]["levelMax"].AsInt;
        int monsterMax = huntingAreaJson[stageNum][huntingAreaNum]["monsterMax"].AsInt;
        int monsterPerRegen = huntingAreaJson[stageNum][huntingAreaNum]["monsterPerRegen"].AsInt;
        float monsterRegenRate = huntingAreaJson[stageNum][huntingAreaNum]["monsterRegenRate"].AsFloat;
        float monsterRatio = huntingAreaJson[stageNum][huntingAreaNum]["monsterRatio"].AsFloat;

        // 몬스터 프로토타입 넣기. 우선 번호부터.
        string monsterSet = huntingAreaJson[stageNum][huntingAreaNum]["monsterSet"];

        int monsterSample1Num = huntingAreaJson[stageNum][huntingAreaNum]["monsterSample1Num"].AsInt;
        int monsterSample2Num = huntingAreaJson[stageNum][huntingAreaNum]["monsterSample2Num"].AsInt;

        // 몬스터 샘플 instantiate
        //GameObject monsterSample1, monsterSample2;
        //LoadMonsterSamples(monsterSet, monsterSample1Num, monsterSample2Num, out monsterSample1, out monsterSample2);
        GameObject monsterSample1 = LoadMonsterFromJson(monsterSet, monsterSample1Num);
        GameObject monsterSample2 = LoadMonsterFromJson(monsterSet, monsterSample2Num);

        huntingArea.InitHuntingArea(conquerCondition, isBossArea, levelMax, monsterMax, monsterPerRegen, monsterRegenRate, monsterRatio, monsterSample1, monsterSample2);
        huntingArea.areaConquered += OnHuntingAreaConquered;

        // 저장용.
        huntingArea.stageNum = stageNum;
        huntingArea.huntingAreaNum = areaNum;
        huntingArea.index = areaIndex;

        //관광 수치 저장해야할 수도 있음. 수정요망.

        //건설공간 지정
        int x = huntingAreaJson[stageNum][huntingAreaNum]["sitewidth"].AsInt;
        huntingArea.extentWidth = x;

        int y = huntingAreaJson[stageNum][huntingAreaNum]["siteheight"].AsInt;
        huntingArea.extentHeight = y;

#if DEBUG_CREATE_HA
        Debug.Log("siteWidth = " + x + ", siteHeight = " + y + ", Extent[0][0] = " + huntingAreaJson[stageNum][huntingAreaNum]["site"][0]);
#endif

        huntingArea.extent = new int[x, y];
        for (int i = 0; i < x * y; i++)
        {
            huntingArea.extent[i % x, i / x] = huntingAreaJson[stageNum][huntingAreaNum]["site"][i].AsInt;
        }
        #endregion

        #region AllocateStructure()
        // 건설할 건물의 position 설정
        constructing.transform.position = pointTile.transform.position; // 문제 생길지도 모름. 수정요망

        TileLayer tileLayer = TileMapGenerator.Instance.tileMap_Object.transform.GetChild(0).GetComponent<TileLayer>();

        SetHuntingAreaPoint(pointTile.GetComponent<Tile>());
        #endregion

        #region ConstructStructure()
        Tile tile = huntingArea.point;
        int[,] extent = huntingArea.GetExtent();

        constructing.tag = "CombatArea";

        // 인덱스 값 넣어줌.
        huntingArea.index = huntingAreas.Count;

        // 리스트에 추가
        huntingAreas.Add(huntingArea);

        for (int i = 0; i < huntingArea.extentHeight; i++)
        {
            for (int j = 0; j < huntingArea.extentWidth; j++)
            {
                Tile thatTile = tileLayer.GetTileAsComponent(tile.GetX() + j, tile.GetY() + i);

                if (extent[j, i] == 1)
                {
                    thatTile.SetBuildingArea(false);
                    thatTile.SetStructed(false);
                    thatTile.SetHuntingArea(true);

                    //디버깅용임시
                    thatTile.gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 140, 40);
                    //
#if DEBUG_CREATE_HA
                    Debug.Log(thatTile);
#endif
                    // 여기서 딕셔너리에 TileForMove 영역을 추가해주자.
                    //foreach(TileForMove child in thatTile.childs)
                    //{
                    //    huntingArea.AddTerritory(child);
                    //}
                    // 디버깅용임시
                    // thatTile.SetPassable(true);
                    for (int k = 0; k < 4; k++)
                    {
                        huntingArea.AddTerritory(thatTile.childs[k]);
                    }

                    //thatTile.SetStructure(structure); // 일단 쓰는 곳 없어서 안넣고 둠.
                }
                else if (extent[j, i] == 2)
                {
                    thatTile.SetStructed(true);
                    thatTile.gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 140, 0);
                    //if (thatTile.GetBuildable())
                    //{
                    huntingArea.addEntrance(thatTile);
                    //}
                }
            }
        }

        constructing = null;
        #endregion
    }

    public void ConstructBossArea(int areaNum, int areaIndex, GameObject pointTile)
    {
        #region InstantiateStructure()
        string stageNum = "stage" + SceneManager.GetActiveScene().name;
        int bossAreaNum = areaNum;
        //Debug.Log("stageNum : " + stageNum + " areaNum : " + huntingAreaNum);
        //Debug.Log("디버그2: " + huntingAreaJson[stageNum][huntingAreaNum]["name"]);
        //Debug.Log("HuntingArea/HuntingAreaPrefabs/" + stageNum + "/" + huntingAreaNum);
        constructing = (GameObject)Instantiate(Resources.Load("CombatArea/BossAreaPrefabs/" + stageNum + "/" + bossAreaNum.ToString()));

        constructing.transform.parent = rootCombatAreaObject.transform;

        //임시
        BossArea bossArea = constructing.GetComponent<BossArea>();
        bossArea.name = bossAreaJson[stageNum][bossAreaNum]["name"];

        // 보스 생성. 우선 번호부터.
        int bossNum = bossAreaJson[stageNum][bossAreaNum]["bossNum"].AsInt;
        int challengeLevel = bossAreaJson[stageNum][bossAreaNum]["challengeLevel"].AsInt;
        int bonus = bossAreaJson[stageNum][bossAreaNum]["bonus"].AsInt;
        //Debug.Log("bonus : " + bonus);
        GameObject boss = LoadMonsterFromJson("Boss", bossNum);

        // 세이브 로드용
        bossArea.stageNum = stageNum;
        bossArea.bossAreaNum = areaNum;
        bossArea.index = areaIndex;

        //건설공간 지정
        int x = bossAreaJson[stageNum][bossAreaNum]["sitewidth"].AsInt;
        bossArea.extentWidth = x;

        int y = bossAreaJson[stageNum][bossAreaNum]["siteheight"].AsInt;
        bossArea.extentHeight = y;

#if DEBUG_CREATE_HA
        Debug.Log("siteWidth = " + x + ", siteHeight = " + y + ", Extent[0][0] = " + huntingAreaJson[stageNum][huntingAreaNum]["site"][0]);
#endif

        bossArea.extent = new int[x, y];
        for (int i = 0; i < x * y; i++)
        {
            bossArea.extent[i % x, i / x] = bossAreaJson[stageNum][bossAreaNum]["site"][i].AsInt;
        }
        #endregion

        #region AllocateStructure()
        // 건설할 건물의 position 설정
        constructing.transform.position = pointTile.transform.position; // 문제 생길지도 모름. 수정요망

        TileLayer tileLayer = TileMapGenerator.Instance.tileMap_Object.transform.GetChild(0).GetComponent<TileLayer>();

        SetBossAreaPoint(pointTile.GetComponent<Tile>());
        #endregion

        #region ConstructStructure()
        Tile tile = bossArea.point;
        int[,] extent = bossArea.GetExtent();

        constructing.tag = "CombatArea";

        // 인덱스 값 넣어줌.
        bossArea.index = bossAreas.Count;

        // 리스트에 추가
        bossAreas.Add(bossArea);

        for (int i = 0; i < bossArea.extentHeight; i++)
        {
            //string debugStr = "";
            for (int j = 0; j < bossArea.extentWidth; j++)
            {
                Tile thatTile = tileLayer.GetTileAsComponent(tile.GetX() + j, tile.GetY() + i);

                //debugStr += " ";
                //debugStr += extent[j, i];
                if (extent[j, i] == 1)
                {
                    thatTile.SetBuildingArea(false);
                    thatTile.SetStructed(false);
                    thatTile.SetHuntingArea(true);

                    //디버깅용임시
                    thatTile.gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                    //
                    for (int k = 0; k < 4; k++)
                    {
                        bossArea.AddTerritory(thatTile.childs[k]);
                    }
                }
                else if (extent[j, i] == 2)
                {
                    thatTile.SetStructed(true);
                    thatTile.gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 140, 0);
                    //if (thatTile.GetBuildable())
                    //{
                    bossArea.addEntrance(thatTile);
                    //}
                }
            }

            //Debug.Log(debugStr);
        }

        bossArea.InitBossArea(boss, challengeLevel, bonus);
        boss.GetComponent<Monster>().SetHabitat(bossArea);
        constructing = null;
        #endregion
    }

    GameObject LoadMonsterFromJson(string monsterSet, int monsterNum)
    {
        GameObject monster = (GameObject)Instantiate(Resources.Load("MonsterPrefabs/" + monsterSet + "/" + monsterNum));
        monster.SetActive(false);
        monster.transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);

        #region 몬스터 스탯 로드
        BattleStat tempBattleStat = new BattleStat();

        //Debug.Log("num : " + sample1Num + ", " + sample2Num);
        //Debug.Log("level : " + monsterJson[monsterSet][sample1Num]["level"]);
        tempBattleStat.Level = monsterJson[monsterSet][monsterNum]["level"].AsInt;
        tempBattleStat.BaseHealthMax = monsterJson[monsterSet][monsterNum]["hp"].AsFloat;
        tempBattleStat.BaseDefence = monsterJson[monsterSet][monsterNum]["def"].AsFloat;
        tempBattleStat.BaseAvoid = monsterJson[monsterSet][monsterNum]["avoid"].AsFloat;
        tempBattleStat.BaseAttack = monsterJson[monsterSet][monsterNum]["atk"].AsFloat;
        tempBattleStat.BaseAttackSpeed = monsterJson[monsterSet][monsterNum]["atkspeed"].AsFloat;
        tempBattleStat.BaseCriticalChance = monsterJson[monsterSet][monsterNum]["critical"].AsFloat;
        tempBattleStat.BaseCriticalDamage = monsterJson[monsterSet][monsterNum]["atkcritical"].AsFloat;
        tempBattleStat.BasePenetrationFixed = monsterJson[monsterSet][monsterNum]["penetration"].AsFloat;
        tempBattleStat.BaseMoveSpeed = monsterJson[monsterSet][monsterNum]["movespeed"].AsFloat;
        tempBattleStat.BaseAttackRange = monsterJson[monsterSet][monsterNum]["range"].AsInt;

        RewardStat tempRewardStat = new RewardStat();
        tempRewardStat.Exp = monsterJson[monsterSet][monsterNum]["exp"].AsInt;
        tempRewardStat.Gold = monsterJson[monsterSet][monsterNum]["gold"].AsInt;
        #endregion

        Monster monsterComp = monster.GetComponent<Monster>();
        if (monsterSet == "Boss")
            monsterComp.InitMonster(monsterNum, tempBattleStat, tempRewardStat, false);
        else
            monsterComp.InitMonster(monsterNum, tempBattleStat, tempRewardStat);
        SetMonsterDefaultEffects(monsterComp);

        return monster;
    }

    public static void SetMonsterDefaultEffects(Monster monster)
    {
        monster.SetAttackEffect((GameObject)Instantiate(Resources.Load("EffectPrefabs/Default_AttackEffect")));
        monster.SetDefaultEffects();
    }

    #region Stage Progress
    public void InitCombatAreas()
    {
        // 일단 하나 열어두고 시작.
        OnHuntingAreaConquered();
    }

    private void HuntingAreaOpenToPublic()
    {
        huntingAreas[PublicHuntingAreaIndex].OpenToPublic();
    }

    private void HuntingAreaConquerStart()
    {
        // Active는 항상 되어있고, 장애물 해체 관련된 거 들어가면 됨.
    }

    private void BossAreaOpenToPublic()
    {
        bossAreas[BossAreaIndex].OpenToPublic();
    }

    private void BossAreaConquerStart()
    {
        GameManager.Instance.OnBossAreaConquerStarted();
    }

    private void OnHuntingAreaConquered(bool isLoaded = false)
    {
        PublicHuntingAreaIndex++;
        HuntingAreaOpenToPublic();
        if(isLoaded == false)
            GameManager.Instance.OnHuntingAreaOpenToPublic();

        HuntingAreaConquerStart();
        //Debug.Log("OnHuntingAreaConquered");
        if (huntingAreas[ConqueringHuntingAreaIndex].IsBossArea)
        {
            // 보스전 신청용 UI 띄우기
            BossAreaConquerStart();
        }
    }

    public void OnBossAreaConquered(bool isLoaded = false)
    {
        if (ConqueringHuntingAreaIndex < huntingAreas.Count - 1)
        {
            PublicHuntingAreaIndex++;
            HuntingAreaOpenToPublic();
            if(isLoaded == false)
                GameManager.Instance.OnHuntingAreaOpenToPublic();
            HuntingAreaConquerStart();
        }

        if (BossAreaIndex < bossAreas.Count - 1)
        {
            BossAreaOpenToPublic();
            BossAreaIndex++;
        }
    }

    public void StartConquer()
    {
        HuntingAreaOpenToPublic();
        HuntingAreaConquerStart();
    }
    #endregion

    #region SaveLoad
    public void LoadCombatAreasFromSave(GameSavedata savedata)
    {
        OpenCombatAreasToSavedIndex(savedata);
        LoadCombatAreas(savedata);
    }

    public void LoadCombatAreas(GameSavedata savedata)
    {

        for (int i = 0; i <= savedata.combatAreaManager.publicHuntingAreaIndex + 1; i++)
        {
            //Debug.Log("LoadHA");
            huntingAreas[i].InitFromSaveData(savedata.combatAreaManager.huntingAreas[i]);
            huntingAreas[i].isNew = false;
        }

        for (int i = 0; i <= savedata.combatAreaManager.bossAreaIndex; i++)
        {
            bossAreas[i].InitFromSaveData(savedata.combatAreaManager.bossAreas[i]);
            bossAreas[i].isNew = false;
        }
    }

    private void OpenCombatAreasToSavedIndex(GameSavedata savedata)
    {
        while (PublicHuntingAreaIndex < savedata.combatAreaManager.publicHuntingAreaIndex)
        {
            if (huntingAreas[ConqueringHuntingAreaIndex].IsBossArea)
                OnBossAreaConquered(true);
            else
                OnHuntingAreaConquered(true);
        }
    }

    public void SetMonstersEnemy(GameSavedata savedata)
    {
        for (int i = 0; i <= ConqueringHuntingAreaIndex; i++)
            foreach (int key in huntingAreas[i].monstersEnabled.Keys)
            {
                if(savedata.combatAreaManager.huntingAreas[i].monstersEnabled[key].enemy != null)
                    GameManager.Instance.SetICombatantEnemy(huntingAreas[i].monstersEnabled[key].GetComponent<Monster>(), savedata.combatAreaManager.huntingAreas[i].monstersEnabled[key].enemy);
            }
        for (int i = 0; i <= BossAreaIndex; i++)
        {
            foreach (int key in bossAreas[i].monstersEnabled.Keys)
            {
                if (savedata.combatAreaManager.bossAreas[i].monstersEnabled[key].enemy != null)
                    GameManager.Instance.SetICombatantEnemy(bossAreas[i].monstersEnabled[key].GetComponent<Monster>(), savedata.combatAreaManager.bossAreas[i].monstersEnabled[key].enemy);
            }
        }
    }

    public void ActivateMonsters()
    {
        for (int i = 0; i <= ConqueringHuntingAreaIndex; i++)
            foreach (GameObject mob in huntingAreas[i].monstersEnabled.Values)
                mob.SetActive(true);
        for (int i = 0; i <= BossAreaIndex; i++)
            foreach (GameObject mob in bossAreas[i].monstersEnabled.Values)
                mob.SetActive(true);
    }
    #endregion
}