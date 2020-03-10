using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class HuntingAreaManager : MonoBehaviour
{
    static HuntingAreaManager _instance;

    public static HuntingAreaManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("StructureManager is null");
                return null;
            }
            else
                return _instance;
        }
    }

    // 건설할 사냥터
    public GameObject constructing;

    // 사냥터들의 부모가 될 오브젝트
    public GameObject rootStructureObject;

    // 사냥터 정보 읽어오기용
    public JSONNode huntingAreaJson;
    // 몬스터 목록 읽어오기용
    public JSONNode monsterJson;

    #region 세이브!
    public List<HuntingArea> huntingAreas;
    #endregion

    // Use this for initialization
    void Start()
    {
        _instance = this;
        LoadHuntingAreaData();
        LoadMonsterData();
        huntingAreas = new List<HuntingArea>();
    }

    void LoadHuntingAreaData()
    {
        TextAsset huntingAreaText = Resources.Load<TextAsset>("HuntingArea/huntingareas");
        huntingAreaJson = JSON.Parse(huntingAreaText.text);
        Debug.Log("hunting Area1: " + huntingAreaText.text);
        Debug.Log("hunting Area2: " + huntingAreaJson);
        Debug.Log("hunting Area3: " + huntingAreaJson["stage1"][0]["name"]);
    }
    void LoadMonsterData()
    {
        TextAsset monsterText = Resources.Load<TextAsset>("Monsters/Monsters");
        monsterJson = JSON.Parse(monsterText.text);
        Debug.Log("monster1 : " + monsterText.text);
        Debug.Log("monster2 : " + monsterJson["Standard"][0]["level"]);
    }

    public List<HuntingArea> GetHuntingAreas()
    {
        return huntingAreas;
    }


    // 사냥터 찾기. 캐릭터 레벨에 맞는 사냥터를 찾아줌.
    public HuntingArea FindHuntingArea(int level)
    {
        HuntingArea searchResult = null;

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
        int childCount = rootStructureObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
            Destroy(rootStructureObject.transform.GetChild(i).gameObject);

        //structureJson 는 일단 손 안댐.
    }

    // 사냥터 건설
    public void ConstructHuntingArea(int areaNum, Vector3 pos, GameObject pointTile)
    {
        #region InstantiateStructure()
        string stageNum = "stage" + SceneManager.GetActiveScene().name;
        int huntingAreaNum = areaNum;
        Debug.Log("stageNum : " + stageNum + " areaNum : " + huntingAreaNum);
        Debug.Log("디버그2: " + huntingAreaJson[stageNum][huntingAreaNum]["name"]);
        Debug.Log("HuntingArea/HuntingAreaPrefabs/" + stageNum + "/" + huntingAreaNum);
        constructing = (GameObject)Instantiate(Resources.Load("HuntingArea/HuntingAreaPrefabs/" + stageNum + "/" + huntingAreaNum.ToString()));
        //수정요망 부모 오브젝트를 뭘로 둘지?
        constructing.transform.parent = rootStructureObject.transform;

       

        //임시
        HuntingArea huntingArea = constructing.GetComponent<HuntingArea>();
        huntingArea.name = huntingAreaJson[stageNum][huntingAreaNum]["name"];
        int levelMax = huntingAreaJson[stageNum][huntingAreaNum]["levelMax"].AsInt;
        Debug.Log("레벨" + levelMax);
        int monsterMax = huntingAreaJson[stageNum][huntingAreaNum]["monsterMax"].AsInt;
        int monsterPerRegen = huntingAreaJson[stageNum][huntingAreaNum]["monsterPerRegen"].AsInt;
        int monsterRegenRate = huntingAreaJson[stageNum][huntingAreaNum]["monsterRegenRate"].AsInt;

        // 몬스터 프로토타입 넣기. 우선 번호부터.
        string monsterSet = huntingAreaJson[stageNum][huntingAreaNum]["monsterSet"];
        Debug.Log(monsterSet);
        int monsterSample1Num = huntingAreaJson[stageNum][huntingAreaNum]["monsterSample1Num"].AsInt;
        Debug.Log(monsterSample1Num);
        int monsterSample2Num = huntingAreaJson[stageNum][huntingAreaNum]["monsterSample2Num"].AsInt;
        Debug.Log(monsterSample2Num);

        // 몬스터 샘플 instantiate
        GameObject monsterSample1, monsterSample2;
        LoadMonsterSamples(monsterSet, monsterSample1Num, monsterSample2Num, out monsterSample1, out monsterSample2);

        huntingArea.InitHuntingArea(levelMax, monsterMax, monsterPerRegen, monsterRegenRate, monsterSample1, monsterSample2);

        // 저장용.
        huntingArea.stageNum = stageNum;
        huntingArea.huntingAreaNum = areaNum;

        //관광 수치 저장해야할 수도 있음. 수정요망.

        //건설공간 지정
        int x = huntingAreaJson[stageNum][huntingAreaNum]["sitewidth"].AsInt;
        huntingArea.extentWidth = x;

        int y = huntingAreaJson[stageNum][huntingAreaNum]["sitelheight"].AsInt;
        huntingArea.extentHeight = y;

        huntingArea.extent = new int[x, y];
        for (int i = 0; i < x * y; i++)
        {
            huntingArea.extent[i % x, i / x] = huntingAreaJson[stageNum][huntingAreaNum]["site"][i].AsInt;
        }
        #endregion

        #region AllocateStructure()
        // 건설할 건물의 position 설정
        constructing.transform.position = pos; // 문제 생길지도 모름. 수정요망

        TileLayer tileLayer = TileMapGenerator.Instance.tileMap_Object.transform.GetChild(0).GetComponent<TileLayer>();

        SetHuntingAreaPoint(pointTile.GetComponent<Tile>());
        #endregion

        #region ConstructStructure()
        Tile tile = huntingArea.point;
        int[,] extent = huntingArea.GetExtent();

        constructing.tag = "HuntingArea";

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
                    thatTile.SetStructed(true);
                    thatTile.SetHuntingArea(true);
                    //thatTile.SetStructure(structure); // 일단 쓰는 곳 없어서 안넣고 둠.
                }
                else if (extent[j, i] == 2)
                {
                    thatTile.SetStructed(true);
                    if (thatTile.GetBuildable())
                    {
                        huntingArea.addEntrance(thatTile);
                    }
                }
            }
        }

        constructing = null;
        #endregion
    }

    // 몬스터 샘플 로드해서 instantiate해주는 함수.
    void LoadMonsterSamples(string monsterSet, int sample1Num, int sample2Num, out GameObject monsterSample1, out GameObject monsterSample2)
    {
        // 몬스터 샘플 1 할당
        Debug.Log("MonsterPrefabs/" + monsterSet + "/" + sample1Num);
        monsterSample1 = (GameObject)Instantiate(Resources.Load("MonsterPrefabs/" + monsterSet + "/" + sample1Num));
        monsterSample1.SetActive(false);
        // 스탯 설정
        BattleStat tempBattleStat = new BattleStat();

        Debug.Log("num : " + sample1Num + ", " + sample2Num);
        Debug.Log("level : " + monsterJson[monsterSet][sample1Num]["level"]);
        tempBattleStat.Level = monsterJson[monsterSet][sample1Num]["level"].AsInt;
        tempBattleStat.BaseHealthMax = monsterJson[monsterSet][sample1Num]["hp"].AsFloat ;
        tempBattleStat.BaseDefence = monsterJson[monsterSet][sample1Num]["def"].AsFloat;
        tempBattleStat.BaseAttack = monsterJson[monsterSet][sample1Num]["atk"].AsFloat;
        tempBattleStat.BaseAttackSpeed = monsterJson[monsterSet][sample1Num]["atkspeed"].AsFloat;
        tempBattleStat.BaseCriticalChance = monsterJson[monsterSet][sample1Num]["critical"].AsFloat;
        tempBattleStat.BaseCriticalDamage = monsterJson[monsterSet][sample1Num]["atkcritical"].AsFloat;
        tempBattleStat.BasePenetrationFixed = monsterJson[monsterSet][sample1Num]["penetration"].AsFloat;
        tempBattleStat.BaseMoveSpeed = monsterJson[monsterSet][sample1Num]["movespeed"].AsFloat;
        tempBattleStat.BaseRange = monsterJson[monsterSet][sample1Num]["range"].AsInt;

        RewardStat tempRewardStat = new RewardStat();
        tempRewardStat.Exp = monsterJson[monsterSet][sample1Num]["exp"].AsInt;
        tempRewardStat.Gold = monsterJson[monsterSet][sample1Num]["gold"].AsInt;

        Monster tempMonsterComp = monsterSample1.GetComponent<Monster>();
        tempMonsterComp.InitMonster(sample1Num, tempBattleStat, tempRewardStat);

        // 몬스터 샘플 2 할당
        monsterSample2 = (GameObject)Instantiate(Resources.Load("MonsterPrefabs/" + monsterSet + "/" + sample2Num));

        // 스탯 설정
        tempBattleStat = new BattleStat();

        tempBattleStat.Level = monsterJson[monsterSet][sample2Num]["level"].AsInt;
        tempBattleStat.BaseHealthMax = monsterJson[monsterSet][sample2Num]["hp"].AsFloat;
        tempBattleStat.BaseDefence = monsterJson[monsterSet][sample2Num]["def"].AsFloat;
        tempBattleStat.BaseAttack = monsterJson[monsterSet][sample2Num]["atk"].AsFloat;
        tempBattleStat.BaseAttackSpeed = monsterJson[monsterSet][sample2Num]["atkspeed"].AsFloat;
        tempBattleStat.BaseCriticalChance = monsterJson[monsterSet][sample2Num]["critical"].AsFloat;
        tempBattleStat.BaseCriticalDamage = monsterJson[monsterSet][sample2Num]["atkcritical"].AsFloat;
        tempBattleStat.BasePenetrationFixed = monsterJson[monsterSet][sample2Num]["penetration"].AsFloat;
        tempBattleStat.BaseMoveSpeed = monsterJson[monsterSet][sample2Num]["movespeed"].AsFloat;
        tempBattleStat.BaseRange = monsterJson[monsterSet][sample2Num]["range"].AsInt;

        tempRewardStat = new RewardStat();
        tempRewardStat.Exp = monsterJson[monsterSet][sample2Num]["exp"].AsInt;
        tempRewardStat.Gold = monsterJson[monsterSet][sample2Num]["gold"].AsInt;

        tempMonsterComp = monsterSample1.GetComponent<Monster>();
        tempMonsterComp.InitMonster(sample2Num, tempBattleStat, tempRewardStat);

        return;
    }
}
