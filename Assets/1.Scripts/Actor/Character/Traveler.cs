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
	protected const int wanderCountMax = 3;
    protected Coroutine curCoroutine;
    //protected Tile destinationTile;
    protected Place destinationPlace;
    protected Structure[] structureListByPref;
	private const float MAX_WAIT_SECONDS = 120.0f;
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
        SetCurTile(GameManager.Instance.GetRandomEntrance()); // 여기서 가끔 터짐. 수정요망. nullpointer 뜸
        SetCurTileForMove(GetCurTile().GetChild(Random.Range(0, 3)));

        // 이동가능한 타일인지 확인할 delegate 설정.
        pathFinder.SetValidateTile(ValidateNextTile);
        // PathFind 성공/실패에 따라 호출할 delegate 설정.
        SetPathFindEvent();
        // 아마 실패 횟수인 듯.
        pathFindCount = 0;
		wanderCount = 0;
        curCoroutine = null;
        structureListByPref = null;

        // 타일레이어 받기.
        tileLayer = GameManager.Instance.GetMap().GetLayer(0).GetComponent<TileLayer>(); // 이거도 마찬가지 가끔 멈췄다가 세우면 터짐.

        // 기본은 Idle.
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
    protected virtual void EnterState(State nextState)
    {
        switch (nextState)
        {
            case State.Idle:
                Debug.Log("Idle");
                if (structureListByPref == null)
                {
                    //Do something at first move...
                }
                superState = SuperState.Idle;
				curCoroutine = StartCoroutine(Idle()); // State -> SearchingStructure
                //Traveler이므로 무조건 SearchingStructure 부터
                //이외에 체크할거 있으면 여기서
                break;
            case State.SolvingDesire_Wandering:
                Debug.Log("SolvingDesire_Wandering");
                superState = SuperState.SolvingDesire_Wandering;
                curCoroutine = StartCoroutine(SolvingDesire_Wandering()); // State -> SearchingExit(if wanderCount exceeded) or PathFinding(normal)
                break;
            case State.SearchingStructure:
                Debug.Log("SS");
                superState = SuperState.SolvingDesire;
                curCoroutine = StartCoroutine(SearchingStructure()); // State -> SolvingDesrie_Wandering(if no structure) or PathFinding or SearchingExit
                break;
            case State.PathFinding:
                Debug.Log("PF");
                curCoroutine = StartCoroutine(PathFinding()); // State -> MovingToDestination(if Succeed) or SearchingExit(if pathfindCount Exceeded) or SearchingStructure(if pathfindCount under maxValue)
                break;
            case State.MovingToDestination:
                Debug.Log("MTS");
                curCoroutine = StartCoroutine(MoveToDestination()); // State -> SearchingStructure(if
                break;
            case State.WaitingStructure:
                Debug.Log("WS");
                //destinationPlace.Visit(this);
				curCoroutine = StartCoroutine(WaitingStructure());
                break;
            case State.UsingStructure:
                Debug.Log("US");
                //욕구 감소
                //소지 골드 감소
                curCoroutine = StartCoroutine(UsingStructure());
                break;
            case State.SearchingExit:
                Debug.Log("SE");
                superState = SuperState.ExitingDungeon;
				curCoroutine = StartCoroutine(SearchingExit());
                //Going to outside 
                break;
            case State.Exit:
                Debug.Log("EXIT");
				curCoroutine = StartCoroutine(Exit());
                break;
            case State.None: // Idle과 동일
				//curState = State.Idle;
				curCoroutine = StartCoroutine(Idle());
                break;
        }
    }
    protected virtual void ExitState()
    {
		if (curCoroutine != null)
			StopCoroutine(curCoroutine);
        switch (curState)
        {
            case State.Idle:
                break;
            case State.SolvingDesire_Wandering:
                break;
            case State.SearchingStructure:
                break;
            case State.PathFinding:
                break;
            case State.MovingToDestination:
                break;
            case State.WaitingStructure:
                break;
            case State.UsingStructure:
                break;
            case State.SearchingExit:
                break;
            case State.Exit:
                break;
            case State.None:
                break;
        }
    }

	
	#region StateCoroutine
	protected IEnumerator Idle()
	{
		yield return null;
		curState = State.SearchingStructure;
	}
    protected IEnumerator SolvingDesire_Wandering()
    {
        if (wanderCount < wanderCountMax)
        {
            //랜덤 거리, 사방으로 이동
            do
            {
                destinationTile = tileLayer.GetTileAsComponent(Random.Range(0, tileLayer.GetLayerWidth() - 1), Random.Range(0, tileLayer.GetLayerHeight() - 1));
                yield return null;
            } while (!ValidateNextTile(destinationTile)); // Wandering -> 지나갈 수 있는 타일이면
			
            curState = State.PathFinding;
        }
        else
        {
			curState = State.SearchingExit;
        }
    }


    protected IEnumerator SearchingStructure()
    {
		yield return null;
		if(structureListByPref == null) // List가 비어있을때 . // 건물 이용 후 List 비워줘야함.
		{
			structureListByPref = StructureManager.Instance.FindStructureByDesire(stat.GetHighestDesire(), this);
		}
		if(structureListByPref.Length <= pathFindCount) // 검색 결과가 없거나 검색한 건물 모두 길찾기 실패했을때...
		{
			pathFindCount = 0;
			curState = State.SolvingDesire_Wandering;
		}
		else // 검색 결과 건물로 길찾기진행.
		{
			destinationTile = structureListByPref[pathFindCount].GetEntrance();
			destinationPlace = structureListByPref[pathFindCount];
			curState = State.PathFinding;
		}
		
		//길찾기 시작
		//pathfind success, fail delegate call

	}

	protected virtual IEnumerator PathFinding()
    {
        //yield return null;
        yield return StartCoroutine(pathFinder.Moves(curTile, destinationTile));
        //curState = State.MovingToDestination;
		//Delegate 이벤트에서 State 변경처리.
    }

    protected virtual IEnumerator MoveToDestination()
    {
        //길찾기 성공!
        wayForMove = GetWay(pathFinder.GetPath()); // TileForMove로 변환
        animator.SetBool("MoveFlg", true); // animation 이동으로
        yield return curCoroutine = StartCoroutine(MoveAnimation(wayForMove)); // 이동 한번에 코루틴으로 처리 // 이동 중지할 일 있으면 StopCoroutine moveAnimation // traveler니까 없을듯?																//순번 or 대기 여부 결정

        switch(superState)
        {
            case SuperState.SolvingDesire: // 건물로 이동했을때
                StartCoroutine(CheckStructure());
                break;
            case SuperState.SolvingDesire_Wandering: // 랜덤 위치로 배회할때
                wanderCount++;
                curState = State.SearchingStructure;
                break;
			case SuperState.ExitingDungeon:
				//exit 처리
				//gameObject 비활성화 처리 해주고
				//GameManager에서 disabledqueue에 추가해주기.
				break;
        }
    }
	//건물 대기 코루틴
	protected virtual IEnumerator WaitingStructure() //-CurState == WaitingStructure.
	{
		//일단 건물 입장 대기처리...AddWaiting
		//건물 대기시간 검사는 끝난상태라 바로 입장처리 하면됨
		//위치 이동 후 대기..?
		//이벤트 처리는?~//0505

		Structure destinationStructure = destinationPlace as Structure; // 건물 캐싱
		destinationStructure.Visit(this);
		
		yield return null;
	}

	protected IEnumerator UsingStructure()
	{
		yield return null;
		Structure destinationStructure = destinationPlace as Structure;
		stat.gold -= destinationStructure.charge;
		stat.GetSpecificDesire(destinationStructure.resolveType).desireValue -= destinationStructure.resolveAmount; // ??

		// 건물 사용에 대한 것 아직 구현 덜 됨. 수정요망.

		// 사용 후에는 비워주기.
		destinationPlace = null;
		wanderCount = 0;
	}

	protected virtual IEnumerator CheckStructure() // 해당 건물로 이동하고 입장 가능한지 검사.
    {
		yield return null;
		
		
		if (!(destinationPlace is Structure)) // destinationPlace 가 건물이 아닐때?
		{
			structureListByPref = null;
			curState = State.SearchingStructure;
			yield break;
		}
		
		if(stat.gold < (destinationPlace as Structure).charge)// 골드 부족 확인
		{
			curState = State.SearchingExit;
			yield break;
		}
        if ((destinationPlace as Structure).GetWaitSeconds() > MAX_WAIT_SECONDS)// 대기시간 2분 이상이면 -> 다른건물 찾기. structureListByPref 새로 
        {
			structureListByPref = null;
			curState = State.SearchingStructure;
			
        }
        else
        {
            curState = State.WaitingStructure;
        }
    }
	
	protected virtual IEnumerator SearchingExit()
	{
		destinationTile = GameManager.Instance.GetRandomEntrance();
		curState = State.PathFinding;
		yield return null;
	}
    
	protected virtual IEnumerator Exit()
	{
		yield return null;
	}
	#endregion

	public void PathFindSuccess() // Pathfinder 길찾기 성공 Delegate
	{
		/*pathFindCount = 0;
		Debug.Log("Success!!");
		//if (destinationPlace != null)
			//curState = State.MovingToDestination;*/
		StartCoroutine(CoroutinePathFindSuccess());
	}
	IEnumerator CoroutinePathFindSuccess()
	{
		if(destinationPlace != null)
		{
			pathFindCount = 0;
			curState = State.MovingToDestination;
		}
		else
		{
			StartCoroutine(CoroutinePathFindFail());
		}
		yield return null;
	}
	public void PathFindFail() // PathFinder 길찾기 실패 Delegate
	{
		/*pathFindCount++;
		//pathfindcount 일정 이상 초과하면 SearchingExit...
		Debug.Log("Failed!!");
		if (curState == State.SearchingExit)
		{
			//타일맵 입구가 막혔을때!
			//즉시 탈출
			//최대 손님 - 1
			//평판 --
			//알림
			//disable
		}
		curState = State.SearchingStructure;*/
		StartCoroutine(CoroutinePathFindFail());
	}
	IEnumerator CoroutinePathFindFail()
	{
		if(superState == SuperState.ExitingDungeon)
		{
			//즉시탈출
			//평판 --
			yield break;
		}
		pathFindCount++;
		curState = State.SearchingStructure;
		yield return null;
	}

	public override void SetPathFindEvent() // Pathfinder Delegate 설정
    {
		pathFinder.SetNotifyEvent(PathFindSuccess, PathFindFail);
	}

	public override bool ValidateNextTile(Tile tile) // Pathfinder delegate
	{
		//return tile.GetPassableTraveler();
		//TileMapGenerator에서 PassableTraveler 설정해줘야함. 아직 안돼있음
		return tile.GetPassable(); //임시조치
	}

	protected List<TileForMove> GetWay(List<PathVertex> path) // Pathvertex -> TileForMove
    {
		List<TileForMove> tileForMoveWay = new List<TileForMove>();
		TileForMove next, cur;
		int count = 1;
		cur = curTileForMove;
		//Debug.Log("Start : " + cur.GetParent().GetX() + " , " + cur.GetParent().GetY() + "dest = " + destinationTile.GetX() + " , " + destinationTile.GetY());
        // 다음 타일로의 방향
		Direction dir = curTile.GetDirectionFromOtherTile(path[count].myTilePos);
		Vector2 dirVector = DirectionVector.GetDirectionVector(dir);

        //현재 타일 추가
		tileForMoveWay.Add(curTileForMove);

		//Debug.Log(dir.ToString());

		string pathString = "";

		for (int i = 0; i<path.Count; i++)
		{
			pathString += path[i].myTilePos.GetX() + " , " + path[i].myTilePos.GetY() + "\n";
			
		}

		//Debug.Log(pathString);
		//Debug.Log("progress : " + cur.GetX() + "(" + cur.GetParent().GetX() + ")" + " , " + cur.GetY() + "(" + cur.GetParent().GetY() + ")"); //19 49


		while (!(path[count].myTilePos.Equals(destinationTile)))
		{
			next = tileLayer.GetTileForMove(cur.GetX() + (int)dirVector.x, cur.GetY() + (int)dirVector.y);

			//Debug.Log("progress : " + next.GetX() + "(" + next.GetParent().GetX() + ")" + " , " + next.GetY() + "(" + next.GetParent().GetY() + ")");
			tileForMoveWay.Add(next);
			if(cur.GetParent().Equals(next.GetParent() )) //한칸 진행했는데도 같은 타일일때
			{
				//Debug.Log("SameTile..");
				next = tileLayer.GetTileForMove(next.GetX() + (int)dirVector.x, next.GetY() + (int)dirVector.y);
				//Debug.Log("progress : " + next.GetX() + "(" + next.GetParent().GetX() + ")" + " , " + next.GetY() + "(" + next.GetParent().GetY() + ")");
				tileForMoveWay.Add(next);
				cur = next;
			}
			else
			{
				cur = next;
			}
			if(Random.Range(0, 2) >= 1)
			{
				
				next = tileLayer.GetTileForMove(cur.GetX() + (int)dirVector.x, cur.GetY() + (int)dirVector.y);
				if (next == null)
					continue;

				//Debug.Log("progress : " + next.GetX() + "(" + next.GetParent().GetX() + ")" + " , " + next.GetY() + "(" + next.GetParent().GetY() + ")");
				tileForMoveWay.Add(next);
				cur = next;
			}
			count++;
			dir = cur.GetParent().GetDirectionFromOtherTile(path[count].myTilePos);
			dirVector = DirectionVector.GetDirectionVector(dir);

			//Debug.Log(dir.ToString());
		}
		//Debug.Log("Done!!!!");
		return tileForMoveWay;
	}

    protected IEnumerator MoveAnimation(List<TileForMove> tileForMoveWay)
	{
		yield return null;
		if (tileForMoveWay == null || tileForMoveWay.Count < 1) //Path가 없을때 
			yield break;
		Direction dir = Direction.DownLeft;
		//FlipX true == Left, false == Right
		Vector3 dirVector;
		float distance, sum = 0.0f;
		
		for (int i = 0; i < tileForMoveWay.Count - 1; i++)
		{
			switch (dir = tileForMoveWay[i].GetDirectionFromOtherTileForMove(tileForMoveWay[i + 1]))
			{
				case Direction.DownRight:
					animator.SetTrigger("UpToDownFlg");
					foreach (SpriteRenderer sr in spriteRenderers)
					{
						sr.flipX = true;
					}
					break;
				case Direction.UpRight:

					animator.SetTrigger("DownToUpFlg");
					foreach (SpriteRenderer sr in spriteRenderers)
					{
						sr.flipX = true;
					}
					break;
				case Direction.DownLeft:

					animator.SetTrigger("UpToDownFlg");
					foreach (SpriteRenderer sr in spriteRenderers)
					{
						sr.flipX = false;
					}
					break;
				case Direction.UpLeft:

					animator.SetTrigger("DownToUpFlg");
					foreach (SpriteRenderer sr in spriteRenderers)
					{
						sr.flipX = false;
					}
					break;
				default:
					break;
			}
			//transform.position = tileForMoveWay[i].GetPosition();
			dirVector = tileForMoveWay[i + 1].GetPosition() - tileForMoveWay[i].GetPosition();
			distance = Vector3.Distance(tileForMoveWay[i].GetPosition(), tileForMoveWay[i + 1].GetPosition());
			while(Vector3.Distance(transform.position, tileForMoveWay[i].GetPosition()) < distance)
			{
				yield return null;
				transform.Translate(dirVector * Time.deltaTime);

			}
			sum = 0.0f;
			transform.position = tileForMoveWay[i + 1].GetPosition();
			tileForMoveWay[i+1].SetRecentActor(this);
			SetCurTile(tileForMoveWay[i+1].GetParent());
			SetCurTileForMove(tileForMoveWay[i+1]);

		}

	} // Adventurer에서 이동 중 피격 구현해야함. // Notify?

	public void OnWaitingStructure()
	{
		curCoroutine = StartCoroutine(_OnWaitingStructure());
	}
	IEnumerator _OnWaitingStructure()
	{
		SetInvisible();
		yield return null;
		//이미 들어왔으니 따로 처리 x
	}
	public void OnUsingStructure()
	{
		curCoroutine = StartCoroutine(_OnUsingStructure());
	}
	IEnumerator _OnUsingStructure()
	{
		yield return null;
		curState = State.UsingStructure;
	}
	public void OnExitStructure()
	{
		curCoroutine = StartCoroutine(_OnExitStructure());
	}
	IEnumerator _OnExitStructure()
	{
		yield return null;
		//퇴장 처리
		(destinationPlace as Structure).ExitTraveler();
		SetVisible();
		curState = State.SearchingStructure;
	}
	protected void SetInvisible()
	{
		foreach (SpriteRenderer s in spriteRenderers)
		{
			s.color = new Color(s.color.r, s.color.g, s.color.b, 0.0f);
		}
	}
	protected void SetVisible()
	{
		foreach (SpriteRenderer s in spriteRenderers)
		{
			s.color = new Color(s.color.r, s.color.g, s.color.b, 1.0f);
		}
	}
   
}
