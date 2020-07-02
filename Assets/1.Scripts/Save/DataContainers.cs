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

    // 카메라 설정
    public Vector3Data cameraPosition;
    public float cameraSize;

    // CombatAreaManager관련 정보
    public int publicHuntingAreaIndex;
    public int bossAreaIndex;

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

        publicHuntingAreaIndex = CombatAreaManager.Instance.PublicHuntingAreaIndex;
        bossAreaIndex = CombatAreaManager.Instance.BossAreaIndex;

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

        for (int i = 0; i < structureManager.structures.Count; i++)
        {
            structureDatas.Add(new StructureData(structureManager.structures[i]));
        }
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

[System.Serializable]
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

        isPassable = inputTile.GetRoad();
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

        //float timeNow = Time.fixedTime;
        float[] timeArr = input.GetElapsedTimeQueueAsArray();
        for (int i = 0; i < timeArr.Length; i++)
            elapsedTimeQueue.Enqueue(timeArr[i]);

        structureCategory = input.structureCategory;
        structureNumber = input.structureNumber;
        structureIndex = input.structureIndex;
    }
}

[System.Serializable]
public class CombatAreaData
{
    Dictionary<int, MonsterData> monstersEnabled;
    Dictionary<int, MonsterData> monstersDisabled;

    List<AdventurerData> adventurersInside;

    string stageNum;

    public CombatAreaData(CombatArea input)
    {
        monstersEnabled = new Dictionary<int, MonsterData>();
        foreach (int key in input.GetMonstersEnabled().Keys.ToList())
            monstersEnabled.Add(key, new MonsterData(input.GetMonstersEnabled()[key]));


        monstersDisabled = new Dictionary<int, MonsterData>();
        foreach (int key in input.GetMonstersDisabled().Keys.ToList())
            monstersDisabled.Add(key, new MonsterData(input.GetMonstersDisabled()[key]));

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
    public int huntingAreaIndex;
    public int killCount;

    public HuntingAreaData(HuntingArea input) : base(input)
    {
        huntingAreaNum = input.huntingAreaNum;
        huntingAreaIndex = input.huntingAreaIndex;
        killCount = input.GetKillCount();
    }
}

[System.Serializable]
public class BossAreaData : CombatAreaData
{
    public int bossAreaNum;
    public int bossAreaIndex;

    public BossAreaData(BossArea input) : base(input)
    {
        bossAreaNum = input.bossAreaNum;
        bossAreaIndex = input.bossAreaIndex;
    }
}

[System.Serializable]
public class MonsterData
{
    public SuperState superState;
    public State state;

    public bool isActive;
    public Vector3Data position;

    public int index;
    public int monsterNum;

    BattleStat battleStat;
    CombatantData enemy;

    List<string> skills; // 이름만 알고있음 됨
    Dictionary<string, TemporaryEffectData> temporaryEffects;

    public MonsterData(GameObject input)
    {
        isActive = input.activeSelf;
        position = new Vector3Data(input.transform.position);

        Monster monster = input.GetComponent<Monster>();

        superState = monster.superState;
        state = monster.state;

        index = monster.index;
        monsterNum = monster.monsterNum;

        battleStat = monster.GetBattleStat();
        enemy = new CombatantData(monster.GetEnemy());

        skills = monster.GetSkills().Keys.ToList();
        temporaryEffects = new Dictionary<string, TemporaryEffectData>();
        Dictionary<string, TemporaryEffect> tempEffectsOrigin = monster.GetTemporaryEffects();
        foreach (string key in tempEffectsOrigin.Keys.ToList())
            temporaryEffects.Add(key, new TemporaryEffectData(tempEffectsOrigin[key]));
    }
}

[System.Serializable]
public class AdventurerData : TravelerData
{
    BattleStat battleStat;
    //RewardStat rewardStat; <- 로드시에 알아서 넣어줄 것.

    CombatantData enemy;

    List<string> skills;
    Dictionary<string, TemporaryEffectData> temporaryEffects;

    public AdventurerData(GameObject input) : base(input)
    {
        Adventurer adventurer = input.GetComponent<Adventurer>();

        battleStat = adventurer.GetBattleStat();

        enemy = new CombatantData(adventurer.GetEnemy());

        skills = adventurer.GetSkills().Keys.ToList();

        temporaryEffects = new Dictionary<string, TemporaryEffectData>();
        Dictionary<string, TemporaryEffect> tempEffectsOrigin = adventurer.GetTemporaryEffects();
        foreach (string key in tempEffectsOrigin.Keys.ToList())
            temporaryEffects.Add(key, new TemporaryEffectData(tempEffectsOrigin[key]));
    }
}

[System.Serializable]
public class SpecialAdventurerData : AdventurerData
{
    public ItemData weapon, armor, accessory1, accessory2;
    public bool willBossRaid;

    public SpecialAdventurerData(GameObject input) : base (input)
    {
        SpecialAdventurer specialAdventurer = input.GetComponent<SpecialAdventurer>();

        willBossRaid = specialAdventurer.willBossRaid;

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
        itemNum = item.itemNum;
    }
}


[System.Serializable]
public class CombatantData
{
    public int index;
    CombatantType combatantType;

    public CombatantData(ICombatant combatant)
    {
        this.index = combatant.GetIndex();
        combatantType = combatant.GetCombatantType();
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
