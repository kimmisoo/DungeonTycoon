using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Monster : Actor, ICombatant//:Actor, IDamagable {
{
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

    // 수정해야 할듯?
    public void InitMonster(Stat stat, BattleStat battleStat) //
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
        switch (nextState)
        {
            case State.Idle:
                Debug.Log("Idle");
                if (structureListByPref == null)
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
            case State.MovingToDestination:
                Debug.Log("MTS");
                curCoroutine = StartCoroutine(MoveToDestination());
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
        switch (curState)
        {
            case State.Idle:
                break;
            case State.Wandering:
                break;
            case State.PathFinding:
                break;
            case State.MovingToDestination:
                break;
            case State.Exit:
                break;
            case State.None:
                break;
        }
    }

    protected IEnumerator Wandering()
    {
        while (wanderCount < 10)
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

        curState = State.MovingToDestination;
        //이동 끝난 후 State = Idle.
    }

    protected IEnumerator PathFinding()
    {
        yield return StartCoroutine(pathFinder.Moves(curTile, destinationTile));
    }

    protected IEnumerator MoveToDestination()
    {
        //길찾기 성공!
        wayForMove = GetWay(pathFinder.GetPath()); // TileForMove로 변환
        animator.SetBool("MoveFlg", true); // animation 이동으로
        yield return curCoroutine = StartCoroutine(MoveAnimation(wayForMove)); // 이동 한번에 코루틴으로 처리 // 이동 중지할 일 있으면 StopCoroutine moveAnimation // traveler니까 없을듯?																//순번 or 대기 여부 결정

        //if (destinationStructure != null && destinationStructure.GetWaitSeconds() > 120.0f) // const? // 대기시간 2분 이상이면
        //{
        //    curState = State.Idle;
        //    yield break;
        //}
        //else
        //{
        //    if (destinationStructure != null)
        //    {
        //        curState = State.WaitingStructure;
        //        yield break;
        //    }
        //    else
        //    {
        //        curState = State.Idle;
        //        //대기 or 다시 길찾기
        //    }
        //}
    }


    public override void SetPathFindEvent() // Pathfinder Delegate 설정
    {
        pathFinder.SetNotifyEvent(PathFindSuccess, PathFindFail);
    }

    public void PathFindSuccess() // Pathfinder 길찾기 성공 Delegate
    {
        pathFindCount = 0;
        //if (destinationStructure != null)
        //    curState = State.MovingToDestination;
    }
    public void PathFindFail() // PathFinder 길찾기 실패 Delegate
    {
        pathFindCount++;
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
        int count = 1;
        cur = curTileForMove;
        Debug.Log("Start : " + cur.GetParent().GetX() + " , " + cur.GetParent().GetY() + "dest = " + destinationTile.GetX() + " , " + destinationTile.GetY());
        Direction dir = curTile.GetDirectionFromOtherTile(path[count].myTilePos);
        Vector2 dirVector = DirectionVector.GetDirectionVector(dir);
        tileForMoveWay.Add(curTileForMove);
        Debug.Log(dir.ToString());
        string pathString = "";
        for (int i = 0; i < path.Count; i++)
        {
            pathString += path[i].myTilePos.GetX() + " , " + path[i].myTilePos.GetY() + "\n";
            //Debug.Log("path = " + path[i].myTilePos.GetX() + " , " + path[i].myTilePos.GetY());
        }
        Debug.Log(pathString);
        Debug.Log("progress : " + cur.GetX() + "(" + cur.GetParent().GetX() + ")" + " , " + cur.GetY() + "(" + cur.GetParent().GetY() + ")"); //19 49
        while (!(path[count].myTilePos.Equals(destinationTile)))
        {
            next = tileLayer.GetTileForMove(cur.GetX() + (int)dirVector.x, cur.GetY() + (int)dirVector.y);
            Debug.Log("progress : " + next.GetX() + "(" + next.GetParent().GetX() + ")" + " , " + next.GetY() + "(" + next.GetParent().GetY() + ")");
            tileForMoveWay.Add(next);
            if (cur.GetParent().Equals(next.GetParent())) //한칸 진행했는데도 같은 타일일때
            {
                Debug.Log("SameTile..");
                next = tileLayer.GetTileForMove(next.GetX() + (int)dirVector.x, next.GetY() + (int)dirVector.y);
                Debug.Log("progress : " + next.GetX() + "(" + next.GetParent().GetX() + ")" + " , " + next.GetY() + "(" + next.GetParent().GetY() + ")");
                tileForMoveWay.Add(next);
                cur = next;
            }
            else
            {
                cur = next;
            }
            if (Random.Range(0, 2) >= 1)
            {

                next = tileLayer.GetTileForMove(cur.GetX() + (int)dirVector.x, cur.GetY() + (int)dirVector.y);
                if (next == null)
                    continue;
                Debug.Log("progress : " + next.GetX() + "(" + next.GetParent().GetX() + ")" + " , " + next.GetY() + "(" + next.GetParent().GetY() + ")");
                tileForMoveWay.Add(next);
                cur = next;
            }
            count++;
            dir = cur.GetParent().GetDirectionFromOtherTile(path[count].myTilePos);
            dirVector = DirectionVector.GetDirectionVector(dir);
            Debug.Log(dir.ToString());
        }
        Debug.Log("Done!!!!");
        return tileForMoveWay;



    }
    // dX = 1 : UR
    // dX = -1: DL
    // dY = 1 : DR
    // dY = -1: UL


    protected IEnumerator MoveAnimation(List<TileForMove> tileForMoveWay)
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
        Direction dir = Direction.DownLeft;
        for (int i = 0; i < tileForMoveWay.Count; i++)
        {
            if (i < tileForMoveWay.Count - 1)
            {
                switch (dir = tileForMoveWay[i].GetDirectionFromOtherTileForMove(tileForMoveWay[i + 1]))
                {
                    case Direction.DownRight:
                        animator.SetBool("DownToUpFlg", false);
                        animator.SetBool("UpToDownFlg", true);
                        foreach (SpriteRenderer sr in spriteRenderers)
                        {
                            sr.flipX = true;
                        }
                        break;
                    case Direction.UpRight:
                        animator.SetBool("UpToDownFlg", false);
                        animator.SetBool("DownToUpFlg", true);
                        foreach (SpriteRenderer sr in spriteRenderers)
                        {
                            sr.flipX = true;
                        }
                        break;
                    case Direction.DownLeft:
                        animator.SetTrigger("MDL");
                        foreach (SpriteRenderer sr in spriteRenderers)
                        {
                            sr.flipX = false;
                        }
                        break;
                    case Direction.UpLeft:
                        animator.SetTrigger("MUL");
                        foreach (SpriteRenderer sr in spriteRenderers)
                        {
                            sr.flipX = false;
                        }
                        break;
                    default:
                        break;
                }
            }
            yield return new WaitForSeconds(1.0f);
            tileForMoveWay[i].SetRecentActor(this);
            transform.position = tileForMoveWay[i].GetPosition();
            SetCurTile(tileForMoveWay[i].GetParent());
            SetCurTileForMove(tileForMoveWay[i]);
        }
        switch (dir)
        {
            case Direction.DownRight:
                animator.SetTrigger("IDL");
                foreach (SpriteRenderer sr in spriteRenderers)
                {
                    sr.flipX = true;
                }
                break;
            case Direction.UpRight:
                animator.SetTrigger("IUL");
                foreach (SpriteRenderer sr in spriteRenderers)
                {
                    sr.flipX = true;
                }
                break;
            case Direction.DownLeft:
                animator.SetTrigger("IDL");
                foreach (SpriteRenderer sr in spriteRenderers)
                {
                    sr.flipX = false;
                }
                break;
            case Direction.UpLeft:
                animator.SetTrigger("IUL");
                foreach (SpriteRenderer sr in spriteRenderers)
                {
                    sr.flipX = false;
                }
                break;
        }
    } // Adventurer에서 이동 중 피격 구현해야함. // Notify?

    public IEnumerator WaitForEnteringStructure()
    {
        while (true)
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

    public bool SetCurTileForMoveLoad(int childNum)
    {
        if (childNum == -1)
            return false;

        SetCurTileForMove(curTile.GetChild(childNum));
        return true;
    }
    #endregion

    /*
	//Stat 관련 처리. 데미지 처리. 등 ...
	//선공 인식 범위 십자 3칸
	
	
	int monsterCode;
	
	public BattleStatus battleStatus
	{
		get
		{
			return _battleStatus;
		}
	}

	private BattleStatus _battleStatus;
	
	Monster monster;
	Adventurer target;
	Coroutine currentCoroutine;
	WaitForSeconds idleInterval;

	//이동시 Monster Tile만 체크
	public void Awake()
	{
		base.Awake();
		SetBattleStatus(GetComponent<BattleStatus>());
	}
	public void Start()
	{
		idleInterval = new WaitForSeconds(1.0f);
		tileLayer = GetCurTile().GetLayer();
		direction = Direction.DownRight;
		state = State.Idle;
	}
	private void OnEnable()
	{
		//moving = StartCoroutine(Moving());
		//attacking = StartCoroutine(Attacking());
		state = new State();
		state = State.Idle;
		StartCoroutine(Act());
	}
	private void OnDisable()
	{	
		//reset...
	}
	
	public IEnumerator Act()
	{
		yield return null;
		while (true)
		{
			yield return null;
			switch (state)
			{
				case State.Idle:
					yield return currentCoroutine = StartCoroutine(Idle());
					break;
				case State.Chasing:
					yield return currentCoroutine = StartCoroutine(Chasing());
					break;
				case State.Battle:
					yield return currentCoroutine = StartCoroutine(Battle());
					break;
				case State.Dead:
					yield return currentCoroutine = StartCoroutine(Dead());
					break;
			}
		}
	}

	IEnumerator Idle()
	{
		while (state == State.Idle)
		{
			yield return StartCoroutine(EnemySearch());
			if (state != State.Chasing)
				yield return StartCoroutine(Wander());
		}
	}

	IEnumerator Chasing()
	{
		int chaseCount = 0;
		if (target == null)
		{
			state = State.Idle;
		}
		else if (target != null && !(target.GetState() == State.Dead || state == State.Dead) || !target.GetCurTile().GetMonsterArea() || chaseCount > 7)
		{
			//chase once coroutine.
			yield return StartCoroutine(MoveOnce(target.GetCurTileForMove()));
			chaseCount++;
			if (GetDistanceFromOtherActorForMove(target) <= battleStatus.GetCalculatedAttackRange())
			{
				state = State.Battle;
				//adventurer.Startbattle trigger!
			}
		}
		else
		{
			yield return new WaitForSeconds(1.0f);
			state = State.Idle;
		}

		yield return null;
	}
	IEnumerator Battle()
	{
		yield return null;
		StartBattle(target);
		float originAttackSpeed = battleStatus.GetCalculatedAttackspeed();
		float eps = 0.0001f;
		WaitForSeconds interval = new WaitForSeconds(1.0f / originAttackSpeed);
		while (target != null && !(state == State.Dead || target.GetState() == State.Dead))
		{
			if (Mathf.Abs(originAttackSpeed - battleStatus.GetCalculatedAttackspeed()) > eps)
			{
				originAttackSpeed = battleStatus.GetCalculatedAttackspeed();
				interval = new WaitForSeconds(1.0f / originAttackSpeed);
			}
			Attack(target);
			yield return interval;
			//공격속도에 따라 한번씩 공격
			//Callback 제대로 처리하기
			
		}
		//target.StartBattle!
		EndBattle(target);
		if(state == State.Dead)
		{
			Die(target as IDamagable);
		}
		else
		{
			state = State.Idle;
		}
		//둘 중 하나가 죽을때까지 싸움
	}
	IEnumerator Dead()
	{
		yield return null;
		yield return new WaitForSeconds(30.0f);
		
		state = State.Idle;
		//respawn;
		//N초 기다린 후 리스폰
	}

	
	
	
	
	public void SetMonsterCode(int code)
	{
		monsterCode = code;
	}
	public int GetMonsterCode()
	{
		return monsterCode;
	}
	public Adventurer GetCurrentTarget()
	{
		return target;
	}

	public IEnumerator EnemySearch() //trigger state
	{
		target = null;
		yield return null;
		foreach (Actor a in GetAdjacentActor(3))
		{
			if ((a is Adventurer || a is SpecialAdventurer) && a.GetState() != State.Dead)
			{
				if (target == null || GetDistanceFromOtherActorForMove(target) > GetDistanceFromOtherActorForMove(a))
				{
					target = a as Adventurer;
					state = State.Chasing;
				}
			}
		}
	}
	public IEnumerator Wander() // 1칸이동
	{
		TileForMove tempTileForMove;

		int rand = 0;
		//랜덤 돌려서 4방향중 1방향으로.(Monster Area, 체크해야함)
		rand = Random.Range(0, 4);
		switch (rand)
		{
			case 0:
				tempTileForMove = GetCurTile().GetLayer().GetTileForMove(GetCurTileForMove().GetX() - 1, GetCurTileForMove().GetY());
				break;
			case 1:
				tempTileForMove = GetCurTile().GetLayer().GetTileForMove(GetCurTileForMove().GetX() + 1, GetCurTileForMove().GetY());
				break;
			case 2:
				tempTileForMove = GetCurTile().GetLayer().GetTileForMove(GetCurTileForMove().GetX(), GetCurTileForMove().GetY() - 1);
				break;
			case 3:
				tempTileForMove = GetCurTile().GetLayer().GetTileForMove(GetCurTileForMove().GetX(), GetCurTileForMove().GetY() + 1);
				break;
			default:
				tempTileForMove = GetCurTileForMove();
				break;
		}
		if (!tempTileForMove.GetRecentActor().GetCurTileForMove().Equals(tempTileForMove))
		{
			yield return StartCoroutine(MoveOnce(tempTileForMove));
		}
	}

	IEnumerator MoveOnce(TileForMove destination)
	{
		if (destination.GetParent().GetMonsterArea())
		{
			TileForMove origin = GetCurTileForMove();
			direction = GetDirectionFromOtherTileForMove(destination);
			SetCurTileForMove(destination);
			SetCurTile(destination.GetParent());
			SetAnimFlagFromDirection(direction);
			animator.SetBool("MoveFlg", true);
			yield return StartCoroutine(MoveAnim(origin.GetPosition(), destination.GetPosition()));
			animator.SetBool("MoveFlg", false);
		}
	}
	public override bool ValidateNextTile(Tile tile)
	{
		return tile.GetMonsterArea();
	}
	public void SetAnimFlagFromDirection(Direction direction)
	{
		switch (direction)
		{
			case Direction.DownRight:
				spriteRenderer.flipX = true;
				animator.SetTrigger("UpToDownFlg");
				break;
			case Direction.UpRight:
				spriteRenderer.flipX = true;
				animator.SetTrigger("DownToUpFlg");
				break;
			case Direction.DownLeft:
				spriteRenderer.flipX = false;
				animator.SetTrigger("UpToDownFlg");
				break;
			case Direction.UpLeft:
				spriteRenderer.flipX = false;
				animator.SetTrigger("DownToUpFlg");
				break;
			case Direction.None:
				break;
			default:
				break;
		}
	}

	public IEnumerator MoveAnim(Vector3 start, Vector3 end)
	{
		Vector3 d = end - start;
		float mag = 0;

		while (mag <= Vector3.Magnitude(d))
		{
			yield return null;
			mag += Vector3.Magnitude(d * battleStatus.GetCalculatedMovespeed() * Time.deltaTime);
			if (mag >= Vector3.Magnitude(d))
			{
				break;
			}
			transform.Translate(d * battleStatus.GetCalculatedMovespeed() * Time.deltaTime);
		}
	}


	public List<TileForMove> GetPathForMove(List<PathVertex> pathList)
	{
		List<TileForMove> wayForMove = new List<TileForMove>();
		wayForMove.Add(monster.GetCurTileForMove());
		for (int i = 0; i < pathList.Count - 1; i++)
		{
			switch (wayForMove[wayForMove.Count - 1].GetChildNum())
			{
				case 0:
					if (pathList[i + 1].myTilePos.GetX() - pathList[i].myTilePos.GetX() == 1) // move down right
					{
						wayForMove.Add(pathList[i].curTile.GetChild(1));
						wayForMove.Add(pathList[i + 1].curTile.GetChild(0));
						if (Random.Range(0, 2) == 0)
						{
							//3칸가기
							wayForMove.Add(pathList[i + 1].curTile.GetChild(1));
						}
					}
					else if (pathList[i + 1].myTilePos.GetX() - pathList[i].myTilePos.GetX() == -1) // move up left
					{
						wayForMove.Add(pathList[i + 1].curTile.GetChild(1));
						if (Random.Range(0, 2) == 0)
						{
							//2칸가기
							wayForMove.Add(pathList[i + 1].curTile.GetChild(0));
						}
					}
					else if (pathList[i + 1].myTilePos.GetY() - pathList[i].myTilePos.GetY() == 1) // move down left
					{
						wayForMove.Add(pathList[i].curTile.GetChild(2));
						wayForMove.Add(pathList[i + 1].curTile.GetChild(0));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(2));
							//3칸가기
						}
					}
					else // move up right
					{
						wayForMove.Add(pathList[i + 1].curTile.GetChild(2));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(0));
							//2칸가기
						}
					}
					break;
				case 1:
					if (pathList[i + 1].myTilePos.GetX() - pathList[i].myTilePos.GetX() == 1) // move down right
					{
						wayForMove.Add(pathList[i + 1].curTile.GetChild(0));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(1));
							//2칸가기
						}

					}
					else if (pathList[i + 1].myTilePos.GetX() - pathList[i].myTilePos.GetX() == -1) // move up left
					{
						wayForMove.Add(pathList[i].curTile.GetChild(0));
						wayForMove.Add(pathList[i + 1].curTile.GetChild(1));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(0));
							//2칸가기
						}
					}
					else if (pathList[i + 1].myTilePos.GetY() - pathList[i].myTilePos.GetY() == 1) // move down left
					{
						wayForMove.Add(pathList[i].curTile.GetChild(3));
						wayForMove.Add(pathList[i + 1].curTile.GetChild(1));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(3));
							//2칸가기
						}

					}
					else // move up right
					{
						wayForMove.Add(pathList[i + 1].curTile.GetChild(3));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(1));
							//2칸가기
						}


					}
					break;
				case 2:
					if (pathList[i + 1].myTilePos.GetX() - pathList[i].myTilePos.GetX() == 1) // move down right
					{
						wayForMove.Add(pathList[i].curTile.GetChild(3));
						wayForMove.Add(pathList[i + 1].curTile.GetChild(2));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(3));
							//3칸가기
						}

					}
					else if (pathList[i + 1].myTilePos.GetX() - pathList[i].myTilePos.GetX() == -1) // move up left
					{
						wayForMove.Add(pathList[i + 1].curTile.GetChild(3));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(2));
							//2칸가기
						}


					}
					else if (pathList[i + 1].myTilePos.GetY() - pathList[i].myTilePos.GetY() == 1) // move down left
					{
						wayForMove.Add(pathList[i + 1].curTile.GetChild(0));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(2));
							//2칸가기
						}

					}
					else // move up right
					{
						wayForMove.Add(pathList[i].curTile.GetChild(0));
						wayForMove.Add(pathList[i + 1].curTile.GetChild(2));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(0));
							//3칸가기
						}


					}
					break;
				case 3:
					if (pathList[i + 1].myTilePos.GetX() - pathList[i].myTilePos.GetX() == 1) // move down right
					{
						wayForMove.Add(pathList[i + 1].curTile.GetChild(2));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(3));
							//2칸가기
						}

					}
					else if (pathList[i + 1].myTilePos.GetX() - pathList[i].myTilePos.GetX() == -1) // move up left
					{
						wayForMove.Add(pathList[i].curTile.GetChild(2));
						wayForMove.Add(pathList[i + 1].curTile.GetChild(3));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(2));
							//3칸가기
						}
					}
					else if (pathList[i + 1].myTilePos.GetY() - pathList[i].myTilePos.GetY() == 1) // move down left
					{
						wayForMove.Add(pathList[i + 1].curTile.GetChild(1));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(3));
							//2칸가기
						}

					}
					else // move up right
					{
						wayForMove.Add(pathList[i].curTile.GetChild(1));
						wayForMove.Add(pathList[i + 1].curTile.GetChild(3));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(1));
							//2칸가기
						}


					}
					break;
				default:
					break;
			}
		}
		return wayForMove;
	}

	public void SetAnimFlgByDirection(Direction dir)
	{
		switch (dir)
		{
			case Direction.DownRight:
				spriteRenderer.flipX = true;
				animator.SetTrigger("UpToDownFlg");
				break;
			case Direction.UpRight:
				spriteRenderer.flipX = true;
				animator.SetTrigger("DownToUpFlg");
				break;
			case Direction.DownLeft:
				spriteRenderer.flipX = false;
				animator.SetTrigger("UpToDownFlg");
				break;
			case Direction.UpLeft:
				spriteRenderer.flipX = false;
				animator.SetTrigger("DownToUpFlg");
				break;
			case Direction.None:
				break;
		}
	}
	public IEnumerator MoveAnim(Vector3 dest)
	{
		yield return null;
		animator.SetBool("MoveFlg", true);
		Vector3 d = dest - monster.transform.position;
		float mag = 0;

		while (mag <= Vector3.Magnitude(d))
		{
			yield return null;
			mag += Vector3.Magnitude(d * battleStatus.GetCalculatedMovespeed() * Time.deltaTime);
			if (mag >= Vector3.Magnitude(d))
			{
				break;
			}
			transform.Translate(d * battleStatus.GetCalculatedMovespeed() * Time.deltaTime);
		}
	}
	
	

	
	void Die(IDamagable Opponent)
	{
		//object 비활성화 및 respawn 준비
		foreach(Enchantment e in enchantmentList)
		{
			e.OnDead(this, Opponent as Actor, GetAdjacentActor(2));
		}
		state = State.Dead;
	}
	#region IDamagable
	public void SetBattleStatus(BattleStatus bs)
	{
		_battleStatus = bs;
	}

	public bool Attack(IDamagable enemy)
	{
		if (enemy != null && (enemy as Adventurer).state != State.Dead)
		{
			float dealtDamage = 0.0f;
			bool isCritical = false;
			bool isHit = false;
			bool isDead = false;
			isCritical = battleStatus.GetIsCriticalAttack();

			enemy.Attacked(this, isCritical, out isHit);
			foreach (Enchantment e in enchantmentList)
			{
				e.OnAttack(this, enemy as Actor, GetAdjacentActor(battleStatus.GetCalculatedAttackRange()), isCritical);
			}
			if (isHit == true)
			{
				enemy.TakeDamage(this, isCritical, out isDead, out dealtDamage);
				foreach (Enchantment e in enchantmentList)
				{
					e.OnDamage(this, enemy as Actor, GetAdjacentActor(2), dealtDamage, isCritical);
				}
			}
			if (isDead)
			{
				EndBattle(enemy);
				state = State.Idle;
			}
			return isHit;
		}
		return false;
	}

	public void Attacked(IDamagable from, bool isCritical, out bool isHit)
	{
		if (Random.Range(0.0f, 1.0f) < battleStatus.GetCalculatedAvoidMult())
			isHit = false;
		else
			isHit = true;
		foreach (Enchantment e in enchantmentList)
		{
			e.OnAttacked(this, from as Actor, GetAdjacentActor(2), isCritical);
		}
		return;
	}

	public void AttackedFromEnchantment(IDamagable from, Enchantment enchantment, bool isCritical, out bool isHit)
	{
		if (Random.Range(0.0f, 1.0f) < battleStatus.GetCalculatedAvoidMult())
			isHit = false;
		else
			isHit = true;
		return;
	}

	public void TakeDamage(IDamagable from, bool isCritical, out bool isDead, out float damage)
	{
		float calculatedDamage = battleStatus.GetCalculatedDamage(from.battleStatus, isCritical);
		damage = calculatedDamage;
		isDead = battleStatus.TakeDamageProcess(calculatedDamage);
		foreach (Enchantment e in enchantmentList)
		{
			e.OnDamaged(this, from as Actor, GetAdjacentActor(2), damage, isCritical);
		}
		if (isDead)
		{
			EndBattle(from);
			Die(from);
			state = State.Dead;
			foreach (Enchantment e in enchantmentList)
			{
				e.OnEndBattle(this, from as Actor, GetAdjacentActor(2));
				e.OnDead(this, from as Actor, GetAdjacentActor(2));
			}
		}

	}

	public void TakeDamageFromEnchantment(float damage, IDamagable from, Enchantment enchantment, bool isCritical, out bool isDead)
	{
		isDead = battleStatus.TakeDamageProcess(damage);
		if (isDead)
		{
			EndBattle(from);

		}
	}

	public void TakeHeal(float heal, IDamagable from)
	{

	}
	public void TakeHealFromEnchantment(float heal, IDamagable from, Enchantment enchantment)
	{

	}

	public void StartBattle(IDamagable Opponent)
	{
		foreach (Enchantment e in enchantmentList)
		{
			e.OnStartBattle(this, Opponent as Actor, GetAdjacentActor(2));
		}

	}

	public void EndBattle(IDamagable Opponent)
	{
		foreach (Enchantment e in enchantmentList)
		{
			e.OnEndBattle(this, Opponent as Actor, GetAdjacentActor(2));
		}

	}

	public void AddEnchantment(Enchantment enchantment)
	{
		enchantmentList.Add(enchantment);
	}

	public void RemoveEnchantmnet(Enchantment enchantment)
	{
		enchantmentList.Remove(enchantment);
	}

	public void TakeStunned(IDamagable from, Enchantment enchantment, float during)
	{

	}

	public bool IsAttackable(IDamagable enemy)
	{
		if (!(enemy is Adventurer))
			return false;
		if (enemy != null && (enemy as Adventurer).GetCurTile().GetMonsterArea() && GetDistanceFromOtherActorForMove(enemy as Adventurer) <= battleStatus.GetCalculatedAttackRange())
			return true;
		return false;
	}

	public IEnumerator Stun(float duration)
	{
		yield return null;
	}

	#endregion
	*/
}
