using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossArea : HuntingArea
{
    private GameObject bossMonster;

    #region SaveLoad
    public int bossAreaNum;
    //public int index;
    #endregion

    public int Bonus
    {
        get; private set;
    }

    public int ChallengeLevel
    {
        get; private set;
    }

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
        bossMonster.transform.parent = this.gameObject.transform;

        monstersEnabled.Add(0, bossMonster);
    }

    public GameObject GetBossMonster()
    {
        return bossMonster;
    }
	public Monster GetBossMonsterAsComponent()
	{
		return bossMonster.GetComponent<Monster>();
	}
    public void OnBossKilled(int idx)
    {
        InvokeAreaConqueredEvent();
    }

    public void OpenToPublic()
    {

    }

    #region SaveLoad
    public override void InitFromSaveData(CombatAreaData input)
    {
        //base.InitFromSaveData(input);
        GameManager.InitLoadedMonster(bossMonster, bossMonster, input.monstersEnabled[0], false);
        bossMonster.transform.parent = this.gameObject.transform;
        bossMonster.GetComponent<Monster>().SetHabitat(this);

        int key = monstersEnabled.Keys.ToList()[0];
        bossMonster = monstersEnabled[key];
    }

    public override PlaceType GetPlaceType()
    {
        return PlaceType.HuntingArea;
    }
    #endregion
}
