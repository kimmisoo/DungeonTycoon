using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adventurer : Traveler, ICombatant//, IDamagable {
{
    #region Battle
    //전투 스탯
    BattleStat battleStat;
    //리워드 스탯(현재는 별 필요 없음)
    RewardStat rewardStat;

    private ICombatant enemy;

    protected readonly float RecoveryTick = 6.0f;
    protected readonly int RecoveryTimes = 10;
    protected readonly float RecoveryMult = 0.01f;

    private int monsterSearchCnt;
    private readonly int MonsterSearchMax = 5;

    HuntingArea curHuntingArea;

    public event HealthBelowZeroEventHandler healthBelowZeroEvent;
    #endregion

    public void InitAdventurer(Stat stat, BattleStat battleStat) //
    {
        // 이동가능한 타일인지 확인할 delegate 설정.
        pathFinder.SetValidateTile(ValidateNextTile);
        SetPathFindEvent();
        //stat 초기화
        //pathfinder 초기화 // delegate 그대로
    }

    public void OnEnable()
    {
        base.OnEnable();
        monsterSearchCnt = 0;
    }

    #region StateMachine
    protected override void EnterState(State nextState)
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
            case State.Exit:
                Debug.Log("EXIT");
                //Going to outside 
                break;
            // 모험가 전투관련
            case State.SearchingHuntingArea:
                curCoroutine = StartCoroutine(SearchingHuntingArea());
                break;
            case State.SearchingMonster:
                curCoroutine = StartCoroutine(SearchingMonster());
                break;
            case State.ApproachingToEnemy:
                curCoroutine = StartCoroutine(ApproachingToEnemy());
                break;
            case State.AfterBattle:
                AfterBattle();
                break;
            case State.ExitingHuntingArea:
                curCoroutine = StartCoroutine(ExitingHuntingArea());
                break;
            case State.SearchingShrine:
                SearchingShrine();
                break;
            case State.PassedOut:
                curCoroutine = StartCoroutine(PassedOut());
                break;
            case State.Rescued:
                curCoroutine = StartCoroutine(Rescued());
                break;
            case State.None:
                curState = State.Idle;
                break;
        }
    }
    protected override void ExitState()
    {
        switch (curState)
        {
            case State.Idle:
                break;
            case State.Wandering:
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
            case State.Exit:
                break;
            // 모험가 전투관련
            case State.SearchingHuntingArea:
                break;
            case State.SearchingMonster:
                break;
            case State.ApproachingToEnemy:
                break;
            case State.AfterBattle:
                break;
            case State.ExitingHuntingArea:
                break;
            case State.SearchingShrine:
                break;
            case State.PassedOut:
                break;
            case State.Rescued:
                break;
            case State.None:
                break;
        }
    }
    #endregion

    protected override IEnumerator MoveToDestination()
    {
        //길찾기 성공!
        wayForMove = GetWay(pathFinder.GetPath()); // TileForMove로 변환
        animator.SetBool("MoveFlg", true); // animation 이동으로
        yield return curCoroutine = StartCoroutine(MoveAnimation(wayForMove)); // 이동 한번에 코루틴으로 처리 // 이동 중지할 일 있으면 StopCoroutine moveAnimation // traveler니까 없을듯?																//순번 or 대기 여부 결정

        if (destinationPlace == null)
            curState = State.Idle;
        else if (destinationPlace is Structure)
            VisitStructure();
        else if (destinationPlace is HuntingArea)
            VisitHuntingGround();
    }

    protected void VisitHuntingGround()
    {
        curHuntingArea = destinationPlace as HuntingArea;
        curState = State.SearchingMonster;

        destinationPlace = null; // 사용 후에는 비워주기.
    }

    #region Battle
    protected IEnumerator SearchingHuntingArea()
    {
        // 이거 어떻게 담지?
        destinationPlace = HuntingAreaManager.Instance.FindHuntingArea(battleStat.Level);
        if (destinationPlace == null)
        {
            curState = State.Exit;
            yield break;
        }
        curState = State.MovingToDestination;
    }
    protected IEnumerator SearchingMonster()
    {
        Dictionary<int, GameObject> monsterDict = curHuntingArea.GetMonstersEnabled();
        Monster target = null;
        Monster tempMonster;

        foreach (KeyValuePair<int, GameObject> item in monsterDict) //최단거리 몬스터 찾기
        {
            tempMonster = item.Value.GetComponent<Monster>();
            if (target == null && !tempMonster.isFighting())
                target = tempMonster;
            else if (DistanceBetween(curTileForMove, target.GetCurTileForMove()) > DistanceBetween(curTileForMove, tempMonster.GetCurTileForMove()))
            {
                target = tempMonster;
            }
        }

        if (target == null)
        {
            if (monsterSearchCnt++ >= MonsterSearchMax)
                curState = State.ExitingHuntingArea;
            curState = State.Wandering;
        }
        else
        {
            monsterSearchCnt = 0;
            curState = State.ApproachingToEnemy;
        }
        yield break;
    }


    protected List<TileForMove> GetWayToActor(List<PathVertex> path) // Actor에게 접근하는 메서드(TileForMove 기반)
    {
        List<TileForMove> tileForMoveWay = GetWay(path);
        TileLayer tileLayer = GameManager.Instance.GetTileLayer().GetComponent<TileLayer>();
        TileForMove destTileForMove = enemy.GetCurTileForMove();

        // 적의 TileForMove에 맞춰서 접근
        if (tileForMoveWay[tileForMoveWay.Count-1].GetX() != destTileForMove.GetX())
        {
            tileForMoveWay.Add(tileLayer.GetTileForMove(destTileForMove.GetX(), tileForMoveWay[tileForMoveWay.Count - 1].GetY()));
        }
        if(tileForMoveWay[tileForMoveWay.Count - 1].GetY() != destTileForMove.GetY())
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

            // 몬스터가 이미 죽었다면
            if (enemy.GetState() == State.Dead)
            {
                curState = State.SearchingMonster;
                yield break;
            }

            // 레인지 검사, 레인지 안이면 공격
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

        // 몬스터가 이미 누웠다면.
        if (enemy.GetState() == State.Dead)
        {
            curState = State.SearchingMonster;
            yield break;
        }

        // 레인지 검사, 레인지 안이면 공격
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
        bool isCrit;
        float calculatedDamage;
        battleStat.CalDamage(out calculatedDamage, out isCrit);
        enemy.TakeDamage(index, calculatedDamage, battleStat.PenetrationFixed, battleStat.PenetrationMult, isCrit);
    }

    protected void AfterBattle() // 몬스터 죽인 경험치, 골드 획득.
    {
        stat.gold += enemy.RewardGold();
        battleStat.CurExp += enemy.RewardExp();
        // 포션 음용 넣을까? 넣으면 몇%회복? 몇회?
        curState = State.Idle;
    }

    // 사냥터에서 퇴장
    protected IEnumerator ExitingHuntingArea()
    {
        destinationTile = curHuntingArea.GetEntrance();

        yield return StartCoroutine(pathFinder.Moves(curTile, destinationTile));

        wayForMove = GetWay(pathFinder.GetPath()); // TileForMove로 변환
        animator.SetBool("MoveFlg", true); // animation 이동으로
        yield return curCoroutine = StartCoroutine(MoveAnimation(wayForMove));

        curHuntingArea = null;
        curState = State.Idle;
    }

    // 체력회복할 성소 검색. 욕구만 설정해주고 SearchingStructure로 넘김.
    protected void SearchingShrine()
    {
        stat.GetSpecificDesire(DesireType.Health).SetHighestPriority();
        curState = State.SearchingStructure;
    }

    protected bool SearchRescueTeam()
    {
        //SearchStructure 마저 완성되면 하는걸로.
        //Rescue 건물을 찾음. 그 뒤에는 Rescued()호출
        return true;
    }

    protected IEnumerator PassedOut() //1분동안 10%회복 즉 6초에 1%회복. 회복후에 사냥터에서 퇴장.
    {
        if(SearchRescueTeam())
        {
            curState = State.Rescued;
            yield break;
        }
        // 이 앞에 구조대 정원 있는지 찾아봐야함 우선.
        int recoveryCnt = 0;
        animator.SetBool("DeadFlg", true);
        while (recoveryCnt < RecoveryTimes)
        {
            battleStat.Health += battleStat.HealthMax * RecoveryMult;
            recoveryCnt++;

            yield return new WaitForSeconds(RecoveryTick);
        }
        animator.SetBool("DeadFlg", false);

        curState = State.ExitingHuntingArea;
    }

    protected IEnumerator Rescued()
    {
        //구조대 와서 실어가는 코루틴.
        yield break;
    }

    protected int DistanceBetween(TileForMove pos1, TileForMove pos2)
    {
        return Mathf.Abs(pos1.GetX() - pos1.GetX()) + Mathf.Abs(pos1.GetY() - pos2.GetY());
    }

    #region ICombatant
    public void TakeDamage(float damage, float penFixed, float penMult, bool isCrit) // 데미지 받기. 이펙트 처리를 위해 isCrit도 받음.
    {
        StopAllCoroutines();
        float actualDamage;
        bool isEvaded;
        battleStat.TakeDamage(damage, penFixed, penMult, out actualDamage, out isEvaded);
        StartCoroutine(DisplayHitEffect(actualDamage, isCrit, isEvaded));

        if (curState != State.Battle)
        {
            StopCoroutine(curCoroutine);
            curState = State.Battle;
        }
    }

    public IEnumerator DisplayHitEffect(float actualDamage, bool isCrit, bool isEvaded)
    {
        // 수정요망. 데미지랑 크리 혹은 회피에 따라서 다른 문구가 위에 뜨도록.
        yield return null;
    }

    public int RewardGold()
    {
        //return rewardStat.Gold;
        return 0;
    }
    public int RewardExp()
    {
        //return rewardStat.Exp;
        return 0;
    }
    public float CurHealth()
    {
        return battleStat.Health;
    }

    public void TakeDamage(int attackerIndex, float damage, float penFixed, float penMult, bool isCrit)
    {
        throw new System.NotImplementedException();
    }

    public void HealthBelowZeroNotify(int victimIndex, int attackerIndex)
    {
        throw new System.NotImplementedException();
    }
    #endregion
    #endregion
}