using UnityEngine;
using System.Collections;

// 모험가 이동 타일 클래스
public class TileForMove
{
	private Vector3 pos;
	private int x;
	private int y;
	private Tile parent;
	private int childNum = 0;
	private Actor recentActor = null;

	public TileForMove(int _x, int _y, Vector3 _pos, Tile _parent)
	{
		x = _x;
		y = _y;
		pos = _pos;
		parent = _parent;
	}

	public Vector3 GetPosition()
	{
		return pos;
	}
	public int GetX()
	{
		return x;
	}
	public int GetY()
	{
		return y;
	}
	public bool GetPassableParent()
	{
		return parent.GetPassable();
	}
    public Tile GetParent()
    {
        return parent;
    }
	public void SetChildNum(int num)
	{
		childNum = num;
	}
	public int GetChildNum()
	{
		return childNum;
	}
	public Actor GetRecentActor()
	{
		return recentActor;
	}
	public void SetRecentActor(Actor recent)
	{
		recentActor = recent;
	}
	public int GetDistance(TileForMove another)
	{
		return Mathf.Abs(x - another.GetX()) + Mathf.Abs(y - another.GetY());
	}
	
}
