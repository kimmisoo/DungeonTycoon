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

    #region Battle
    BattleStat battleStat;
    RewardStat rewardStat;
    private ICombatant enemy;
    private readonly float RecoveryTimer = 3.0f;
    private readonly float RecoveryTick = 0.5f;
    private readonly float DeadTimer = 3.0f;
    #endregion

    #region initialization
    // Use this for initialization

    #region 수정!
    public void InitMonster(Stat stat) //
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
    #endregion

    IEnumerator LateStart()
    {
        yield return null;
        curState = State.Idle;
    }
    #endregion

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

    #region StateMachine
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
            case State.PathFinding:
                Debug.Log("PF");
                curCoroutine = StartCoroutine(PathFinding());
                break;
            case State.MovingToDestination:
                Debug.Log("MTS");
                curCoroutine = StartCoroutine(MoveToDestination());
                break;
            case State.ApproachingToEnemy:
                Debug.Log("Approaching to the enemy.");
                curCoroutine = StartCoroutine(ApproachingToEnemy());
                break;
            case State.Battle:
                Debug.Log("Battle");
                curCoroutine = StartCoroutine(Battle());
                break;
            case State.AfterBattle:
                Debug.Log("After Battle");
                curCoroutine = StartCoroutine(AfterBattle());
                break;
            case State.Dead:
                Debug.Log("Dead");
                curCoroutine = StartCoroutine(Dead());
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
            case State.ApproachingToEnemy:
                break;
            case State.Battle:
                break;
            case State.AfterBattle:
                break;
            case State.Dead:
                break;
            case State.None:
                break;
        }
    }
    #endregion

    #region moving
    // 사냥터 내에서만 움직이게 수정할 것.
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

        curState = State.Idle;
    }


    public override void SetPathFindEvent() // Pathfinder Delegate 설정
    {
        pathFinder.SetNotifyEvent(PathFindSuccess, PathFindFail);
    }

    public void PathFindSuccess() // Pathfinder 길찾기 성공 Delegate
    {
        pathFindCount = 0;
        if (destinationStructure != null)
            curState = State.MovingToDestination;
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
		{X
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

        Direction dir = Direction.DownLeft;
        //FlipX true == Left, false == Right
        Vector3 dirVector;
        float distance, sum = 0.0f;

        // PathFinder에서 받은 경로대로 이동
        for (int i = 0; i < tileForMoveWay.Count - 1; i++)
        {
            tileForMoveWay[i].SetRecentActor(this);
            SetCurTile(tileForMoveWay[i].GetParent());
            SetCurTileForMove(tileForMoveWay[i]);

            // 방향에 따른 애니메이션 설정.
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
            // 이동
            dirVector = tileForMoveWay[i + 1].GetPosition() - tileForMoveWay[i].GetPosition();
            distance = Vector3.Distance(tileForMoveWay[i].GetPosition(), tileForMoveWay[i + 1].GetPosition());
            while (Vector3.Distance(transform.position, tileForMoveWay[i].GetPosition()) < distance)
            {
                yield return null;
                transform.Translate(dirVector * Time.deltaTime);

            }
            sum = 0.0f;
            transform.position = tileForMoveWay[i + 1].GetPosition();

        }

    } // Adventurer에서 이동 중 피격 구현해야함. // Notify?
    #endregion

    #region Battle
    protected List<TileForMove> GetWayToActor(List<PathVertex> path) // Actor에게 접근하는 메서드(TileForMove 기반)
    {
        List<TileForMove> tileForMoveWay = GetWay(path);
        TileLayer tileLayer = GameManager.Instance.GetTileLayer().GetComponent<TileLayer>();
        TileForMove destTileForMove = enemy.GetCurTileForMove();

        // 적의 TileForMove에 맞춰서 접근
        if (tileForMoveWay[tileForMoveWay.Count - 1].GetX() != destTileForMove.GetX())
        {
            tileForMoveWay.Add(tileLayer.GetTileForMove(destTileForMove.GetX(), tileForMoveWay[tileForMoveWay.Count - 1].GetY()));
        }
        if (tileForMoveWay[tileForMoveWay.Count - 1].GetY() != destTileForMove.GetY())
        {
            tileForMoveWay.Add(tileLayer.GetTileForMove(tileForMoveWay[tileForMoveWay.Count - 1].GetX(), tileForMoveWay[tileForMoveWay.Count - 1].GetY()));
        }

        return tileForMoveWay;
    }
    protected IEnumerator Charge(List<TileForMove> tileForMoveWay)
    {
        yield return null;

        Direction dir = Direction.DownLeft;
        //FlipX true == Left, false == Right
        Vector3 dirVector;
        float distance, sum = 0.0f;

        // PathFinder에서 받은 경로대로 이동
        for (int i = 0; i < tileForMoveWay.Count - 1; i++)
        {
            tileForMoveWay[i].SetRecentActor(this);
            SetCurTile(tileForMoveWay[i].GetParent());
            SetCurTileForMove(tileForMoveWay[i]);

            // 모험가가 이미 누웠다면.
            if (enemy.GetState() == State.PassedOut)
            {
                curState = State.AfterBattle;
                yield break;
            }

            // 레인지 검사
            if (DistanceBetween(curTileForMove, enemy.GetCurTileForMove()) <= battleStat.Range)
            {
                curState = State.Battle;
                yield break;
            }

            // 방향에 따른 애니메이션 설정.
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
            // 이동
            dirVector = tileForMoveWay[i + 1].GetPosition() - tileForMoveWay[i].GetPosition();
            distance = Vector3.Distance(tileForMoveWay[i].GetPosition(), tileForMoveWay[i + 1].GetPosition());
            while (Vector3.Distance(transform.position, tileForMoveWay[i].GetPosition()) < distance)
            {
                yield return null;
                transform.Translate(dirVector * Time.deltaTime);

            }
            sum = 0.0f;
            transform.position = tileForMoveWay[i + 1].GetPosition();
        }

        // 모험가가 이미 누웠다면.
        if (enemy.GetState() == State.PassedOut)
        {
            curState = State.AfterBattle;
            yield break;
        }

        // 레인지 검사
        if (DistanceBetween(curTileForMove, enemy.GetCurTileForMove()) <= battleStat.Range)
        {
            curState = State.Battle;
            yield break;
        }

        // 목적지 도착했지만 공격 범위 안에 안 들어올 때.
        curState = State.ApproachingToEnemy;
    }

    protected IEnumerator ApproachingToEnemy()
    {
        destinationTile = tileLayer.GetTileAsComponent(Random.Range(0, tileLayer.GetLayerWidth() - 1), Random.Range(0, tileLayer.GetLayerHeight() - 1));
        destinationTile = enemy.GetCurTile(); // TileForMove가 아니라도 문제없나?

        yield return StartCoroutine(pathFinder.Moves(curTile, destinationTile));

        wayForMove = GetWayToActor(pathFinder.GetPath()); // TileForMove로 변환
        animator.SetBool("MoveFlg", true); // animation 이동으로
        yield return curCoroutine = StartCoroutine(Charge(wayForMove));
    }

    protected IEnumerator Battle()
    {
        while (enemy.CurHealth() > 0)
        {
            yield return curCoroutine = StartCoroutine(Attack());
        }

        curState = State.AfterBattle;
    }

    protected IEnumerator Attack()
    {
        yield return null; // 애니메이션 관련 넣을 것.
        enemy.TakeDamage(battleStat.CalDamage(), battleStat.PenetrationFixed, battleStat.PenetrationMult);
    }

    protected IEnumerator AfterBattle() // 전투 끝나고 체력회복. 가만히 서서 회복함. 기본적으로 3초.
    {
        while (battleStat.Health < battleStat.HealthMax) // float 비교연산인데 문제 안 생기는지. 수정요망.
        {
            battleStat.Health += battleStat.HealthMax * (RecoveryTick / RecoveryTimer);
            yield return new WaitForSeconds(RecoveryTick);
        }
        curState = State.Idle;
        yield break;
    }

    protected IEnumerator Dead()
    {
        // 여기 애니메이션 설정 넣으면 됨.
        yield return new WaitForSeconds(DeadTimer);
        gameObject.SetActive(false);
    }
    #endregion

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

    protected int DistanceBetween(TileForMove pos1, TileForMove pos2)
    {
        return Mathf.Abs(pos1.GetX() - pos1.GetX()) + Mathf.Abs(pos1.GetY() - pos2.GetY());
    }

    public bool isFighting()
    {
        if (curState == State.Battle || curState == State.ApproachingToEnemy)
            return true;
        else
            return false;
    }
    #region ICombatant
    public void TakeDamage(float damage, float penFixed, float penMult)
    {
        battleStat.TakeDamage(damage, penFixed, penMult);
    }
    public int RewardGold()
    {
        return rewardStat.Gold;
    }
    public int RewardExp()
    {
        return rewardStat.Exp;
    }
    public float CurHealth()
    {
        return battleStat.Health;
    }
    Tile GetCurTile()
    {
        return curTile;
    }
    #endregion
    #endregion
}
