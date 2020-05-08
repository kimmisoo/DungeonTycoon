//#define DEBUG_ADV
//#define DEBUG_ADV_STATE
//#define DEBUG_ADV_BATTLE
//#define DEBUG_CHARGE

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Adventurer : Traveler, ICombatant//, IDamagable {
{
    #region Battle
    //전투 스탯
    protected BattleStat battleStat;
    //리워드 스탯(현재는 별 필요 없음)
    protected RewardStat rewardStat;

    // 스킬들(아이템, 고유능력 등 모두)
    List<Skill> skills;

    public ICombatant enemy;
    protected GameObject attackEffect;
    protected GameObject damageText;
    protected GameObject healEffect;
    protected GameObject healText;

    protected readonly float RecoveryTick = 5.0f;
    protected readonly int RecoveryTimes = 5;
    protected readonly float RecoveryMult = 0.02f;

    protected int monsterSearchCnt;
    protected readonly int MonsterSearchMax = 5;

    protected HuntingArea curHuntingArea;

    public event HealthBelowZeroEventHandler healthBelowZeroEvent;
    //public event MoveStartedEventHandler moveStartedEvent;

    #region UI
    public Canvas canvas;
    public GameObject hpBar;
    public Slider hpSlider;
    #endregion

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

        this.battleStat = new BattleStat(battleStat);
        this.rewardStat = new RewardStat(rewardStat);
        //stat 초기화
        this.stat = new Stat(stat, this);
        //pathfinder 초기화 // delegate 그대로

        skills = new List<Skill>();
    }

    public void OnEnable()
    {
        base.OnEnable();
        monsterSearchCnt = 0;
        SetUI();
        SkillActivate();
    }

    public void OnDisable()
    {
        base.OnDisable();
        SkillDeactivate();
    }

    #region StateMachine
    protected override void EnterState(State nextState)
    {
        switch (nextState)
        {
            case State.Idle:
#if DEBUG_ADV_STATE
                Debug.Log("Idle");
#endif
                superState = SuperState.Idle;
                Idle();
                //Traveler이므로 무조건 SearchingStructure 부터
                //이외에 체크할거 있으면 여기서
                break;
            case State.SolvingDesire_Wandering:
#if DEBUG_ADV_STATE
                Debug.Log("SolvingDesire_Wandering");
#endif
                superState = SuperState.SolvingDesire_Wandering;
                curCoroutine = StartCoroutine(SolvingDesire_Wandering());
                break;
            case State.SearchingMonster_Wandering:
#if DEBUG_ADV_STATE
                Debug.Log("SearchingMonster_Wandering");
#endif
                superState = SuperState.SearchingMonster_Wandering;
                curCoroutine = StartCoroutine(SearchingMonster_Wandering());
                break;
            case State.SearchingStructure:
#if DEBUG_ADV_STATE
                Debug.Log("SearchingStructure");
#endif
                superState = SuperState.SolvingDesire;
                curCoroutine = StartCoroutine(SearchingStructure());
                break;
            case State.PathFinding:
#if DEBUG_ADV_STATE
                Debug.Log("PathFinding");
#endif
                curCoroutine = StartCoroutine(PathFinding());
                break;
            case State.MovingToDestination:
#if DEBUG_ADV_STATE
                Debug.Log("MovingToDestination");
#endif
                animator.SetBool("MoveFlg", true);
                curCoroutine = StartCoroutine(MoveToDestination());
                break;
            case State.WaitingStructure:
#if DEBUG_ADV_STATE
                Debug.Log("WaitingStructure");
#endif
                destinationPlace.Visit(this);
                break;
            case State.UsingStructure:
#if DEBUG_ADV_STATE
                Debug.Log("UsingStructure");
#endif
                //욕구 감소
                //소지 골드 감소
                UsingStructure();
                break;
            case State.SearchingExit:
#if DEBUG_ADV_STATE
                Debug.Log("SearchingExit");
#endif
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
#if DEBUG_ADV_STATE
                Debug.Log("SearchingHuntingArea");
#endif
                superState = SuperState.SearchingHuntingArea;
                curCoroutine = StartCoroutine(SearchingHuntingArea());
                break;
            case State.EnteringHuntingArea:
                superState = SuperState.EnteringHuntingArea;
                EnteringHuntingArea();
                break;
            case State.SearchingMonster:
#if DEBUG_ADV_STATE
                Debug.Log("SearchingMonster");
#endif
                superState = SuperState.SearchingMonster;
                curCoroutine = StartCoroutine(SearchingMonster());
                break;
            case State.ApproachingToEnemy:
#if DEBUG_ADV_STATE
                Debug.Log("ApproachingToEnemy");
#endif
                animator.SetBool("MoveFlg", true);
                curCoroutine = StartCoroutine(ApproachingToEnemy());
                break;
            case State.InitiatingBattle:
#if DEBUG_ADV_STATE
                Debug.Log("InitiatingBattle");
#endif
                hpBar.GetComponent<HPBar>().Show();
                superState = SuperState.Battle;
                InitiatingBattle();
                break;
            case State.Battle:
#if DEBUG_ADV_STATE
                Debug.Log("Battle");
#endif
                curCoroutine = StartCoroutine(Battle());
                break;
            case State.AfterBattle:
#if DEBUG_ADV_STATE
                Debug.Log("AfterBattle");
#endif
                superState = SuperState.AfterBattle;
                curCoroutine = StartCoroutine(AfterBattle());
                break;
            case State.ExitingHuntingArea:
#if DEBUG_ADV_STATE
                Debug.Log("ExitingHuntingArea");
#endif
                superState = SuperState.ExitingHuntingArea;
                curCoroutine = StartCoroutine(ExitingHuntingArea());
                break;
            case State.PassedOut:
#if DEBUG_ADV_STATE
                Debug.Log("PassedOut");
#endif
                superState = SuperState.PassedOut;
                animator.SetTrigger("DeathFlg");
                PassedOut();
                break;
            case State.SpontaneousRecovery:
#if DEBUG_ADV_STATE
                Debug.Log("SpontaneousRecovery");
#endif
                curCoroutine = StartCoroutine(SpontaneousRecovery());
                break;
            case State.Rescued:
#if DEBUG_ADV_STATE
                Debug.Log("Rescued");
#endif
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
                animator.SetBool("MoveFlg", false);
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
            case State.EnteringHuntingArea:
                break;
            case State.SearchingMonster:
                break;
            case State.ApproachingToEnemy:
                animator.SetBool("MoveFlg", false);
                break;
            case State.InitiatingBattle:
                break;
            case State.Battle:
                break;
            case State.AfterBattle:
                hpBar.GetComponent<HPBar>().Hide();
                break;
            case State.ExitingHuntingArea:
                break;
            case State.PassedOut:
                break;
            case State.SpontaneousRecovery:
                hpBar.GetComponent<HPBar>().Hide();
                animator.SetTrigger("ResurrectionFlg");
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

        switch (superState)
        {
            case SuperState.SearchingMonster:
                curState = State.ApproachingToEnemy;
                break;
            case SuperState.PassedOut:
                curState = State.Rescued;
                break;
            case SuperState.Battle:
                curState = State.ApproachingToEnemy;
                break;
            default: //SearchingHuntingArea, EnteringHuntingArea, ExitingDungeon, ExitingHuntingArea, SearchingMonster_Wandering, SolvingDesire_Wandering, SolvingDesire
                curState = State.MovingToDestination;
                break;
        }
    }

    // 수정요망
    protected override IEnumerator MoveToDestination()
    {
        //길찾기 성공
        destinationTileForMove = destinationTile.childs[Random.Range(0, 4)];
        wayForMove = GetWayTileForMove(pathFinder.GetPath(), destinationTileForMove); // TileForMove로 변환
        // TODO: GetWayForMove로 고치기
        //MoveStartedNotify();
        StartCoroutine(AlignPositionToCurTileForMoveSmoothly());
        yield return curSubCoroutine = StartCoroutine(MoveAnimation(wayForMove)); // 이동 한번에 코루틴으로 처리 // 이동 중지할 일 있으면 StopCoroutine moveAnimation // traveler니까 없을듯?																//순번 or 대기 여부 결정

        switch (superState)
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
            case SuperState.SearchingHuntingArea:
                curState = State.EnteringHuntingArea;
                break;
            case SuperState.EnteringHuntingArea:
                curState = State.SearchingMonster;
                break;
            case SuperState.ExitingDungeon:
                curState = State.SearchingExit;
                break;
            case SuperState.SearchingMonster_Wandering:
                curState = State.SearchingMonster;
                break;
        }
    }

    #region StateMachine
    public void Idle()
    {
        //Debug.Log("GetSpecificDesire() : " + stat.GetHighestDesire());
        if (stat.GetSpecificDesire(stat.GetHighestDesire()).desireValue < 50.0f)
            curState = State.SearchingHuntingArea;
        else
            curState = State.SearchingStructure;
#if DEBUG_ADV
        Debug.Log("Adv.EnterState()");
#endif
    }



    //protected void VisitHuntingGround()
    //{
    //    curHuntingArea = destinationPlace as HuntingArea;
    //    curHuntingArea.EnterAdventurer(this.gameObject);

    //    destinationPlace = null; // 사용 후에는 비워주기.
    //    curState = State.EnteringHuntingArea;
    //}

    // 수정요망
    protected IEnumerator SearchingHuntingArea()
    {
        yield return null;
        // (이 모험가의 level <= 사냥터의 maxLevel)인 사냥터 중 maxLevel이 가장 낮은 걸 찾음.
        destinationPlace = HuntingAreaManager.Instance.FindHuntingArea(battleStat.Level);
#if DEBUG_ADV
        if (destinationPlace == null)
            Debug.Log("dest is null");
        else
            Debug.Log("dest : " + destinationPlace);
#endif
        if (destinationPlace == null)
            curState = State.SearchingExit;
        else
        {
            destinationTile = destinationPlace.GetEntrance();
            // TODO: 이거 destination TileForMove에 뭐 집어넣게 바꿔야
            curState = State.PathFinding;
        }
    }

    protected void EnteringHuntingArea()
    {
        curHuntingArea = destinationPlace as HuntingArea;
        curHuntingArea.EnterAdventurer(this.gameObject);

        destinationPlace = null; // 사용 후에는 비워주기.

        destinationTileForMove = curHuntingArea.FindNearestBlank(curTileForMove);
        destinationTile = destinationTileForMove.GetParent();

        curState = State.PathFinding;
    }

    protected IEnumerator SearchingMonster()
    {
        yield return null;

        enemy = curHuntingArea.FindNearestMonster(this); // 가장 가까운 몬스터 찾기.

        if (enemy == null)
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
            SetDestinationTowardEnemy();

            curState = State.PathFinding;
        }
    }

    //수정요망
    protected IEnumerator SearchingMonster_Wandering()
    {
        // 빈 칸이 없으면?? 수정요망 이걸 막기위해서 사냥터도 출입인원 제한해야 함. 나머진 기다리고.
        destinationTileForMove = curHuntingArea.FindBlanks(1)[0];
        destinationTile = destinationTileForMove.GetParent();

        yield return null; // 있든 없든 별 상관 X

        curState = State.PathFinding;
    }

    protected IEnumerator ExitingHuntingArea()
    {
        destinationTile = curHuntingArea.GetEntrance();
        curHuntingArea.ExitAdventurer(this.gameObject);

        curHuntingArea = null;
        ResetBattleEventHandlers();

        curState = State.PathFinding;
        yield return null;
    }

    protected virtual void PassedOut()
    {
        StopCurActivities();

        ResetBattleEventHandlers();

        //Structure[] tempArr = StructureManager.Instance.FindRescue(this);

        //if (tempArr.Length == 0)
        //{
        curState = State.SpontaneousRecovery;
        //}
        //else
        //{
        //    destinationPlace = tempArr[0];
        //    curState = State.PathFinding;
        //}
    }

    protected IEnumerator Rescued()
    {
        List<TileForMove> temp = GetWay(pathFinder.GetPath());
        // TODO: GetWayForMove로 고치기. 이건 아직 구현안됐으니 별 상관없음

        yield return null; // 구조대 관련 애니메이션 추가해야 함. 수정요망
    }

    protected IEnumerator SpontaneousRecovery()
    {
        float healAmount = 0;
        for (int i = 0; i < RecoveryTimes; i++)
        {
            yield return new WaitForSeconds(RecoveryTick);

            healAmount = battleStat.HealthMax * RecoveryMult;
            battleStat.Health += healAmount;
            DisplayHeal(healAmount);
            //StartCoroutine(RecoveryEffect(battleStat.HealthMax * RecoveryMult));
        }

        curState = State.ExitingHuntingArea;
    }

    protected void ResetBattleEventHandlers()
    {
        // 이벤트 핸들러 초기화
        healthBelowZeroEvent = null;
        //moveStartedEvent = null;
        enemy.healthBelowZeroEvent -= OnEnemyHealthBelowZero;
    }

    #endregion

    public override bool ValidateNextTile(Tile tile)
    {
        return tile.GetPassableAdventurer();
    }

    protected IEnumerator RecoveryEffect(float healingAmount) // 체력 회복 이펙트
    {
        yield return null;
    }

    #region Battle
    protected IEnumerator Charge(List<TileForMove> tileForMoveWay)
    {
        //enemy.AddMoveStartedEventHandler(OnEnemyMoveStarted);

        Direction dir = Direction.DownLeft;
        //FlipX true == Left, false == Right
        Vector3 dirVector;
        float distance, sum = 0.0f;
        int walkCnt = 0;
        const int posCheckRate = 4;

#if DEBUG_CHARGE
        Debug.Log("적: " + enemy + ", 목적지: " + destinationTile);
#endif
        // 적이 이미 누웠거나, 사냥터에서 나갔다면
        if (!ValidatingEnemy(enemy))
        {
            curState = State.AfterBattle;
            yield break;
        }
        // 레인지 검사. 적이 공격 범위 안으로 들어왔을 때.
        if (CheckInRange())
        {
            curState = State.InitiatingBattle;
            yield break;
        }

        // PathFinder에서 받은 경로대로 이동
        for (int i = 0; i < tileForMoveWay.Count - 1; i++)
        {
            //tileForMoveWay[i].SetRecentActor(this);
            //SetCurTile(tileForMoveWay[i].GetParent());
            //SetCurTileForMove(tileForMoveWay[i]);



            // curTileForMove.GetDirectionFromOtherTileForMove(enemy.GetCurTileForMove());
            // 방향에 따른 애니메이션 설정.
            SetAnimDirection(tileForMoveWay[i].GetDirectionFromOtherTileForMove(tileForMoveWay[i + 1]));

            //SetCurTile(tileForMoveWay[tileForMoveWay.Count - 1].GetParent());
            //SetCurTileForMove(tileForMoveWay[tileForMoveWay.Count - 1]);
            //transform.position = tileForMoveWay[i].GetPosition();
            // 이동
            dirVector = tileForMoveWay[i + 1].GetPosition() - tileForMoveWay[i].GetPosition();
            distance = Vector3.Distance(tileForMoveWay[i].GetPosition(), tileForMoveWay[i + 1].GetPosition());
            while (Vector3.Distance(transform.position, tileForMoveWay[i].GetPosition()) < distance / 2)
            {
                yield return null;
                transform.Translate(dirVector * Time.deltaTime);

            }

            // 절반 넘어가면 다음 타일로 위치지정해줌.
            SetCurTile(tileForMoveWay[i + 1].GetParent());
            SetCurTileForMove(tileForMoveWay[i + 1]);

            while (Vector3.Distance(transform.position, tileForMoveWay[i].GetPosition()) < distance)
            {
                yield return null;
                transform.Translate(dirVector * Time.deltaTime);

            }
            sum = 0.0f;
            transform.position = tileForMoveWay[i + 1].GetPosition();

            // 적이 이미 누웠거나, 사냥터에서 나갔다면
            if (!ValidatingEnemy(enemy))
            {
                curState = State.AfterBattle;
                yield break;
            }
            // 레인지 검사. 적이 공격 범위 안으로 들어왔을 때.
            if (CheckInRange())
            {
                curState = State.InitiatingBattle;
                yield break;
            }

            walkCnt++;
            if (walkCnt % posCheckRate == 0 && destinationTileForMove != enemy.GetCurTileForMove())
            {
                break;
            }
        }

        SetDestinationTowardEnemy();
        curState = State.PathFinding;

        //// 모험가가 이미 누웠다면.
        //if (!ValidatingEnemy())
        //{
        //    curState = State.AfterBattle;
        //    yield break;
        //}

        //// 레인지 검사
        //if (CheckInRange())
        //{
        //    curState = State.Battle;
        //    yield break;
        //}
        //else // 목적지 도착했지만 공격 범위 안에 안 들어올 때. 이게 틀림. 새 목적지를 설정해야하는데 하지않고 그냥 PathFinding
        //{
        //    SetDestinationTowardEnemy();
        //    curState = State.PathFinding;
        //}
    }

    protected IEnumerator ApproachingToEnemy()
    {
        wayForMove = GetWayTileForMove(pathFinder.GetPath(), destinationTileForMove);
        StartCoroutine(AlignPositionToCurTileForMoveSmoothly());

        //MoveStartedNotify();
        yield return curSubCoroutine = StartCoroutine(Charge(wayForMove));
    }

    // 전투 시작
    protected void InitiatingBattle()
    {
        SetDestinationTowardEnemy();

        // 적이 공격 범위 안에 있다면 바로 전투.
        if (CheckInRange())
            curState = State.Battle;
        else
            curState = State.PathFinding;
    }

    protected IEnumerator Battle()
    {
        //enemy.healthBelowZeroEvent += OnEnemyHealthBelowZero;

        while (ValidatingEnemy(enemy))
        {
            if (CheckInRange())
                yield return curSubCoroutine = StartCoroutine(Attack());
            else
            {
                curState = State.InitiatingBattle;
                yield break;
            }
        }
        curState = State.AfterBattle;
    }

    protected IEnumerator Attack() // 공격
    {
        //방향 설정
        if (curTileForMove != enemy.GetCurTileForMove())
            SetAnimDirection(curTileForMove.GetDirectionFromOtherTileForMove(enemy.GetCurTileForMove()));
        else
        {
            SetAnimDirection(GetDirectionToPosition(enemy.GetPosition()));
        }
        animator.SetTrigger("AttackFlg");
        animator.SetFloat("AttackSpeed", battleStat.AttackSpeed);
        yield return new WaitForSeconds(0.43f / battleStat.AttackSpeed); // 애니메이션 관련 넣을 것.

        // 어차피 이벤트로 나가는데 필요한지?
        if (!ValidatingEnemy(enemy))
            yield break;

        bool isCrit;
        float calculatedDamage;
        battleStat.CalDamage(out calculatedDamage, out isCrit);

        float actualDamage;
        if (enemy.TakeDamage(this, calculatedDamage, battleStat.PenetrationFixed, battleStat.PenetrationMult, isCrit, out actualDamage))
        {
            DisplayAttackEffect(enemy);
            SkillOnAttack(actualDamage, isCrit, false);
        }

        yield return new WaitForSeconds(0.57f / battleStat.AttackSpeed); // 애니메이션 관련 넣을 것.
        //attackEffect.GetComponent<AttackEffect>().StopEffect();
    }

    public void SetAttackEffect(GameObject input)
    {
        attackEffect = input;
        attackEffect.transform.SetParent(GameObject.Find("EffectPool").transform);
    }

    public void SetHealEffect(GameObject input)
    {
        healEffect = input;
        healEffect.transform.SetParent(transform);
        healEffect.transform.position = new Vector3(transform.position.x, transform.position.y + 0.12f, transform.position.z);
    }

    public void SetDamageText(GameObject input)
    {
        damageText = input;
        damageText.SetActive(false);
        damageText.transform.SetParent(GameObject.Find("EffectPool").transform);
    }
    public void SetHealText(GameObject input)
    {
        healText = input;
        healText.SetActive(false);
        healText.transform.SetParent(GameObject.Find("EffectPool").transform);
    }

    protected void StopCurActivities()
    {
        if (curSubCoroutine != null)
            StopCoroutine(curSubCoroutine);
        if (curCoroutine != null)
            StopCoroutine(curCoroutine);
        curSubCoroutine = null;
    }


    // 죽인 몬스터로부터 보상 획득.
    protected bool GetBattleReward(ICombatant attacker)
    {
        bool levelUp = battleStat.TakeExp(attacker.RewardExp());
        stat.gold += attacker.RewardGold();

        return levelUp;
    }

    // 수정요망
    protected IEnumerator AfterBattle()
    {
        yield return new WaitForSeconds(2.0f);

        // 포션 음용 넣어야한다면 여기에.
        if (battleStat.Health < battleStat.HealthMax / 4) //체력이 25%미만이면 
            curState = State.ExitingHuntingArea;
        else
            curState = State.SearchingMonster;
    }

    // 죽을 때 호출. 이 모험가를 공격대상으로 하고있는 몬스터들에게 알려줌.
    // 몬스터들은 누굴 죽였는지 알 필요 X. 단 전투는 중단.
    public void HealthBelowZeroNotify(ICombatant victim, ICombatant attacker)
    {
        healthBelowZeroEvent?.Invoke(victim, attacker);
    }

    protected bool CheckInRange()
    {
        return (DistanceBetween(curTileForMove, enemy.GetCurTileForMove()) <= battleStat.Range);
    }

    protected void SetDestinationTowardEnemy()
    {
        destinationTile = enemy.GetCurTile();
        destinationTileForMove = enemy.GetCurTileForMove();
    }

    public bool isFighting()
    {
        return superState == SuperState.Battle;
    }

    protected void AddHealthBelowZeroEventHandler(HealthBelowZeroEventHandler newEvent) // 이벤트에 추가된 적 없는 이벤트면 추가.
    {
        if (healthBelowZeroEvent == null)
        {
            healthBelowZeroEvent += newEvent;
            return;
        }

        System.Delegate[] invocations = healthBelowZeroEvent.GetInvocationList();

        bool isNew = true;
        for (int i = 0; i < invocations.Length; i++)
        {
            if (invocations[i].Target == newEvent.Target)
                isNew = false;
        }

        if (isNew)
            healthBelowZeroEvent += newEvent;
    }

    //public void AddMoveStartedEventHandler(MoveStartedEventHandler newEvent)
    //{
    //    if (moveStartedEvent == null)
    //    {
    //        moveStartedEvent += newEvent;
    //        return;
    //    }

    //    System.Delegate[] invocations = moveStartedEvent.GetInvocationList();

    //    bool isNew = true;
    //    for (int i = 0; i < invocations.Length; i++)
    //    {
    //        if (invocations[i].Target == newEvent.Target)
    //            isNew = false;
    //    }

    //    if (isNew)
    //        moveStartedEvent += newEvent;
    //}
    #endregion

    #region ICombatant
    public bool ValidatingEnemy(ICombatant enemy)
    {
        SuperState enemySuperState = enemy.GetSuperState();
        // 적이 살아 있을 때.
        if (enemySuperState != SuperState.Dead)
            return true;
        else
            return false;
    }

    public bool TakeDamage(ICombatant attacker, float damage, float penFixed, float penMult, bool isCrit, out float actualDamage) // 데미지 받기. 이펙트 처리를 위해 isCrit도 받음.
    {
        bool isDodged;

        AddHealthBelowZeroEventHandler(attacker.OnEnemyHealthBelowZero); // 이벤트 리스트에 추가.

        battleStat.TakeDamage(damage, penFixed, penMult, out actualDamage, out isDodged); // 데미지 입음
        StartCoroutine(DisplayHitEffect(actualDamage, isCrit, isDodged));

        SkillOnStruck(actualDamage, isDodged, attacker);
        if (isDodged)
            DisplayDodge();

#if DEBUG_ADV_BATTLE
        Debug.Log(this + "가 " + attacker + "에게 " + actualDamage + "의 피해를 입음."
            + "\n남은 체력 : " + this.battleStat.Health);
#endif

        // 조건?
        if (battleStat.Health <= 0)
        {
            HealthBelowZeroNotify(this, attacker);
            StopCurActivities();
            curState = State.PassedOut;
        }
        else if (superState != SuperState.Battle)
        {
            StopCurActivities();
            animator.SetTrigger("DamageFlg");
            enemy = attacker;
            curState = State.InitiatingBattle;
        }

        return !isDodged;
    }

    // 적이 죽었을 때 호출되는 메서드
    public void OnEnemyHealthBelowZero(ICombatant victim, ICombatant attacker)
    {
        //StopCurActivities();
        //animator.SetTrigger("StopAttackFlg");

        if (attacker == this) // 내가 죽였다면.
        {
#if DEBUG_ADV_BATTLE
            int expBefore = battleStat.CurExp;
            int levelBefore = battleStat.Level;
#endif

            if (GetBattleReward(victim))
                LevelUp();

#if DEBUG_ADV_BATTLE
            Debug.Log(this + "가 몬스터 처치." +
                "\n기존 exp: " + expBefore + ", 현재 exp: " + battleStat.CurExp + ", 요구 exp: " + battleStat.NextExp +
                "\n기존 level: " + levelBefore + ", 현재 level: " + battleStat.Level);
#endif
        }
        // curState = State.AfterBattle;
    }

    private void LevelUp()
    {
        DisplayLevelUp(battleStat.Level);
        rewardStat.SetRewardStatToLevel(battleStat.Level);
    }

    public void DisplayAttackEffect(ICombatant enemy)
    {
        attackEffect.transform.position = new Vector3(enemy.GetPosition().x * 0.9f + transform.position.x * 0.1f, enemy.GetPosition().y * 0.9f + transform.position.y * 0.1f, enemy.GetPosition().z * 0.5f + transform.position.z * 0.5f);
        attackEffect.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 180f));
        attackEffect.GetComponent<AttackEffect>().StartEffect();
    }

    private void DisplayLevelUp(int Level)
    {
        GameObject levelUpEffect = Instantiate((GameObject)Resources.Load("EffectPrefabs/LevelUpEffect"));
        levelUpEffect.transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        levelUpEffect.SetActive(true);
        Debug.Log("LV UP!");
    }

    private void DisplayDamage(float damage)
    {
        GameObject tempDamageText = Instantiate(damageText);
        Vector3 textPos = new Vector3(transform.position.x + Random.Range(-0.05f, 0.05f), transform.position.y + Random.Range(0.0f, 0.1f), transform.position.z);
        tempDamageText.GetComponent<FloatingText>().InitFloatingText(((int)damage).ToString(), textPos);
        //tempDamageText.transform.SetParent(canvas.transform);
        tempDamageText.SetActive(true);
    }

    public void DisplayHeal(float healed)
    {
        GameObject tempHealText = Instantiate(healText);
        Vector3 textPos = new Vector3(transform.position.x + Random.Range(-0.07f, 0.07f), transform.position.y + Random.Range(-0.05f, 0.05f), transform.position.z);
        tempHealText.GetComponent<FloatingText>().InitFloatingText(((int)healed).ToString(), textPos);
        //tempDamageText.transform.SetParent(canvas.transform);
        tempHealText.GetComponent<TextMeshPro>().fontSize = 600;
        tempHealText.SetActive(true);

        healEffect.SetActive(true);
        healEffect.GetComponent<AttackEffect>().StartEffect();
    }

    private void DisplayDodge()
    {
        GameObject tempDamageText = Instantiate(damageText);
        Vector3 textPos = new Vector3(transform.position.x + Random.Range(-0.05f, 0.05f), transform.position.y + Random.Range(0.0f, 0.1f), transform.position.z);
        tempDamageText.GetComponent<FloatingText>().InitFloatingText("Dodged", textPos);
        tempDamageText.GetComponent<RectTransform>().localScale = new Vector3(0.12f, 0.12f, 0.12f);
        tempDamageText.SetActive(true);
    }

    //public void OnEnemyMoveStarted(TileForMove newDest)
    //{
    //    // 이거 고치자. 순간이동하고 상태 자꾸 바뀌고 문제임.
    //    //StopCurActivities();

    //    //destinationTileForMove = newDest;
    //    //destinationTile = newDest.GetParent();

    //    //curState = State.PathFinding;
    //}

    //public void MoveStartedNotify()
    //{
    //    moveStartedEvent?.Invoke(destinationTileForMove);
    //}

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

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public BattleStat GetBattleStat()
    {
        return battleStat;
    }

    public ICombatant GetEnemy()
    {
        return enemy;
    }

    public Transform GetTransform()
    {
        return transform;
    }
    #endregion

    #region UI
    public void SetUI()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        hpBar = (GameObject)Instantiate(Resources.Load("UIPrefabs/Battle/HPSlider"));
        hpSlider = hpBar.GetComponentInChildren<Slider>();
        hpBar.transform.SetParent(canvas.transform);
        hpBar.GetComponent<HPBar>().SetSubject(this);

        hpBar.SetActive(true);
    }
    #endregion

    #region Skill
    protected void AddSkill(Skill skill)
    {
        skills.Add(skill);
        skill.SetOwner(this);
        skill.InitSkill();
        //skill.Activate();
    }
    protected void RemoveSkill(Skill skill)
    {
        skills.Remove(skill);
        skill.Deactivate();
    }
    protected void SkillOnAttack(float actualDamage, bool isCrit, bool isDodged)
    {
        foreach(Skill item in skills)
        {
            item.OnAttack(actualDamage, isCrit, isDodged);
        }
    }
    protected void SkillOnStruck(float actualDamage, bool isDodged, ICombatant attacker)
    {
        foreach(Skill item in skills)
        {
            item.OnStruck(actualDamage, isDodged, attacker);
        }
    }
    protected void SkillActivate()
    {
        foreach (Skill item in skills)
        {
            if(!item.isActive)
                item.Activate();
        }
    }
    protected void SkillDeactivate()
    {
        foreach (Skill item in skills)
        {
            item.Deactivate();
        }
    }
    #endregion
}