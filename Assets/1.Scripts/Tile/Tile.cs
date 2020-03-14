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

    bool isStructed = false;
    bool isNonTile = false;
    //수정요망. 임시로 퍼블릭으로 해놓음
    public bool isPassable = false;
    public bool isBuildable = false;
	public bool isHuntingArea = false;

    [SerializeField]
    // 수정요망 이거 Place로 고치고 관련 코드 고쳐야 함. 일단 보류 쓰는 곳 없음.
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
	public TileForMove[] childs = new TileForMove[4];// up, right, left, down

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
		while(isPassable || isHuntingArea)
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
        return isPassable || isHuntingArea;
    }
    public bool GetPassableMonster()
    {
        return isHuntingArea;
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
	public void SetHuntingArea(bool _huntingArea)
	{
        isHuntingArea = _huntingArea;
	}
	public bool GetHuntingArea()
	{
		return isHuntingArea;
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
		if (distanceX == 0 && distanceY == 0)
			return Direction.None;
		if(Mathf.Abs(distanceX) > Mathf.Abs(distanceY)) //x축 이동
		{
			if(distanceX > 0) //t가 작은 경우
				return Direction.UpLeft;
			else 
				return Direction.DownRight;
		}
		else // y축 이동
		{
			if (distanceY > 0) //t가 작은 경우
				return Direction.UpRight;
			else
				return Direction.DownLeft;
		}
        
    }

	public string ToString()
	{
		return GetX() + " , " + GetY();
	}
    
}
