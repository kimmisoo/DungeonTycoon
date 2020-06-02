using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArea : Place
{
    private GameObject bossMonster;

    #region Save
    public string stageNum;
    public int bossAreaNum;
    public int bossAreaIndex;
    #endregion

    // 스테이지 진행/공략 용
    public delegate void AreaConqueredEventHandler();
    public event AreaConqueredEventHandler areaConquered;

    public void InitBossArea(GameObject bossMonsterIn)
    {
        bossMonster = bossMonsterIn;
        bossMonster.GetComponent<Monster>().corpseDecayEvent += OnBossKilled;

        // + 보스 몬스터 좌표설정.
    }

    public void OnBossKilled(int idx)
    {
        areaConquered?.Invoke();
    }
}
