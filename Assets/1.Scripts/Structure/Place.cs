using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum PlaceType
{
    Structure, HuntingArea, BossArea
}

public abstract class Place : MonoBehaviour
{
    // 세이브 로드용
    public int index;
    public bool isNew = true;

    public Tile point //extent 기준 0,0의 타일
    {
        get; set;
    }

    public bool isEnterable
    {
        get; set;
    }
    public string names
    {
        get; set;
    }
    public int capacity
    {
        get; set;
    }

    public List<Tile> entrance = new List<Tile>();


    public void addEntrance(Tile t)
    {
        entrance.Add(t);
    } // 활성화된 입구만 담겨있음
    public Tile GetEntrance()
    {
        int randNum = Random.Range(0, entrance.Count);
		if (entrance == null || entrance.Count <= 0)
			return null;
        return entrance[randNum];
    }
    public int extentWidth
    {
        get; set;
    }
    public int extentHeight
    {
        get; set;
    }
    public int[,] extent;


    public int[,] GetExtent()
    {
        return extent;
    }

    public virtual void Visit(Actor visitor)
    {    }

    #region SaveLoad
    public abstract PlaceType GetPlaceType();
    #endregion
}


