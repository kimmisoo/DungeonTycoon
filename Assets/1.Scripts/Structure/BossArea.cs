using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArea : HuntingArea
{
    private GameObject bossMonster;
    public int Bonus
    {
        get; private set;
    }

    public int ChallengeLevel
    {
        get; private set;
    }

    #region Save
    public int bossAreaNum;
    public int bossAreaIndex;
    #endregion

    public void InitBossArea(GameObject bossMonsterIn, int challengeLevel, int bonus)
    {
        bossMonster = bossMonsterIn;
        //bossMonster.GetComponent<Monster>().corpseDecayEvent += OnBossKilled;
        ChallengeLevel = challengeLevel;
        Bonus = bonus;

        // 보스몬스터 위치 지정
        TileLayer tileLayer = TileMapGenerator.Instance.tileMap_Object.transform.GetChild(0).GetComponent<TileLayer>();

        Tile centerTile = tileLayer.GetTile(point.GetX() + 2, point.GetY() + 2).GetComponent<Tile>();
        bossMonster.GetComponent<Monster>().SetCurTile(centerTile);
        bossMonster.GetComponent<Monster>().SetCurTileForMove(centerTile.GetChild(0));
        //GameObject center = tileLayer.GetTile(point.GetX() + extentWidth / 2, point.GetY() + extentHeight / 2);
        
        //Tile centerTile = tileLayer.GetTile(point.GetX() + extentWidth / 2, point.GetY() + extentHeight / 2).GetComponent<Tile>();
        //List<TileForMove> tfm = FindBlanks(1);
        //bossMonster.GetComponent<Monster>().SetCurTile(tfm[0].GetParent());
        //bossMonster.GetComponent<Monster>().SetCurTileForMove(tfm[0]);

        bossMonster.GetComponent<Monster>().AlignPositionToCurTileForMove();
        bossMonster.SetActive(true);

        monstersEnabled.Add(0, bossMonster);
    }

    public GameObject GetBossMonster()
    {
        return bossMonster;
    }

    public void OnBossKilled(int idx)
    {
        InvokeAreaConqueredEvent();
    }

    public void OpenToPublic()
    {

    }
}
