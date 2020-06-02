using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArea : HuntingArea
{
    private GameObject bossMonster;

    #region Save
    public int bossAreaNum;
    public int bossAreaIndex;
    #endregion

    public void InitBossArea(GameObject bossMonsterIn)
    {
        bossMonster = bossMonsterIn;
        bossMonster.GetComponent<Monster>().corpseDecayEvent += OnBossKilled;

        // 보스몬스터 위치 지정
        TileLayer tileLayer = TileMapGenerator.Instance.tileMap_Object.transform.GetChild(0).GetComponent<TileLayer>();

        //Tile centerTile = tileLayer.GetTile(point.GetX() + extentWidth / 2, point.GetY() + extentHeight / 2).GetComponent<Tile>();
        //GameObject center = tileLayer.GetTile(point.GetX() + extentWidth/2, point.GetY() + extentHeight/2);
        //Tile centerTile = tileLayer.GetTile(point.GetX() + extentWidth / 2, point.GetY() + extentHeight / 2).GetComponent<Tile>();
        List<TileForMove> tfm = FindBlanks(1);
        bossMonster.GetComponent<Monster>().SetCurTile(tfm[0].GetParent());
        bossMonster.GetComponent<Monster>().SetCurTileForMove(tfm[0]);
        bossMonster.GetComponent<Monster>().AlignPositionToCurTileForMove();
        bossMonster.SetActive(true);

        monstersEnabled.Add(0, bossMonster);
    }

    public void OnBossKilled(int idx)
    {
        InvokeAreaConqueredEvent();
    }
}
