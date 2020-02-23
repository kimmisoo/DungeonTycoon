using System.Collections;
using System.Collections.Generic;
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
    private List<GameObject> monstersEnabled;
    private List<GameObject> monstersDisabled; // 초기화는 MonstersMax + MonsterPerRegen

    int index;

    GameObject monsterSample1;
    GameObject monsterSample2;

    public void InitHuntingArea(int lvMax, int mobMax = 42, int mobPerRegen = 7, float mobRegenRate = 5.5f)
    { 
        levelMax = lvMax;
        monsterMax = mobMax;
        monsterPerRegen = mobPerRegen;
        monsterRegenRate = mobRegenRate;
    }

    // 현재 살아있는 몬스터 리스트 Get
    public List<GameObject> GetMonstersEnabled()
    {
        return monstersEnabled;
    }

    // 게임 내내 Regen
    private IEnumerator MonsterRegenCycle()
    {
        int temp = 0;
        while (true)
        {
            while (temp < monsterPerRegen && monstersEnabled.Count < monsterMax)
            {
                // 몬스터 리젠
                MonsterRegen();
                temp++;
            }
            yield return new WaitForSeconds(monsterRegenRate);
        }
    }

    private void MonsterRegen()
    {
        int index = monstersDisabled.Count - 1;
        if (Random.Range(0, 2) == 0)
        {

            //monstersDisabled[index]에 monsterSample1 의 정보 대입
            //monstersDisabled[index] = monsterSample1;

        }
        else
        {
            //monstersDisabled[index]에 monsterSample2 의 정보 대입
            //monstersDisabled[index] = monsterSample2;
        }
        monstersDisabled[index].SetActive(true);
        monstersEnabled.Add(monstersDisabled[index]);
        monstersDisabled.RemoveAt(index);
    }

    // Use this for initialization
    #region 수정!
    void Start()
    {
        // 몬스터 초기화
        for (int i = 0; i < monsterMax + monsterPerRegen; i++)
        {
            monsterSample1 = (GameObject)Resources.Load("CharacterPrefabs/Traveler_test"); //수정요망.

            // 생성만 해놓고 비활성화
            monsterSample1.SetActive(false);

            // List에 추가
            monstersDisabled.Add(Instantiate(monsterSample1));
            monsterSample1.transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);
            monstersDisabled[i].transform.parent = GameObject.FindGameObjectWithTag("HuntingArea").transform;
            monstersDisabled[i].GetComponent<Monster>().index = i;
            // Debug.Log("character instantiate - " + i);
        }
    }
    #endregion
}
