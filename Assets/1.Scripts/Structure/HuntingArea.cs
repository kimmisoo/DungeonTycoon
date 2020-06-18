using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HuntingArea : CombatArea
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

    private int conquerCondition;
    private int killCount;


    private int index;

    private GameObject monsterSample1;
    private GameObject monsterSample2;
    

    #region SaveLoad
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


    public void InitHuntingArea(int conquerConditionIn, bool isBossAreaIn, int lvMax, int mobMax/* = 42*/, int mobPerRegen/* = 7*/, float mobRegenRate/* = 5.5f*/, float mobRatio,
        GameObject mobSample1, GameObject mobSample2)
    {
        conquerCondition = conquerConditionIn;
        IsBossArea = isBossAreaIn;
        levelMax = lvMax;
        monsterMax = mobMax;
        monsterPerRegen = mobPerRegen;
        monsterRegenRate = mobRegenRate;
        monsterRatio = mobRatio;

        // 이부분 복사 제대로 되는지 봐야. 수정요망. 아마 될듯, 복사가 아니라 참조로.
        monsterSample1 = mobSample1;
        monsterSample2 = mobSample2;

        killCount = 0;
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

    

    public override void OnMonsterCorpseDecay(int index)
    {
        // 몬스터 제거 및 다시 객체풀에 넣어주기
        monstersEnabled[index].SetActive(false);
        monstersDisabled.Add(index, monstersEnabled[index]);
        monstersEnabled.Remove(index);

        // 공략 조건 관련
        if (killCount < conquerCondition)
        {
            killCount++;
            if (killCount >= conquerCondition)
                InvokeAreaConqueredEvent();

            //Debug.Log("killCount : " + killCount);
        }
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

    #region StageProgress
    public bool IsBossArea
    {
        get; private set;
    }

    /// <summary>
    /// 일선 모험가 말고도 일반 모험가와 관광객에게 개방
    /// </summary>
    public void OpenToPublic()
    {

    }

    public int GetKillCount()
    {
        return killCount;
    }

    public void SetKillCount(int killCountIn)
    {
        killCount = killCountIn;
    }
    #endregion
}
