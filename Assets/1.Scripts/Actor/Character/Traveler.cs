using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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

	// Disable, Enable시 변경가능한 속성
	//이름, 종족, 성별, 선호도, 캐릭터 스프라이트와 애니메이션, 

public class Traveler : Actor {
	//acting 구성
	//useStructure ~ 구현
	
	public State curState
	{
		get
		{
			return state;
		}
		set
		{
			ExitState();
			state = value;
			EnterState(state);
		}
	}
	protected int pathFindCount = 0;
	Coroutine act;
	Coroutine moveAnimation;
	Structure destinationStructure;
	
	
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
	public void InitTraveler(Stat stat) //
	{
		pathFinder.SetValidateTile(ValidateNextTile);
		SetPathFindEvent();
		//stat 초기화
		//pathfinder 초기화 // delegate 그대로

	}
	public void OnEnable()
	{
		act = StartCoroutine(Act());
        SetCurTile(GameManager.Instance.GetRandomEntrance());
        SetCurTileForMove(GetCurTile().GetChild(Random.Range(0, 3)));
		//매니저로 부터 세부 stat 받아오기
	}
	public void OnDisable()
	{
		StopCoroutine(act);
		
		//골드, 능력치 초기화...  // current , origin 따로둬야할까?
	}
	
	public Stat stat
	{
		get;
		set;
	}
	private Stat _stat;
	//FSM Pattern...
	protected void EnterState(State nextState)
	{
		switch(nextState)
		{
			/*case State.Idle:
				StartCoroutine(StructureFinding());
				break;
			case State.Moving:
				StartCoroutine(MoveToDestination());
				break;
			case State.Indoor:
				//욕구 낮추기 및 사용시간 대기
				//
				break;
			case State.Exit:
				break;
			default:
				curState = State.Exit;
				break;
			*/
			case State.Idle:
				//Traveler이므로 무조건 SearchingStructure 부터
				
				break;
			case State.SearchingStructure:
				break;
			case State.MovingToStructure:
				break;
			case State.WaitingStructure:
				break;
			case State.UsingStructure:
				break;
			case State.Exit:
				break;
			case State.None:
				curState = State.Idle;
				break;
		}
	}
	protected void ExitState()
	{
		switch(curState)
		{
			case State.Idle:
				break;
			case State.Moving:
				break;
			case State.Indoor:
				break;
			case State.Exit:
				break;
			default:
				curState = State.Exit;
				break;
		}
	}
	IEnumerator StructureFinding()
	{
		Structure[] structureListByPref;
		structureListByPref = StructureManager.Instance.FindStructureByDesire(stat.GetHighestDesire(), this);
		while (curState == State.Idle)
		{
			if (pathFindCount < structureListByPref.Length) // 길찾기 횟수가 선호건물 수 보다 적다면
			{
				destination = structureListByPref[pathFindCount].GetEntrance(); // 목적지 설정
				destinationStructure = structureListByPref[pathFindCount];
			}
			else
			{
				curState = State.Exit;
				pathFindCount = 0;
				break;
			}
			yield return StartCoroutine(pathFinder.Moves(curTile, destination));   //길찾기 시작
			//pathfind success, fail delegate call
		}
	}
	IEnumerator MoveToDestination()
	{
		//길찾기 성공!
		wayForMove = GetWay(pathFinder.GetPath()); // TileForMove로 변환
		animator.SetBool("MoveFlg", true); // animation 이동으로
		yield return moveAnimation = StartCoroutine(MoveAnimation(wayForMove)); // 이동 한번에 코루틴으로 처리 // 이동 중지할 일 있으면 StopCoroutine moveAnimation
																				//순번 or 대기 여부 결정
		if (destinationStructure.GetWaitSeconds() > 120.0f) // const?
		{
			curState = State.Idle;
			yield break;
		}
		else
		{
			if (destinationStructure.EnterTraveler(this))
			{
				curState = State.Indoor;
				yield break;
			}
			else
			{
				
				//대기 or 다시 길찾기
			}
		}
	}
	/*
	IEnumerator Act()
	{
		
		while(true)
		{
			yield return null;
			switch(state)
			{
				
                case State.Moving: // path 존재
					wayForMove = GetWay(pathFinder.GetPath()); // TileForMove로 변환
					animator.SetBool("MoveFlg", true); // animation 이동으로
					yield return moveAnimation = StartCoroutine(MoveAnimation(wayForMove)); // 이동 한번에 코루틴으로 처리 // 이동 중지할 일 있으면 StopCoroutine moveAnimation
																							//순번 or 대기 여부 결정
					if(destinationStructure.GetWaitSeconds() > 120.0f) // const?
					{
						state = State.Idle;
						break;
					}
					else
					{
						if(destinationStructure.EnterTraveler(this))
						{
							state = State.Indoor;
							break;
						}
						else
						{

						}
					}

					//if(destinationStructure.GetCurWaitingSec() > 120.0f) // 2분 이상 걸리면 다른건물 검색, 
					//state = State.Indoor 
					//바로입장.
					//입장까지 대기 할 수도 있고
					//state = State.Idle 결정하기


					///////////////////////////////
					 animator.SetBool("MoveFlg", false);
					spriteRenderer.color = Color.clear;
					while (true)
					{
						yield return new WaitForSeconds(1.0f); // 순번 설정해야되는디
						if (destinationStructure.EnterTraveler(this))
						{
							stat.GetSpecificDesire(curDesire).desireValue -= destinationStructure.resolveAmount;
							stat.gold -= destinationStructure.charge;
							yield return new WaitForSeconds(destinationStructure.duration);
							break;
						}
						//붐비고있음 알림?
					}
					spriteRenderer.color = Color.white;
					 ///////////////////////////////////
					break;
				case State.Indoor:
					//Do nothing...
					
					break;
				case State.Exit:
                    destination = GameManager.Instance.GetRandomEntrance();
					yield return StartCoroutine(pathFinder.Moves(curTile, destination));
					destinationStructure = null;
					wayForMove = GetWay(pathFinder.GetPath());
					yield return moveAnimation = StartCoroutine(MoveAnimation(wayForMove));
					gameObject.SetActive(false);
					break;
				default:
					break;
			}
		}
	}
	*/
	
	public override void SetPathFindEvent() // Pathfinder Delegate 설정
    {
		pathFinder.SetNotifyEvent(PathFindSuccess, PathFindFail);
	}

	public void PathFindSuccess() // Pathfinder 길찾기 성공 Delegate
	{
		pathFindCount = 0;
		curState = State.Moving;
	}
	public void PathFindFail() // PathFinder 길찾기 실패 Delegate
	{
		pathFindCount++;
		if(state == State.Exit)
		{
			//즉시 탈출
			//최대 손님 - 1
			//평판 --
			//알림
			//disable
		}
	}

	public override bool ValidateNextTile(Tile tile) // Pathfinder delegate
	{
        return tile.GetPassableTraveler();
	}

	protected List<TileForMove> GetWay(List<PathVertex> path) // Pathvertex -> TileForMove
    {
        List<TileForMove> tileForMoveWay = new List<TileForMove>();
		int childNum = GetCurTileForMove().GetChildNum();
		tileForMoveWay.Add(GetCurTileForMove());
		Direction dir;
        for(int i= 1; i<path.Count - 1; i++)
        {
			dir = GetCurTile().GetDirectionFromOtherTile(path[i].myTilePos);
			switch(dir)
			{
				case Direction.UpRight: // 2
					if(childNum >= 2) //이동할 다음 이동타일이 현재 타일의 Child인지?
					{
						childNum -= 2;
						tileForMoveWay.Add(path[i].myTilePos.GetChild(childNum)); // tile내부에서 1칸 이동
					}
					//다음 타일
					childNum += 2;
					tileForMoveWay.Add(path[i + 1].myTilePos.GetChild(childNum));
					//한칸 더 갈지 말지?
					if (Random.Range(0, 2) < 1)
					{
						childNum -= 2;
						tileForMoveWay.Add(path[i + 1].myTilePos.GetChild(childNum));
					}
					break;
				case Direction.UpLeft: //  1

					if (childNum % 2 == 1)
					{
						childNum -= 1;
						tileForMoveWay.Add(path[i].myTilePos.GetChild(childNum)); // tile내부에서 1칸 이동
					}
					childNum += 1;
					tileForMoveWay.Add(path[i + 1].myTilePos.GetChild(childNum));
					if (Random.Range(0, 2) < 1)
					{
						childNum -= 1;
						tileForMoveWay.Add(path[i + 1].myTilePos.GetChild(childNum));
					}
					break;
				case Direction.DownRight: // 1

					if (childNum % 2 == 0)
					{
						childNum += 1;
						tileForMoveWay.Add(path[i].myTilePos.GetChild(childNum));
					}
					childNum -= 1;
					tileForMoveWay.Add(path[i + 1].myTilePos.GetChild(childNum));
					if (Random.Range(0, 2) < 1)
					{
						childNum += 1;
						tileForMoveWay.Add(path[i + 1].myTilePos.GetChild(childNum));
					}
					break;
				case Direction.DownLeft: // + 2

					if (childNum <= 1)
					{
						childNum += 2;
						tileForMoveWay.Add(path[i].myTilePos.GetChild(childNum));
					}
					childNum -= 2;
					tileForMoveWay.Add(path[i + 1].myTilePos.GetChild(childNum));
					if (Random.Range(0, 2) < 1)
					{
						childNum += 2;
						tileForMoveWay.Add(path[i + 1].myTilePos.GetChild(childNum));
					}
					break;
			}
        }
		return tileForMoveWay;
    }
    // dX = 1 : UR
    // dX = -1: DL
    // dY = 1 : DR
    // dY = -1: UL

	IEnumerator MoveAnimation(List<TileForMove> tileForMoveWay)
	{
		yield return null;
		
		Direction dir;
		Vector3 destPos, before, directionVec;
		float distance, moveDistanceSum = 0.0f;
		for(int i=1; i<tileForMoveWay.Count; i++)
		{
			tileForMoveWay[i].SetRecentActor(this); // tileForMove에 거쳐갔다고 기록.
			destPos = tileForMoveWay[i].GetPosition(); // 다음 목적지 포지션
			distance = Vector3.Distance(transform.position, destPos); // 1칸 이동 끝 검사를 위해 벡터 거리 계산해놓기
			moveDistanceSum = 0.0f; // 총 이동한 거리 초기화
			directionVec = Vector3.Normalize(destPos - transform.position); // 방향벡터 캐싱
			while (moveDistanceSum >= distance)
			{
				before = transform.position; // 이전 프레임 위치 기록
				yield return null;
				transform.Translate(directionVec * stat.GetCalculatedMovespeed() * Time.deltaTime); //TimeScale 말고
				moveDistanceSum += Vector3.Distance(before, transform.position); // 총 이동한 거리 합산.
			}
			transform.position = destPos;
			SetCurTileForMove(tileForMoveWay[i]); // 현재 타일 기록.
			SetCurTile(tileForMoveWay[i].GetParent());
		}
	} // Adventurer에서 이동 중 피격 구현해야함. // Notify?

	public IEnumerator WaitForEnteringStructure()
	{
		while(true)
		{
			yield return null;
		}
	}
	public IEnumerator WaitForUsingStrcuture()
	{
		while (true)
		{
			yield return null;
		}
	}
	

}
