#define DEBUG_ADV
//#define DEBUG_GEN_ADV

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using SimpleJSON;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

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
    public int activeSpAdvCnt;
    public int playerSpAdvIndex = -1;

     public List<GameObject> travelersDisabled;
    public List<GameObject> travelersEnabled;

    public List<GameObject> adventurersEnabled;
    public List<GameObject> adventurersDisabled;

    public List<GameObject> specialAdventurers;
   
	public Queue<GameObject> trvEnterQ;
    public Queue<GameObject> advEnterQ;
    public Queue<GameObject> spAdvEnterQ;


    public int corporateNum = 1;
    public List<float> popular;
    #endregion
    public JSONNode items;
    #endregion

    #region SceneDatas
    string sceneName;
    JSONNode sceneData;

    int travelerMax = 0; // 전체 방문객 수 /2 + 전체 방문객 수 %2.
    int adventurerMax = 0;
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

	int stageIndex = 0;
	int stageMax = 0;

    // 모험가, 관광객 생성용 비율 저장 리스트
    List<KeyValuePair<string, float>> raceRatios;
    List<KeyValuePair<string, float>> trvWealthRatios;
    List<WealthRatioByLevel> advWealthRatios; // 이거 어떻게 저장?

    // 보스 사냥 이벤트
    public delegate void BossRaidEvent();
    public BossRaidEvent BossRaidCallEventHandler;
    public BossRaidEvent PlayerOrderedRaidEventHandler;


    // 사냥터 정보
    int huntingAreaCount = 0;
    // 보스지역 정보
    int bossAreaCount = 0;
    // 사냥터 개방에 따라 필요해질 데이터 저장
    List<ProgressInformation> progressInformations;

    public JSONNode advStatData;
    JSONNode advWealthRatioData;

    Dictionary<string, JSONNode> spAdvStatDatas;

    JSONNode spAdvSummary;

    JSONNode namesData;
    JSONNode trvInitialGoldData;

    JSONNode desireData;

    // Save!
    #region BossPhase
    Skirmish curSkirmish;
    //// 보스 페이즈인가?
    public bool isBossPhase;
    //// 응답한 일선 모험가 수
    public int responsedSpAdvCnt;
    //// 보스 레이드 준비 타이머 끝났나?
    public bool isBossRaidPrepTimeOver;
    public float bossRaidPrepWaitedTime;
    //// 토너먼트 끝?
    public float retryTimeLeft;
    public bool canCallBossRaid;
    // Save!

    #endregion

    private int CurGuestMax //현재 맵에 들어올 수 있는 모험가+관광객 최대치
    {
        get
        {
            int total = 0;
            for (int i = 0; i <= CombatAreaManager.Instance.PublicHuntingAreaIndex; i++)
            {
                total += progressInformations[i].guestCapacity;
            }

            return total;
        }
    }

    private int CurAdvMaxPerHuntingArea // 한 사냥터당 배당된 모험가(최대)수
    {
        get
        {
            //Debug.Log("publicHA Idx : " + CombatAreaManager.Instance.PublicHuntingAreaIndex);
            return CurGuestMax / ((CombatAreaManager.Instance.PublicHuntingAreaIndex + 1) * 2); // Int연산인데 버려지는 건? 버려지면 나머지는 traveler에서 보충해도 되긴함.
        }
    }

    private int CurAdvMax // 최대로 들어올 수 있는 모험가 수
    {
        get
        {
            return CurAdvMaxPerHuntingArea * (CombatAreaManager.Instance.PublicHuntingAreaIndex + 1); // 사냥터별 수가 있고 정수 때문에 이렇게 계산.
        }
    }

    private int CurTrvMax // 최대로 들어올 수 있는 관광객 수. 나머지는 무조건 관광객이 가져감.
    {
        get
        {
            return CurGuestMax - CurAdvMax;
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

        //Debug.Log("mapName" + sceneName);
        ReadDatasFromJSON();

        wait = new WaitForSeconds(0.11f);
        countLogWait = new WaitForSeconds(3.0f);

		//게임 해상도 설정
		Screen.SetResolution(1920, 1080, true);
        //isBossPhase = false;
        //skirmishSurvivors = new List<SpecialAdventurer>();
        //skirmishLosers = new List<SpecialAdventurer>();
        //skirmishBracket = new List<SpecialAdventurer>();
    }

    void Start()
    {
        SetMap();

        // Scene 정보 세팅
        SetSceneData(sceneData);
		stageMax = GetMap().GetLayerCount();
        if (SaveLoadManager.Instance.isLoadedGame == false)
            SetCombatAreas(sceneData);

        travelersEnabled = new List<GameObject>();
        travelersDisabled = new List<GameObject>();

        adventurersDisabled = new List<GameObject>();
        adventurersEnabled = new List<GameObject>();

        specialAdventurers = new List<GameObject>();
        activeSpAdvCnt = 0;


        trvEnterQ = new Queue<GameObject>();
        advEnterQ = new Queue<GameObject>();
        spAdvEnterQ = new Queue<GameObject>();
        //Debug.Log("traveler_max : " + travelerMax);
        // Scene별로 미리 정의된 관광객의 최대 수에 따라 생성
        for (int i = 0; i < travelerMax; i++)
        {
            string prefabPath = "CharacterPrefabs/Traveler_test";
            GameObject go = (GameObject)Resources.Load(prefabPath);

            // 생성만 해놓고 비활성화
            go.SetActive(false);

            // List에 추가
            travelersDisabled.Add(Instantiate(go));
            go.transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);
            travelersDisabled[i].transform.parent = GameObject.FindGameObjectWithTag("Characters").transform;

            travelersDisabled[i].GetComponent<Traveler>().index = i;
            travelersDisabled[i].GetComponent<Traveler>().prefabPath = prefabPath;
            //Debug.Log("path : " + travelers[i].GetComponent<Traveler>().prefabPath);
            // Debug.Log("character instantiate - " + i);
        }

        for (int i = 0; i < adventurerMax; i++)
        {
            string prefabPath = "CharacterPrefabs/Adventurer_test";
            GameObject go = (GameObject)Resources.Load(prefabPath);
            //go.GetComponent<Adventurer>().SetAttackEffect((GameObject)Instantiate(Resources.Load("EffectPrefabs/Default_AttackEffect")));

            // 생성만 해놓고 비활성화
            go.SetActive(false);

            // List에 추가
            adventurersDisabled.Add(Instantiate(go));
            go.transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);
            adventurersDisabled[i].transform.parent = GameObject.FindGameObjectWithTag("Characters").transform;
            Adventurer tempAdventurer = adventurersDisabled[i].GetComponent<Adventurer>();
            tempAdventurer.index = i;
            tempAdventurer.prefabPath = prefabPath;
            SetAdvDefaultEffects(tempAdventurer);
            //tempAdventurer.SetDamageText((GameObject)Instantiate(Resources.Load("UIPrefabs/Battle/DamageText")));
            //tempAdventurer.SetHealText((GameObject)Instantiate(Resources.Load("UIPrefabs/Battle/HealText")));
            //tempAdventurer.SetHealEffect((GameObject)Instantiate(Resources.Load("EffectPrefabs/Default_HealEffect")));
            // Debug.Log("character instantiate - " + i);
        }
        StartCoroutine(TrvEnter());
        StartCoroutine(AdvEnter());
        StartCoroutine(SpAdvEnter());

		
#if DEBUG_ADV
        //GenAndEnqueueSingleAdventurer(1, 10);
        //GenAndEnqueueSpecialAdvenuturer("Hana", 85);
        //GenAndEnqueueSpecialAdvenuturer("Iris", 26);
        //GenAndEnqueueSpecialAdvenuturer("Maxi", 25);
        //GenAndEnqueueSpecialAdvenuturer("Murat", 25);
        //GenAndEnqueueSpecialAdvenuturer("Nyang", 25);
        //GenAndEnqueueSpecialAdvenuturer("Wal", 25);
        //GenAndEnqueueSpecialAdvenuturer("Yeonhwa", 25);
        //GenAndEnqueueSpecialAdvenuturer("OldMan", 5);
        //GenAndEnqueueSingleTraveler();
#endif
        //StartCoroutine(GCcall())
        GenerateSpecialAdventurers(sceneData);
        StartCoroutine(LateStart());
    }

    public void SetAdvDefaultEffects(Adventurer inputAdventurer)
    {
        inputAdventurer.SetAttackEffect((GameObject)Instantiate(Resources.Load("EffectPrefabs/Default_AttackEffect")));
        inputAdventurer.SetDefaultEffects();
    }

    IEnumerator LateStart()
    {
        yield return null;
#if DEBUG_ADV
        //DebugHuntingArea();
#endif
        if (SaveLoadManager.Instance.isLoadedGame == false)
            CombatAreaManager.Instance.InitCombatAreas();
        SaveLoadManager.Instance.InstantiateFromSave();
    }

    private void ReadDatasFromJSON()
    {
        //scene 정보 로드
        TextAsset sceneTxt = Resources.Load<TextAsset>("SceneData/scenedata");
        sceneData = JSON.Parse(sceneTxt.ToString());

        // 이름 프리셋 로드
        TextAsset nameTxt = Resources.Load<TextAsset>("Characters/names");
        namesData = JSON.Parse(nameTxt.ToString());

        // 욕구 관련 데이터(초기값, 틱당 리젠값) 로드
        TextAsset desireTxt = Resources.Load<TextAsset>("Characters/desiredata");
        desireData = JSON.Parse(desireTxt.ToString());

        // 계층별 초기 소지금 데이터 로드
        TextAsset trvInitialGoldTxt = Resources.Load<TextAsset>("Characters/travelerInitialGold");
        trvInitialGoldData = JSON.Parse(trvInitialGoldTxt.ToString());

        // Adventurer 데이터 로드(레벨에 따른 스탯 등)
        TextAsset advStatTxt = Resources.Load<TextAsset>("Characters/adventurerstat");
        advStatData = JSON.Parse(advStatTxt.ToString());

        // SpecialAdventurer 데이터 로드(이름 및 메타데이터)
        TextAsset spAdvSummaryTxt = Resources.Load<TextAsset>("Characters/SpAdv_Summary");
        spAdvSummary = JSON.Parse(spAdvSummaryTxt.ToString());

        // SpecialAdventurer별 스탯 데이터 로드
        spAdvStatDatas = new Dictionary<string, JSONNode>();

        int sceneNumber = int.Parse(SceneManager.GetActiveScene().name);
        for (int i = 0; i < sceneData["scene"][sceneNumber]["specialadventurers"].Count; i++)
        {
            string spAdvName = sceneData["scene"][sceneNumber]["specialadventurers"][i];
            string battleStatFileName = spAdvSummary[spAdvName]["BattleStatJSON"];

            //Debug.Log("name:" + battleStatFileName);
            TextAsset spAdvStatTxt = Resources.Load<TextAsset>("Characters/SpecialAdventurers/" + battleStatFileName);
            spAdvStatDatas.Add(spAdvName, JSON.Parse(spAdvStatTxt.ToString()));
        }

        // 레벨에 따른 계층 비율 데이터 로드
        TextAsset advWealthRatioText = Resources.Load<TextAsset>("Characters/adventurerWealthRatio");
        advWealthRatioData = JSON.Parse(advWealthRatioText.ToString());

        // 아이템 목록 로드
        TextAsset itemText = Resources.Load<TextAsset>("Items/items");
        items = JSON.Parse(itemText.ToString());
    }
    #endregion

    #region Generate Characters
    // 모험가 입장 코루틴
    // 이거에 i 값을 던져주면 활성화 문제는 해결됨.
    
    IEnumerator TEnter()
    {
        for (int i = 0; i < travelerMax; i++)
        {
            //yield return wait;
            yield return null;
            travelersDisabled[i].SetActive(true);
        }
    }

    //private Stat GenStat() // 관광객용
    //{
    //    Stat tempStat = new Stat();
    //    if (Random.Range(0, 2) == 0)
    //    {
    //        tempStat.gender = Gender.Male;
    //        tempStat.actorName = namesData["names"]["malename"][Random.Range(0, namesData["malename"].Count)];
    //    }
    //    else
    //    {
    //        tempStat.gender = Gender.Female;
    //        tempStat.actorName = namesData["ㄴnames"]["femalename"][Random.Range(0, namesData["malename"].Count)];
    //    }
    //    // 임시
    //    tempStat.explanation = "";

    //    tempStat.job = JobType.Traveler;
    //    tempStat.race = GetRandomRace();
    //    tempStat.wealth = GetRandomWealth();
    //    tempStat.gold = GetRandomInitialGold(tempStat.wealth);

    //    SetDesires(ref tempStat, JobType.Traveler);

    //    return tempStat;
    //}
    private Stat GenStat(Traveler trv) // 관광객용
    {
        Stat tempStat = trv.stat;
        if (Random.Range(0, 2) == 0)
        {
            tempStat.gender = Gender.Male;
            tempStat.actorName = namesData["names"]["malename"][Random.Range(0, namesData["malename"].Count)];
        }
        else
        {
            tempStat.gender = Gender.Female;
            tempStat.actorName = namesData["ㄴnames"]["femalename"][Random.Range(0, namesData["malename"].Count)];
        }
        // 임시
        tempStat.explanation = "";

        tempStat.job = JobType.Traveler;
        tempStat.race = GetRandomRace();
        tempStat.wealth = GetRandomWealth();
        tempStat.gold = GetRandomInitialGold(tempStat.wealth);

        SetDesires(ref tempStat, JobType.Traveler);

        return tempStat;
    }

    //private Stat GenStat(int level) // 모험가용
    //{
    //    Stat tempStat = new Stat();
    //    if (Random.Range(0, 2) == 0)
    //    {
    //        tempStat.gender = Gender.Male;
    //        tempStat.actorName = namesData["names"]["malename"][Random.Range(0, namesData["malename"].Count)];
    //    }
    //    else
    //    {
    //        tempStat.gender = Gender.Female;
    //        tempStat.actorName = namesData["names"]["femalename"][Random.Range(0, namesData["malename"].Count)];
    //    }
    //    // 임시
    //    tempStat.explanation = "";

    //    tempStat.job = JobType.Adventurer;
    //    tempStat.race = GetRandomRace();
    //    tempStat.wealth = GetRandomWealth(level);
    //    tempStat.gold = advStatData[level - 1]["gold"].AsInt;

    //    SetDesires(ref tempStat, JobType.Adventurer);

    //    return tempStat;
    //}

    private Stat GenStat(Adventurer adv, int level) // 모험가용
    {
        Stat tempStat = adv.stat;
        if (Random.Range(0, 2) == 0)
        {
            tempStat.gender = Gender.Male;
            tempStat.actorName = namesData["names"]["malename"][Random.Range(0, namesData["malename"].Count)];
        }
        else
        {
            tempStat.gender = Gender.Female;
            tempStat.actorName = namesData["names"]["femalename"][Random.Range(0, namesData["malename"].Count)];
        }
        // 임시
        tempStat.explanation = "";

        tempStat.job = JobType.Adventurer;
        tempStat.race = GetRandomRace();
        tempStat.wealth = GetRandomWealth(level);
        tempStat.gold = advStatData[level - 1]["gold"].AsInt;

        SetDesires(ref tempStat, JobType.Adventurer);

        return tempStat;
    }

    private Stat GenStat(SpecialAdventurer spAdv, string name, int level) // 일선 모험가용
    {
        Stat tempStat = spAdv.stat;

        tempStat.actorName = spAdvSummary[name]["Stat"]["Name"];
        if (spAdvSummary[name]["Stat"]["Gender"] == "Male")
            tempStat.gender = Gender.Male;
        else
            tempStat.gender = Gender.Female;
        // 임시
        tempStat.explanation = spAdvSummary[name]["Stat"]["Explanation"];

        tempStat.job = JobType.SpecialAdventurer;
        string tempStr = spAdvSummary[name]["Stat"]["Race"];

        switch (tempStr)
        {
            case "Human":
                tempStat.race = RaceType.Human;
                break;
            case "Elf":
                tempStat.race = RaceType.Elf;
                break;
            case "Dwarf":
                tempStat.race = RaceType.Dwarf;
                break;
            case "Orc":
                tempStat.race = RaceType.Orc;
                break;
            case "Dog":
                tempStat.race = RaceType.Dog;
                break;
            case "Cat":
                tempStat.race = RaceType.Cat;
                break;
            default:
                tempStat.race = RaceType.Human;
                break;
        }
        tempStat.wealth = WealthType.Upper;
        tempStat.gold = spAdvStatDatas[name][level - 1]["gold"].AsInt;

        SetDesires(ref tempStat, JobType.Adventurer); // 어차피 같으니 그냥 Adventurer로 넣어줌.

        //Debug.Log("name :" + tempStat.name + " race :" + tempStat.race + " gender :" + tempStat.gender);

        return tempStat;
    }


    private RaceType GetRandomRace()
    {
        float num = Random.Range(0.0f, 1.0f);
        int i;
        float total = 0.0f;

#if DEBUG_GEN_ADV
        Debug.Log("[GetRandomRace] num: " + num);
#endif

        for (i = 0; i < raceRatios.Count; i++)
        {
#if DEBUG_GEN_ADV
            Debug.Log("[GetRandomRace] raceRatio.Key: " + raceRatios[i].Key);
            Debug.Log("[GetRandomRace] raceRatio.Value: " + raceRatios[i].Value);
           
#endif
            total += raceRatios[i].Value;
            if (num <= total)
                break;
        }
#if DEBUG_GEN_ADV
        Debug.Log("[GetRandomRace] i: " + i);
#endif

        switch (raceRatios[i].Key)
        {
            case "human":
                return RaceType.Human;
            case "elf":
                return RaceType.Elf;
            case "dwarf":
                return RaceType.Dwarf;
            case "orc":
                return RaceType.Orc;
            case "dog":
                return RaceType.Dog;
            case "cat":
                return RaceType.Cat;
            default:
                return RaceType.Human;
        }
    }

    private WealthType GetRandomWealth() //모험가용
    {
        float num = Random.Range(0.0f, 1.0f);
        int i;
        float total = 0.0f;

        //Debug.Log("num : " + num);

        for (i = 0; i < trvWealthRatios.Count; i++)
        {
            total += trvWealthRatios[i].Value;
            //Debug.Log(i + " total : " + total);
            if (num <= total)
                break;
        }


		if (i >= trvWealthRatios.Count)
			i = trvWealthRatios.Count - 1;

        //Debug.Log("final idx : " + i);
        switch (trvWealthRatios[i].Key)
        {
            case "lower":
                return WealthType.Lower;
            case "middle":
                return WealthType.Middle;
            case "upper":
                return WealthType.Upper;
            default:
                return WealthType.Lower;
        }
    }

    private WealthType GetRandomWealth(int level) // 모험가용 계층 설정 메서드
    {
        int i;
        WealthRatioByLevel temp;

        // 레벨에 맞는 계층비율 찾기.
        for (i = 0; i < advWealthRatios.Count; i++)
        {
            if (level >= advWealthRatios[i].levelMin && level <= advWealthRatios[i].levelMax)
            {
                break;
            }
        }
        temp = advWealthRatios[i];

        // 비율에 따라 랜덤으로 계층 설정
        float num = Random.Range(0.0f, 1.0f);

        float total = 0.0f;

        for (i = 0; i < temp.wealthRatio.Count; i++)
        {
            total += temp.wealthRatio[i].Value;
            if (num <= total)
                break;
        }

        switch (temp.wealthRatio[i].Key)
        {
            case "lower":
                return WealthType.Lower;
            case "middle":
                return WealthType.Middle;
            case "upper":
                return WealthType.Upper;
            default:
                return WealthType.Lower;
        }
    }

    private int GetRandomInitialGold(WealthType wealth)
    {
        switch (wealth)
        {
            case WealthType.Lower:
                return Random.Range(trvInitialGoldData["wealth"]["lower"]["min"].AsInt, trvInitialGoldData["wealth"]["lower"]["max"].AsInt + 1);
            case WealthType.Middle:
                return Random.Range(trvInitialGoldData["wealth"]["middle"]["min"].AsInt, trvInitialGoldData["wealth"]["middle"]["max"].AsInt + 1);
            case WealthType.Upper:
                return Random.Range(trvInitialGoldData["wealth"]["upper"]["min"].AsInt, trvInitialGoldData["wealth"]["upper"]["max"].AsInt + 1);
            default:
                return 0;
        }
    }

    private void SetDesires(ref Stat inputStat, JobType jobType)
    {
        AddDesire(ref inputStat, DesireType.Thirsty, jobType);
        AddDesire(ref inputStat, DesireType.Hungry, jobType);
        AddDesire(ref inputStat, DesireType.Sleep, jobType);
        AddDesire(ref inputStat, DesireType.Tour, jobType);
        AddDesire(ref inputStat, DesireType.Convenience, jobType);
        AddDesire(ref inputStat, DesireType.Fun, jobType);
        AddDesire(ref inputStat, DesireType.Equipment, jobType);
        AddDesire(ref inputStat, DesireType.Health, jobType);
    }

    // 욕구 1개 추가. 수치는 JSON에 저장된 min/max 기반으로 랜덤돌림.
    private void AddDesire(ref Stat inputStat, DesireType desireType, JobType jobType)
    {
#if DEBUG_GEN_ADV
        Debug.Log("AddDesire() : " + desireType);
#endif
        const float desireTickBetween = 1.0f;
        const float desireTickMult = 1.0f;

        string typeStr;
        switch (desireType)
        {
            case DesireType.Thirsty:
                typeStr = "thirsty";
                break;
            case DesireType.Hungry:
                typeStr = "hungry";
                break;
            case DesireType.Sleep:
                typeStr = "sleep";
                break;
            case DesireType.Tour:
                typeStr = "tour";
                break;
            case DesireType.Convenience:
                typeStr = "convenience";
                break;
            case DesireType.Fun:
                typeStr = "fun";
                break;
            case DesireType.Equipment:
                typeStr = "equipment";
                break;
            case DesireType.Health:
                typeStr = "health";
                break;
            default:
                typeStr = "";
                break;
        }

        float initialMin = desireData[(int)jobType]["initialValue"][typeStr]["min"].AsFloat;
        float initialMax = desireData[(int)jobType]["initialValue"][typeStr]["max"].AsFloat;
        float regenMin = desireData[(int)jobType]["regenValue"][typeStr]["min"].AsFloat;
        float regenMax = desireData[(int)jobType]["regenValue"][typeStr]["max"].AsFloat;

        inputStat.AddDesire(new DesireBase(desireType, Random.Range(initialMin, initialMax), Random.Range(regenMin, regenMin), desireTickMult, desireTickBetween, null));
    }

    IEnumerator TrvEnter()
    {
        GameObject temp;
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 10.0f)); // 임시 수치
            if (trvEnterQ.Count > 0)
            {
                temp = trvEnterQ.Dequeue();
                //temp.GetComponent<Traveler>().ResetBattleStat();
                temp.GetComponent<Traveler>().index = travelersEnabled.Count;
                temp.SetActive(true);
                travelersEnabled.Add(temp);
            }
        }
    }

    // 큐에 있는 모험가 순차적으로 입장시킴.
    IEnumerator AdvEnter()
    {
        GameObject temp;
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 10.0f)); // 임시 수치
            if (advEnterQ.Count > 0)
            {
                temp = advEnterQ.Dequeue();
                temp.GetComponent<Adventurer>().ResetBattleStat();
                temp.GetComponent<Adventurer>().index = adventurersEnabled.Count;
                temp.SetActive(true);
                adventurersEnabled.Add(temp);
            }
        }
    }

    private IEnumerator SpAdvEnter()
    {
        GameObject temp;
        int sceneNumber = int.Parse(SceneManager.GetActiveScene().name);

        while (activeSpAdvCnt < sceneData["scene"][sceneNumber]["specialadventurers"].Count)
        {
            if (spAdvEnterQ.Count > 0)
            {
                temp = spAdvEnterQ.Dequeue();
                //temp.GetComponent<SpecialAdventurer>().ResetBattleStat();
                temp.SetActive(true);
                activeSpAdvCnt++;
            }
            yield return new WaitForSeconds(3.0f); // 임시 수치
        }
    }
    private void GenAndEnqueueSingleTraveler()
    {
        GameObject temp = travelersDisabled[travelersDisabled.Count - 1];
        Traveler tempTraveler = temp.GetComponent<Traveler>();

        tempTraveler.InitTraveler();
        GenStat(tempTraveler);
		tempTraveler.stat.SetOwner(tempTraveler);
        tempTraveler.ResetToReuse();
        travelersDisabled.RemoveAt(travelersDisabled.Count - 1);

        trvEnterQ.Enqueue(temp);
    }

    // 모험가 하나 생성해서 큐에 집어넣음
    private void GenAndEnqueueSingleAdventurer(int minLevel, int maxLevel) // 모험가 하나 생성하고 큐에 집어넣음.
    {
        GameObject temp = adventurersDisabled[adventurersDisabled.Count - 1];
        Adventurer tempAdventurer = temp.GetComponent<Adventurer>();
        //adventurersDisabled.RemoveAt(adventurersDisabled.Count - 1); // 객체 풀에서 빼서 씀.

        int advLevel = Random.Range(minLevel, maxLevel + 1);
        BattleStat tempBattleStat = GenBattleStat(advLevel);
        //Stat tempStat = GenStat(advLevel);
        RewardStat tempRewardStat = GenRewardStat(advLevel);

        //Debug.Log("Adv " + tempStat.name + " hp: " + tempBattleStat.Health + " atk: " + tempBattleStat.BaseAttack);

        tempAdventurer.InitAdventurer(tempBattleStat, tempRewardStat);
        tempAdventurer.ResetToReuse();
        GenStat(tempAdventurer, advLevel);
		tempAdventurer.stat.SetOwner(tempAdventurer);
		//if (tempAdventurer.stat == null)
		//    Debug.Log("Genned adv's stat is null!");

		adventurersDisabled.RemoveAt(adventurersDisabled.Count - 1); // 객체 풀에서 빼서 씀.
        advEnterQ.Enqueue(temp);
    }

    // 전체 일선 모험가 생성
    public void GenerateSpecialAdventurers(JSONNode aData)
    {
        int sceneNumber = int.Parse(SceneManager.GetActiveScene().name);
        int spAdvLevel = aData["scene"][sceneNumber]["specialadv_level"].AsInt;
        for (int i = 0; i < aData["scene"][sceneNumber]["specialadventurers"].Count; i++)
            GenAndEnqueueSpecialAdvenuturer(aData["scene"][sceneNumber]["specialadventurers"][i], spAdvLevel);
    }

    // 일선모험가 하나 생성해서 큐에 집어넣음. 모험가 이름을 통해 데이터를 불러와서 집어넣음.
    private void GenAndEnqueueSpecialAdvenuturer(string name, int level)
    {
        /*
         * 할배: "OldMan"
         * 맥시밀리안: "Maxi"
         * 아이리스: "Iris"
         * 하나: "Hana"
         * 연화: "Yeonhwa"
         * 뮈라: "Murat"
         * 냥냐리우스: "Nyang"
         * 왈멍멍: "Wal"
         */
        // 아마 고쳐야할 거임
        string prefabPath = "CharacterPrefabs/" + name;
        GameObject go = Instantiate((GameObject)Resources.Load(prefabPath));
        //go.GetComponent<Adventurer>().SetAttackEffect((GameObject)Instantiate(Resources.Load("EffectPrefabs/Default_AttackEffect")));

        // 생성만 해놓고 비활성화
        go.SetActive(false);

        // List에 추가
        specialAdventurers.Add(go);
        go.transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);
        go.transform.parent = GameObject.FindGameObjectWithTag("Characters").transform;
        SpecialAdventurer tempSpAdv = go.GetComponent<SpecialAdventurer>();

        SetSpAdvEffects(tempSpAdv, name);

        //Save용
        tempSpAdv.index = specialAdventurers.Count - 1;
        tempSpAdv.prefabPath = prefabPath;
        tempSpAdv.nameKey = name;

        // Debug.Log("character instantiate - " + i);
        BattleStat tempBattleStat = GenBattleStat(name, level);
        //Stat tempStat = GenStat(name, level);
        RewardStat tempRewardStat = GenRewardStat(level);

        //Debug.Log("Adv " + tempStat.name + " hp: " + tempBattleStat.Health + " atk: " + tempBattleStat.BaseAttack);
        //tempBattleStat.ResetBattleStat();
        //int skillID = spAdvSummary[name]["SkillID"].AsInt;

        //Debug.Log(tempStat.name + " hp: " + tempBattleStat.Health + " atk: " + tempBattleStat.BaseAttack);

        tempSpAdv.InitSpecialAdventurer(tempBattleStat, tempRewardStat, name);		
        GenStat(tempSpAdv, name, level);
		tempSpAdv.stat.SetOwner(tempSpAdv);
		spAdvEnterQ.Enqueue(go);
    }

    public void SetSpAdvEffects(SpecialAdventurer spAdv, string name)
    {
        string attackEffectFileName = "EffectPrefabs/" + name + "_AttackEffect";
        spAdv.SetAttackEffect((GameObject)Instantiate(Resources.Load(attackEffectFileName)));
        spAdv.SetDefaultEffects();
    }

    public BattleStat GenBattleStat(int level) // 레벨에 따라 BattleStat 생성. JSON에서 읽은 데이터로 생성함.
    {
        BattleStat tempBattleStat = new BattleStat();

        tempBattleStat.Level = level;
        tempBattleStat.BaseHealthMax = advStatData[level - 1]["health"].AsFloat;
        tempBattleStat.BaseDefence = advStatData[level - 1]["defense"].AsFloat;
        tempBattleStat.BaseAvoid = advStatData[level - 1]["avoid"].AsFloat;
        tempBattleStat.BaseAttack = advStatData[level - 1]["attack"].AsFloat;
        tempBattleStat.BaseAttackSpeed = advStatData[level - 1]["attackspeed"].AsFloat;
        tempBattleStat.BaseCriticalChance = advStatData[level - 1]["criticalchance"].AsFloat;
        tempBattleStat.BaseCriticalDamage = advStatData[level - 1]["criticalattack"].AsFloat;
        tempBattleStat.BasePenetrationFixed = advStatData[level - 1]["penetration"].AsFloat;
        tempBattleStat.BaseMoveSpeed = advStatData[level - 1]["movespeed"].AsFloat;
        tempBattleStat.BaseAttackRange = advStatData[level - 1]["attackrange"].AsInt;

        tempBattleStat.NextExp = advStatData[level - 1]["exp"].AsInt;
        tempBattleStat.ownerType = "Adventurer";

        return tempBattleStat;
    }

    public BattleStat GenBattleStat(string name, int level) // 레벨에 따라 BattleStat 생성. JSON에서 읽은 데이터로 생성함.
    {
        BattleStat tempBattleStat = new BattleStat();

        tempBattleStat.Level = level;
        tempBattleStat.BaseHealthMax = spAdvStatDatas[name][level - 1]["health"].AsFloat;
        tempBattleStat.BaseDefence = spAdvStatDatas[name][level - 1]["defense"].AsFloat;
        tempBattleStat.BaseAvoid = spAdvStatDatas[name][level - 1]["avoid"].AsFloat;
        tempBattleStat.BaseAttack = spAdvStatDatas[name][level - 1]["attack"].AsFloat;
        tempBattleStat.BaseAttackSpeed = spAdvStatDatas[name][level - 1]["attackspeed"].AsFloat;
        tempBattleStat.BaseCriticalChance = spAdvStatDatas[name][level - 1]["criticalchance"].AsFloat;
        tempBattleStat.BaseCriticalDamage = spAdvStatDatas[name][level - 1]["criticalattack"].AsFloat;
        tempBattleStat.BasePenetrationFixed = spAdvStatDatas[name][level - 1]["penetration"].AsFloat;
        tempBattleStat.BaseMoveSpeed = spAdvStatDatas[name][level - 1]["movespeed"].AsFloat;
        tempBattleStat.BaseAttackRange = spAdvStatDatas[name][level - 1]["attackrange"].AsInt;

        tempBattleStat.NextExp = spAdvStatDatas[name][level - 1]["exp"].AsInt;
        tempBattleStat.ownerType = name;

        //Debug.Log("[GenBattleStat] " + name + ", hp : " + tempBattleStat.BaseHealthMax + ", atk : " + tempBattleStat.BaseAttack + ", atkspeed : " + tempBattleStat.AttackSpeed);

        return tempBattleStat;
    }

    public RewardStat GenRewardStat(int level) // 레벨에 따라 RewardStat 생성
    {
        RewardStat tempRewardStat = new RewardStat();

        tempRewardStat.Exp = level * 50;
        tempRewardStat.Gold = 5 * (Mathf.RoundToInt(level * 0.9f) + 20);

        return tempRewardStat;
    }

    private void GenerateTravelers(int needed)
    {
        for(int i = 0; i < needed; i++)
        {
            GenAndEnqueueSingleTraveler();
        }
    }

    private void ResetAdvStatistics()
    {
        for (int i = 0; i <= CombatAreaManager.Instance.PublicHuntingAreaIndex; i++)
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

        List<GameObject> advEnterQConveted = advEnterQ.ToList();
        for (int i = 0; i < advEnterQConveted.Count; i++)
        {
            for (int j = 0; j < progressInformations.Count; j++)
            {
                if (advEnterQConveted[i].GetComponent<Adventurer>().Level <= progressInformations[j].maxLevel
                    && advEnterQConveted[i].GetComponent<Adventurer>().Level >= progressInformations[j].minLevel)
                    progressInformations[j].curAdvNum++;
            }
        }
    }
    
    private void GenerateAdventurers(int needed)
    {
        //List<ProgressInformation> tempList = progressInformations.CopyTo()
        CompileAdvStatistics();

        List<ProgressInformation> tempArr = new List<ProgressInformation>();

        for (int i = 0; i <= CombatAreaManager.Instance.PublicHuntingAreaIndex; i++)
            tempArr.Add(progressInformations[i]);

        tempArr.Sort(CompareByCurHuntingAreaAsc);

        // PublicHuntingAreaIndex 기준으로 새 모험가 생성
        int totalAdv = CurAdvMaxPerHuntingArea * (CombatAreaManager.Instance.PublicHuntingAreaIndex + 1);
        tempArr.Add(new ProgressInformation(CurAdvMaxPerHuntingArea)); // 왜 필요함?

        tempArr.Add(new ProgressInformation(CurAdvMax));
        //Debug.Log("Before");
        //for (int i = 0; i <= CombatAreaManager.Instance.PublicHuntingAreaIndex + 1; i++)
        //{
        //    Debug.Log(i + " curAdvNum : " + tempArr[i].curAdvNum + ", min : " + tempArr[i].minLevel + " max : " +tempArr[i].maxLevel);
        //}

        int generated = 0;

        //for (int i = 0; i < tempArr.Count - 1; i++)
        for (int i = 0; i <= CombatAreaManager.Instance.PublicHuntingAreaIndex; i++)
        {
            while (tempArr[i].curAdvNum < tempArr[i + 1].curAdvNum)
            {
                for (int j = 0; j <= i; j++)
                {
                    GenAndEnqueueSingleAdventurer(tempArr[j].minLevel, tempArr[j].maxLevel);
                    generated++;
                    tempArr[j].curAdvNum++;

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
    #endregion

    #region Stage Progress
    // 보스 레이드 준비 끝났나?
    public bool IsBossRaidPrepEnded
    {
        get
        {
            return (responsedSpAdvCnt == specialAdventurers.Count) || isBossRaidPrepTimeOver;
        }
    }
    #endregion

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

    // 사냥터 및 보스지역 건설
    private void SetCombatAreas(JSONNode aData)
    {
        int sceneNumber = int.Parse(SceneManager.GetActiveScene().name);
        int areaNum, tileX, tileY;
        GameObject pointTile;

        // 사냥터 건설
        for (int i = 0; i < huntingAreaCount; i++)
        {
            areaNum = aData["scene"][sceneNumber]["huntingArea"][i]["huntingAreaNum"].AsInt;
            tileX = aData["scene"][sceneNumber]["huntingArea"][i]["x"].AsInt;
            tileY = aData["scene"][sceneNumber]["huntingArea"][i]["y"].AsInt;
            pointTile = GetTileLayer().GetComponent<TileLayer>().GetTile(tileX, tileY);
            //Debug.Log("haNum : " + areaNum + " ["+tileX + ", " + tileY + "]");
            CombatAreaManager.Instance.ConstructHuntingArea(areaNum, i, pointTile);
        }

        // 보스지역 건설
        for (int i = 0; i < bossAreaCount; i++)
        {
            areaNum = aData["scene"][sceneNumber]["bossArea"][i]["bossAreaNum"].AsInt;
            tileX = aData["scene"][sceneNumber]["bossArea"][i]["x"].AsInt;
            tileY = aData["scene"][sceneNumber]["bossArea"][i]["y"].AsInt;
            pointTile = GetTileLayer().GetComponent<TileLayer>().GetTile(tileX, tileY);
            CombatAreaManager.Instance.ConstructBossArea(areaNum, i, pointTile);
        }
    }

    public void SetCombatAreas()
    {
        SetCombatAreas(sceneData);
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
    public void SetSceneData(JSONNode aData)
    {
        int sceneNumber = int.Parse(SceneManager.GetActiveScene().name);

#if DEBUG_SCENESETTING
        Debug.Log("Scenedata 읽어오기 디버그 시작");
        Debug.Log("현재 씬 : " + sceneNumber);
        Debug.Log(aData["scene"]);
        Debug.Log(aData["scene"][sceneNumber]);
        Debug.Log(aData["scene"][sceneNumber]["traveler_Max"].AsInt);
#endif

#if DEBUG
        //traveler_Max = aData["scene"][sceneNumber]["traveler_Max"].AsInt;
        //adventurer_Max = aData["scene"][sceneNumber]["adventurer_Max"].AsInt;
        complete_Popularity = aData["scene"][sceneNumber]["complete_Popularity"].AsInt;
#endif

        // 지을 수 있는 건물에 대한 정보(몇번째 건물까지 지을 수 있는지. 스테이지에 따라 다름) 로드.
        SetSceneStructureDatas(aData, sceneNumber);

        // 인구 비율에 대한 정보 로드. Guest의 계층, 종족 비율.
        SetSceneRatioDatas(aData, sceneNumber);

        // 맵 입구 정보 로드.
        SetSceneEntrances(aData, sceneNumber);

        // 사냥터 개방될 때마다 전체 수용인원이 몇명 늘어나는지와 각 사냥터의 레벨대를 로드.
        SetSceneProgressInfos(aData, sceneNumber);

#if DEBUG
        //travelerMax = 50;
        //adventurer_Max = 1;
        //specialAdventurer_Max = 800;
#endif
    }



    // 입구 랜덤으로 찾기
    public Tile GetRandomEntrance()
    {
        int rand = Random.Range(0, mapEntranceCount);

        return mapEntrance[rand].GetParent();
    }

    public void TravelersExit(Traveler trv)
    {
        travelersEnabled.RemoveAt(trv.index);
        travelersDisabled.Add(trv.gameObject);
        trv.ResetToReuse();
        trv.gameObject.SetActive(false);

        FillTrvAdvVacancies();
    }

    public void AdventurerExit(Adventurer adv)
    {
        adventurersEnabled.RemoveAt(adv.index);
        adventurersDisabled.Add(adv.gameObject);
        adv.ResetToReuse();
        adv.gameObject.SetActive(false);

        FillTrvAdvVacancies();
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
        playerSpAdvIndex = savedata.playerSpAdvIndex;

        Camera.main.transform.position = new Vector3(savedata.cameraPosition.x, savedata.cameraPosition.y, savedata.cameraPosition.z);
        Camera.main.orthographicSize = savedata.cameraSize;
    }

    // 일단 완성
    // 비활성화 상태로 로드만 해놓음
    public void LoadTravelers(GameSavedata savedata)
    {
        GameObject newObject;
        //GameObject tileMapLayer;

        #region travelersDisabled
        // 할당 해제. 다른 Scene일 수도 있으니.
        foreach (GameObject traveler in travelersDisabled)
        {
            traveler.GetComponent<Traveler>().StopAllCoroutines();
            Destroy(traveler);
        }
        travelersDisabled.Clear();

        // Active 관련 요소는 이야기해보고 결정.
        for (int i = 0; i < savedata.travelersDisabled.Count; i++)
        {
            TravelerData tempData = savedata.travelersDisabled[i];
            //Debug.Log(i + " : " + savedata.travelers[i].prefabPath);
            newObject = (GameObject)Resources.Load(tempData.prefabPath);
            newObject.SetActive(false);

            // List에 추가
            travelersDisabled.Add(Instantiate(newObject));
            travelersDisabled[i].SetActive(false);

            travelersDisabled[i].transform.parent = GameObject.FindGameObjectWithTag("Characters").transform;
            Traveler tempTraveler = travelersDisabled[i].GetComponent<Traveler>();

            //if (savedata.travelersDisabled[i].isActive)
            //{
            //    InitLoadedTraveler(travelersDisabled[i], savedata.travelersDisabled[i]);
            //    travelersDisabled[i].GetComponent<Traveler>().InitTraveler(tempData.stat);
            //    //travelers[i].SetActive(true);
            //    travelersDisabled[i].GetComponent<Traveler>().isNew = false;
            //}
            //else
            //{
                tempTraveler.index = tempData.index;
                tempTraveler.prefabPath = tempData.prefabPath;
            //}
            //SetLoadedTravelerState(travelers[i], savedata.travelers[i]);
        }
        #endregion

        #region travelersEnabled
        // 할당 해제. 다른 Scene일 수도 있으니.
        foreach (GameObject traveler in travelersEnabled)
        {
            traveler.GetComponent<Traveler>().StopAllCoroutines();
            Destroy(traveler);
        }
        travelersEnabled.Clear();

        for (int i = 0; i < savedata.travelersEnabled.Count; i++)
        {
            TravelerData tempData = savedata.travelersEnabled[i];
            //Debug.Log(i + " : " + savedata.travelers[i].prefabPath);
            newObject = (GameObject)Resources.Load(tempData.prefabPath);
            newObject.SetActive(false);

            // List에 추가
            travelersEnabled.Add(Instantiate(newObject));
            travelersEnabled[i].SetActive(false);

            travelersEnabled[i].transform.parent = GameObject.FindGameObjectWithTag("Characters").transform;
            Traveler tempTraveler = travelersEnabled[i].GetComponent<Traveler>();

            InitLoadedTraveler(travelersEnabled[i], savedata.travelersEnabled[i]);
            travelersEnabled[i].GetComponent<Traveler>().InitTraveler(tempData.stat);
            travelersEnabled[i].GetComponent<Traveler>().isNew = false;

        }
        #endregion

        #region trvEnterQ
        TravelerData inputTrvData;
        Traveler newTraveler;
        while (trvEnterQ.Count > 0)
        {
            GameObject temp = trvEnterQ.Dequeue();
            temp.GetComponent<Traveler>().StopAllCoroutines();
            Destroy(temp);
        }

        while (savedata.trvEnterQ.Count > 0)
        {
            inputTrvData = savedata.trvEnterQ.Dequeue();
            newObject = (GameObject)Resources.Load(inputTrvData.prefabPath);
            newObject.SetActive(false);

            // List에 추가
            GameObject tempObject = Instantiate(newObject);
            tempObject.SetActive(false);
            trvEnterQ.Enqueue(tempObject);

            newTraveler = tempObject.GetComponent<Traveler>();

            tempObject.transform.parent = GameObject.FindGameObjectWithTag("Characters").transform;

            InitLoadedTraveler(tempObject, inputTrvData);
            newTraveler.InitTraveler(inputTrvData.stat);
            //SetLoadedAdventurerState(tempObject, inputAdvData);
        }
        #endregion
    }

    // 모험가 로드. 비활성화 상태로 로드만 해놓음(enemy때문)
    public void LoadAdventurers(GameSavedata savedata)
    {
        GameObject newObject;
        AdventurerData inputAdvData;
        Adventurer newAdventurer;
        //GameObject tileMapLayer;

        #region adventurerEnabled
        // 할당 해제. 다른 Scene일 수도 있으니.
        foreach (GameObject adventurer in adventurersEnabled)
        {
            adventurer.GetComponent<Adventurer>().StopAllCoroutines();
            Destroy(adventurer);
        }
        adventurersEnabled.Clear();

        // Active 관련 요소는 이야기해보고 결정.
        for (int i = 0; i < savedata.adventurersEnabled.Count; i++)
        {
            newObject = (GameObject)Resources.Load(savedata.adventurersEnabled[i].prefabPath);
            newObject.SetActive(false);

            inputAdvData = savedata.adventurersEnabled[i];

            // List에 추가
            adventurersEnabled.Add(Instantiate(newObject));
            adventurersEnabled[i].SetActive(false);

            newAdventurer = adventurersEnabled[i].GetComponent<Adventurer>();

            adventurersEnabled[i].transform.parent = GameObject.FindGameObjectWithTag("Characters").transform;

            InitLoadedAdventurer(adventurersEnabled[i], savedata.adventurersEnabled[i]);
            newAdventurer.isNew = false;
            //adventurersEnabled[i].SetActive(true);
            //SetLoadedAdventurerState(adventurersEnabled[i], savedata.adventurersEnabled[i]);
        }
        #endregion

        #region adventurerDisabled
        foreach (GameObject adventurer in adventurersDisabled)
        {
            adventurer.GetComponent<Adventurer>().StopAllCoroutines();
            Destroy(adventurer);
        }
        adventurersDisabled.Clear();

        //Debug.Log("advCnt : " + savedata.adventurersDisabled.Count);
        for (int i = 0; i < savedata.adventurersDisabled.Count; i++)
        {
            //Debug.Log("i : " + i);
            //Debug.Log("path : " + savedata.adventurersDisabled[i].prefabPath);
            newObject = (GameObject)Resources.Load(savedata.adventurersDisabled[i].prefabPath);
            newObject.SetActive(false);

            inputAdvData = savedata.adventurersDisabled[i];

            // List에 추가
            adventurersDisabled.Add(Instantiate(newObject));
            adventurersDisabled[i].SetActive(false);

            newAdventurer = adventurersDisabled[i].GetComponent<Adventurer>();

            adventurersDisabled[i].transform.parent = GameObject.FindGameObjectWithTag("Characters").transform;

            InitLoadedAdventurer(adventurersDisabled[i], savedata.adventurersDisabled[i]);
            //SetLoadedAdventurerState(adventurersDisabled[i], savedata.adventurersDisabled[i]);
        }
        #endregion

        #region advEnterQ
        while (advEnterQ.Count > 0)
        {
            GameObject temp = advEnterQ.Dequeue();
            temp.GetComponent<Adventurer>().StopAllCoroutines();
            Destroy(temp);
        }

        while (savedata.advEnterQ.Count > 0)
        {
            inputAdvData = savedata.advEnterQ.Dequeue();
            newObject = (GameObject)Resources.Load(inputAdvData.prefabPath);
            newObject.SetActive(false);

            // List에 추가
            GameObject tempObject = Instantiate(newObject);
            tempObject.SetActive(false);
            advEnterQ.Enqueue(tempObject);

            newAdventurer = tempObject.GetComponent<Adventurer>();

            tempObject.transform.parent = GameObject.FindGameObjectWithTag("Characters").transform;

            InitLoadedAdventurer(tempObject, inputAdvData);
            //SetLoadedAdventurerState(tempObject, inputAdvData);
        }
        #endregion

        // TEnter 관련은 아직 저장안함
        // 현 상태에서, 모험가 입장도중 저장하면 다른 모험가가 들어오지 않을 가능성 있음.
        // 어디까지 활성화했나 보고 그 뒤에꺼 이어가면 문제 X.
    }


    // 비활성화 상태로 로드만 해놓음
    public void LoadSpecialAdventurers(GameSavedata savedata)
    {
        #region specialAdventurers
        for (int i = 0; i < specialAdventurers.Count; i++)
        {
            specialAdventurers[i].GetComponent<SpecialAdventurer>().StopAllCoroutines();
            Destroy(specialAdventurers[i]);
        }
        specialAdventurers.Clear();

        for (int i = 0; i < savedata.specialAdventurers.Count; i++)
        {
            string prefabPath = savedata.specialAdventurers[i].prefabPath;
            GameObject newObject = Instantiate((GameObject)Resources.Load(prefabPath));
            //go.GetComponent<Adventurer>().SetAttackEffect((GameObject)Instantiate(Resources.Load("EffectPrefabs/Default_AttackEffect")));

            // 생성만 해놓고 비활성화
            newObject.SetActive(false);

            // List에 추가
            specialAdventurers.Add(newObject);
            newObject.transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);
            newObject.transform.parent = GameObject.FindGameObjectWithTag("Characters").transform;
            SpecialAdventurer tempSpAdv = newObject.GetComponent<SpecialAdventurer>();

            SetSpAdvEffects(tempSpAdv, savedata.specialAdventurers[i].nameKey);
            InitLoadedSpecialAdventurer(newObject, savedata.specialAdventurers[i]);
            if (savedata.specialAdventurers[i].isActive)
                tempSpAdv.isNew = false;
            //newObject.SetActive(savedata.specialAdventurers[i].isActive);
            //SetLoadedAdventurerState(newObject, savedata.specialAdventurers[i]);
        }
        #endregion

        #region spAdvEnterQ

        while (spAdvEnterQ.Count > 0)
        {
            GameObject temp = spAdvEnterQ.Dequeue();
            temp.GetComponent<Adventurer>().StopAllCoroutines();
            Destroy(temp);
        }

        while (savedata.spAdvEnterQ.Count > 0)
        {
            int spAdvIndex = savedata.spAdvEnterQ.Dequeue();
            spAdvEnterQ.Enqueue(specialAdventurers[spAdvIndex]);
        }
        #endregion
    }

    public void SetAdvsEnemy(GameSavedata savedata)
    {
        for (int i = 0; i < adventurersEnabled.Count; i++)
        {
            if (savedata.adventurersEnabled[i].enemy != null)
                SetICombatantEnemy(adventurersEnabled[i].GetComponent<Adventurer>(), savedata.adventurersEnabled[i].enemy);
        }

        for (int i = 0; i < specialAdventurers.Count; i++)
        {
            if (savedata.specialAdventurers[i].enemy != null)
                SetICombatantEnemy(specialAdventurers[i].GetComponent<SpecialAdventurer>(), savedata.specialAdventurers[i].enemy);
        }
    }

    public void SetICombatantEnemy(ICombatant input, CombatantPtr data)
    {
        switch (data.combatantType)
        {
            case ActorType.Monster: // Mob vs Mob는 없음.
                Adventurer tempAdv = input as Adventurer;
                tempAdv.SetEnemy(tempAdv.curHuntingArea.monstersEnabled[data.index].GetComponent<Monster>());
                break;
            case ActorType.BossMonster:
                SpecialAdventurer tempSpAdv = input as SpecialAdventurer;
                tempSpAdv.SetEnemy(tempSpAdv.curBossArea.monstersEnabled[data.index].GetComponent<Monster>());
                break;
            case ActorType.Adventurer:
                input.SetEnemy(adventurersEnabled[data.index].GetComponent<Adventurer>());
                break;
            case ActorType.SpecialAdventurer:
                input.SetEnemy(specialAdventurers[data.index].GetComponent<SpecialAdventurer>());
                break;
        }
    }

    public void ActivateLoadedActors(GameSavedata savedata)
    {
        for (int i = 0; i < travelersEnabled.Count; i++)
            //travelersEnabled[i].SetActive(false);
            travelersEnabled[i].SetActive(savedata.travelersEnabled[i].isActive);
        for (int i = 0; i < adventurersEnabled.Count; i++)
            adventurersEnabled[i].SetActive(true);
        for (int i = 0; i < specialAdventurers.Count; i++)
            specialAdventurers[i].SetActive(savedata.specialAdventurers[i].isActive);

        CombatAreaManager.Instance.ActivateMonsters();
    }

    // Actor정보 로드
    private static void InitLoadedActor(GameObject input, ActorData data)
    {
        Actor newActor = input.GetComponent<Actor>();

        //Debug.Log("data.PrefabPath : " + data.prefabPath);
        newActor.prefabPath = data.prefabPath;
        //newTraveler.index = inputTravelerData.index;
        //TileLayer tileLayer = GetTileLayer(0);

        if (data.isActive)
        {
            
            newActor.SetDestinationTileLoad(data.destinationTile);
            newActor.SetDetinationTileForMoveLoad(data.destinationTileForMove);
            newActor.SetCurTileLoad(data.curTile);
            newActor.SetCurTileForMoveLoad(data.curTileForMove);

            //if(newActor.GetDestinationTile() != null)
            //    Debug.Log(newActor.GetDestinationTile().x + ", " + newActor.GetDestinationTile().y);
            input.transform.position = new Vector3(data.position.x, data.position.y, data.position.z);
            //Debug.Log("loadedPos : [" + input.transform.position.x + ", " + input.transform.position.y + ", " + input.transform.position.z);
            //newActor.StopAllCoroutines();
        }
    }

    // Traveler 정보 로드
    private void InitLoadedTraveler(GameObject input, TravelerData data)
    {
        InitLoadedActor(input, data);

        Traveler newTraveler = input.GetComponent<Traveler>();
        newTraveler.index = data.index;
        //TileLayer tileLayer = GetTileLayer(0);

        if (data.destPlaceIndex != -1)
        {
            switch (data.destPlaceType)
            {
                case PlaceType.Structure:
                    newTraveler.destinationPlace = StructureManager.Instance.rootStructureObject.transform.GetChild(data.destPlaceIndex).GetComponent<Place>();
                    break;
                case PlaceType.HuntingArea:
                    newTraveler.destinationPlace = CombatAreaManager.Instance.huntingAreas[data.destPlaceIndex];
                    break;
                case PlaceType.BossArea:
                    newTraveler.destinationPlace = CombatAreaManager.Instance.bossAreas[data.destPlaceIndex];
                    break;
            }
        }

        SetLoadedActorState(input, data);
    }

    // State 로드해서 넣는 메서드
    private static void SetLoadedActorState(GameObject input, ActorData data)
    {
        Actor newActor = input.GetComponent<Actor>();

        newActor.SetSuperState(data.superState);
        if (data.state == State.MovingToDestination || data.state == State.ApproachingToEnemy)
            newActor.state = State.PathFinding;
        else
            newActor.state = data.state;
    }

    private void InitLoadedAdventurer(GameObject input, AdventurerData data)
    {
        InitLoadedTraveler(input, data);

        Adventurer newAdventurer = input.GetComponent<Adventurer>();
        newAdventurer.index = data.index;
        SetAdvDefaultEffects(newAdventurer);

        //newAdventurer.InitAdventurer(new Stat(data.stat, newAdventurer), data.battleStat, GenRewardStat(data.battleStat.Level));


        //if (data.isActive)
        //{
        // 스탯 세팅
        if (data.stat != null && data.battleStat != null)
        {
            newAdventurer.InitAdventurer(data.stat, data.battleStat, GenRewardStat(data.battleStat.Level));
            newAdventurer.GetBattleStat().Health = data.battleStat.Health;
            //if (data.battleStat != null)
            //    newAdventurer.SetBattleStat(data.battleStat);


            foreach (string skillName in data.skills)
                newAdventurer.AddSkill(skillName);
            foreach (TemporaryEffectData tempEffectData in data.temporaryEffects.Values)
                newAdventurer.AddTemporaryEffect(new TemporaryEffect(tempEffectData));

            if (data.curHuntingArea != -1)
            {
                newAdventurer.curHuntingArea = CombatAreaManager.Instance.huntingAreas[data.curHuntingArea];
                newAdventurer.curHuntingArea.EnterAdventurer(input);
            }
        }
        //}
    }

    //private void SetLoadedAdventurerState(GameObject input, TravelerData data)
    //{
    //    Adventurer newAdv = input.GetComponent<Adventurer>();

    //    newAdv.SetSuperState(data.superState);

    //    if (data.state == State.MovingToDestination || data.state == State.ApproachingToEnemy)
    //        newAdv.curState = State.PathFinding;
    //    else
    //        newAdv.curState = data.state;
    //}

    private void InitLoadedSpecialAdventurer(GameObject input, SpecialAdventurerData data)
    {
        InitLoadedTraveler(input, data);

        SpecialAdventurer newSpecialAdventurer = input.GetComponent<SpecialAdventurer>();
        newSpecialAdventurer.InitSpecialAdventurer(data.stat, data.battleStat, GenRewardStat(data.battleStat.Level), data.nameKey);
        newSpecialAdventurer.GetBattleStat().Health = data.battleStat.Health;

        newSpecialAdventurer.index = data.index;
        newSpecialAdventurer.nameKey = data.nameKey;

        newSpecialAdventurer.willBossRaid = data.willBossRaid;

        //newSpecialAdventurer.InitSpecialAdventurer(new Stat(data.stat, newSpecialAdventurer), data.battleStat, GenRewardStat(data.battleStat.Level), data.nameKey);

        if (data.weapon != null)
            newSpecialAdventurer.EquipWeapon(ItemManager.Instance.CreateItem(data.weapon.itemCategory, data.weapon.itemNum));
        if (data.armor != null)
            newSpecialAdventurer.EquipArmor(ItemManager.Instance.CreateItem(data.armor.itemCategory, data.armor.itemNum));
        if (data.accessory1 != null)
            newSpecialAdventurer.EquipAccessory1(ItemManager.Instance.CreateItem(data.accessory1.itemCategory, data.accessory1.itemNum));
        if (data.accessory2 != null)
            newSpecialAdventurer.EquipAccessory2(ItemManager.Instance.CreateItem(data.accessory2.itemCategory, data.accessory2.itemNum));


        // HA가 있다면 입장시킴
        if (data.curHuntingArea != -1)
        {
            newSpecialAdventurer.curHuntingArea = CombatAreaManager.Instance.huntingAreas[data.curHuntingArea];
            newSpecialAdventurer.curHuntingArea.EnterAdventurer(input);
        }

        // BA가 있다면 입장시킴
        if (data.curBossArea != -1)
        {
            newSpecialAdventurer.curBossArea = CombatAreaManager.Instance.bossAreas[data.curBossArea];
            newSpecialAdventurer.curBossArea.EnterAdventurer(input);
        }
    }

    public static void InitLoadedMonster(GameObject obj, GameObject sample, MonsterData data, bool canWander)
    {
        InitLoadedActor(obj, data);
        Monster monsterComp = obj.GetComponent<Monster>();
        //Monster sampleMobComp = sample.GetComponent<Monster>();
        RewardStat sampleRewardStat = sample.GetComponent<Monster>().GetRewardStat();

        monsterComp.index = data.index;
        //Debug.Log("[InitLoadedMonster] data index : " + data.index + ", " + monsterComp.index);

        monsterComp.InitMonster(data.monsterNum, data.battleStat, sampleRewardStat, canWander);
        monsterComp.SetSuperState(data.superState);
        SetLoadedActorState(obj, data);
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

    public void LoadBossPhaseData(GameSavedata savedata)
    {
        if(savedata.curSkirmish != null)
        {
            curSkirmish.LoadSkirmishData(savedata.curSkirmish);
        }

        isBossPhase = savedata.isBossPhase;
        responsedSpAdvCnt = savedata.responsedSpAdvCnt;
        isBossRaidPrepTimeOver = savedata.isBossRaidPrepTimeOver;
        bossRaidPrepWaitedTime = savedata.bossRaidPrepWatiedTime;
        retryTimeLeft = savedata.retryTimeLeft;
        canCallBossRaid = savedata.canCallBossRaid;
    }

    public Skirmish GetCurSkirmish()
    {
        return curSkirmish;
    }
    #endregion

    #region Stage Progress
    /// <summary>
    /// 일선 모험가 선택 메서드
    /// </summary>
    /// <param name="spAdvIndex">선택대상 모험가의 인덱스</param>
    public void ChooseSpAdv(int spAdvIndex)
    {
        playerSpAdvIndex = spAdvIndex;
        specialAdventurers[spAdvIndex].GetComponent<SpecialAdventurer>().SignExclusiveContract();
    }

    /// <summary>
    /// 플레이어가 보스 공략 신청 했을 때의 메서드
    /// </summary>
    public void PlayerCalledBossRaid()
    {
        PlayerOrderedRaidEventHandler?.Invoke();
        SomeoneCalledBossRaid();
        DisableBossRaidUI();
    }

    public void PlayerRejectedBossRaid()
    {
        DisableBossRaidUI();
    }

    public void AICalledBossRaid()
    {
        SomeoneCalledBossRaid();
        ShowBossRaidDecisionUI();
    }

    public void SpAdvResponsed(bool isAccepted, SpecialAdventurer respondent)
    {
        responsedSpAdvCnt++;
        if (isAccepted)
            curSkirmish.AddSkirmishParticipant(respondent);
    }

    /// <summary>
    /// 보스 공략을 누군가 신청했을 때 나머지에게 알려주는 메서드.
    /// </summary>
    public void SomeoneCalledBossRaid()
    {
        BossRaidCallEventHandler?.Invoke();
        StartCoroutine(BossRaidPrepTimer());
    }

    /// <summary>
    /// 보스 페이즈 시작될 때(== 보스 에어리어 개방시) 호출
    /// </summary>
    public void OnBossAreaConquerStarted()
    {
        isBossPhase = true;

        responsedSpAdvCnt = 0;
        retryTimeLeft = 0.0f;
        canCallBossRaid = true;
        isBossRaidPrepTimeOver = false;
        bossRaidPrepWaitedTime = 0.0f;

        // 전초전 하기 전에 리셋
        curSkirmish = new Skirmish();

        //StartCoroutine(BossRaidPrepTimer());
    }

    public IEnumerator BossRaidPrepTimer()
    {
        bossRaidPrepWaitedTime = 0.0f;

        while (true)
        {
            yield return new WaitForSeconds(SkillConsts.TICK_TIME);
            bossRaidPrepWaitedTime += SkillConsts.TICK_TIME;
            // TODO: 남은 시간 UI에 띄워주기

            if (bossRaidPrepWaitedTime >= SceneConsts.BOSSRAID_PREP_TIME)
                isBossRaidPrepTimeOver = true;

            if (IsBossRaidPrepEnded)
                break;
        }

        DisableBossRaidUI();
        curSkirmish.StartSkirmish();
    }

    public void OnSkirmishEnd()
    {
        curSkirmish = null;
    }

    /// <summary>
    /// 보스 페이즈 종료 시
    /// </summary>
    public void OnBossAreaConquered()
    {
        isBossPhase = false;
    }

    public void OnHuntingAreaConquered()
    {

    }

    /// <summary>
    /// HA가 새로 열리면 인원 제한이 느니 이때 액터들 활성화해줌.
    /// </summary>
    public void OnHuntingAreaOpenToPublic()
    {
        //int regenAmount = progressInformations[CombatAreaManager.Instance.PublicHuntingAreaIndex].guestCapacity;
        //GenerateTravelers(regenAmount/ 2 + regenAmount % 2); // 나머지는 무조건 관광객이 가져감.
        //GenerateAdventurers(regenAmount / 2);
        FillTrvAdvVacancies();
    }
    
    public void FillTrvAdvVacancies()
    {
        GenerateTravelers(CurTrvMax - travelersEnabled.Count - trvEnterQ.Count);
        GenerateAdventurers(CurAdvMax - adventurersEnabled.Count - advEnterQ.Count);
    }


    public void StartRetryTimer(float waitingTime = SceneConsts.BOSSRAID_RETRY_TIME)
    {
        StartCoroutine(RetryTimer(waitingTime));
    }

    private IEnumerator RetryTimer(float watingTime)
    {
        const int TICK_MULT = 2;

        while (true)
        {
            yield return new WaitForSeconds(TICK_MULT * SkillConsts.TICK_TIME);
            retryTimeLeft += TICK_MULT * SkillConsts.TICK_TIME;

            if (retryTimeLeft >= watingTime)
            {
                break;
            }
        }

        retryTimeLeft = 0;
        EnableBossRaidUI();
    }

    /// <summary>
    /// 패배 시 리스트에서 빼줌.
    /// </summary>
    /// <param name="loser"></param>
    public void ReportMatchDefeated(SpecialAdventurer loser)
    {
        curSkirmish.ReportMatchDefeated(loser);
    }

    public void ReportMatchWon(SpecialAdventurer winner)
    {
        curSkirmish.ReportMatchWon(winner);
    }

    public void ReportBossBattleWon(SpecialAdventurer conquerer)
    {
        int remainBoss = bossAreaCount - CombatAreaManager.Instance.BossAreaIndex - 1; // 남은 보스 수
        isBossPhase = false;

        switch (remainBoss)
        {
            case 0: //최종 보스
                if (conquerer.index == playerSpAdvIndex)
                    PlayerWon();
                else
                    PlayerLose();
                break;
            case 1: //중간 보스
                if (conquerer.index == playerSpAdvIndex)
                    GivePlayerMidBossBonus();
                break;
            case 2: //수문장
                if (conquerer.index == playerSpAdvIndex)
                    GivePlayerGatekeeperBonus();
                break;
        }
		OpenNextStage();
        CombatAreaManager.Instance.OnBossAreaConquered();
    }

    private void PlayerWon()
    {
        ShowStageClearUI();
        SetTimeScale(0);
    }

    private void PlayerLose()
    {
        ShowStageRetryUI();
        SetTimeScale(0);
    }

    private void GivePlayerMidBossBonus()
    {
        ShowMidBossClearUI();
        AddGold(CombatAreaManager.Instance.FindBossArea().Bonus);
    }

    private void GivePlayerGatekeeperBonus()
    {
        ShowGatekeeperClearUI();
        AddGold(CombatAreaManager.Instance.FindBossArea().Bonus);
    }


    #region UI
    public void ShowBossRaidDecisionUI()
    {

    }

    public void EnableBossRaidUI()
    {

    }

    public void DisableBossRaidUI()
    {

    }

    public void ShowSpAdvSelectionUI()
    {

    }

    public void ShowMidBossClearUI()
    {
        Debug.Log("Player got MidBoss.");
    }

    public void ShowGatekeeperClearUI()
    {
        Debug.Log("Player got GateKeeper.");
    }

    private void ShowStageRetryUI()
    {
        Debug.Log("You lose.");
    }

    private void ShowStageClearUI()
    {
        Debug.Log("Victory!");
    }
    #endregion

    #endregion

    #region For Debugging
    public void DebugBossCall()
    {
        ChooseSpAdv(0);
        BossRaidCallEventHandler?.Invoke();
    }
    #endregion

    #region JSON 처리(Scene 정보 설정)
    private void SetSceneStructureDatas(JSONNode aData, int sceneNumber)
    {
        drink_Max = aData["scene"][sceneNumber]["buildable"]["drink"].AsInt;
        food_Max = aData["scene"][sceneNumber]["buildable"]["food"].AsInt;
        lodge_Max = aData["scene"][sceneNumber]["buildable"]["lodge"].AsInt;
        equipment_Max = aData["scene"][sceneNumber]["buildable"]["equipment"].AsInt;
        tour_Max = aData["scene"][sceneNumber]["buildable"]["tour"].AsInt;
        convenience_Max = aData["scene"][sceneNumber]["buildable"]["convenience"].AsInt;
        fun_Max = aData["scene"][sceneNumber]["buildable"]["fun"].AsInt;
        santuary_Max = aData["scene"][sceneNumber]["buildable"]["santuary"].AsInt;
        rescue_Max = aData["scene"][sceneNumber]["buildable"]["rescue"].AsInt;
    }

    private void SetSceneRatioDatas(JSONNode aData, int sceneNumber)
    {
        raceRatios = new List<KeyValuePair<string, float>>();

#if DEBUG_GEN_ADV
        Debug.Log("sceneNumber: " + sceneNumber);
#endif

        raceRatios.Add(new KeyValuePair<string, float>("human", aData["scene"][sceneNumber]["compositionOfPopulation"]["race"]["human"].AsFloat));
        raceRatios.Add(new KeyValuePair<string, float>("elf", aData["scene"][sceneNumber]["compositionOfPopulation"]["race"]["elf"].AsFloat));
        raceRatios.Add(new KeyValuePair<string, float>("dwarf", aData["scene"][sceneNumber]["compositionOfPopulation"]["race"]["dwarf"].AsFloat));
        raceRatios.Add(new KeyValuePair<string, float>("orc", aData["scene"][sceneNumber]["compositionOfPopulation"]["race"]["orc"].AsFloat));
        raceRatios.Add(new KeyValuePair<string, float>("dog", aData["scene"][sceneNumber]["compositionOfPopulation"]["race"]["dog"].AsFloat));
        raceRatios.Add(new KeyValuePair<string, float>("cat", aData["scene"][sceneNumber]["compositionOfPopulation"]["race"]["cat"].AsFloat));

        trvWealthRatios = new List<KeyValuePair<string, float>>();

        trvWealthRatios.Add(new KeyValuePair<string, float>("lower", aData["scene"][sceneNumber]["compositionOfPopulation"]["wealth"]["lower"].AsFloat));
        trvWealthRatios.Add(new KeyValuePair<string, float>("middle", aData["scene"][sceneNumber]["compositionOfPopulation"]["wealth"]["middle"].AsFloat));
        trvWealthRatios.Add(new KeyValuePair<string, float>("higher", aData["scene"][sceneNumber]["compositionOfPopulation"]["wealth"]["upper"].AsFloat));

        advWealthRatios = new List<WealthRatioByLevel>();
        WealthRatioByLevel temp;
        for (int i = 0; i < 3; i++)
        {
            temp = new WealthRatioByLevel();
            temp.levelMin = advWealthRatioData["GroupByLevel"][i]["level"]["min"].AsInt;
            temp.levelMax = advWealthRatioData["GroupByLevel"][i]["level"]["max"].AsInt;
            temp.wealthRatio.Add(new KeyValuePair<string, float>("lower", advWealthRatioData["GroupByLevel"][i]["wealthRatio"]["lower"].AsFloat));
            temp.wealthRatio.Add(new KeyValuePair<string, float>("middle", advWealthRatioData["GroupByLevel"][i]["wealthRatio"]["middle"].AsFloat));
            temp.wealthRatio.Add(new KeyValuePair<string, float>("upper", advWealthRatioData["GroupByLevel"][i]["wealthRatio"]["upper"].AsFloat));

            advWealthRatios.Add(temp);
        }
    }

    private void SetSceneEntrances(JSONNode aData, int sceneNumber)
    {
        mapEntranceCount = aData["scene"][sceneNumber]["mapEntranceCount"].AsInt;
        TileLayer layer = GetTileLayer(0);
        int x = 0;
        int y = 0;
        for (int i = 0; i < mapEntranceCount; i++)
        {
            x = aData["scene"][sceneNumber]["mapEntrance"][i]["x"].AsInt;
            y = aData["scene"][sceneNumber]["mapEntrance"][i]["y"].AsInt;
#if DEBUG_ADV
            //Debug.Log(x + "   " + y);
#endif
            mapEntrance.Add(layer.GetTileForMove(x * 2, y * 2));
            mapEntrance.Add(layer.GetTileForMove((x * 2) + 1, y * 2));
            mapEntrance.Add(layer.GetTileForMove(x * 2, (y * 2) + 1));
            mapEntrance.Add(layer.GetTileForMove((x * 2) + 1, (y * 2) + 1));
        }
    }

    private void SetSceneProgressInfos(JSONNode aData, int sceneNumber)
    {
        progressInformations = new List<ProgressInformation>();
        // 사냥터 관련 정보 저장
        huntingAreaCount = aData["scene"][sceneNumber]["huntingAreaCount"].AsInt;
        for (int i = 0; i < huntingAreaCount; i++)
        {
            progressInformations.Add(new ProgressInformation());
            progressInformations[i].huntingAreaIndex = aData["scene"][sceneNumber]["progressInformation"][i]["huntingAreaIndex"].AsInt; // 이거 지워도 될듯.
            progressInformations[i].guestCapacity = aData["scene"][sceneNumber]["progressInformation"][i]["guestCapacity"].AsInt;
            progressInformations[i].minLevel = aData["scene"][sceneNumber]["progressInformation"][i]["minLevel"].AsInt;
            progressInformations[i].maxLevel = aData["scene"][sceneNumber]["progressInformation"][i]["maxLevel"].AsInt;
        }

        bossAreaCount = aData["scene"][sceneNumber]["bossAreaCount"].AsInt;

        int guestMax = 0;
        for(int i = 0; i < huntingAreaCount; i++)
            guestMax += progressInformations[i].guestCapacity = aData["scene"][sceneNumber]["progressInformation"][i]["guestCapacity"].AsInt;
        adventurerMax = guestMax / 2;
        travelerMax = guestMax / 2 + guestMax % 2;
    }

    public TileLayer GetTileLayer(int layerNum)
    {
        return tileMap.GetComponent<TileMap>().GetLayer(layerNum).GetComponent<TileLayer>();
    }

	public void OpenNextStage()
	{
		//인덱스 변수 하나 추가하고
		//보스 잡히면 진행하는걸로 ----
		//호출은 아마 HuntingArea에서 할듯?
		if (stageIndex >= stageMax)
			return;
			//or call OpenNextScene();
		stageIndex += 1;
		TileLayer layer = GetTileLayer(0);
		for(int i = 0; i< layer.GetLayerHeight(); i++)
		{
			for(int j = 0; j<layer.GetLayerWidth(); j++)
			{
				Tile tile = layer.GetTileAsComponent(j, i);
				if (!tile.GetNonTile())
				{
					if (tile.GetLayerNum() == stageIndex)
						tile.SetIsActive(true);
				}			
			}
		}
	}

	public void OpenNextScene()
	{
		//다음 씬으로 ...
	}
#endregion    
}