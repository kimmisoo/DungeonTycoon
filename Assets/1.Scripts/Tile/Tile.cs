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

	/*
	 * 공통 사항
	 * 1.isNonTile == false - 타일인가?
	 * 
	 * 몬스터
	 * 1.isHuntingArea == true - 사냥터인가?
	 * 여행자
	 * 1.isStructed == false - 타일 위에 건설되었는가?
	 * 2.isHuntingArea == false - 사냥터인가?
	 * 모험가 - 사냥터 안에서
	 * 1.isHuntingArea == true - 사냥터인가?
	 * 2.isStructed == true - 건설되었는가?
	 * 모험가 - 사냥터 밖에서
	 * 1.isHuntingArea == false - 사냥터인가?
	 * 2.isStructed == false - 건설되었는가?
	 */
	
    bool isStructed = false;
    bool isNonTile = false;
	
	bool isBuildingArea = false;
	bool isHuntingArea = false;
	bool isRoad = false;
	bool isActive = false;
	bool isEntrance = false;
	bool isWall = false;
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
	Sprite unknown;
	Sprite origin;
    public int prefabInfo;

	public void Awake()
	{
		tile = this;
		position = transform.position;
		sr = GetComponent<SpriteRenderer>();
		origin = sr.sprite;
	}
	//타일 색상 변경
	public void SetTileColor(Color color)
	{
		sr.color = color;
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
    public void SetIsStructed(bool _isStructed)
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

	public bool GetBuildingArea()
	{
		return isNonTile==false && isActive == true ? isBuildingArea : false;
	}
    public bool GetBuildingAreaSave()
    {
        return isBuildingArea;
    }
	//Validate Tile 기본 조건 - 활성화된 타일 && 존재하는 타일 && 건설 되지 않음!
	//Traveler - Road, 
    public bool GetPassableTraveler() 
    {
		return isNonTile == false && isActive == true && isStructed == false && isHuntingArea == false && isWall == false; //&& isStructed == false && isHuntingArea == false;// && isStructed == false && isHuntingArea == false;//isStructed == false ? isRoad || isBuildingArea : false;
    }
    public bool GetPassableAdventurer() //
    {
		return isNonTile == false && isActive == true && isStructed == false && isWall == false;//isActive && (isNonTile == false || isHuntingArea == true) && isStructed == false;//&& isStructed == false;//isStructed == false ? isRoad || isBuildingArea || isHuntingArea : false; 
	}
	public bool GetPassableSpecialAdventurer()
	{
		return isNonTile == false && isStructed == false && isWall == false;
	}
    public bool GetPassableMonster()
    {
		return isHuntingArea;//isNonTile == false && isActive == true;// ? isHuntingArea : false;
	}
	public void SetIsBuildingArea(bool _isBuildingArea)
	{
		isBuildingArea = _isBuildingArea;
	}
	public void SetIsHuntingArea(bool _huntingArea)
	{
        isHuntingArea = _huntingArea;
	}
	public bool GetHuntingArea()
	{
		return isNonTile == false && isActive == true ? isHuntingArea : false;
	}
    public bool GetHuntingAreaSave()
    {
        return isHuntingArea;
    }
	public void SetRoad(bool _isRoad)
	{
		isRoad = _isRoad;
	}
	public bool GetRoad()
	{
		return isNonTile == false && isActive == true ? isRoad : false;
	}
    public bool GetRoadSave()
    {
        return isRoad; 
    }
    public void SetUnknownSprite(Sprite _unknown)
	{
		unknown = _unknown;
	}
	public void SetIsActive(bool _isActive)
	{
		isActive = _isActive;
		if(isActive == false)
		{
			//sr.sprite = unknown;
			sr.color = new Color(0.3f, 0.3f, 0.3f);
		}
		else
		{
			//sr.sprite = origin;
			sr.color = new Color(1.0f, 1.0f, 1.0f);
		}
	}
	public bool GetIsActive()
	{
		return isActive;
	}
	public void SetIsEntrance(bool _isEntrance)
	{
		isEntrance = _isEntrance;
	}
	public bool GetIsEntrance()
	{
		return isEntrance;
	}
	public void SetIsWall(bool _isWall)
	{
		isWall = _isWall;
	}
	public bool GetIsWall()
	{
		return isWall;
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
