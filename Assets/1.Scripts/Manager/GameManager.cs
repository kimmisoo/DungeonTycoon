#define DEBUG_ADV
//#define DEBUG_GEN_ADV

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using SimpleJSON;
using UnityEngine.Events;
using UnityEngine.UI;

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
    public int activeSpAdvCnt;
    public List<GameObject> inactiveTravelers;
    public List<GameObject> adventurersDisabled;

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

    // 모험가, 관광객 생성용 비율 저장 리스트
    List<KeyValuePair<string, float>> raceRatios;
    List<KeyValuePair<string, float>> trvWealthRatios;
    List<WealthRatioByLevel> advWealthRatios; // 이거 어떻게 저장?

    // 사냥터 정보
    int huntingAreaCount = 0;
    // 사냥터 개방에 따라 필요해질 데이터 저장
    List<ProgressInformation> progressInformations;

    public JSONNode advStatData;
    JSONNode advWealthRatioData;

    Dictionary<string, JSONNode> spAdvStatDatas;

    JSONNode spAdvSummary;

    JSONNode namesData;
    JSONNode trvInitialGoldData;

    JSONNode desireData;

    private int CurGuestMax //현재 맵에 들어올 수 있는 모험가+관광객 최대치
    {
        get
        {
            int total = 0;
            for (int i = 0; i <= HuntingAreaManager.Instance.ActiveIndex; i++)
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
            return CurGuestMax / ((HuntingAreaManager.Instance.ActiveIndex + 1) / 2); // Int연산인데 버려지는 건? 버려지면 나머지는 traveler에서 보충해도 되긴함.
        }
    }

    private int CurAdvMax // 최대로 들어올 수 있는 모험가 수
    {
        get
        {
            return CurAdvMaxPerHuntingArea * (HuntingAreaManager.Instance.ActiveIndex + 1); // 사냥터별 수가 있고 정수 때문에 이렇게 계산.
        }
    }

    private int CurTrvMax // 최대로 들어올 수 있는 관광객 수
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

        Debug.Log("mapName" + sceneName);
        ReadDatasFromJSON();

        wait = new WaitForSeconds(0.11f);
        countLogWait = new WaitForSeconds(3.0f);
    }

    void Start()
    {
        SetMap();

        // Scene 정보 세팅
        SetSceneData(sceneData);

        travelers = new List<GameObject>();
        adventurersEnabled = new List<GameObject>();
        specialAdventurers = new List<GameObject>();
        activeSpAdvCnt = 0;
        inactiveTravelers = new List<GameObject>();
        adventurersDisabled = new List<GameObject>();

        advEnterQ = new Queue<GameObject>();
        spAdvEnterQ = new Queue<GameObject>();

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
            //go.GetComponent<Adventurer>().SetAttackEffect((GameObject)Instantiate(Resources.Load("EffectPrefabs/Default_AttackEffect")));

            // 생성만 해놓고 비활성화
            go.SetActive(false);

            // List에 추가
            adventurersDisabled.Add(Instantiate(go));
            go.transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);
            adventurersDisabled[i].transform.parent = GameObject.FindGameObjectWithTag("Characters").transform;
            Adventurer tempAdventurer = adventurersDisabled[i].GetComponent<Adventurer>();
            tempAdventurer.index = i;
            tempAdventurer.SetAttackEffect((GameObject)Instantiate(Resources.Load("EffectPrefabs/Default_AttackEffect")));
            tempAdventurer.SetDefaultEffects();
            //tempAdventurer.SetDamageText((GameObject)Instantiate(Resources.Load("UIPrefabs/Battle/DamageText")));
            //tempAdventurer.SetHealText((GameObject)Instantiate(Resources.Load("UIPrefabs/Battle/HealText")));
            //tempAdventurer.SetHealEffect((GameObject)Instantiate(Resources.Load("EffectPrefabs/Default_HealEffect")));
            // Debug.Log("character instantiate - " + i);
        }
        StartCoroutine(AdvEnter());
        StartCoroutine(SpAdvEnter());

#if DEBUG_ADV
        GenAndEnqueueSingleAdventurer(1, 1);
        GenAndEnqueueSpecialAdvenuturer("Hana", 1);
#endif
        //StartCoroutine(GCcall());
        for (int i = 0; i < corporateNum; i++)
        {
            popular.Add(0);
        }

        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return null;
#if DEBUG_ADV
        DebugHuntingArea();
#endif
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
        //TextAsset[] spAdvStatTxts = Resources.LoadAll<TextAsset>("Characters/SpecialAdventurers");
        //for (int i = 0; i < spAdvStatTxts.Length; i++)
        //{
        //    spAdvStatDatas.Add(JSON.Parse(spAdvStatTxts[i].ToString()));
        //}
        int sceneNumber = int.Parse(SceneManager.GetActiveScene().name);
        for (int i = 0; i < sceneData["scene"][sceneNumber]["specialadventurers"].Count; i++)
        {
            string spAdvName = sceneData["scene"][sceneNumber]["specialadventurers"][i];
            string battleStatFileName = spAdvSummary[spAdvName]["BattleStatJSON"];

            //Debug.Log("name:" + battleStatFileName);
            TextAsset spAdvStatTxt = Resources.Load<TextAsset>("Characters/SpecialAdventurers/" + battleStatFileName);
            spAdvStatDatas.Add(spAdvName, JSON.Parse(spAdvStatTxt.ToString()));
        }
        //Debug.Log("statdata:" + spAdvStatDatas["Hana"][0]["exp"]);

        //Debug.Log("scene 1:" + sceneData["scene"][0]["specialadventurers"][0]);
        //Debug.Log("scene 2:" + sceneData["scene"][1]["specialadventurers"][2]);
        //Debug.Log("adv 1:" + advStatData[0]["level"]);
        //Debug.Log("SP len:" + spAdvStatTxts.Length);
        //Debug.Log("SP 1:" + spAdvStatDatas[0][0]["level"]);
        //Debug.Log("SP 1:" + spAdvStatDatas[0][0][0]);
        //Debug.Log("SP Sum:" + spAdvSummary["Maxi"]["Stat"]["Explanation"]);
        //Debug.Log("SP Sum2:" + spAdvSummary["Maxi"]["Stat"].Count);

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
        for (int i = 0; i < traveler_Max; i++)
        {
            //yield return wait;
            yield return null;
            travelers[i].SetActive(true);
        }
    }

    private Stat GenStat() // 관광객용
    {
        Stat tempStat = new Stat();
        if (Random.Range(0, 2) == 0)
        {
            tempStat.gender = Gender.Male;
            tempStat.name = namesData["names"]["malename"][Random.Range(0, namesData["malename"].Count)];
        }
        else
        {
            tempStat.gender = Gender.Female;
            tempStat.name = namesData["ㄴnames"]["femalename"][Random.Range(0, namesData["malename"].Count)];
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

    private Stat GenStat(int level) // 모험가용
    {
        Stat tempStat = new Stat();
        if (Random.Range(0, 2) == 0)
        {
            tempStat.gender = Gender.Male;
            tempStat.name = namesData["names"]["malename"][Random.Range(0, namesData["malename"].Count)];
        }
        else
        {
            tempStat.gender = Gender.Female;
            tempStat.name = namesData["names"]["femalename"][Random.Range(0, namesData["malename"].Count)];
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

    private Stat GenStat(string name, int level) // 일선 모험가용
    {
        Stat tempStat = new Stat();

        tempStat.name = spAdvSummary[name]["Stat"]["Name"];
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

        for (i = 0; i<raceRatios.Count; i++)
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

        for (i = 0; i < trvWealthRatios.Count; i++)
        {
            total += trvWealthRatios[i].Value;
            if (num <= total)
                break;
        }

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
        switch(wealth)
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
        switch(desireType)
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

    // 모험가 하나 생성해서 큐에 집어넣음
    private void GenAndEnqueueSingleAdventurer(int minLevel, int maxLevel) // 모험가 하나 생성하고 큐에 집어넣음.
    {
        GameObject temp = adventurersDisabled[adventurersDisabled.Count - 1];
        Adventurer tempAdventurer = temp.GetComponent<Adventurer>();
        adventurersDisabled.RemoveAt(adventurersDisabled.Count - 1); // 객체 풀에서 빼서 씀.

        int advLevel = Random.Range(minLevel, maxLevel+1);
        BattleStat tempBattleStat = GenBattleStat(advLevel);
        Stat tempStat = GenStat(advLevel);
        RewardStat tempRewardStat = GenRewardStat(advLevel);

        //Debug.Log("Adv " + tempStat.name + " hp: " + tempBattleStat.Health + " atk: " + tempBattleStat.BaseAttack);

        tempAdventurer.InitAdventurer(tempStat, tempBattleStat, tempRewardStat);

        advEnterQ.Enqueue(temp);
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
        GameObject go = Instantiate((GameObject)Resources.Load("CharacterPrefabs/"+name));
        //go.GetComponent<Adventurer>().SetAttackEffect((GameObject)Instantiate(Resources.Load("EffectPrefabs/Default_AttackEffect")));

        // 생성만 해놓고 비활성화
        go.SetActive(false);

        // List에 추가
        specialAdventurers.Add(go);
        go.transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);
        go.transform.parent = GameObject.FindGameObjectWithTag("Characters").transform;
        SpecialAdventurer tempSpAdv = go.GetComponent<SpecialAdventurer>();
        tempSpAdv.index = specialAdventurers.Count - 1;
        string attackEffectFileName = "EffectPrefabs/" + name +"_AttackEffect";
        tempSpAdv.SetAttackEffect((GameObject)Instantiate(Resources.Load(attackEffectFileName)));
        tempSpAdv.SetDefaultEffects();
        // Debug.Log("character instantiate - " + i);


        BattleStat tempBattleStat = GenBattleStat(name, level);
        Stat tempStat = GenStat(name, level);
        RewardStat tempRewardStat = GenRewardStat(level);

        //Debug.Log("Adv " + tempStat.name + " hp: " + tempBattleStat.Health + " atk: " + tempBattleStat.BaseAttack);
        //tempBattleStat.ResetBattleStat();
        int skillID = spAdvSummary[name]["SkillID"].AsInt;


        //Debug.Log(tempStat.name + " hp: " + tempBattleStat.Health + " atk: " + tempBattleStat.BaseAttack);

        tempSpAdv.InitSpecialAdventurer(tempStat, tempBattleStat, tempRewardStat, name);

        spAdvEnterQ.Enqueue(go);
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

        Debug.Log("[GenBattleStat] " + name + ", hp : " + tempBattleStat.BaseHealthMax + ", atk : " + tempBattleStat.BaseAttack + ", atkspeed : " + tempBattleStat.AttackSpeed);

        return tempBattleStat;
    }

    public RewardStat GenRewardStat(int level) // 레벨에 따라 RewardStat 생성
    {
        RewardStat tempRewardStat = new RewardStat();

        tempRewardStat.Exp = level * 50;
        tempRewardStat.Gold = 5 * (Mathf.RoundToInt(level * 0.9f) + 20);

        return tempRewardStat;
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
        traveler_Max = aData["scene"][sceneNumber]["traveler_Max"].AsInt;
        adventurer_Max = aData["scene"][sceneNumber]["adventurer_Max"].AsInt;
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
        traveler_Max = 0;
        adventurer_Max = 1;
        //specialAdventurer_Max = 800;
#endif
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
        HuntingAreaManager.Instance.ConstructHuntingArea(0, 0, GetTileLayer().transform.GetChild(1868).gameObject);
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
        trvWealthRatios.Add(new KeyValuePair<string, float>("higher", aData["scene"][sceneNumber]["compositionOfPopulation"]["wealth"]["higher"].AsFloat));

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
            Debug.Log(x + "   " + y);
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
            progressInformations[i].huntingAreaIndex = aData["scene"][sceneNumber]["progressInformation"]["huntingAreaIndex"].AsInt; // 이거 지워도 될듯.
            progressInformations[i].guestCapacity = aData["scene"][sceneNumber]["progressInformation"]["guestCapacity"].AsInt;
            progressInformations[i].minLevel = aData["scene"][sceneNumber]["progressInformation"]["minLevel"].AsInt;
            progressInformations[i].maxLevel = aData["scene"][sceneNumber]["progressInformation"]["maxLevel"].AsInt;
        }
    }

    public TileLayer GetTileLayer(int layerNum)
    {
        return tileMap.GetComponent<TileMap>().GetLayer(layerNum).GetComponent<TileLayer>();
    }
#endregion
}