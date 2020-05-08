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

    public int Level
    {
        get { return battleStat.Level; }
    }    
    #endregion

    public void InitAdventurer(Stat stat, BattleStat battleStat, RewardStat rewardStat) //
    {
        // 이동가능한 타일인지 확인할 delegate 설정.
        pathFinder.SetValidateTile(ValidateNextTile);
        SetPathFindEvent();

        //this.monsterNum = monsterNum; //JSON에서 불러오기용 정보 지금은 ㅇ벗음.
        this.battleStat = new BattleStat(battleStat);
        this.rewardStat = new RewardStat(rewardStat);
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
                superState = SuperState.Idle;
                Idle();
                //Traveler이므로 무조건 SearchingStructure 부터
                //이외에 체크할거 있으면 여기서
                break;
            case State.SolvingDesire_Wandering:
                Debug.Log("Wandering");
                superState = SuperState.SolvingDesire_Wandering;
                curCoroutine = StartCoroutine(SolvingDesire_Wandering());
                break;
            case State.SearchingMonster_Wandering:
                Debug.Log("Wandering");
                superState = SuperState.SearchingMonster_Wandering;
                curCoroutine = StartCoroutine(SearchingMonster_Wandering());
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
                superState = SuperState.ExitingDungeon;
                //Traveler에서 구현
                //curCoroutine = StartCoroutine(SearchingExit()); 
                break;
            case State.Exit:
                //Treaveler에서 구현
                //Exit();
                break;
            // 모험가 전투관련
            case State.SearchingHuntingArea:
                superState = SuperState.EnteringHuntingArea;
                curCoroutine = StartCoroutine(SearchingHuntingArea());
                break;
            case State.SearchingMonster:
                superState = SuperState.SearchingMonster;
                curCoroutine = StartCoroutine(SearchingMonster());
                break;
            case State.ApproachingToEnemy:
                curCoroutine = StartCoroutine(ApproachingToEnemy());
                break;
            case State.InitiatingBattle:
                superState = SuperState.Battle;
                InitiatingBattle();
                break;
            case State.Battle:
                curCoroutine = StartCoroutine(Battle());
                break;
            case State.AfterBattle:
                superState = SuperState.AfterBattle;
                curCoroutine = StartCoroutine(AfterBattle());
                break;
            case State.ExitingHuntingArea:
                superState = SuperState.ExitingHuntingArea;
                curCoroutine = StartCoroutine(ExitingHuntingArea());
                break;
            case State.PassedOut:
                superState = SuperState.PassedOut;
                PassedOut();
                break;
            case State.SpontaneousRecovery:
                curCoroutine = StartCoroutine(SpontaneousRecovery());
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
            case State.SolvingDesire_Wandering:
                break;
            case State.SearchingMonster_Wandering:
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
            // 모험가 전투관련
            case State.SearchingHuntingArea:
                break;
            case State.SearchingMonster:
                break;
            case State.ApproachingToEnemy:
                break;
            case State.InitiatingBattle:
                break;
            case State.Battle:
                break;
            case State.AfterBattle:
                break;
            case State.ExitingHuntingArea:
                break;
            case State.PassedOut:
                break;
            case State.SpontaneousRecovery:
                break;
            case State.Rescued:
                break;
            case State.None:
                break;
        }
    }
    #endregion

    protected override IEnumerator PathFinding()
    {
        yield return StartCoroutine(pathFinder.Moves(curTile, destinationTile));

        switch(superState)
        {
            case SuperState.SearchingMonster:
                curState = State.ApproachingToEnemy;
                break;
            case SuperState.PassedOut:
                curState = State.Rescued;
                break;
            default: //EnteringHuntingArea, ExitingDungeon, ExitingHuntingArea, SearchingMonster_Wandering, SolvingDesire_Wandering, SolvingDesire
                curState = State.MovingToDestination;
                break;
        }
    }

    // 수정요망
    protected override IEnumerator MoveToDestination()
    {
        //길찾기 성공!
        wayForMove = GetWay(pathFinder.GetPath()); // TileForMove로 변환

        yield return curCoroutine = StartCoroutine(MoveAnimation(wayForMove)); // 이동 한번에 코루틴으로 처리 // 이동 중지할 일 있으면 StopCoroutine moveAnimation // traveler니까 없을듯?																//순번 or 대기 여부 결정

        switch(superState)
        {
            case SuperState.SolvingDesire:
                CheckStructure();
                break;
            case SuperState.SolvingDesire_Wandering:
                wanderCount++;
                curState = State.SearchingStructure;
                break;
            case SuperState.ExitingHuntingArea:
                curState = State.SearchingStructure;
                break;
            case SuperState.EnteringHuntingArea:
                VisitHuntingGround();
                break;
            case SuperState.ExitingDungeon:
                curState = State.SearchingExit;
                break;
            case SuperState.SearchingMonster_Wandering:
                curState = State.SearchingMonster;
                break;
        }
    }

    public void Idle()
    {
        //if (stat.GetSpecificDesire(stat.GetHighestDesire()).desireValue < 50.0f)
            curState = State.SearchingHuntingArea;
        //else
        //    curState = State.SearchingStructure;
    }

    

    protected void VisitHuntingGround()
    {
        curHuntingArea = destinationPlace as HuntingArea;
        curHuntingArea.EnterAdventurer(this.gameObject);

        destinationPlace = null; // 사용 후에는 비워주기.
        curState = State.SearchingMonster;
    }

    // 수정요망
    protected IEnumerator SearchingHuntingArea()
    {
        yield return null;
        // (이 모험가의 level <= 사냥터의 maxLevel)인 사냥터 중 maxLevel이 가장 낮은 걸 찾음.
        curHuntingArea = HuntingAreaManager.Instance.FindHuntingArea(battleStat.Level);

        if (curHuntingArea == null)
            curState = State.SearchingExit;
        else
            curState = State.PathFinding;
    }

    protected IEnumerator SearchingMonster()
    {
        yield return null;

        enemy = curHuntingArea.FindNearestMonster(this); // 가장 가까운 몬스터 찾기.

        if(enemy == null)
        {
            if (monsterSearchCnt >= MonsterSearchMax)
            {
                monsterSearchCnt = 0;
                curState = State.ExitingHuntingArea;
            }
            else
            {
                monsterSearchCnt++;
                curState = State.SearchingMonster_Wandering;
            }
        }
        else
        {
            monsterSearchCnt = 0;
            curState = State.PathFinding;
        }
    }

    //수정요망
    protected IEnumerator SearchingMonster_Wandering()
    {
        // 빈 칸이 없으면?? 수정요망 이걸 막기위해서 사냥터도 출입인원 제한해야 함. 나머진 기다리고.
        destTileForMove = curHuntingArea.FindBlanks(1)[0];
        destinationTile = destTileForMove.GetParent();

        yield return null; // 있든 없든 별 상관 X

        curState = State.PathFinding;
    }

    protected IEnumerator ExitingHuntingArea()
    {
        destinationTile = curHuntingArea.GetEntrance();
        curHuntingArea.ExitAdventurer(this.gameObject);

        curHuntingArea = null;

        curState = State.PathFinding;
        yield return null;
    }

    protected virtual void PassedOut()
    {
        animator.SetBool("isDead", true);
        Structure[] tempArr = StructureManager.Instance.FindRescue(this);

        if (tempArr.Length == 0)
        {
            curState = State.SpontaneousRecovery;
        }
        else
        {
            destinationPlace = tempArr[0];
            curState = State.PathFinding;
        }
    }

    protected IEnumerator Rescued()
    {
        List<TileForMove> temp = GetWay(pathFinder.GetPath());

        yield return null; // 구조대 관련 애니메이션 추가해야 함. 수정요망
    }

    protected IEnumerator SpontaneousRecovery()
    {
        for(int i = 0; i < RecoveryTimes; i++)
        {
            battleStat.Health += battleStat.HealthMax * RecoveryMult;
            StartCoroutine(RecoveryEffect(battleStat.HealthMax * RecoveryMult));

            yield return new WaitForSeconds(RecoveryTick);
        }

        curState = State.ExitingHuntingArea;
    }

    protected IEnumerator RecoveryEffect(float healingAmount) // 체력 회복 이펙트
    {
        yield return null;
    }

    #region Battle
    protected IEnumerator Charge(List<TileForMove> tileForMoveWay)
    {
        yield return null;

        Direction dir = Direction.DownLeft;
        //FlipX true == Left, false == Right
        Vector3 dirVector;
        float distance, sum = 0.0f;

        animator.SetBool("MoveFlg", true);

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
            animator.SetBool("MoveFlg", false);
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
        // 적이 살아 있을 때.
        if (enemySuperState != SuperState.Dead)
            return true;
        else
            return false;
    }

    protected IEnumerator ApproachingToEnemy()
    {
        yield return curCoroutine = StartCoroutine(Charge(wayForMove));
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

        while (ValidatingEnemy())
        {
            yield return curCoroutine = StartCoroutine(Attack());
        }

        curState = State.AfterBattle; // 이 대신 이벤트로 처리해줘야 함.
    }

    protected IEnumerator Attack() // 공격
    {
        yield return null; // 애니메이션 관련 넣을 것.

        // 어차피 이벤트로 나가는데 필요한지?
        if (!ValidatingEnemy())
        {
            yield break;
        }

        bool isCrit;
        float calculatedDamage;
        battleStat.CalDamage(out calculatedDamage, out isCrit);

        enemy.TakeDamage(index, calculatedDamage, battleStat.PenetrationFixed, battleStat.PenetrationMult, isCrit);
    }

    protected void OnEnemyHealthBelowZero(int victimIndex, int attackerIndex)
    {
        StopCoroutine(curCoroutine);

        if (attackerIndex == index) // 내가 죽였다면.
        {
            GetBattleReward(victimIndex);// 보상 받기.
        }
        enemy = null;

        curState = State.AfterBattle;
    }

    // 죽인 몬스터로부터 보상 획득.
    protected void GetBattleReward(int victimIndex)
    {
        Monster killedMonster = curHuntingArea.GetMonstersEnabled()[victimIndex].GetComponent<Monster>();
        battleStat.CurExp += killedMonster.RewardExp();
        stat.gold += killedMonster.RewardGold();
    }

    // 수정요망
    protected IEnumerator AfterBattle()
    {
        // 포션 음용 넣어야한다면 여기에.
        yield return null;
        if (battleStat.Health < battleStat.HealthMax / 4) //체력이 25%미만이면 
            curState = State.ExitingHuntingArea;
        else
            curState = State.SearchingMonster;
    }

    // 죽을 때 호출. 이 모험가를 공격대상으로 하고있는 몬스터들에게 알려줌.
    // 몬스터들은 누굴 죽였는지 알 필요 X. 단 전투는 중단.
    public void HealthBelowZeroNotify(int victimIndex, int attackerIndex)
    {
        healthBelowZeroEvent?.Invoke(victimIndex, attackerIndex);
    }

    protected bool CheckInRange()
    {
        return (DistanceBetween(curTileForMove, enemy.GetCurTileForMove()) <= battleStat.Range);
    }

    public bool isFighting()
    {
        return superState == SuperState.Battle;
    }
    #endregion

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
        return rewardStat.Gold; // 이거 평소엔 별 필요없지만 일선모험가 vs 일선모험가 시에는 필요.
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
}