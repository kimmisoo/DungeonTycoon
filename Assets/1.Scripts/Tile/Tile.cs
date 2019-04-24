﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Tile : MonoBehaviour {

	Tile up;
	Tile left;
	Tile right;
	Tile down;
	Tile tile;
	public int x = -1;
	public int y = -1;
	int layer_Num = -1;
	int tile_Type = -1;
	bool isPassable = false;
    bool isStructed = false;
    bool isNonTile = false;
	bool isBuildable = false;
	bool isMonsterArea = false;
    Structure structure;
	SpriteRenderer sr;
	
	
	Vector3 position;
	bool isOpen = false;
	bool isClose = false;
	TileLayer layer;
	TileForMove[] childs = new TileForMove[4];// l u d r
	public void Start()
	{
		tile = this;
		position = transform.position;
		sr = GetComponent<SpriteRenderer>();
		
		//StartCoroutine(CheckColor());
			
	}
	IEnumerator CheckColor()
	{
		while(isPassable || isMonsterArea)
		{
			yield return new WaitForSeconds(0.1f);
			if(isOpen == true)
				sr.color = Color.red;
			if(isClose == true)
				sr.color = Color.black;
		}
	}
	public void AddedOpenList()
	{
		isOpen = true;
	}
	public void AddedCloseList()
	{
		isClose = true;	
	}
	public Vector3 GetPosition()
	{
		return position;
	}
	public bool GetNonTile()
    {
        return isNonTile;
    }
    public void SetNonTile(bool t)
    {
        isNonTile = t;
    }
    public bool GetStructed()
    {
        return isStructed;
    }
    public void SetStructed(bool _isStructed)
    {
        isStructed = _isStructed;
    }
    public Structure GetStructure()
    {
        return structure;
    }
    public void SetStructure(Structure s)
    {
        structure = s;
    }
	public bool GetPassable()
	{
		return isPassable;
	}
	public void SetPassable(bool _isPassable)
	{
		isPassable = _isPassable;
	}
	public bool GetBuildable()
	{
		return isBuildable;
	}
	public void SetBuildable(bool _isBuildable)
	{
		isBuildable = _isBuildable;
	}
	public void SetMonsterArea(bool _monsterArea)
	{
		isMonsterArea = _monsterArea;
	}
	public bool GetMonsterArea()
	{
		return isMonsterArea;
	}
	public void SetX(int _x)
	{
		x = _x;
	}
	public void SetY(int _y)
	{
		y = _y;
    }
	public void SetLayerNum(int _layer_Num)
	{
		layer_Num = _layer_Num;
	}
	public void SetLayer(TileLayer _layer)
	{
		layer = _layer;
	}
	public TileLayer GetLayer()
	{
		return layer;
	}
	public void SetTileType(int _tile_Type)
	{
		tile_Type = _tile_Type;
	}
    public int GetTileType()
	{
		return tile_Type;
	}
	public int GetX()
	{
		return x;
	}
	public int GetY()
	{
		return y;
	}
	public void SetChild(int index, TileForMove child)
	{
		childs[index] = child;
		child.SetChildNum(index);
	}
	public TileForMove GetChild(int index)
	{
		if (childs[index] == null)
		{
			Debug.Log("Child is null.");
			return null;
		}
		else
			return childs[index];
	}
	
}
