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
	protected int wanderCount = 0;
	protected Coroutine curCoroutine;
	protected Tile destinationTile;
	protected Structure destinationStructure;
	protected Structure[] structureListByPref;

    // 저장 및 로드를 위한 인덱스. travelerList에서 몇번째인지 저장.
    public int index;

	protected void Awake()
	{
		base.Awake();
	}
	// Use this for initialization
	
	public void InitTraveler(Stat stat) //
	{ 
        // 이동가능한 타일인지 확인할 delegate 설정.
		pathFinder.SetValidateTile(ValidateNextTile);
		SetPathFindEvent();
		//stat 초기화
		//pathfinder 초기화 // delegate 그대로
	}

	public void OnEnable()
	{
		// 입구 타일 랜덤으로 받아오기.
		SetCurTile(GameManager.Instance.GetRandomEntrance());
        SetCurTileForMove(GetCurTile().GetChild(Random.Range(0, 3)));

        // 이동가능한 타일인지 확인할 delegate 설정.
        pathFinder.SetValidateTile(ValidateNextTile);
        // PathFind 성공/실패에 따라 호출할 delegate 설정.
		SetPathFindEvent();
        // 아마 실패 횟수인 듯.
		pathFindCount = 0;
		curCoroutine = null;
		structureListByPref = null;

        // 타일레이어 받기.
		tileLayer = GameManager.Instance.GetMap().GetLayer(0).GetComponent<TileLayer>();

        // 기본은 Idle.
		
        Debug.Log("OnEnable에서");
		StartCoroutine(LateStart());
	}
	IEnumerator LateStart()
	{
		yield return null;
		curState = State.Idle;
	}
	public void OnDisable()
	{
		StopAllCoroutines();
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
			case State.Idle:
				Debug.Log("Idle");
				if(structureListByPref == null)
				{
					//Do something at first move...
				}
				curState = State.SearchingStructure;
				//Traveler이므로 무조건 SearchingStructure 부터
				//이외에 체크할거 있으면 여기서
				break;
			case State.Wandering:
				Debug.Log("Wandering");
				curCoroutine = StartCoroutine(Wandering());
				break;
			case State.SearchingStructure:
				Debug.Log("SS");
				curCoroutine = StartCoroutine(StructureFinding());
				break;
			case State.PathFinding:
				Debug.Log("PF");
				curCoroutine = StartCoroutine(PathFinding());
				break;
			case State.MovingToStructure:
				Debug.Log("MTS");
				curCoroutine = StartCoroutine(MoveToDestination());
				break;
			case State.WaitingStructure:
				Debug.Log("WS");
				destinationStructure.AddWaitTraveler(this);
				break;
			case State.UsingStructure:
				Debug.Log("US");
				//욕구 감소
				//소지 골드 감소
				stat.gold -= destinationStructure.charge; 
				stat.GetSpecificDesire(destinationStructure.resolveType).desireValue -= destinationStructure.resolveAmount; // ??
				break;
			case State.Exit:
				Debug.Log("EXIT");
				//Going to outside 
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
			case State.Wandering:
				break;
			case State.SearchingStructure:
				break;
			case State.PathFinding:
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
				break;
		}
	}

	IEnumerator Wandering()
	{
		while (wanderCount<10)
		{
			//랜덤 거리, 사방으로 이동
			do
			{
				destinationTile = tileLayer.GetTileAsComponent(Random.Range(0, tileLayer.GetLayerWidth() - 1), Random.Range(0, tileLayer.GetLayerHeight() - 1));
				yield return null;
			} while (!destinationTile.GetPassable());
			yield return StartCoroutine(pathFinder.Moves(curTile, destinationTile));
			//Debug.Log("길찾기 완료 : " + gameObject.GetInstanceID()+" isNoPath : " +pathFinder.isNoPath);

			// 코루틴 증식 이거 때문인 거 같아서 뺌. #CorutineErr
			//yield return StartCoroutine(MoveToDestination());
			//yield return null;

			wayForMove = GetWay(pathFinder.GetPath()); // TileForMove로 변환
			animator.SetBool("MoveFlg", true); // animation 이동으로
			yield return curCoroutine = StartCoroutine(MoveAnimation(wayForMove));


			wanderCount++;
		}

        curState = State.MovingToStructure;
		//이동 끝난 후 State = Idle.
	}
	IEnumerator StructureFinding()
	{
		if(pathFindCount <= 0 && structureListByPref == null) // Fail 기록 없을때
			//structureListByPref = StructureManager.Instance.FindStructureByDesire(stat.GetHighestDesire(), this);
		while (curState == State.SearchingStructure)
			{
			//temporary
			if (structureListByPref == null || structureListByPref.Length == 0)
			{
				curState = State.Wandering;
				yield break;
			}
			//temporary
				yield return null;
			if (pathFindCount < structureListByPref.Length && structureListByPref[pathFindCount] != null) // 길찾기 횟수가 선호건물 수 보다 적다면
			{
				destinationTile = structureListByPref[pathFindCount].GetEntrance(); // 목적지 설정
				destinationStructure = structureListByPref[pathFindCount];
				curState = State.PathFinding;
			}
			
			else
			{
				pathFindCount = 0;
				curState = State.Exit;
				break;
			}
			//길찾기 시작
			//pathfind success, fail delegate call
		}
	}

	IEnumerator PathFinding()
	{
		yield return StartCoroutine(pathFinder.Moves(curTile, destinationTile));
	}

	IEnumerator MoveToDestination()
	{
		//길찾기 성공!
		wayForMove = GetWay(pathFinder.GetPath()); // TileForMove로 변환
		animator.SetBool("MoveFlg", true); // animation 이동으로
		yield return curCoroutine = StartCoroutine(MoveAnimation(wayForMove)); // 이동 한번에 코루틴으로 처리 // 이동 중지할 일 있으면 StopCoroutine moveAnimation // traveler니까 없을듯?																//순번 or 대기 여부 결정
		
		if (destinationStructure != null && destinationStructure.GetWaitSeconds() > 120.0f) // const? // 대기시간 2분 이상이면
		{
			curState = State.Idle;
			yield break;
		}
		else
		{
			if (destinationStructure != null)
			{
				curState = State.WaitingStructure;
				yield break;
			}
			else
			{
				curState = State.Idle;
				//대기 or 다시 길찾기
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
		if(destinationStructure != null)
			curState = State.MovingToStructure;
	}
	public void PathFindFail() // PathFinder 길찾기 실패 Delegate
	{
		pathFindCount++;
		if(curState == State.Exit)
		{
			//타일맵 입구가 막혔을때!
			//즉시 탈출
			//최대 손님 - 1
			//평판 --
			//알림
			//disable
		}
		curState = State.SearchingStructure;
	}

	public override bool ValidateNextTile(Tile tile) // Pathfinder delegate
	{
		//return tile.GetPassableTraveler();
		//TileMapGenerator에서 PassableTraveler 설정해줘야함. 아직 안돼있음
		return tile.GetPassable(); //임시조치
	}

	protected List<TileForMove> GetWay(List<PathVertex> path) // Pathvertex -> TileForMove
    {
		#region 기존 GetWay
		/*List<TileForMove> tileForMoveWay = new List<TileForMove>();

		#region 새 GetWay
		TileForMove cur = GetCurTileForMove();
		TileForMove next;
		int count = 0;
		Vector2 dir = DirectionVector.GetDirectionVector(cur.GetParent().GetDirectionFromOtherTile(path[count].myTilePos));
		while((!(cur.GetParent().Equals(destinationTile))) && (count < path.Count))
		{
			next = tileLayer.GetTileForMove(cur.GetX() + (int)dir.x, cur.GetY() + (int)dir.y);
			tileForMoveWay.Add(next);
			if(cur.GetParent().Equals(next.GetParent()))
			{
				cur = next;
				next = tileLayer.GetTileForMove(cur.GetX() + (int)dir.x, cur.GetY() + (int)dir.y);
				tileForMoveWay.Add(next);
			}
			if(Random.Range(0, 2) >= 1)
			{
				cur = next;
				next = tileLayer.GetTileForMove(cur.GetX() + (int)dir.x, cur.GetY() + (int)dir.y);
				tileForMoveWay.Add(next);
			}
			count++;
			dir = DirectionVector.GetDirectionVector(next.GetParent().GetDirectionFromOtherTile(path[count].myTilePos));
		}
		return tileForMoveWay;

		#endregion
		#region 구 GetWay
		/*
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
		Debug.Log("CurTile = " + curTile.ToString() + " // Destination = " + destinationTile.ToString());
		for (int i=0; i<tileForMoveWay.Count; i++)
		{
			Debug.Log("Path - " + path[i].X + " , " + path[i].Y);
		}
		return tileForMoveWay;*/
		#endregion
		List<TileForMove> tileForMoveWay = new List<TileForMove>();
		TileForMove next, cur;
		int count = 0;
		cur = GetCurTileForMove();
		Direction dir = curTile.GetDirectionFromOtherTile(path[count + 1].myTilePos);
		Vector2 dirVector = DirectionVector.GetDirectionVector(dir);
		while (!(path[count].myTilePos.Equals(destinationTile)))
		{
			next = tileLayer.GetTileForMove(cur.GetX() + (int)dirVector.x, cur.GetY() + (int)dirVector.y);
			tileForMoveWay.Add(next);
			if(cur.GetParent().Equals(next.GetParent()))
			{
				cur = next;
				next = tileLayer.GetTileForMove(cur.GetX() + (int)dirVector.x, cur.GetY() + (int)dirVector.y);
				tileForMoveWay.Add(next);
			}
			if(Random.Range(0, 2) >= 1)
			{
				cur = next;
				next = tileLayer.GetTileForMove(cur.GetX() + (int)dirVector.x, cur.GetY() + (int)dirVector.y);
				tileForMoveWay.Add(next);
			}
			count++;
			dir = next.GetParent().GetDirectionFromOtherTile(path[count].myTilePos);
		}
		

	
		
	}
    // dX = 1 : UR
    // dX = -1: DL
    // dY = 1 : DR
    // dY = -1: UL


	IEnumerator MoveAnimation(List<TileForMove> tileForMoveWay)
	{
		yield return null;
		#region 기존 MoveAnimation
		/*
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
				transform.Translate(directionVec * 1.0f * Time.deltaTime); //TimeScale 말고
				moveDistanceSum += Vector3.Distance(before, transform.position); // 총 이동한 거리 합산.
			}
			transform.position = destPos;
			SetCurTileForMove(tileForMoveWay[i]); // 현재 타일 기록.
			SetCurTile(tileForMoveWay[i].GetParent());
		}*/
		#endregion
		for(int i=0; i<tileForMoveWay.Count; i++)
		{
			yield return new WaitForSeconds(1.0f);
			tileForMoveWay[i].SetRecentActor(this);
			transform.position = tileForMoveWay[i].GetPosition();
			SetCurTile(tileForMoveWay[i].GetParent());
			SetCurTileForMove(tileForMoveWay[i]);
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

    #region Save Load
    public int GetDestinationTileSave()
    {
        if (destinationTile != null)
            return int.Parse(destinationTile.name);
        else
            return -1;
    }

    public bool SetDestinationTileLoad(int tileNum)
    {
        if (tileNum == -1)
            return false;

        GameObject tileLayer = GameManager.Instance.GetTileLayer();
        destinationTile = tileLayer.transform.GetChild(tileNum).gameObject.GetComponent<Tile>();

        if (destinationTile == null)
            return false;
        else
            return true;
    }

    public int GetCurTileSave()
    {
        if (curTile != null)
            return int.Parse(curTile.name);
        else
            return -1;
    }

    public bool SetCurTileLoad(int tileNum)
    {
        if (tileNum == -1)
            return false; 

        GameObject tileLayer = GameManager.Instance.GetTileLayer();
        if (tileLayer == null)
            Debug.Log("타일 레이어 없음");
        else
            Debug.Log("타일레이어 존재!");
        Debug.Log("Child 수 : " + tileLayer.transform.childCount);
        curTile = tileLayer.transform.GetChild(tileNum).gameObject.GetComponent<Tile>();

        if (curTile == null)
            return false;
        else
            return true;
    }
    #endregion
}
