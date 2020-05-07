using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 모험가 이동 타일 클래스
public class TileForMove
{
    public static readonly float Y_COMPENSATION = 0.133f; // position 보정값

	private Vector3 pos;
	private int x;
	private int y;
	private Tile parent;
	private int childNum = 0;
    private List<Actor> recentActors;

	public TileForMove(int _x, int _y, Vector3 _pos, Tile _parent)
	{
		x = _x;
		y = _y;
        //pos = _pos;
        pos = new Vector3(_pos.x, _pos.y + Y_COMPENSATION, _pos.z);
		parent = _parent;
        recentActors = new List<Actor>();
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
	public List<Actor> GetRecentActors()
	{
		return recentActors;
	}
	public void AddRecentActor(Actor recent)
	{
		recentActors.Add(recent);
	}
    public void RemoveRecentActor(Actor recent)
    {
        recentActors.Remove(recent);
    }
	public int GetDistance(TileForMove another)
	{
		return Mathf.Abs(x - another.GetX()) + Mathf.Abs(y - another.GetY());
	}
	public Direction GetDirectionFromOtherTileForMove(TileForMove t)
	{
		int distanceX = this.GetX() - t.GetX();
		int distanceY = this.GetY() - t.GetY();
		if (distanceX == 0 && distanceY == 0)
			return Direction.None;
		if (Mathf.Abs(distanceX) > Mathf.Abs(distanceY)) //x축 이동
		{
			if (distanceX > 0) //t가 작은 경우
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
}