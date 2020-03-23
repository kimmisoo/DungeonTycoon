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
    //protected Tile destinationTile;
    protected Place destinationPlace;
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
        SetCurTile(GameManager.Instance.GetRandomEntrance()); // 여기서 가끔 터짐. 수정요망. nullpointer 뜸
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
                curState = State.SearchingStructure;
                //Traveler이므로 무조건 SearchingStructure 부터
                //이외에 체크할거 있으면 여기서
                break;
            case State.SolvingDesire_Wandering:
                Debug.Log("SolvingDesire_Wandering");
                superState = SuperState.SolvingDesire_Wandering;
                curCoroutine = StartCoroutine(SolvingDesire_Wandering());
                break;
            case State.SearchingStructure:
                Debug.Log("SS");
                superState = SuperState.SolvingDesire;
                curCoroutine = StartCoroutine(SearchingStructure());
                break;
            case State.PathFinding:
                Debug.Log("PF");
                curCoroutine = StartCoroutine(PathFinding());
                break;
            case State.MovingToDestination:
                Debug.Log("MTS");
                curCoroutine = StartCoroutine(MoveToDestination());
                break;
            case State.WaitingStructure:
                Debug.Log("WS");
                destinationPlace.Visit(this);
                break;
            case State.UsingStructure:
                Debug.Log("US");
                //욕구 감소
                //소지 골드 감소
                UsingStructure();
                break;
            case State.SearchingExit:
                Debug.Log("SE");
                superState = SuperState.ExitingDungeon;
                //Going to outside 
                break;
            case State.Exit:
                Debug.Log("EXIT");
                break;
            case State.None:
                curState = State.Idle;
                break;
        }
    }
    protected virtual void ExitState()
    {
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

    //protected IEnumerator Wandering()
    //{
    //    while (wanderCount < 10)
    //    {
    //        //랜덤 거리, 사방으로 이동
    //        do
    //        {
    //            destinationTile = tileLayer.GetTileAsComponent(Random.Range(0, tileLayer.GetLayerWidth() - 1), Random.Range(0, tileLayer.GetLayerHeight() - 1));
    //            yield return null;
    //        } while (!destinationTile.GetPassable());
    //        yield return StartCoroutine(pathFinder.Moves(curTile, destinationTile));
    //        //Debug.Log("길찾기 완료 : " + gameObject.GetInstanceID()+" isNoPath : " +pathFinder.isNoPath);

    //        // 코루틴 증식 이거 때문인 거 같아서 뺌. #CorutineErr
    //        //yield return StartCoroutine(MoveToDestination());
    //        //yield return null;

    //        wayForMove = GetWay(pathFinder.GetPath()); // TileForMove로 변환
    //        animator.SetBool("MoveFlg", true); // animation 이동으로
    //        yield return curCoroutine = StartCoroutine(MoveAnimation(wayForMove));


    //        wanderCount++;
    //    }

    //    curState = State.MovingToDestination;
    //    //이동 끝난 후 State = Idle.
    //}

    protected IEnumerator SolvingDesire_Wandering()
    {
        if (wanderCount < 10)
        {
            //랜덤 거리, 사방으로 이동
            do
            {
                destinationTile = tileLayer.GetTileAsComponent(Random.Range(0, tileLayer.GetLayerWidth() - 1), Random.Range(0, tileLayer.GetLayerHeight() - 1));
                yield return null;
            } while (!ValidateNextTile(destinationTile));

            curState = State.PathFinding;
        }
        else
        {
            //curState = State.SearchingExit;
        }
    }


    protected IEnumerator SearchingStructure()
    {
        if (pathFindCount <= 0 && structureListByPref == null) // Fail 기록 없을때
                                                               //structureListByPref = StructureManager.Instance.FindStructureByDesire(stat.GetHighestDesire(), this);
            while (curState == State.SearchingStructure)
            {
                //temporary
                if (structureListByPref == null || structureListByPref.Length == 0)
                {
                    curState = State.SolvingDesire_Wandering;
                    yield break;
                }
                //temporary
                yield return null;
                if (pathFindCount < structureListByPref.Length && structureListByPref[pathFindCount] != null) // 길찾기 횟수가 선호건물 수 보다 적다면
                {
                    destinationTile = structureListByPref[pathFindCount].GetEntrance(); // 목적지 설정
                    destinationPlace = structureListByPref[pathFindCount];
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

    protected virtual IEnumerator PathFinding()
    {
        //yield return null;
        yield return StartCoroutine(pathFinder.Moves(curTile, destinationTile));

        curState = State.MovingToDestination;
    }

    protected virtual IEnumerator MoveToDestination()
    {
        //길찾기 성공!
        wayForMove = GetWay(pathFinder.GetPath()); // TileForMove로 변환
        animator.SetBool("MoveFlg", true); // animation 이동으로
        yield return curCoroutine = StartCoroutine(MoveAnimation(wayForMove)); // 이동 한번에 코루틴으로 처리 // 이동 중지할 일 있으면 StopCoroutine moveAnimation // traveler니까 없을듯?																//순번 or 대기 여부 결정

        switch(superState)
        {
            case SuperState.SolvingDesire:
                VisitStructure();
                break;
            case SuperState.SolvingDesire_Wandering:
                wanderCount++;
                curState = State.SearchingStructure; // 수정요망
                break;
        }
    }

    protected void VisitStructure()
    {
        Structure destinationStructure = destinationPlace as Structure;
        if (destinationStructure.GetWaitSeconds() > 120.0f) // const? // 대기시간 2분 이상이면
        {
            // 리스트에서 이 건물을 빼주든 해야할 듯. 수정요망.
            curState = State.Idle;
        }
        else
        {
            curState = State.WaitingStructure;
        }
    }
	
    protected void UsingStructure()
    {
        Structure destinationStructure = destinationPlace as Structure;
        stat.gold -= destinationStructure.charge;
        stat.GetSpecificDesire(destinationStructure.resolveType).desireValue -= destinationStructure.resolveAmount; // ??

        // 건물 사용에 대한 것 아직 구현 덜 됨. 수정요망.

        // 사용 후에는 비워주기.
        destinationPlace = null;
        wanderCount = 0;
    }
	
	public override void SetPathFindEvent() // Pathfinder Delegate 설정
    {
		pathFinder.SetNotifyEvent(PathFindSuccess, PathFindFail);
	}

	public void PathFindSuccess() // Pathfinder 길찾기 성공 Delegate
	{
		pathFindCount = 0;
		Debug.Log("Success!!");
		if(destinationPlace != null)
			curState = State.MovingToDestination;
	}
	public void PathFindFail() // PathFinder 길찾기 실패 Delegate
	{
		pathFindCount++;
		Debug.Log("Failed!!");
		if (curState == State.Exit)
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


    //protected override List<TileForMove> GetWay(List<PathVertex> path) // Pathvertex -> TileForMove
    //{
    //    #region 기존 GetWay
    //    /*List<TileForMove> tileForMoveWay = new List<TileForMove>();

    //	#region 새 GetWay
    //	TileForMove cur = GetCurTileForMove();
    //	TileForMove next;
    //	int count = 0;
    //	Vector2 dir = DirectionVector.GetDirectionVector(cur.GetParent().GetDirectionFromOtherTile(path[count].myTilePos));
    //	while((!(cur.GetParent().Equals(destinationTile))) && (count < path.Count))
    //	{
    //		next = tileLayer.GetTileForMove(cur.GetX() + (int)dir.x, cur.GetY() + (int)dir.y);
    //		tileForMoveWay.Add(next);
    //		if(cur.GetParent().Equals(next.GetParent()))
    //		{
    //			cur = next;
    //			next = tileLayer.GetTileForMove(cur.GetX() + (int)dir.x, cur.GetY() + (int)dir.y);
    //			tileForMoveWay.Add(next);
    //		}
    //		if(Random.Range(0, 2) >= 1)
    //		{
    //			cur = next;
    //			next = tileLayer.GetTileForMove(cur.GetX() + (int)dir.x, cur.GetY() + (int)dir.y);
    //			tileForMoveWay.Add(next);
    //		}
    //		count++;
    //		dir = DirectionVector.GetDirectionVector(next.GetParent().GetDirectionFromOtherTile(path[count].myTilePos));
    //	}
    //	return tileForMoveWay;

    //	#endregion
    //	#region 구 GetWay
    //	/*
    //	int childNum = GetCurTileForMove().GetChildNum();
    //	tileForMoveWay.Add(GetCurTileForMove());
    //	Direction dir;
    //       for(int i= 1; i<path.Count - 1; i++)
    //       {
    //		dir = GetCurTile().GetDirectionFromOtherTile(path[i].myTilePos);
    //		switch(dir)
    //		{
    //			case Direction.UpRight: // 2
    //				if(childNum >= 2) //이동할 다음 이동타일이 현재 타일의 Child인지?
    //				{
    //					childNum -= 2;
    //					tileForMoveWay.Add(path[i].myTilePos.GetChild(childNum)); // tile내부에서 1칸 이동
    //				}
    //				//다음 타일
    //				childNum += 2;
    //				tileForMoveWay.Add(path[i + 1].myTilePos.GetChild(childNum));
    //				//한칸 더 갈지 말지?
    //				if (Random.Range(0, 2) < 1)
    //				{
    //					childNum -= 2;
    //					tileForMoveWay.Add(path[i + 1].myTilePos.GetChild(childNum));
    //				}
    //				break;
    //			case Direction.UpLeft: //  1

    //				if (childNum % 2 == 1)
    //				{
    //					childNum -= 1;
    //					tileForMoveWay.Add(path[i].myTilePos.GetChild(childNum)); // tile내부에서 1칸 이동
    //				}
    //				childNum += 1;
    //				tileForMoveWay.Add(path[i + 1].myTilePos.GetChild(childNum));
    //				if (Random.Range(0, 2) < 1)
    //				{
    //					childNum -= 1;
    //					tileForMoveWay.Add(path[i + 1].myTilePos.GetChild(childNum));
    //				}
    //				break;
    //			case Direction.DownRight: // 1

    //				if (childNum % 2 == 0)
    //				{
    //					childNum += 1;
    //					tileForMoveWay.Add(path[i].myTilePos.GetChild(childNum));
    //				}
    //				childNum -= 1;
    //				tileForMoveWay.Add(path[i + 1].myTilePos.GetChild(childNum));
    //				if (Random.Range(0, 2) < 1)
    //				{
    //					childNum += 1;
    //					tileForMoveWay.Add(path[i + 1].myTilePos.GetChild(childNum));
    //				}
    //				break;
    //			case Direction.DownLeft: // + 2

    //				if (childNum <= 1)
    //				{
    //					childNum += 2;
    //					tileForMoveWay.Add(path[i].myTilePos.GetChild(childNum));
    //				}
    //				childNum -= 2;
    //				tileForMoveWay.Add(path[i + 1].myTilePos.GetChild(childNum));
    //				if (Random.Range(0, 2) < 1)
    //				{
    //					childNum += 2;
    //					tileForMoveWay.Add(path[i + 1].myTilePos.GetChild(childNum));
    //				}
    //				break;
    //		}
    //       }
    //	Debug.Log("CurTile = " + curTile.ToString() + " // Destination = " + destinationTile.ToString());
    //	for (int i=0; i<tileForMoveWay.Count; i++)
    //	{
    //		Debug.Log("Path - " + path[i].X + " , " + path[i].Y);
    //	}
    //	return tileForMoveWay;*/
    //    #endregion
    //    List<TileForMove> tileForMoveWay = new List<TileForMove>();

    //    TileForMove next, cur;
    //    int count = 1;
    //    cur = curTileForMove;
    //    //Debug.Log("Start : " + cur.GetParent().GetX() + " , " + cur.GetParent().GetY() + "dest = " + destinationTile.GetX() + " , " + destinationTile.GetY());
    //    // 다음 타일로의 방향
    //    Direction dir = curTile.GetDirectionFromOtherTile(path[count].myTilePos);
    //    Vector2 dirVector = DirectionVector.GetDirectionVector(dir);

    //    //현재 타일 추가
    //    tileForMoveWay.Add(curTileForMove);
    //    //Debug.Log(dir.ToString());
    //    string pathString = "";

    //    for (int i = 0; i < path.Count; i++)
    //    {
    //        pathString += path[i].myTilePos.GetX() + " , " + path[i].myTilePos.GetY() + "\n";
    //        //Debug.Log("path = " + path[i].myTilePos.GetX() + " , " + path[i].myTilePos.GetY());
    //    }
    //    //Debug.Log(pathString);
    //    //Debug.Log("progress : " + cur.GetX() + "(" + cur.GetParent().GetX() + ")" + " , " + cur.GetY() + "(" + cur.GetParent().GetY() + ")"); //19 49

    //    while (!(path[count].myTilePos.Equals(destinationTile)))
    //    {
    //        next = tileLayer.GetTileForMove(cur.GetX() + (int)dirVector.x, cur.GetY() + (int)dirVector.y);
    //        //Debug.Log("progress : " + next.GetX() + "(" + next.GetParent().GetX() + ")" + " , " + next.GetY() + "(" + next.GetParent().GetY() + ")");
    //        tileForMoveWay.Add(next);
    //        if (cur.GetParent().Equals(next.GetParent())) //한칸 진행했는데도 같은 타일일때
    //        {
    //            //Debug.Log("SameTile..");
    //            next = tileLayer.GetTileForMove(next.GetX() + (int)dirVector.x, next.GetY() + (int)dirVector.y);
    //            //Debug.Log("progress : " + next.GetX() + "(" + next.GetParent().GetX() + ")" + " , " + next.GetY() + "(" + next.GetParent().GetY() + ")");
    //            tileForMoveWay.Add(next);
    //            cur = next;
    //        }
    //        else
    //        {
    //            cur = next;
    //        }
    //        if (Random.Range(0, 2) >= 1)
    //        {

    //            next = tileLayer.GetTileForMove(cur.GetX() + (int)dirVector.x, cur.GetY() + (int)dirVector.y);
    //            if (next == null)
    //                continue;
    //            //Debug.Log("progress : " + next.GetX() + "(" + next.GetParent().GetX() + ")" + " , " + next.GetY() + "(" + next.GetParent().GetY() + ")");
    //            tileForMoveWay.Add(next);
    //            cur = next;
    //        }
    //        count++;
    //        dir = cur.GetParent().GetDirectionFromOtherTile(path[count].myTilePos);
    //        dirVector = DirectionVector.GetDirectionVector(dir);
    //        //Debug.Log(dir.ToString());
    //    }
    //    //Debug.Log("Done!!!!");
    //    return tileForMoveWay;
    //}

    //   protected IEnumerator MoveAnimation(List<TileForMove> tileForMoveWay)
    //{
    //	yield return null;

    //	Direction dir = Direction.DownLeft;
    //	//FlipX true == Left, false == Right
    //	Vector3 dirVector;
    //	float distance, sum = 0.0f;
    //	for (int i = 0; i < tileForMoveWay.Count - 1; i++)
    //	{
    //		tileForMoveWay[i].SetRecentActor(this);
    //		SetCurTile(tileForMoveWay[i].GetParent());
    //		SetCurTileForMove(tileForMoveWay[i]);

    //		switch (dir = tileForMoveWay[i].GetDirectionFromOtherTileForMove(tileForMoveWay[i + 1]))
    //		{
    //			case Direction.DownRight:
    //				animator.SetTrigger("UpToDownFlg");
    //				foreach (SpriteRenderer sr in spriteRenderers)
    //				{
    //					sr.flipX = true;
    //				}
    //				break;
    //			case Direction.UpRight:

    //				animator.SetTrigger("DownToUpFlg");
    //				foreach (SpriteRenderer sr in spriteRenderers)
    //				{
    //					sr.flipX = true;
    //				}
    //				break;
    //			case Direction.DownLeft:

    //				animator.SetTrigger("UpToDownFlg");
    //				foreach (SpriteRenderer sr in spriteRenderers)
    //				{
    //					sr.flipX = false;
    //				}
    //				break;
    //			case Direction.UpLeft:

    //				animator.SetTrigger("DownToUpFlg");
    //				foreach (SpriteRenderer sr in spriteRenderers)
    //				{
    //					sr.flipX = false;
    //				}
    //				break;
    //			default:
    //				break;
    //		}
    //		//transform.position = tileForMoveWay[i].GetPosition();
    //		dirVector = tileForMoveWay[i + 1].GetPosition() - tileForMoveWay[i].GetPosition();
    //		distance = Vector3.Distance(tileForMoveWay[i].GetPosition(), tileForMoveWay[i + 1].GetPosition());
    //		while(Vector3.Distance(transform.position, tileForMoveWay[i].GetPosition()) < distance)
    //		{
    //			yield return null;
    //			transform.Translate(dirVector * Time.deltaTime);

    //		}
    //		sum = 0.0f;
    //		transform.position = tileForMoveWay[i + 1].GetPosition();

    //	}	

    //} // Adventurer에서 이동 중 피격 구현해야함. // Notify?
}
