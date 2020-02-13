using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum State
{
	//Idle, Chasing, Moving, Battle, Indoor, Dead, Exit
	None, Idle, Wandering, SearchingStructure, PathFinding, MovingToDestination, WaitingStructure, UsingStructure, SearhcingMonster, MovingToMonster, Battle, Dead, Exit
}
/*
 * Animator Tirggers
 * MoveFlg
 * AttackFlg
 * JumpFlg
 * DamageFlg
 * WinFlg
 * DeathFlg
 * SkillFlg
 * DownToUpFlg
 * UpToDownFlg
 * ResurrectingFlg
 */

public abstract class Actor : MonoBehaviour {

	[SerializeField]
	protected State state;
	/*public string actorName { get; set; }
	public string explanation { get; set; }
	public int gold { get; set; }*/

	protected PathFinder pathFinder;
	protected List<TileForMove> wayForMove;
	protected Direction direction;
	protected Animator animator;
	protected SpriteRenderer[] spriteRenderers;
	


	protected Tile curTile;
	protected TileForMove curTileForMove;
	protected  TileLayer tileLayer;
	
	
	protected void Awake()
	{
		animator = GetComponent<Animator>();
		spriteRenderers = GetComponents<SpriteRenderer>();
		pathFinder = GetComponent<PathFinder>();
		wayForMove = new List<TileForMove>();
		state = new State();
		direction = new Direction();
	}
	public Actor[] GetAdjacentActor(int range)
	{
		List<Actor> adjacentActors = new List<Actor>();
		TileLayer layer = pathFinder.tileLayer;
		int x = GetCurTileForMove().GetX();
		int y = GetCurTileForMove().GetY();
		TileForMove tileForMoveTemp;
		for (int i = -range; i <= range; i++)
		{
			for (int j = -range; j <= range; j++)
			{
				if (Mathf.Abs(i) + Mathf.Abs(j) > range) // + 실제 actor가 타일 위에 있는지
					continue;
				tileForMoveTemp = layer.GetTileForMove(x + i, y + j);
				if(tileForMoveTemp.Equals(tileForMoveTemp.GetRecentActor().GetCurTileForMove())) // tileForMoveTemp에 기록된 recentActor의 현재위치가 tileForMoveTemp와 일치하는지
				{
					adjacentActors.Add(layer.GetTileForMove(x + i, y + j).GetRecentActor());
				}
				else
				{
					tileForMoveTemp.SetRecentActor(null);
				}
			}
		}
		return adjacentActors.ToArray();
	}
	
	public void SetCurTile(Tile _tile)
	{
		curTile = _tile;
		pathFinder.SetCurTile(_tile);
	}
	public Tile GetCurTile()
	{
		return curTile;//pathFinder.GetCurTile();
	}
	public void SetCurTileForMove(TileForMove _tileForMove)
	{
		curTileForMove = _tileForMove;
		
	}
	public TileForMove GetCurTileForMove()
	{
		return curTileForMove;//pathFinder.GetCurTileForMove();
	}
	public int GetDistanceFromOtherActorForMove(Actor actor)
	{
		if(actor != null)
		{
			return Mathf.Abs(actor.GetCurTileForMove().GetX() - GetCurTileForMove().GetX()) + Mathf.Abs(actor.GetCurTileForMove().GetY() - GetCurTileForMove().GetY()); 
		}
		else
		{
			return int.MaxValue;
		}
	}
	
	public Direction GetDirectionFromOtherTileForMove(TileForMove tileForMove)
	{
		Direction direction = Direction.DownLeft;
		int distanceX = GetCurTileForMove().GetX() - tileForMove.GetX();
		int distanceY = GetCurTileForMove().GetY() - tileForMove.GetY();
		int absX = Mathf.Abs(distanceX);
		int absY = Mathf.Abs(distanceY);

		if(absX == 0 && absY == 0)
		{
			direction = Direction.None; // 겹침
			return direction;
		}
		if (distanceX >= 0 && distanceY >= 0)
		{
			if (absX > absY)
				direction = Direction.DownRight;
			else
				direction = Direction.DownLeft;
			
		}
		else if (distanceX >=0 && distanceY < 0) //Right
		{
			if (absX > absY)
				direction = Direction.DownRight;
			else
				direction = Direction.UpRight;
		}
		else if(distanceX < 0 && distanceY >= 0) //Left
		{
			if (absX > absY)
				direction = Direction.UpLeft;
			else
				direction = Direction.DownLeft;
		}
		else
		{
			if (absX > absY)
				direction = Direction.UpLeft;
			else
				direction = Direction.UpRight;
		}
		return direction;
	}
	public TileForMove GetNextTileForMoveFromDirection(Direction direction)
	{
		//TileForMove tempTileForMove;
		switch (direction)
		{
			case Direction.DownRight:
				return tileLayer.GetTileForMove(GetCurTile().GetX() + 1, GetCurTile().GetY());
				break;
			case Direction.UpRight:
				return tileLayer.GetTileForMove(GetCurTile().GetX(), GetCurTile().GetY() - 1);
				break;
			case Direction.DownLeft:
				return tileLayer.GetTileForMove(GetCurTile().GetX() + 1, GetCurTile().GetY());
				break;
			case Direction.UpLeft:
				return tileLayer.GetTileForMove(GetCurTile().GetX() - 1, GetCurTile().GetY());
				break;
			case Direction.None:
				return GetCurTileForMove();
				break;
			default:
				return GetCurTileForMove();
				break;
		}
	}
	public State GetState()
	{
		return state;
	}
	public void SetDirection(Direction dir)
	{
		direction = dir;
	}
	public Direction GetDirection()
	{
		return direction;
	}

	

	public abstract bool ValidateNextTile(Tile tile);
	public abstract void SetPathFindEvent();

}


