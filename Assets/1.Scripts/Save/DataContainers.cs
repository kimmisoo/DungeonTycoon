//#define DEBUG_SAVE

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class GameSavedata
{
    // scene 정보
    public string sceneName;

    public int playerGold;
    public int playerPopularity;
    public int playerSpAdvIndex;

    // 아이템 보유 및 장착 현황
    public Dictionary<string, Dictionary<int, ItemCondition>> itemStorage; 

    // 카메라 설정
    public Vector3Data cameraPosition;
    public float cameraSize;

    #region CombatAreas
    public CombatAreaManagerData combatAreaManager;
    #endregion

    #region Actors
    public List<TravelerData> travelersDisabled;
    public List<TravelerData> travelersEnabled;
    public List<AdventurerData> adventurersEnabled;
    public List<AdventurerData> adventurersDisabled;
    public List<SpecialAdventurerData> specialAdventurers;

    public Queue<TravelerData> trvEnterQ;
    public Queue<AdventurerData> advEnterQ;
    public Queue<int> spAdvEnterQ; // 이미 specialAdventurers에 있던 걸 활성화해주니 인덱스만 저장.
    #endregion

    public List<TileData> tileDatas;
    public List<StructureData> structureDatas;

    #region BossPhase
    public SkirmishData curSkirmish;
    public bool isBossPhase;
    public int responsedSpAdvCnt;
    public bool isBossRaidPrepTimeOver;
    public float bossRaidPrepWatiedTime;
    public float retryTimeLeft;
    public bool canCallBossRaid;
    #endregion

    #region UI
    public BossRaidUI.BossRaidUIState bossRaidUIState;
    public string bossRaidStateText;
    #endregion

    public GameSavedata(string sceneNameInput)
    {
        GameManager gameManager = GameManager.Instance;
        StructureManager structureManager = StructureManager.Instance;

        sceneName = sceneNameInput;
        playerGold = gameManager.playerGold;
        playerPopularity = gameManager.playerPopularity;
        playerSpAdvIndex = gameManager.playerSpAdvIndex;

        itemStorage = gameManager.GetItemStorage();

        cameraPosition = new Vector3Data(Camera.main.transform.position);
        cameraSize = Camera.main.orthographicSize;
        Camera.main.GetComponent<InputManager>().isLoading = true;

        combatAreaManager = new CombatAreaManagerData();
        combatAreaManager.InitCombatAreaManagerData();

        #region Actors
        // travelers 저장.
        travelersDisabled = new List<TravelerData>();
        for (int i = 0; i < gameManager.travelersDisabled.Count; i++)
        {
            travelersDisabled.Add(new TravelerData(gameManager.travelersDisabled[i]));
            travelersDisabled[i].index = i;
        }

        travelersEnabled = new List<TravelerData>();
        for (int i = 0; i < gameManager.travelersEnabled.Count; i++)
        {
            travelersEnabled.Add(new TravelerData(gameManager.travelersEnabled[i]));
            travelersEnabled[i].index = i;
        }

        trvEnterQ = new Queue<TravelerData>();
        List<GameObject> trvEnterQConverted = gameManager.trvEnterQ.ToList();
        for (int i = 0; i < trvEnterQConverted.Count; i++)
            trvEnterQ.Enqueue(new TravelerData(trvEnterQConverted[i]));


        adventurersEnabled = new List<AdventurerData>();
        for (int i = 0; i < gameManager.adventurersEnabled.Count; i++)
        {
            adventurersEnabled.Add(new AdventurerData(gameManager.adventurersEnabled[i]));
            // 인덱스 수정해서 넣어주기
            adventurersEnabled[i].index = i;
        }

        adventurersDisabled = new List<AdventurerData>();
        for (int i = 0; i < gameManager.adventurersDisabled.Count; i++)
        {
            adventurersDisabled.Add(new AdventurerData(gameManager.adventurersDisabled[i]));
            adventurersDisabled[i].index = i;
        }

        advEnterQ = new Queue<AdventurerData>();
        List<GameObject> advEnterQConverted = gameManager.advEnterQ.ToList();
        for (int i = 0; i < advEnterQConverted.Count; i++)
            advEnterQ.Enqueue(new AdventurerData(advEnterQConverted[i]));

        specialAdventurers = new List<SpecialAdventurerData>();
        for (int i = 0; i < gameManager.specialAdventurers.Count; i++)
            specialAdventurers.Add(new SpecialAdventurerData(gameManager.specialAdventurers[i]));

        spAdvEnterQ = new Queue<int>();
        List<GameObject> spAdvEnterQConverted = gameManager.spAdvEnterQ.ToList();
        for (int i = 0; i < spAdvEnterQConverted.Count; i++)
            spAdvEnterQ.Enqueue(spAdvEnterQConverted[i].GetComponent<SpecialAdventurer>().index);
        #endregion

        #region BossPhase
        if (gameManager.GetCurSkirmish() != null)
            curSkirmish = new SkirmishData(gameManager.GetCurSkirmish());
        isBossPhase = gameManager.isBossPhase;
        responsedSpAdvCnt = gameManager.responsedSpAdvCnt;
        isBossRaidPrepTimeOver = gameManager.isBossRaidPrepTimeOver;
        bossRaidPrepWatiedTime = gameManager.bossRaidPrepWaitedTime;
        retryTimeLeft = gameManager.retryTimeLeft;
        canCallBossRaid = gameManager.canCallBossRaid;
        #endregion

        #region UI
        bossRaidUIState = UIManager.Instance.bossRaidUI.State;
        bossRaidStateText = UIManager.Instance.bossRaidUI.GetRaidStateText();
        #endregion

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

        for (int i = 0; i < structureManager.structures.Count; i++)
        {
            structureDatas.Add(new StructureData(structureManager.structures[i]));
        }
    }
}

[System.Serializable]
public class CombatAreaManagerData
{
    public int publicHuntingAreaIndex;
    public int bossAreaIndex;

    public List<HuntingAreaData> huntingAreas;
    public List<BossAreaData> bossAreas;

    public void InitCombatAreaManagerData()
    {
        publicHuntingAreaIndex = CombatAreaManager.Instance.PublicHuntingAreaIndex;
        bossAreaIndex = CombatAreaManager.Instance.BossAreaIndex;

        huntingAreas = new List<HuntingAreaData>();
        for (int i = 0; i < CombatAreaManager.Instance.huntingAreas.Count; i++)
        {
            huntingAreas.Add(new HuntingAreaData(CombatAreaManager.Instance.huntingAreas[i]));
        }

        bossAreas = new List<BossAreaData>();
        for (int i = 0; i < CombatAreaManager.Instance.bossAreas.Count; i++)
        {
            bossAreas.Add(new BossAreaData(CombatAreaManager.Instance.bossAreas[i]));
        }
    }
}


[System.Serializable]
public class SkirmishData
{
    public List<int> skirmishParticipants;
    public List<int> skirmishLosers;
    public List<int> skirmishBracket;
    //public bool isSkirmishEnded;
    public int roundCnt;
    public int curRound;
    public int matchCntCurRound;
    public int curMatch;

    public SkirmishData(Skirmish input)
    {
        skirmishParticipants = new List<int>();
        skirmishLosers = new List<int>();
        skirmishBracket = new List<int>();

        for (int i = 0; i < input.skirmishParticipants.Count; i++)
            skirmishParticipants.Add(input.skirmishParticipants[i].index);
        for (int i = 0; i < input.skirmishLosers.Count; i++)
            skirmishLosers.Add(input.skirmishLosers[i].index);
        for (int i = 0; i < input.skirmishBracket.Count; i++)
            skirmishBracket.Add(input.skirmishBracket[i].index);

        roundCnt = input.roundCnt;
        curRound = input.curRound;
        matchCntCurRound = input.matchCntCurRound;
        curMatch = input.curMatch;
    }
}

[System.Serializable]
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



[System.Serializable]
public class TileCoordinates
{
    public int x, y;

    public TileCoordinates(Tile input)
    {
        x = input.GetX();
        y = input.GetY();
    }
}

[System.Serializable]
public class TileForMoveCoordinates
{
    public int x, y;

    public TileForMoveCoordinates(TileForMove input)
    {
        x = input.GetX();
        y = input.GetY();
    }
}

[System.Serializable]
public class StatData
{
    public int id;
    public RaceType race;
    public WealthType wealth;
    public JobType job;
    public string actorName;
    public string explanation;
    public Gender gender;
    public int gold;
    public Dictionary<DesireType, DesireBaseData> desireDict;

    public StatData()
    {
        desireDict = new Dictionary<DesireType, DesireBaseData>();
    }

    public StatData(Stat input)
    {
        id = input.id;
        race = input.race;
        wealth = input.wealth;
        job = input.job;
        actorName = input.actorName;
        explanation = input.explanation;
        gender = input.gender;
        gold = input.gold;

        desireDict = new Dictionary<DesireType, DesireBaseData>();
        Dictionary<DesireType, DesireBase> inputDesireDict = input.GetDesireDict();
        foreach (DesireType key in inputDesireDict.Keys.ToList())
            desireDict.Add(key, new DesireBaseData(inputDesireDict[key]));
    }
}

[System.Serializable]
public class DesireBaseData
{
    public float desireValue;
    public float tickAmount;
    public float tickAmountMult;
    public float tickBetween;
    public DesireType desireName;

    public DesireBaseData(DesireBase input)
    {
        desireValue = input.desireValue;
        tickAmount = input.tickAmount;
        tickAmountMult = input.tickAmountMult;
        tickBetween = input.tickBetween;
        desireName = input.desireName;
    }
}


[System.Serializable]
public class TileData
{
    public Vector3Data position;
    public int x, y;
    public string tileName; // 이름.
    public int prefabInfo;
    public int layerNum;

    public bool isStructed;
    public bool isNonTile;

    public bool isBuildingArea;
    public bool isHuntingArea;
    public bool isRoad;
    public bool isActive;
    
    

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

        isStructed = inputTile.GetStructed();
        isNonTile = inputTile.GetNonTile();

        isBuildingArea = inputTile.GetBuildingAreaSave();
        isHuntingArea = inputTile.GetHuntingAreaSave();
        isRoad = inputTile.GetRoadSave();
        isActive = inputTile.GetIsActive();

        Structure tileStructure = inputTile.GetStructure();
        if (tileStructure != null)
            structureIndex = tileStructure.index;
        else
            structureIndex = -1;
    }
}

[System.Serializable]
public class StructureData
{
    public Vector3Data position;
    // 마찬가지로 바꿔야할 수 있음.
    public int pointTile;
    public int[,] extent;
    public List<int> entranceList;
    public int entCount;
    public int sitInCount;

    public Queue<TravelerTimerData> curUsingQueue;
    public Queue<TravelerTimerData> curWaitingQueue;
    //public Queue<float> elapsedTimeQueue;

    public string structureCategory;
    public int structureNumber;
    public int structureIndex;

    public StructureData(Structure input)
    {
        position = new Vector3Data(input.gameObject.transform.position);
        entranceList = new List<int>();
        curUsingQueue = new Queue<TravelerTimerData>();
        curWaitingQueue = new Queue<TravelerTimerData>();
        //elapsedTimeQueue = new Queue<float>();

        pointTile = int.Parse(input.point.gameObject.name);
        extent = input.extent;
        for (int i = 0; i < input.entrance.Count; i++)
        {
            Debug.Log("i : " + i + " entrance count : " + input.entrance.Count);

            entranceList.Add(int.Parse(input.entrance[i].gameObject.name));
        }
        entCount = input.entCount;
        sitInCount = input.sitInCount;

        List<Structure.TravelerTimer> tempList = input.GetCurUsingQueueAsList();
        for (int i = 0; i < tempList.Count; i++)
            curUsingQueue.Enqueue(new TravelerTimerData(tempList[i]));
        tempList = input.GetCurWaitingQueueAsList();
        for (int i = 0; i < tempList.Count; i++)
            curWaitingQueue.Enqueue(new TravelerTimerData(tempList[i]));

        ////float timeNow = Time.fixedTime;
        //float[] timeArr = input.GetElapsedTimeQueueAsArray();
        //for (int i = 0; i < timeArr.Length; i++)
        //    elapsedTimeQueue.Enqueue(timeArr[i]);

        structureCategory = input.structureCategory;
        structureNumber = input.structureNumber;
        structureIndex = input.index;
    }
}

[System.Serializable]
public class TravelerTimerData
{
    public int travelerIndex;
    public ActorType travelerType;

    public int elapsedTime;

    public TravelerTimerData(Structure.TravelerTimer trvTimer)
    {
        travelerIndex = trvTimer.traveler.index;
        travelerType = trvTimer.traveler.GetActorType();

        elapsedTime = trvTimer.elapsedTime;
    }
}


[System.Serializable]
public class CombatAreaData
{
    public Dictionary<int, MonsterData> monstersEnabled;
    public Dictionary<int, MonsterData> monstersDisabled;

    public List<AdventurerData> adventurersInside;

    public string stageNum;
    public int index;

    public CombatAreaData(CombatArea input)
    {
        monstersEnabled = new Dictionary<int, MonsterData>();
        foreach (int key in input.GetMonstersEnabled().Keys.ToList())
        {
            monstersEnabled.Add(key, new MonsterData(input.GetMonstersEnabled()[key]));
            // 로드해서 서로 찾을 수 있게 인덱스 값 저장해줌
            //monstersEnabled[key].index = key;
        }


        monstersDisabled = new Dictionary<int, MonsterData>();
        foreach (int key in input.GetMonstersDisabled().Keys.ToList())
        {
            monstersDisabled.Add(key, new MonsterData(input.GetMonstersDisabled()[key]));
            //monstersDisabled[key].index = key;
        }

        adventurersInside = new List<AdventurerData>();
        foreach (GameObject item in input.GetAdventurersInside())
            adventurersInside.Add(new AdventurerData(item));

        stageNum = input.stageNum;
    }
}

[System.Serializable]
public class HuntingAreaData : CombatAreaData
{
    public int huntingAreaNum;
    //public int index;
    public int killCount;

    public HuntingAreaData(HuntingArea input) : base(input)
    {
        huntingAreaNum = input.huntingAreaNum;
        index = input.index;
        killCount = input.GetKillCount();
    }
}

[System.Serializable]
public class BossAreaData : CombatAreaData
{
    public int bossAreaNum;
    //public int index;

    public BossAreaData(BossArea input) : base(input)
    {
        bossAreaNum = input.bossAreaNum;
        index = input.index;
    }
}

[System.Serializable]
public class ActorData
{
    public SuperState superState;
    public State state;
    public Vector3Data position;
    public bool isActive;
    public string prefabPath;

    // ICombatant용
    public CombatantPtr enemy;

    // 혹은 레이어 번호를 같이 받아야할 수 있음.
    public TileCoordinates destinationTile;
    public TileForMoveCoordinates destinationTileForMove;
    public TileCoordinates curTile;
    public TileForMoveCoordinates curTileForMove;

    public ActorData(GameObject input)
    {
        Actor inputActor = input.GetComponent<Actor>();

        superState = inputActor.GetSuperState();
        state = inputActor.curState;
        position = new Vector3Data(input.transform.position);

        prefabPath = inputActor.prefabPath;

        isActive = input.activeSelf;

        if (isActive)
        {
            if (inputActor.GetDestinationTile() != null)
                destinationTile = new TileCoordinates(inputActor.GetDestinationTile());
            if (inputActor.GetDestinationTileForMove() != null)
                destinationTileForMove = new TileForMoveCoordinates(inputActor.GetDestinationTileForMove());
            //curTile = new TileCoordinates(inputTraveler.GetCurTile());
            if (inputActor.GetCurTile() != null)
                curTile = new TileCoordinates(inputActor.GetCurTile());
            if (inputActor.GetCurTileForMove() != null)
                curTileForMove = new TileForMoveCoordinates(inputActor.GetCurTileForMove());
        }
    }
}

[System.Serializable]
public class MonsterData : ActorData
{
    public int index;
    public int monsterNum;

    public BattleStat battleStat;
    //public CombatantPtr enemy;

    List<string> skills; // 이름만 알고있음 됨
    Dictionary<string, TemporaryEffectData> temporaryEffects;

    public MonsterData(GameObject input) : base(input)
    {
        Monster inputMonster = input.GetComponent<Monster>();

        index = inputMonster.index;
        monsterNum = inputMonster.monsterNum;

        battleStat = inputMonster.GetBattleStat();

        if (inputMonster.GetEnemy() != null)
            enemy = new CombatantPtr(inputMonster.GetEnemy());

        skills = inputMonster.GetSkills().Keys.ToList();
        temporaryEffects = new Dictionary<string, TemporaryEffectData>();
        Dictionary<string, TemporaryEffect> tempEffectsOrigin = inputMonster.GetTemporaryEffects();
        foreach (string key in tempEffectsOrigin.Keys.ToList())
            temporaryEffects.Add(key, new TemporaryEffectData(tempEffectsOrigin[key]));
    }
}

[System.Serializable]
public class TravelerData : ActorData
{

    //    public Vector3Data rotation;
    public int destPlaceIndex = -1;
    public PlaceType destPlaceType; // 이거부터
    // inactive List가 있으니 거기 넣을건지 이야기해봐야함.

    // 로드용(참조용) 인덱스
    public int index;

    public StatData stat;

    // 어떤 프리팹을 로드하는지는 아직 안 적어놨음.

    public TravelerData(GameObject input) : base(input)
    {
        Traveler inputTraveler = input.GetComponent<Traveler>();

        index = inputTraveler.index;

        if (inputTraveler.stat != null)
            stat = new StatData(inputTraveler.stat);

        if (inputTraveler.destinationPlace != null)
        {
            destPlaceIndex = inputTraveler.destinationPlace.index;
            destPlaceType = inputTraveler.destinationPlace.GetPlaceType();
        }

    }
}

[System.Serializable]
public class AdventurerData : TravelerData
{
    public BattleStat battleStat;
    //RewardStat rewardStat; <- 로드시에 알아서 넣어줄 것.

    public int curHuntingArea = -1;

    public List<string> skills;
    public Dictionary<string, TemporaryEffectData> temporaryEffects;

    public AdventurerData(GameObject input) : base(input)
    {
        Adventurer inputAdventurer = input.GetComponent<Adventurer>();

        battleStat = inputAdventurer.GetBattleStat();

        if (inputAdventurer.GetEnemy() != null)
            enemy = new CombatantPtr(inputAdventurer.GetEnemy());
        if (inputAdventurer.curHuntingArea != null)
            curHuntingArea = inputAdventurer.curHuntingArea.index;

        //Debug.Log("name : " + inputAdventurer.name);
        if (inputAdventurer.GetSkills() != null)
            skills = inputAdventurer.GetSkills().Keys.ToList();

        temporaryEffects = new Dictionary<string, TemporaryEffectData>();
        Dictionary<string, TemporaryEffect> tempEffectsOrigin = inputAdventurer.GetTemporaryEffects();
        if (tempEffectsOrigin != null)
            foreach (string key in tempEffectsOrigin.Keys.ToList())
                temporaryEffects.Add(key, new TemporaryEffectData(tempEffectsOrigin[key]));
        //if (!isActive && stat == null)
        //    Debug.Log("stat is null");
    }
}

[System.Serializable]
public class SpecialAdventurerData : AdventurerData
{
    public ItemData weapon, armor, accessory1, accessory2;
    public bool willBossRaid;
    public string nameKey;
    public int curBossArea = -1;

    public SpecialAdventurerData(GameObject input) : base(input)
    {
        SpecialAdventurer specialAdventurer = input.GetComponent<SpecialAdventurer>();

        willBossRaid = specialAdventurer.willBossRaid;
        nameKey = specialAdventurer.nameKey;

        if (specialAdventurer.curBossArea != null)
            curBossArea = specialAdventurer.curBossArea.index;


        Item weaponOrigin, armorOrigin, accessory1Origin, accessory2Origin;

        weaponOrigin = specialAdventurer.GetWeapon();
        armorOrigin = specialAdventurer.GetArmor();
        accessory1Origin = specialAdventurer.GetAccessory1();
        accessory2Origin = specialAdventurer.GetAccessory2();

        if (weaponOrigin != null)
            weapon = new ItemData(weaponOrigin);
        if (armorOrigin != null)
            armor = new ItemData(armorOrigin);
        if (accessory1Origin != null)
            accessory1 = new ItemData(accessory1Origin);
        if (accessory2Origin != null)
            accessory2 = new ItemData(accessory2Origin);
    }
}

[System.Serializable]
public class ItemData
{
    public string itemCategory;
    public int itemNum;

    public ItemData(Item item)
    {
        itemCategory = item.itemCategory;
        itemNum = item.itemIndex;
    }
}


[System.Serializable]
public class CombatantPtr
{
    public int index;
    public ActorType combatantType;

    public CombatantPtr(ICombatant combatant)
    {
        //if(Debug)
        //Debug.Log(combatant);
        this.index = combatant.GetIndex();
        if (index == -1)
            Debug.Log(index);
        combatantType = combatant.GetActorType();
    }
}

[System.Serializable]
public class TemporaryEffectData
{
    public List<StatModContinuous> continuousMods;
    public List<StatModDiscrete> discreteMods;
    //private ICombatant subject;
    //private BattleStat subjectBattleStat;
    public int stackCnt;
    public readonly int stackLimit;

    public readonly string name;

    public readonly float duration; // -1이면 영구.
    public float elapsedTime;

    public TemporaryEffectData(TemporaryEffect tempEffect)
    {
        continuousMods = tempEffect.GetContinousModList();
        discreteMods = tempEffect.GetDiscreteModList();

        stackCnt = tempEffect.GetStackCnt();
        stackLimit = tempEffect.GetStackLimit();

        name = tempEffect.name;

        duration = tempEffect.duration;
        elapsedTime = tempEffect.elapsedTime;
    }
}
