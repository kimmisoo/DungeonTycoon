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
                Debug.Log("HuntingAreaManager is null");
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
    public int ActiveIndex { get; private set; }
    public int BossIndex { get; private set; }


    // 맵 내에서 maxLevel이 가장 높은 사냥터의 maxLevel
    public int LevelUpperBound
    {
        get
        {
            return huntingAreas[ActiveIndex].LevelMax;
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

    // Use this for initialization
    void Start()
    {
        _instance = this;
        LoadHuntingAreaData();
        LoadBossAreaData();
        LoadMonsterData();
        huntingAreas = new List<HuntingArea>();
        bossAreas = new List<BossArea>();
        ActiveIndex = -1;
        BossIndex = 0;
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

    public List<HuntingArea> GetHuntingAreas()
    {
        return huntingAreas;
    }

    // 사냥터 찾기. 캐릭터 레벨에 맞는 사냥터를 찾아줌.
    public HuntingArea FindHuntingArea(int level)
    {
        HuntingArea searchResult = null;

        // LevelMax만 검사함. 사냥터에 진입 못할 모험가는 애초에 생성을 안하는 방향으로.
        for (int i = 0; i < huntingAreas.Count; i++)
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
        huntingArea.huntingAreaIndex = areaIndex;

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
        huntingArea.huntingAreaIndex = huntingAreas.Count;

        // 리스트에 추가
        huntingAreas.Add(huntingArea);

        for (int i = 0; i < huntingArea.extentHeight; i++)
        {
            for (int j = 0; j < huntingArea.extentWidth; j++)
            {
                Tile thatTile = tileLayer.GetTileAsComponent(tile.GetX() + j, tile.GetY() + i);

                if (extent[j, i] == 1)
                {
                    thatTile.SetBuildable(false);
                    thatTile.SetStructed(false);
                    thatTile.SetHuntingArea(true);

                    //디버깅용임시
                    thatTile.gameObject.GetComponent<SpriteRenderer>().color = new Color(180, 110, 0);
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
                    for (int k = 0; k<4; k++)
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

#if DEBUG_ADV
        ActivateNextHuntingArea();
#endif
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

        // 몬스터 샘플 instantiate
        //GameObject monsterSample1, monsterSample2;
        //LoadMonsterSamples(monsterSet, monsterSample1Num, monsterSample2Num, out monsterSample1, out monsterSample2);
        GameObject boss = LoadMonsterFromJson("Boss", bossNum);

        

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

        // 보스몬스터 위치 지정
        //Tile centerTile = tileLayer.GetTile(pointTile.GetComponent<Tile>().GetX() + x/2, pointTile.GetComponent<Tile>().GetY() + y/2).GetComponent<Tile>();
        //boss.GetComponent<Monster>().SetCurTile(centerTile);
        //boss.GetComponent<Monster>().SetCurTileForMove(centerTile.GetChild(0));
        #endregion

        #region ConstructStructure()
        Tile tile = bossArea.point;
        int[,] extent = bossArea.GetExtent();

        constructing.tag = "CombatArea";

        // 인덱스 값 넣어줌.
        bossArea.bossAreaIndex = bossAreas.Count;

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
                    thatTile.SetBuildable(false);
                    thatTile.SetStructed(false);
                    thatTile.SetHuntingArea(true);

                    //디버깅용임시
                    thatTile.gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 110, 0);
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

        bossArea.InitBossArea(boss);
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
        monsterComp.InitMonster(monsterNum, tempBattleStat, tempRewardStat);
        monsterComp.SetAttackEffect((GameObject)Instantiate(Resources.Load("EffectPrefabs/Default_AttackEffect")));
        //tempMonsterComp.SetDamageText((GameObject)Instantiate(Resources.Load("UIPrefabs/Battle/DamageText")));
        monsterComp.SetDefaultEffects();

        return monster;
    }

//    // 몬스터 샘플 로드해서 instantiate해주는 함수.
//    void LoadMonsterSamples(string monsterSet, int sample1Num, int sample2Num, out GameObject monsterSample1, out GameObject monsterSample2)
//    {
//        // 몬스터 샘플 1 할당
//        //Debug.Log("MonsterPrefabs/" + monsterSet + "/" + sample1Num);
//        monsterSample1 = (GameObject)Instantiate(Resources.Load("MonsterPrefabs/" + monsterSet + "/" + sample1Num));
//        monsterSample1.SetActive(false);
//        monsterSample1.transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);

//#region 스탯로드
//        BattleStat tempBattleStat = new BattleStat();

//        //Debug.Log("num : " + sample1Num + ", " + sample2Num);
//        //Debug.Log("level : " + monsterJson[monsterSet][sample1Num]["level"]);
//        tempBattleStat.Level = monsterJson[monsterSet][sample1Num]["level"].AsInt;
//        tempBattleStat.BaseHealthMax = monsterJson[monsterSet][sample1Num]["hp"].AsFloat ;
//        tempBattleStat.BaseDefence = monsterJson[monsterSet][sample1Num]["def"].AsFloat;
//        tempBattleStat.BaseAvoid = monsterJson[monsterSet][sample1Num]["avoid"].AsFloat;
//        tempBattleStat.BaseAttack = monsterJson[monsterSet][sample1Num]["atk"].AsFloat;
//        tempBattleStat.BaseAttackSpeed = monsterJson[monsterSet][sample1Num]["atkspeed"].AsFloat;
//        tempBattleStat.BaseCriticalChance = monsterJson[monsterSet][sample1Num]["critical"].AsFloat;
//        tempBattleStat.BaseCriticalDamage = monsterJson[monsterSet][sample1Num]["atkcritical"].AsFloat;
//        tempBattleStat.BasePenetrationFixed = monsterJson[monsterSet][sample1Num]["penetration"].AsFloat;
//        tempBattleStat.BaseMoveSpeed = monsterJson[monsterSet][sample1Num]["movespeed"].AsFloat;
//        tempBattleStat.BaseAttackRange = monsterJson[monsterSet][sample1Num]["range"].AsInt;

//        RewardStat tempRewardStat = new RewardStat();
//        tempRewardStat.Exp = monsterJson[monsterSet][sample1Num]["exp"].AsInt;
//        tempRewardStat.Gold = monsterJson[monsterSet][sample1Num]["gold"].AsInt;
//#endregion

//        Monster tempMonsterComp = monsterSample1.GetComponent<Monster>();
//        tempMonsterComp.InitMonster(sample1Num, tempBattleStat, tempRewardStat);
//        tempMonsterComp.SetAttackEffect((GameObject)Instantiate(Resources.Load("EffectPrefabs/Default_AttackEffect")));
//        //tempMonsterComp.SetDamageText((GameObject)Instantiate(Resources.Load("UIPrefabs/Battle/DamageText")));
//        tempMonsterComp.SetDefaultEffects();

//        // 몬스터 샘플 2 할당
//        monsterSample2 = (GameObject)Instantiate(Resources.Load("MonsterPrefabs/" + monsterSet + "/" + sample2Num));
//        monsterSample2.SetActive(false);
//        monsterSample2.transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);

//#region 스탯 로드
//        tempBattleStat = new BattleStat();

//        tempBattleStat.Level = monsterJson[monsterSet][sample2Num]["level"].AsInt;
//        tempBattleStat.BaseHealthMax = monsterJson[monsterSet][sample2Num]["hp"].AsFloat;
//        tempBattleStat.BaseDefence = monsterJson[monsterSet][sample2Num]["def"].AsFloat;
//        tempBattleStat.BaseAvoid = monsterJson[monsterSet][sample2Num]["avoid"].AsFloat;
//        tempBattleStat.BaseAttack = monsterJson[monsterSet][sample2Num]["atk"].AsFloat;
//        tempBattleStat.BaseAttackSpeed = monsterJson[monsterSet][sample2Num]["atkspeed"].AsFloat;
//        tempBattleStat.BaseCriticalChance = monsterJson[monsterSet][sample2Num]["critical"].AsFloat;
//        tempBattleStat.BaseCriticalDamage = monsterJson[monsterSet][sample2Num]["atkcritical"].AsFloat;
//        tempBattleStat.BasePenetrationFixed = monsterJson[monsterSet][sample2Num]["penetration"].AsFloat;
//        tempBattleStat.BaseMoveSpeed = monsterJson[monsterSet][sample2Num]["movespeed"].AsFloat;
//        tempBattleStat.BaseAttackRange = monsterJson[monsterSet][sample2Num]["range"].AsInt;
        
//        tempRewardStat = new RewardStat();
//        tempRewardStat.Exp = monsterJson[monsterSet][sample2Num]["exp"].AsInt;
//        tempRewardStat.Gold = monsterJson[monsterSet][sample2Num]["gold"].AsInt;
//#endregion

//        tempMonsterComp = monsterSample2.GetComponent<Monster>();
//        tempMonsterComp.InitMonster(sample2Num, tempBattleStat, tempRewardStat);
//        tempMonsterComp.SetAttackEffect((GameObject)Instantiate(Resources.Load("EffectPrefabs/Default_AttackEffect")));
//        //tempMonsterComp.SetDamageText((GameObject)Instantiate(Resources.Load("UIPrefabs/Battle/DamageText")));
//        tempMonsterComp.SetDefaultEffects();
//        return;
//    }

    public void ActivateNextHuntingArea()
    {
        ActiveIndex++;
        
        // 활성화 관련된 거 더 구현할 것.
        // Active는 항상 되어있고, 사냥터 검색에 걸리는 거랑 장애물 해체 관련된 거 들어가면 됨.
    }

    public void OnHuntingAreaConquered()
    {
        ActivateNextHuntingArea();
        //Debug.Log("OnHuntingAreaConquered");
        if(huntingAreas[ActiveIndex].IsBossArea)
        {
            // 보스전 신청용 UI 띄우기
        }
    }
}
