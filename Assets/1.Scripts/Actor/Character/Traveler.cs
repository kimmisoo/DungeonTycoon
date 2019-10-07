using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traveler : Actor {
	//acting 구성
	//useStructure ~ 구현
	

	Structure destination = null;
	protected int pathFindCount = 0;
	protected void Awake()
	{
		base.Awake();
	}
	// Use this for initialization
	void Start () {
		//_stat = GameManager.Instance.GetNewStats(Type Traveler);
		pathFinder.SetValidateTile(ValidateNextTile);
		SetPathFindEvent();
	}

	public Stat stat
	{
		get
		{
			return _stat;
		}
	}

	private Stat _stat;

	IEnumerator Act()
	{
		while(true)
		{
			yield return null;
			switch(state)
			{
				case State.Idle:
					destination = FindNextDestinationByDesire();
					yield return StartCoroutine(pathFinder.Moves(curTile, destination.GetEntrance()));
					//길찾기 후 State = Moving으로 변경.
					//delegate call됨()
					break;
				case State.Moving:
					//찾은 경로를 통해 1칸씩 이동? 혹은 한번에(코루틴 통해) 이동.
					break;
				case State.Indoor:
					//건물 들어가서 계산을 마치고 invisible로 건물
					break;
				default:
					break;
			}
		}
	}
	
	public Structure FindNextDestinationByDesire()
	{
		Desire next = stat.GetHighestDesire();
		return StructureManager.Instance.FindStructureByDesire(next, stat);
		
	}
	public override void SetPathFindEvent()
	{
		pathFinder.SetNotifyEvent(PathFindSuccess, PathFindFail);
	}

	public void PathFindSuccess()
	{
		pathFindCount = 0;
		state = State.Moving;
	}
	public void PathFindFail()
	{
		pathFindCount++;
		state = State.Idle;
	}

	public override bool ValidateNextTile(Tile tile) // Pathfinder delegate
	{
		if (tile.GetPassable())
			return true;
		return false;
	}

	

	

	
	
}
