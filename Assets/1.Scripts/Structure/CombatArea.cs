using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class CombatArea : Place
{
    // 사냥터 안의 몬스터, 모험가들
    public Dictionary<int, GameObject> monstersEnabled;
    public Dictionary<int, GameObject> monstersDisabled; // 초기화는 MonstersMax + MonsterPerRegen
    public List<GameObject> adventurersInside; // 입장한 모험가들

    // 빈칸. 리젠용.
    public List<TileForMove> blanks;
    public Dictionary<string, TileForMove> territory;
    public Dictionary<string, bool> occupiedTerritory;

    // 스테이지 진행/공략 용
    public delegate void AreaConqueredEventHandler();
    public event AreaConqueredEventHandler areaConquered;


    // 세이브/로드용
    #region Save
    public string stageNum;
    #endregion

    // 현재 살아있는 몬스터 리스트 Get
    public Dictionary<int, GameObject> GetMonstersEnabled()
    {
        return monstersEnabled;
    }

    public Dictionary<int, GameObject> GetMonstersDisabled()
    {
        return monstersDisabled;
    }

    public List<GameObject> GetAdventurersInside()
    {
        return adventurersInside;
    }

    // 빈 칸 판단을 위한 OccupiedTerritoy 최신화.
    protected void RefreshOccupiedTerritory()
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

        foreach (KeyValuePair<string, bool> item in occupiedTerritory)
        {
            if (item.Value == false && territory[item.Key].GetDistance(curPos) <= minDist)
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

    public List<TileForMove> FindBlanks(int needed)
    {
        List<TileForMove> result = new List<TileForMove>();

        string keyXY;

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

    protected int DistanceBetween(TileForMove pos1, TileForMove pos2)
    {
        return Mathf.Abs(pos1.GetX() - pos2.GetX()) + Mathf.Abs(pos1.GetY() - pos2.GetY());
    }

    protected int BlanksCount()
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
        if(adventurersInside.Contains(adventurer) == false)
            adventurersInside.Add(adventurer);
    }

    // 모험가 퇴장시 리스트에서 제거
    public void ExitAdventurer(GameObject adventurer)
    {
        adventurersInside.Remove(adventurer);
    }

    public virtual void OnMonsterCorpseDecay(int index)
    {
        
    }

    public void InvokeAreaConqueredEvent()
    {
        areaConquered?.Invoke();
    }
    #region SaveLoad
    public virtual void InitFromSaveData(CombatAreaData input)
    {
        
    }
    #endregion
}
