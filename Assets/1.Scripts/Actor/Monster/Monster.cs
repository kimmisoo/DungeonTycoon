#define DEBUG_GETWAY
#define DEBUG_TELEPORT

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Monster : Actor, ICombatant//:Actor, IDamagable {
{
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
    protected TileForMove destTileForMove;
    protected Structure destinationStructure;
    protected Structure[] structureListByPref;

    private HuntingArea habitat;
    public delegate void CorpseDecayEventHandler(int index);
    public event CorpseDecayEventHandler corpseDecayEvent;


    // 저장 및 로드를 위한 인덱스. huntingArea에서 몇번째인지 저장.
    public int index;
    public int monsterNum;

    #region Battle
    BattleStat battleStat;
    RewardStat rewardStat;
    private ICombatant enemy;
    private readonly float RecoveryTimer = 3.0f;
    private readonly float RecoveryTick = 0.5f;
    private readonly float DecayTimer = 3.0f;

    public event HealthBelowZeroEventHandler healthBelowZeroEvent;
    #endregion

    #region initialization
    // Use this for initialization

    #region 수정!
    // 몬스터 초기화
    public void InitMonster(int monsterNum, BattleStat battleStat, RewardStat rewardStat)
    {
        // 이동가능한 타일인지 확인할 delegate 설정.
        pathFinder.SetValidateTile(ValidateNextTile);
        SetPathFindEvent();

        this.monsterNum = monsterNum;
        this.battleStat = new BattleStat(battleStat);
        this.rewardStat = new RewardStat(rewardStat);
        //stat 초기화
        //pathfinder 초기화 // delegate 그대로
    }

    // 몬스터에서 받아서 초기화.
    public void InitMonster(Monster sample)
    {
        monsterNum = sample.monsterNum;

        battleStat = new BattleStat(sample.battleStat);
        rewardStat = new RewardStat(sample.rewardStat);
    }

    public void OnEnable()
    {
        // 몬스터는 타일을 HuntingArea에서 정해줘야할듯.
        /*
        SetCurTile(GameManager.Instance.GetRandomEntrance());
        SetCurTileForMove(GetCurTile().GetChild(Random.Range(0, 3)));
        */

        // 이동가능한 타일인지 확인할 delegate 설정.
        pathFinder.SetValidateTile(ValidateNextTile);
        // PathFind 성공/실패에 따라 호출할 delegate 설정.
        SetPathFindEvent();
        // 아마 실패 횟수인 듯.
        pathFindCount = 0;
        curCoroutine = null;
        structureListByPref = null;

        // 타일레이어 받기. 여기 종종 nullPointer 뜨는데 왠지 모르겠음. 수정요망.
        tileLayer = GameManager.Instance.GetMap().GetLayer(0).GetComponent<TileLayer>();
        // 기본은 Idle.
        StartCoroutine(LateStart());
    }
    #endregion

    public void SetHabitat(HuntingArea input)
    {
        habitat = input;
    }

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

    #region StateMachine
    protected void EnterState(State nextState)
    {
        switch (nextState)
        {
            case State.Idle:
                Debug.Log("Idle");
                superState = SuperState.Idle;
                curState = State.Wandering;
                //Traveler이므로 무조건 SearchingStructure 부터
                //이외에 체크할거 있으면 여기서
                break;
            case State.Wandering:
                Debug.Log("Wandering");
                superState = SuperState.Wandering;
                //Wandering();
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
            case State.InitiatingBattle:
                Debug.Log("Initiating Battle");
                superState = SuperState.Battle;
                InitiatingBattle();
                break;
            case State.Battle:
                Debug.Log("Battle");
                curCoroutine = StartCoroutine(Battle());
                break;
            case State.AfterBattle:
                Debug.Log("After Battle");
                superState = SuperState.AfterBattle;
                curCoroutine = StartCoroutine(AfterBattle());
                break;
            case State.Dead:
                Debug.Log("Dead");
                superState = SuperState.Dead;
                curCoroutine = StartCoroutine(Dead());
                break;
            case State.None:
                superState = SuperState.Idle;
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
    protected IEnumerator Wandering()
    {
        yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));

        // 목적지(빈 타일) 찾기.
        destTileForMove = habitat.FindBlanks(1)[0];

        destinationTile = destTileForMove.GetParent();
        curState = State.PathFinding;
    }

    protected IEnumerator PathFinding()
    {
        yield return StartCoroutine(pathFinder.Moves(curTile, destinationTile));

        switch (superState)
        {
            case SuperState.Wandering:
                curState = State.MovingToDestination;
                break;
            case SuperState.Battle:
                curState = State.ApproachingToEnemy;
                break;
        }
    }

    protected IEnumerator MoveToDestination()
    {
        //길찾기 성공!
        wayForMove = GetWayTileForMove(pathFinder.GetPath(), destTileForMove); // TileForMove로 변환
        animator.SetBool("MoveFlg", true); // animation 이동으로
        yield return curCoroutine = StartCoroutine(MoveAnimation(wayForMove)); // 이동 한번에 코루틴으로 처리 // 이동 중지할 일 있으면 StopCoroutine moveAnimation // traveler니까 없을듯?																//순번 or 대기 여부 결정

        animator.SetBool("MoveFlg", false);

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
        return tile.GetPassableMonster();
    }

    protected List<TileForMove> GetWay(List<PathVertex> path) // Pathvertex -> TileForMove
    {
        //Debug.Log("path.Count : " + path.Count);
        // 같은 칸 내에서의 이동
        if (path.Count == 1)
        {
            List<TileForMove> result = new List<TileForMove>();
            result.Add(curTileForMove);
            return result;
        }

        List<TileForMove> tileForMoveWay = new List<TileForMove>();

        // 현재 TileForMove, 다음 TileForMove 선언
        TileForMove next, cur;
        int count = 1;
        cur = curTileForMove;
        //Debug.Log("Start : " + cur.GetParent().GetX() + " , " + cur.GetParent().GetY() + ", dest = " + destinationTile.GetX() + " , " + destinationTile.GetY());

        // 다음 타일로의 방향 벡터
        Direction dir = curTile.GetDirectionFromOtherTile(path[count].myTilePos);
        Vector2 dirVector = DirectionVector.GetDirectionVector(dir);

        tileForMoveWay.Add(curTileForMove);

        // 디버깅용?
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
            //Debug.Log("progress : " + next.GetX() + "(" + next.GetParent().GetX() + ")" + " , " + next.GetY() + "(" + next.GetParent().GetY() + ")");
            tileForMoveWay.Add(next);
            if (cur.GetParent().Equals(next.GetParent())) //한칸 진행했는데도 같은 타일일때
            {
                //Debug.Log("SameTile..");
                next = tileLayer.GetTileForMove(next.GetX() + (int)dirVector.x, next.GetY() + (int)dirVector.y);
                //Debug.Log("progress : " + next.GetX() + "(" + next.GetParent().GetX() + ")" + " , " + next.GetY() + "(" + next.GetParent().GetY() + ")");
                tileForMoveWay.Add(next);
            }
            cur = next;

            if (Random.Range(0, 2) >= 1 && destTileForMove != cur)
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
        return tileForMoveWay;
    }
    // dX = 1 : UR
    // dX = -1: DL
    // dY = 1 : DR
    // dY = -1: UL
    protected List<TileForMove> GetWayTileForMove(List<PathVertex> path, TileForMove destTileForMove)
    {
        List<TileForMove> moveWayTFM = GetWay(path);
        TileLayer tileLayer = GameManager.Instance.GetTileLayer().GetComponent<TileLayer>();

        TileForMove lastTFM = moveWayTFM[moveWayTFM.Count - 1];

        int xDiff = destTileForMove.GetX() - lastTFM.GetX();
        int yDiff = destTileForMove.GetY() - lastTFM.GetY();
        int xSeq, ySeq;

        if (xDiff == 0)
            xSeq = 0;
        else
            xSeq = xDiff / Mathf.Abs(xDiff);

        if (yDiff == 0)
            ySeq = 0;
        else
            ySeq = yDiff / Mathf.Abs(yDiff);

        Debug.Log("원래 끝 TFM : [" + lastTFM.GetX() + ", " + lastTFM.GetY());
        Debug.Log("xDiff : " + xDiff + ", xSeq : " + xSeq);
        Debug.Log("xDiff : " + yDiff + ", xSeq : " + ySeq);

        for (int i = 0; i != xDiff; i += xSeq)
        {
            moveWayTFM.Add(tileLayer.GetTileForMove(lastTFM.GetX() + xSeq + i, lastTFM.GetY()));
        }

        lastTFM = moveWayTFM[moveWayTFM.Count - 1];

        for (int i = 0; i != yDiff; i += ySeq)
        {
            moveWayTFM.Add(tileLayer.GetTileForMove(lastTFM.GetX(), lastTFM.GetY() + ySeq + i));
        }

#if DEBUG_GETWAY
        Debug.Log("끝 타일1 : " + moveWayTFM[moveWayTFM.Count - 1].GetParent().GetX() + ", " + moveWayTFM[moveWayTFM.Count - 1].GetParent().GetY());
        Debug.Log("끝 TFM1 : " + moveWayTFM[moveWayTFM.Count - 1].GetX() + ", " + moveWayTFM[moveWayTFM.Count - 1].GetY());
        Debug.Log("시작지 TFM : " + curTileForMove.GetX() + ", " + curTileForMove.GetY());

        for (int i = 0; i < moveWayTFM.Count; i++)
        {
            if (i == 0)
                Debug.Log("출발");
            Debug.Log("[" + moveWayTFM[i].GetX() + ", " + moveWayTFM[i].GetY());
            if (i == moveWayTFM.Count - 1)
                Debug.Log("끝");
        }

        Debug.Log("끝 타일2 : " + destTileForMove.GetParent().GetX() + ", " + destTileForMove.GetParent().GetY());
        Debug.Log("끝 TFM2 : " + destTileForMove.GetX() + ", " + destTileForMove.GetY());
#endif
        return moveWayTFM;
    }


    protected IEnumerator MoveAnimation(List<TileForMove> tileForMoveWay)
    {
        yield return null;

        Direction dir = Direction.DownLeft;
        //FlipX true == Left, false == Right
        Vector3 dirVector;
        float distance, sum = 0.0f;

        // GewWay에서 받은 경로대로 이동
        for (int i = 0; i < tileForMoveWay.Count - 1; i++)
        {
            tileForMoveWay[i].SetRecentActor(this);
            SetCurTile(tileForMoveWay[i].GetParent());
            SetCurTileForMove(tileForMoveWay[i]);
            Debug.Log("curTileForMove : [" + curTileForMove.GetX() + ", " + curTileForMove.GetY() + "]");

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

        SetCurTile(tileForMoveWay[tileForMoveWay.Count - 1].GetParent());
        SetCurTileForMove(tileForMoveWay[tileForMoveWay.Count - 1]);
        Debug.Log("last curTileForMove : [" + curTileForMove.GetX() + ", " + curTileForMove.GetY() + "]");
        // 한칸 덜감
    } // Adventurer에서 이동 중 피격 구현해야함. // Notify?
    #endregion

    #region Battle
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

            // 모험가가 이미 누웠거나, 사냥터에서 나갔다면
            if (!ValidatingEnemy())
            {
                curState = State.AfterBattle;
                yield break;
            }

            // 레인지 검사. 적이 공격 범위 안으로 들어왔을 때.
            if (CheckInRange())
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
        if (!ValidatingEnemy())
        {
            curState = State.AfterBattle;
            yield break;
        }

        // 레인지 검사
        if (CheckInRange())
        {
            curState = State.Battle;
            yield break;
        }
        else // 목적지 도착했지만 공격 범위 안에 안 들어올 때.
            curState = State.PathFinding;
    }

    protected bool ValidatingEnemy()
    {
        SuperState enemySuperState = enemy.GetSuperState();
        // 적이 사냥터 내에 있으며 살아 있을 때.
        if (enemySuperState == SuperState.Battle || enemySuperState == SuperState.SearchingMonster
            || enemySuperState == SuperState.AfterBattle || enemySuperState == SuperState.ExitingHuntingArea)
            return true;
        else
            return false;
    }

    protected IEnumerator ApproachingToEnemy()
    {
        animator.SetBool("MoveFlg", true); // animation 이동으로
        yield return curCoroutine = StartCoroutine(Charge(wayForMove));
        animator.SetBool("MoveFlg", false);
    }

    // 전투 시작
    protected void InitiatingBattle()
    {
        StopCoroutine(curCoroutine);

        destinationTile = enemy.GetCurTile();
        destTileForMove = enemy.GetCurTileForMove();

        // 적이 공격 범위 안에 있다면 바로 전투.
        if (CheckInRange())
            curState = State.Battle;
        else
            curState = State.PathFinding;
    }

    protected IEnumerator Battle()
    {
        enemy.healthBelowZeroEvent += OnEnemyHealthBelowZero;

        while (enemy.CurHealth() > 0)
        {
            yield return curCoroutine = StartCoroutine(Attack());
        }

        curState = State.AfterBattle; // 이 대신 이벤트로 처리해줘야 함.
    }

    protected IEnumerator Attack() // 공격
    {
        if (!ValidatingEnemy())
        {
            yield break;
        }

        yield return null; // 애니메이션 관련 넣을 것.
        bool isCrit;
        float calculatedDamage;
        battleStat.CalDamage(out calculatedDamage, out isCrit);

        enemy.TakeDamage(index, calculatedDamage, battleStat.PenetrationFixed, battleStat.PenetrationMult, isCrit);
    }

    protected void OnEnemyHealthBelowZero(int victimIndex, int attackerIndex)
    {
        StopCoroutine(curCoroutine);

        if(attackerIndex == index)
        {
            GetBattleReward();// 보상 받기. 몬스터는 없음   
        }
        enemy = null;

        curState = State.AfterBattle;
    }

    protected void GetBattleReward()
    {

    }

    protected IEnumerator AfterBattle() // 전투 끝나고 체력회복. 가만히 서서 회복함. 기본적으로 3초.
    {
        while (battleStat.Health < battleStat.HealthMax) // float 비교연산인데 문제 안 생기는지. 수정요망.
        {
            battleStat.Health += battleStat.HealthMax * (RecoveryTick / RecoveryTimer);
            yield return new WaitForSeconds(RecoveryTick);
        }
        curState = State.Idle;
    }

    protected IEnumerator Dead()
    {
        // 여기 애니메이션 설정 넣으면 됨.
        yield return new WaitForSeconds(DecayTimer);
        corpseDecayEvent?.Invoke(index);
    }

    // 죽을 때 호출. 이 몬스터를 공격대상으로 하고있는 모험가들에게 알려줌.
    public void HealthBelowZeroNotify(int victimIndex, int attackerIndex)
    {
        healthBelowZeroEvent?.Invoke(victimIndex, attackerIndex);
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

    protected bool CheckInRange()
    {
        return (DistanceBetween(curTileForMove, enemy.GetCurTileForMove()) <= battleStat.Range);
    }

    public bool isFighting()
    {
        if (curState == State.Battle || curState == State.ApproachingToEnemy)
            return true;
        else
            return false;
    }

    #region ICombatant
    public void TakeDamage(int attackerIndex, float damage, float penFixed, float penMult, bool isCrit) // 데미지 받기. 이펙트 처리를 위해 isCrit도 받음.
    {
        float actualDamage;
        bool isEvaded;
        battleStat.TakeDamage(damage, penFixed, penMult, out actualDamage, out isEvaded);
        StartCoroutine(DisplayHitEffect(actualDamage, isCrit, isEvaded));

        // 조건?
        if (battleStat.Health <= 0)
        {
            HealthBelowZeroNotify(index, attackerIndex);
            curState = State.Dead;
        }
        else if (superState != SuperState.Battle)
            curState = State.InitiatingBattle;
    }
    public IEnumerator DisplayHitEffect(float actualDamage, bool isCrit, bool isEvaded)
    {
        // 수정요망. 데미지랑 크리 혹은 회피에 따라서 다른 문구가 위에 뜨도록.
        yield return null;
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

    public void ResetBattleStat() // 전투 관련 멤버변수 리셋
    {
        battleStat.ResetBattleStat();
    }
    #endregion
    #endregion
}
