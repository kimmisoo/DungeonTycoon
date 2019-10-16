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

public class Traveler : Actor {
	//acting 구성
	//useStructure ~ 구현
	

	Tile destination = null;
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
		get
		{
			return _stat;
		}
	}

	private Stat _stat;

	IEnumerator Act()
	{
		Structure[] structureListByPref;
        
		while(true)
		{
			yield return null;
			switch(state)
			{
				case State.Idle:
					structureListByPref = StructureManager.Instance.FindStructureByDesire(stat.GetHighestDesire(), stat); // 1위 욕구에 따라 타입 결정하고 정렬된 건물 List 받아옴 // GC?
                    while (state == State.Idle)
                    {
                        if (pathFindCount > structureListByPref.Length)
                        {
                            destination = structureListByPref[pathFindCount].GetEntrance(); // 목적지 설정
							destinationStructure = structureListByPref[pathFindCount];
                        }
                        else
                        {
                            state = State.Exit;
							break;
                        }
                        yield return StartCoroutine(pathFinder.Moves(curTile, destination));   
                    }
                    break;
                case State.Moving: // path 존재
					wayForMove = GetWay(pathFinder.GetPath());
					animator.SetBool("MoveFlg", true);
					yield return moveAnimation = StartCoroutine(MoveAnimation(wayForMove));
					//찾은 경로를 통해 1칸씩 이동? 혹은 한번에(코루틴 통해) 이동.
					break;
				case State.Indoor:
					animator.SetBool("MoveFlg", false);
					spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
					//건물 들어가서 계산을 마치고 invisible로~ //Structure 대기열 및 순번 구현해야함.
					spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

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
	
	
	public override void SetPathFindEvent() // Pathfinder Delegate 설정
    {
		pathFinder.SetNotifyEvent(PathFindSuccess, PathFindFail);
	} 

	public void PathFindSuccess() // Pathfinder 길찾기 성공 Delegate
	{
		pathFindCount = 0;
		state = State.Moving;
	}
	public void PathFindFail() // PathFinder 길찾기 실패 Delegate
	{
		pathFindCount++;
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
				transform.Translate(directionVec * stat.GetCalculatedMovespeed() * Time.deltaTime); 
				moveDistanceSum += Vector3.Distance(before, transform.position); // 총 이동한 거리 합산.
			}
			transform.position = destPos;
			SetCurTileForMove(tileForMoveWay[i]); // 현재 타일 기록.
			SetCurTile(tileForMoveWay[i].GetParent());
		}
	} // Adventurer에서 이동 중 피격 구현해야함.



}
