#define DEBUG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Tile : MonoBehaviour
{
    #region Near Tiles
    Tile up;
	Tile left;
	Tile right;
	Tile down;
	Tile tile;
    #endregion

    #region Tile Attributes
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
    #endregion

    #region Unity Attributes
    SpriteRenderer sr;
    Vector3 position;
    #endregion

    #region for PathFinder
    bool isOpen = false;
	bool isClose = false;
    #endregion

    TileLayer layer;
	TileForMove[] childs = new TileForMove[4];// up, right, left, down

    public int prefabInfo;

	public void Start()
	{
        // 초기화
		tile = this;
		position = transform.position;
		sr = GetComponent<SpriteRenderer>();
        //StartCoroutine(CheckColor());

    }

    // 길찾기 디버깅용인듯
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

    // A*용 메서드
	public void AddedOpenList()
	{
		isOpen = true;
	}
	public void AddedCloseList()
	{
		isClose = true;	
	}

    // get, set
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
    public bool GetPassableTraveler()
    {
        return isPassable;
    }
    public bool GetPassableAdventurer()
    {
        return isPassable || isMonsterArea;
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
    public int GetLayerNum()
    {
        return layer_Num;
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

    // Child set
	public void SetChild(int index, TileForMove child)
	{
		childs[index] = child;
		child.SetChildNum(index);
	}

    // 이동용 타일 Child get
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

	public Direction GetDirectionFromOtherTile(Tile t)
    {
       
        int distanceX = this.GetX() - t.GetX();
        int distanceY = this.GetY() - t.GetY();
        int abs = Mathf.Abs(distanceX) + Mathf.Abs(distanceY);
        if (abs == 0 || abs >= 2)
        {
            return Direction.None;
        }
        // dX = 1 : UR
        // dX = -1: DL
        // dY = 1 : DR
        // dY = -1: UL
        if(distanceX > 1)
        {
            return Direction.UpRight;
        }
        else if(distanceX == 0)
        {
            if (distanceY > 0)
                return Direction.DownRight;
            else
                return Direction.UpLeft;
       
        }
        else if(distanceX < 0)
        {
            return Direction.DownLeft;
        }
        return Direction.None;
        
    }
    
}
