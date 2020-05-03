using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HuntingArea : Place
{
    private int levelMax;
    public int LevelMax
    {
        get
        {
            return levelMax;
        }
    }

    private int monsterMax; // 최대 몬스터 수
    private int monsterPerRegen; // 주기마다 리젠되는 최대 양
    private float monsterRegenRate; // 리젠 주기
    private float monsterRatio; // 몬스터 샘플1의 비율(1-monsterRatio는 샘플2의 비율)
    
    // 사냥터 안의 몬스터, 모험가들
    public Dictionary<int, GameObject> monstersEnabled;
    public Dictionary<int, GameObject> monstersDisabled; // 초기화는 MonstersMax + MonsterPerRegen
    public List<GameObject> adventurersInside; // 입장한 모험가들

    // 빈칸. 리젠용.
    public List<TileForMove> blanks;
    public Dictionary<string, TileForMove> territory;
    public Dictionary<string, bool> occupiedTerritory;

    private int index;

    private GameObject monsterSample1;
    private GameObject monsterSample2;

    #region Save
    public string stageNum;
    public int huntingAreaNum;
    public int huntingAreaIndex;
    #endregion

    public HuntingArea()
    {
        monstersEnabled = new Dictionary<int, GameObject>();
        monstersDisabled = new Dictionary<int, GameObject>();
        territory = new Dictionary<string, TileForMove>();
        occupiedTerritory = new Dictionary<string, bool>();
        blanks = new List<TileForMove>();
        adventurersInside = new List<GameObject>();
    }


    public void InitHuntingArea(int lvMax, int mobMax/* = 42*/, int mobPerRegen/* = 7*/, float mobRegenRate/* = 5.5f*/, float mobRatio,
        GameObject mobSample1, GameObject mobSample2)
    { 
        levelMax = lvMax;
        monsterMax = mobMax;
        monsterPerRegen = mobPerRegen;
        monsterRegenRate = mobRegenRate;
        monsterRatio = mobRatio;

        // 이부분 복사 제대로 되는지 봐야. 수정요망. 아마 될듯, 복사가 아니라 참조로.
        monsterSample1 = mobSample1;
        monsterSample2 = mobSample2;
    }

    // 현재 살아있는 몬스터 리스트 Get
    public Dictionary<int, GameObject> GetMonstersEnabled()
    {
        return monstersEnabled;
    }

    // 게임 내내 Regen
    private IEnumerator MonsterRegenCycle()
    {
        int needed;

        while (true)
        {
            // 몬스터 몇마리 리젠할 것인지 계산.
            if (monsterPerRegen > monsterMax - monstersEnabled.Count)
                needed = monsterMax - monstersEnabled.Count;
            else
                needed = monsterPerRegen;
#if DEBUG_HA_REGEN
            Debug.Log("monsterMax, monsterEnabledCnt, monsterPerRegen : " + monsterMax + ", " + monstersEnabled.Count + ", " + monsterPerRegen);
            Debug.Log("needed : " + needed);
#endif

            if (needed > 0)
            {
                blanks = FindBlanks(needed);
#if DEBUG_HA_REGEN
                Debug.Log("리젠! 몬스터 수 : " + needed + ", 계산된 빈 칸 수 : " + blanks.Count);
                Debug.Log("전체 칸 수 : " + territory.Count + ", 전체 빈 칸 수 : " + BlanksCount());
#endif
                for (int i = 0; i < blanks.Count; i++)
                {
                    MonsterRegen(blanks[i].GetParent(), blanks[i]);
                }
            }
            yield return new WaitForSeconds(monsterRegenRate);
        }
    }

    // 몬스터 1개를 인자로 받은 타일 위에 생성하는 함수.
    private void MonsterRegen(Tile curTile, TileForMove curTileForMove)
    {
        int lastKey = monstersDisabled.Keys.ToArray<int>()[monstersDisabled.Count - 1];

        Monster tempMonsterComp = monstersDisabled[lastKey].GetComponent<Monster>();

        // 스탯 초기화
        tempMonsterComp.ResetBattleStat();
        tempMonsterComp.SetCurTile(curTile);
        tempMonsterComp.SetCurTileForMove(curTileForMove);
        tempMonsterComp.AlignPositionToCurTileForMove();
        
        // 객체 풀 관리. 비활성화 리스트에서 활성화 리스트로.
        monstersDisabled[lastKey].SetActive(true);
        monstersEnabled.Add(lastKey, monstersDisabled[lastKey]);
        monstersDisabled.Remove(lastKey);
    }

    public List<TileForMove> FindBlanks(int needed)
    {
        List<TileForMove> result = new List<TileForMove>();

//        TileForMove tileBeneathActor;
        string keyXY;

//        // occupiedTerritory 초기화.
//        foreach (string key in occupiedTerritory.Keys.ToList())
//            occupiedTerritory[key] = false;

//#if DEBUG_HA_REGEN
//                Debug.Log("monster count : " + monstersEnabled.Count);
//#endif
//        // 몬스터가 들어가 있는 자리 확인
//        for (int i = 0; i < monstersEnabled.Count; i++)
//        {
//            tileBeneathActor = monstersEnabled.Values.ToArray<GameObject>()[i].GetComponent<Monster>().GetCurTileForMove();
//            keyXY = tileBeneathActor.GetX().ToString() + "." + tileBeneathActor.GetY().ToString();

//            if (territory.ContainsKey(keyXY))
//                occupiedTerritory[keyXY] = true;
//        }

//        // 모험가가 들어가 있는 자리 확인
//        for (int i = 0; i < adventurersInside.Count; i++)
//        {
//            tileBeneathActor = adventurersInside[i].GetComponent<Adventurer>().GetCurTileForMove();
//            keyXY = tileBeneathActor.GetX().ToString() + "." + tileBeneathActor.GetY().ToString();

//            if (territory.ContainsKey(keyXY))
//                occupiedTerritory[keyXY] = true;
//        }

        RefreshOccupiedTerritory();

        // 랜덤으로 needed(몬스터 리젠할 칸 수)만큼 빈 칸을 result 에 추가. 
        int insertionCnt = 0;
        int randomNum;
        while (insertionCnt < needed)
        {
            while (true)
            {
                randomNum = Random.Range(0, territory.Count);
#if DEBUG_HA_REGEN
                Debug.Log("randomNum : " + randomNum + ", ocupiedCnt : " + occupiedTerritory.Count);
#endif
                keyXY = occupiedTerritory.Keys.ToList<string>()[randomNum];

                // result에 추가되지 않았고, 빈 칸일 때.
                if (!result.Contains(territory[keyXY]) && occupiedTerritory[keyXY] == false)
                {
                    result.Add(territory[keyXY]);
                    break;
                }
            }
            insertionCnt++;
        }

#if DEBUG_HA_REGEN
        Debug.Log("계산 끝. 리턴한 빈 칸 수 : " + result.Count);
#endif

        return result;
    }

    private void RefreshOccupiedTerritory()
    {
        TileForMove tileBeneathActor;
        string keyXY;

        // occupiedTerritory 초기화.
        foreach (string key in occupiedTerritory.Keys.ToList())
            occupiedTerritory[key] = false;

#if DEBUG_HA_REGEN
        Debug.Log("monster count : " + monstersEnabled.Count);
#endif
        // 몬스터가 들어가 있는 자리 확인
        for (int i = 0; i < monstersEnabled.Count; i++)
        {
            tileBeneathActor = monstersEnabled.Values.ToArray<GameObject>()[i].GetComponent<Monster>().GetCurTileForMove();
            keyXY = tileBeneathActor.GetX().ToString() + "." + tileBeneathActor.GetY().ToString();

            if (territory.ContainsKey(keyXY))
                occupiedTerritory[keyXY] = true;
        }

        // 모험가가 들어가 있는 자리 확인
        for (int i = 0; i < adventurersInside.Count; i++)
        {
            tileBeneathActor = adventurersInside[i].GetComponent<Adventurer>().GetCurTileForMove();
            keyXY = tileBeneathActor.GetX().ToString() + "." + tileBeneathActor.GetY().ToString();

            if (territory.ContainsKey(keyXY))
                occupiedTerritory[keyXY] = true;
        }
    }

    public Monster FindNearestMonster(Adventurer adv) // 인자로 받은 모험가와 가장 가까운 몬스터 찾아서 반환.
    {
        int monsterCnt = 0;
        foreach (KeyValuePair<int, GameObject> item in monstersEnabled)
            if (item.Value.GetComponent<Monster>().GetState() != State.Dead)
                monsterCnt++;
        if (monsterCnt == 0)
            return null; //몬스터가 아예 없다면 null 반환.

        //TileForMove advTFM = adv.GetCurTileForMove();
        //Monster nearest = monstersEnabled.Values.ToArray<GameObject>()[0].GetComponent<Monster>();
        //TileForMove monsterTFM = nearest.GetCurTileForMove();
        //int shortestDist = DistanceBetween(advTFM, monsterTFM);
        TileForMove advTFM = adv.GetCurTileForMove();
        Monster nearest = null;
        TileForMove monsterTFM = null;
        int shortestDist = int.MaxValue;


        foreach (KeyValuePair<int, GameObject> item in monstersEnabled)
        {
            Monster monster = item.Value.GetComponent<Monster>();
            monsterTFM = monster.GetCurTileForMove();
            if (DistanceBetween(advTFM, monsterTFM) < shortestDist && monster.curState != State.Dead)
            {
                shortestDist = DistanceBetween(advTFM, monsterTFM);
                nearest = item.Value.GetComponent<Monster>(); // 일단 애드 나고 안나고 떠나서 가까운 거 먼저 치게 돼있음.
            }
        }

        return nearest;
    }

    public TileForMove FindNearestBlank(TileForMove curPos)
    {
        RefreshOccupiedTerritory();

        TileForMove nearest = null;
        int minDist = int.MaxValue;

        foreach(KeyValuePair<string, bool> item in occupiedTerritory)
        {
            if(item.Value == false && territory[item.Key].GetDistance(curPos) <= minDist)
            {
                if (territory[item.Key].GetDistance(curPos) == minDist)
                {
                    if (Random.Range(0, 2) == 1)
                        nearest = territory[item.Key];
                }
                else
                {
                    nearest = territory[item.Key];
                    minDist = nearest.GetDistance(curPos);
                }
            }
        }

        return nearest;
    }

    protected int DistanceBetween(TileForMove pos1, TileForMove pos2)
    {
        return Mathf.Abs(pos1.GetX() - pos2.GetX()) + Mathf.Abs(pos1.GetY() - pos2.GetY());
    }

    int BlanksCount()
    {
        int result = 0;
        foreach (KeyValuePair<string, bool> member in occupiedTerritory)
            if (!member.Value)
                result++;

        return result;
    }

    // 몬스터 리젠을 위해 사냥터 영역을 받는 함수.
    public void AddTerritory(TileForMove input)
    {
        // 키 값이 "x.y"형태로 저장됨.
        string keyXY = input.GetX().ToString() + "." + input.GetY().ToString();
        territory.Add(keyXY, input);

        occupiedTerritory.Add(keyXY, false);
    }

    // 모험가 입장시 해당 모험가를 리스트에 등록
    public void EnterAdventurer(GameObject adventurer)
    {
        adventurersInside.Add(adventurer);
    }

    // 모험가 퇴장시 리스트에서 제거
    public void ExitAdventurer(GameObject adventurer)
    {
        adventurersInside.Remove(adventurer);
    }

    public void OnMonsterCorpseDecay(int index)
    {
        monstersEnabled[index].SetActive(false);
        monstersDisabled.Add(index, monstersEnabled[index]);
        monstersEnabled.Remove(index);
    }

    // Use this for initialization
#region 수정!
    void Start()
    {
        GameObject tempMonster;

        // 몬스터 초기화
        for (int i = 0; i < monsterMax + monsterPerRegen; i++)
        {
            // 생성만 해놓고 비활성화
            monsterSample1.SetActive(false);

            // List에 추가
            if (Random.Range(0.0f, 1.0f) < monsterRatio)
            {
                tempMonster = Instantiate(monsterSample1);
                tempMonster.GetComponent<Monster>().InitMonster(monsterSample1.GetComponent<Monster>());
                tempMonster.name = i + "_Sample1";
            }
            else
            {
                tempMonster = Instantiate(monsterSample2);
                tempMonster.GetComponent<Monster>().InitMonster(monsterSample2.GetComponent<Monster>());
                tempMonster.name = i + "_Sample2";
            }
            tempMonster.GetComponent<Monster>().index = i;
            tempMonster.GetComponent<Monster>().SetHabitat(this);
            tempMonster.GetComponent<Monster>().corpseDecayEvent += OnMonsterCorpseDecay;
            tempMonster.transform.parent = this.gameObject.transform;

            monstersDisabled.Add(i, tempMonster);

            //monsterSample1.transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);
            //monstersDisabled[i].transform.parent = this.gameObject.transform;
            //monstersDisabled[i].GetComponent<Monster>().SetHabitat(this);
            //monstersDisabled[i].GetComponent<Monster>().index = i;
           
            // Debug.Log("character instantiate - " + i);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(MonsterRegenCycle());
    }
#endregion
}
