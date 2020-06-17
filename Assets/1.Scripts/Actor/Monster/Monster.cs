//#define DEBUG_GETWAY
//#define DEBUG_TELEPORT
//#define DEBUG_ADV_BATTLE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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
    // 대기시간에 움직이는지. 보스면 false
    public bool canWander = true;
    protected Coroutine curCoroutine;
    protected Coroutine curSubCoroutine;

    protected Structure destinationStructure;
    protected Structure[] structureListByPref;

    private CombatArea habitat;
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
    //public event MoveStartedEventHandler moveStartedEvent;
    protected GameObject attackEffect;
    protected GameObject damageText;
    protected GameObject healEffect;
    protected GameObject healText;
    protected GameObject buffEffect;
    protected GameObject debuffEffect;

    // 스킬들(아이템, 고유능력 등 모두)
    Dictionary<string, Skill> skills;
    // 버프/디버프 목록
    Dictionary<string, TemporaryEffect> temporaryEffects;
    Coroutine refreshingTempEffectCoroutine;

    #endregion

    #region UI
    public Canvas canvas;
    public GameObject hpBar;
    public Slider hpSlider;
    #endregion

    #region initialization
    // Use this for initialization

    #region 수정!
    // 몬스터 초기화
    public void InitMonster(int monsterNum, BattleStat battleStat, RewardStat rewardStat, bool canWanderIn = true)
    {
        // 이동가능한 타일인지 확인할 delegate 설정.
        pathFinder.SetValidateTile(ValidateNextTile);
        SetPathFindEvent();

        this.monsterNum = monsterNum;
        this.battleStat = new BattleStat(battleStat);
        this.rewardStat = new RewardStat(rewardStat);
        this.canWander = canWanderIn;
        //stat 초기화
        //pathfinder 초기화 // delegate 그대로
        SetDefaultEffects();
        skills = new Dictionary<string, Skill>();
        temporaryEffects = new Dictionary<string, TemporaryEffect>();
    }

    // 몬스터에서 받아서 초기화.
    public void InitMonster(Monster sample)
    {
        monsterNum = sample.monsterNum;
        canWander = sample.canWander;

        battleStat = new BattleStat(sample.battleStat);
        rewardStat = new RewardStat(sample.rewardStat);

        //SetDamageText(Instantiate(sample.damageText));
        SetAttackEffect(Instantiate(sample.attackEffect));
        SetDefaultEffects();

        skills = new Dictionary<string, Skill>();
        temporaryEffects = new Dictionary<string, TemporaryEffect>();
    }

    public void OnEnable()
    {
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
        refreshingTempEffectCoroutine = StartCoroutine(RefreshTemporaryEffects());

        SetUI();
    }
    #endregion

    public void SetHabitat(CombatArea input)
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
#if DEBUG_MOB_STATE
                Debug.Log("Idle");
#endif
                superState = SuperState.Idle;
                curState = State.Wandering;
                //Traveler이므로 무조건 SearchingStructure 부터
                //이외에 체크할거 있으면 여기서
                break;
            case State.Wandering:
#if DEBUG_MOB_STATE
                Debug.Log("Wandering");
#endif
                superState = SuperState.Wandering;
                //Wandering();
                curCoroutine = StartCoroutine(Wandering());
                break;
            case State.PathFinding:
#if DEBUG_MOB_STATE
                Debug.Log("PF");
#endif
                curCoroutine = StartCoroutine(PathFinding());
                break;
            case State.MovingToDestination:
#if DEBUG_MOB_STATE
                Debug.Log("MTS");
#endif
                animator.SetBool("MoveFlg", true);
                curCoroutine = StartCoroutine(MoveToDestination());
                break;
            case State.ApproachingToEnemy:
#if DEBUG_MOB_STATE
                Debug.Log("Approaching to the enemy.");
#endif
                animator.SetBool("MoveFlg", true);
                curCoroutine = StartCoroutine(ApproachingToEnemy());
                break;
            case State.InitiatingBattle:
#if DEBUG_MOB_STATE
                Debug.Log("Initiating Battle");
#endif
                hpBar.GetComponent<HPBar>().Show();
                superState = SuperState.Battle;
                InitiatingBattle();
                break;
            case State.Battle:
#if DEBUG_MOB_STATE
                Debug.Log("Battle");
#endif
                curCoroutine = StartCoroutine(Battle());
                break;
            case State.AfterBattle:
#if DEBUG_MOB_STATE
                Debug.Log("After Battle");
#endif
                superState = SuperState.AfterBattle;
                curCoroutine = StartCoroutine(AfterBattle());
                break;
            case State.Dead:
#if DEBUG_MOB_STATE
                Debug.Log("Dead");
#endif
                superState = SuperState.Dead;
                animator.SetTrigger("DeathFlg");
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
                animator.SetBool("MoveFlg", false);
                break;
            case State.ApproachingToEnemy:
                animator.SetBool("MoveFlg", false);
                break;
            case State.InitiatingBattle:
                break;
            case State.Battle:
                hpBar.GetComponent<HPBar>().Hide();
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
        yield return new WaitForSeconds(Random.Range(2.0f, 4.0f));

        // 목적지(빈 타일) 찾기.
        //destinationTileForMove = habitat.FindBlanks(1)[0];

        if (canWander)
        {
            // WARNING 이거 부하 있을 수 있음.
            destinationTileForMove = habitat.FindNearestBlank(curTileForMove);

            if (destinationTileForMove == null)
            {
                curState = State.Wandering;
                yield break;
            }

            destinationTile = destinationTileForMove.GetParent();
            curState = State.PathFinding;
        }
        else
        {
            curState = State.Idle;
        }
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
        wayForMove = GetWayTileForMove(pathFinder.GetPath(), destinationTileForMove); // TileForMove로 변환
        //animator.SetBool("MoveFlg", true); // animation 이동으로
        //MoveStartedNotify();
        StartCoroutine(AlignPositionToCurTileForMoveSmoothly());

        yield return curSubCoroutine = StartCoroutine(MoveAnimation(wayForMove)); // 이동 한번에 코루틴으로 처리 // 이동 중지할 일 있으면 StopCoroutine moveAnimation												//순번 or 대기 여부 결정

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

    
#endregion

#region Battle
    protected IEnumerator Charge(List<TileForMove> tileForMoveWay)
    {
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

            // 방향에 따른 애니메이션 설정.
            SetAnimDirection(tileForMoveWay[i].GetDirectionFromOtherTileForMove(tileForMoveWay[i + 1]));

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
    }

    protected IEnumerator ApproachingToEnemy()
    {
        wayForMove = GetWayTileForMove(pathFinder.GetPath(), destinationTileForMove);

        //MoveStartedNotify();
        StartCoroutine(AlignPositionToCurTileForMoveSmoothly());
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
        animator.SetFloat("AttackSpeed", battleStat.AttackSpeed); // 공격 속도에 맞춰 애니메이션.
        yield return new WaitForSeconds(0.43f / battleStat.AttackSpeed);

        // 어차피 이벤트로 나가는데 필요한지?
        if (!ValidatingEnemy(enemy))
        {
            yield break;
        }

        bool isCrit;
        float calculatedDamage;
        battleStat.CalDamage(out calculatedDamage, out isCrit);

        float actualDamage;
        if(enemy.TakeDamage(this, calculatedDamage, battleStat.PenetrationFixed, battleStat.PenetrationMult, isCrit, out actualDamage))
        {
            attackEffect.transform.position = new Vector3(enemy.GetPosition().x * 0.9f + transform.position.x * 0.1f, enemy.GetPosition().y * 0.9f + transform.position.y * 0.1f, enemy.GetPosition().z * 0.5f + transform.position.z * 0.5f);
            attackEffect.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 180f));
            attackEffect.GetComponent<AttackEffect>().StartEffect();
        }

        yield return new WaitForSeconds(0.57f / battleStat.AttackSpeed);
        attackEffect.GetComponent<AttackEffect>().StopEffect();
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

    public void SetBuffEffect(GameObject input)
    {
        buffEffect = input;
        buffEffect.transform.SetParent(transform);
        buffEffect.transform.position = new Vector3(transform.position.x, transform.position.y + 0.08f, transform.position.z);
    }

    public void SetDebuffEffect(GameObject input)
    {
        debuffEffect = input;
        debuffEffect.transform.SetParent(transform);
        debuffEffect.transform.position = new Vector3(transform.position.x, transform.position.y + 0.08f, transform.position.z);
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

    public void SetDefaultEffects()
    {
        SetDamageText((GameObject)Instantiate(Resources.Load("UIPrefabs/Battle/DamageText")));
        SetHealText((GameObject)Instantiate(Resources.Load("UIPrefabs/Battle/HealText")));
        SetHealEffect((GameObject)Instantiate(Resources.Load("EffectPrefabs/Default_HealEffect")));
        SetBuffEffect((GameObject)Instantiate(Resources.Load("EffectPrefabs/Default_BuffEffect")));
        SetDebuffEffect((GameObject)Instantiate(Resources.Load("EffectPrefabs/Default_DebuffEffect")));
    }

    protected void StopCurActivities()
    {
        if (curSubCoroutine != null)
            StopCoroutine(curSubCoroutine);
        if (curCoroutine != null)
            StopCoroutine(curCoroutine);
        curSubCoroutine = null;
    }

    protected void GetBattleReward()
    {

    }

    protected IEnumerator AfterBattle() // 전투 끝나고 체력회복. 가만히 서서 회복함. 기본적으로 3초.
    {
        while (battleStat.Health < battleStat.HealthMax) // float 비교연산인데 문제 안 생기는지. 수정요망.
        {
            yield return new WaitForSeconds(RecoveryTick);
            battleStat.Health += battleStat.HealthMax * (RecoveryTick / RecoveryTimer);
            // 체력 회복 이펙트도 필요.
        }

        battleStat.Health = battleStat.HealthMax; // 체력을 최대체력으로 맞춰줌.

        curState = State.Idle;
    }

    protected IEnumerator Dead()
    {
        StopCurActivities();
        ResetBattleEventHandlers();
        ClearTemporaryEffects();

        yield return new WaitForSeconds(DecayTimer);
        corpseDecayEvent?.Invoke(index);
    }

    protected void ResetBattleEventHandlers()
    {
        // 이벤트 핸들러 초기화
        healthBelowZeroEvent = null;
        //moveStartedEvent = null;
        if(enemy != null)
            enemy.RemoveHealthBelowZeroEventHandler(OnEnemyHealthBelowZero);
    }

    // 죽을 때 호출. 이 몬스터를 공격대상으로 하고있는 모험가들에게 알려줌.
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
        for(int i = 0; i < invocations.Length; i++)
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
        // 적이 사냥터 내에 있으며 살아 있을 때.
        if (enemySuperState == SuperState.Battle || enemySuperState == SuperState.SearchingMonster
            || enemySuperState == SuperState.AfterBattle || enemySuperState == SuperState.ExitingHuntingArea
            || enemySuperState == SuperState.BossBattle)
            return true;
        else
            return false;
    }

    public bool TakeDamage(ICombatant attacker, float damage, float penFixed, float penMult, bool isCrit, out float actualDamage) // 데미지 받기. 이펙트 처리를 위해 isCrit도 받음.
    {
        bool isDodged;

        AddHealthBelowZeroEventHandler(attacker.OnEnemyHealthBelowZero); // 이벤트 리스트에 추가.

        battleStat.TakeDamage(damage, penFixed, penMult, out actualDamage, out isDodged);
        StartCoroutine(DisplayHitEffect(actualDamage, isCrit, isDodged));

        if (isDodged)
            DisplayDodge();
        else
            DisplayDamage(actualDamage);


#if DEBUG_ADV_BATTLE
        Debug.Log(this + "가 " + attacker + "에게 " + actualDamage + "의 피해를 입음."
            + "\n남은 체력 : " + this.battleStat.Health);
#endif
        //Debug.Log("방어력 : " + battleStat.Defence);

        // 조건?
        if (battleStat.Health <= 0)
        {
            HealthBelowZeroNotify(this, attacker);
            curState = State.Dead;
        }
        else if (IsInBattle() == false)
        {
            StopCurActivities();
            animator.SetTrigger("DamageFlg");
            //StartCoroutine(AlignPositionToCurTileForMoveSmoothly());
            enemy = attacker;
            curState = State.InitiatingBattle;
        }

        return !isDodged;
    }

    public bool IsInBattle()
    {
        return superState == SuperState.Battle;
    }

    private void DisplayDamage(float damage)
    {
        GameObject tempDamageText = Instantiate(damageText);
        Vector3 textPos = new Vector3(transform.position.x + Random.Range(-0.05f, 0.05f), transform.position.y + Random.Range(0.0f, 0.1f), transform.position.z);
        tempDamageText.GetComponent<FloatingText>().InitFloatingText(((int)damage).ToString(), textPos);
        //tempDamageText.transform.SetParent(canvas.transform);
        tempDamageText.SetActive(true);
    }

    private void DisplayDodge()
    {
        GameObject tempDamageText = Instantiate(damageText);
        Vector3 textPos = new Vector3(transform.position.x + Random.Range(-0.05f, 0.05f), transform.position.y + Random.Range(0.0f, 0.1f), transform.position.z);
        tempDamageText.GetComponent<FloatingText>().InitFloatingText("Dodged", textPos);
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

    public void DisplayBuff()
    {
        buffEffect.SetActive(true);
        buffEffect.GetComponent<AttackEffect>().StartEffect();
    }

    public void DisplayDebuff()
    {
        debuffEffect.SetActive(true);
        debuffEffect.GetComponent<AttackEffect>().StartEffect();
    }

    public void OnEnemyHealthBelowZero(ICombatant victim, ICombatant attacker)
    {
        //StopCurActivities();
        //if (attackerIndex == index)
        //{
        //    GetBattleReward();// 보상 받기. 몬스터는 없음   
        //}
        //enemy = null;
        //curSubCoroutine = null;

        //curState = State.AfterBattle;
    }

    //public void OnEnemyMoveStarted(TileForMove newDest)
    //{
    //    //StopCurActivities();

    //    //destinationTileForMove = newDest;
    //    //destinationTile = newDest.GetParent();

    //    //StopCoroutine(curCoroutine);
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

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void RemoveHealthBelowZeroEventHandler(HealthBelowZeroEventHandler healthBelowZeroEventHandler)
    {
        if (healthBelowZeroEvent != null)
            healthBelowZeroEvent -= healthBelowZeroEventHandler;
    }

    public IEnumerator RefreshTemporaryEffects()
    {
        while (true)
        {
            yield return new WaitForSeconds(SkillConsts.TICK_TIME);

            foreach(string key in temporaryEffects.Keys.ToList())
            {
                if(temporaryEffects[key].Refresh())
                      RemoveTemporaryEffect(temporaryEffects[key]);
            }

            //int idx = 0;
            //while (idx < temporaryEffects.Count)
            //{
            //    if (temporaryEffects[idx].Refresh())
            //        RemoveTemporaryEffect(temporaryEffects[idx]);
            //    else
            //        idx++;
            //}
        }
    }

    public void ClearTemporaryEffects()
    {
        foreach (string key in temporaryEffects.Keys.ToList())
            temporaryEffects[key].RemoveEffect();
        temporaryEffects.Clear();

        //foreach (TemporaryEffect effect in temporaryEffects)
        //    effect.RemoveEffect();
        //temporaryEffects.Clear();
    }

    public void RemoveTemporaryEffect(TemporaryEffect toBeRemoved)
    {
        if (temporaryEffects.ContainsKey(toBeRemoved.name))
        {
            toBeRemoved.ResetTimer(); // 재활용할 수 있으니 리셋.
            toBeRemoved.ResetStack(); // 스택도 여기서 리셋

            toBeRemoved.RemoveEffect();
            temporaryEffects.Remove(toBeRemoved.name);
        }
    }

    public void AddTemporaryEffect(TemporaryEffect toBeAdded)
    {
        if (!temporaryEffects.ContainsKey(toBeAdded.name))
        {
            temporaryEffects.Add(toBeAdded.name, toBeAdded);
            toBeAdded.SetSubject(this);
            toBeAdded.ApplyEffect();
        }
        else
            toBeAdded.StackUp();
    }

    public void AddSkill(string key)
    {
        if (skills.ContainsKey(key))
            return; // 이미 같은 종류 있으면 그냥 리턴. 같은 스킬 중복 불가.

        skills.Add(key, SkillFactory.CreateSkill(gameObject, key));
        skills[key].SetOwner(this);
        skills[key].InitSkill();
        //skill.Activate();
    }

    public void RemoveSkill(string key)
    {
        if (!skills.ContainsKey(key))
            return;

        skills[key].Deactivate();
        skills.Remove(key);
    }

    protected void SkillBeforeAttack()
    {
        foreach (Skill item in skills.Values)
        {
            item.BeforeAttack();
        }
    }
    protected void SkillAfterAttack()
    {
        foreach (Skill item in skills.Values)
        {
            item.AfterAttack();
        }
    }
    protected void SkillOnAttack(float actualDamage, bool isCrit, bool isDodged)
    {
        foreach (Skill item in skills.Values)
        {
            item.OnAttack(actualDamage, isCrit, isDodged);
        }
    }
    protected void SkillOnStruck(float actualDamage, bool isDodged, ICombatant attacker)
    {
        foreach (Skill item in skills.Values)
        {
            item.OnStruck(actualDamage, isDodged, attacker);
        }
    }
    protected void SkillActivate()
    {
        foreach (Skill item in skills.Values)
        {
            if (!item.isActive)
                item.Activate();
        }
    }
    protected void SkillDeactivate()
    {
        foreach (Skill item in skills.Values)
        {
            item.Deactivate();
        }
    }

    public void HealFullHealth(bool displayEffect)
    {
        if (displayEffect)
            DisplayHeal(battleStat.Heal(battleStat.HealthMax));
        else
            battleStat.Heal(battleStat.HealthMax);
    }

    //public void RemoveTemporaryEffect(TemporaryEffect toBeRemoved)
    //{
    //    if (temporaryEffects.Contains(toBeRemoved))
    //    {
    //        toBeRemoved.ResetTimer(); // 재활용할 수 있으니 리셋.
    //        toBeRemoved.ResetStack(); // 스택도 여기서 리셋

    //        toBeRemoved.RemoveEffect();
    //        temporaryEffects.Remove(toBeRemoved);
    //    }
    //}

    //public void AddTemporaryEffect(TemporaryEffect toBeAdded)
    //{
    //    if (!temporaryEffects.Contains(toBeAdded))
    //    {
    //        temporaryEffects.Add(toBeAdded);
    //        toBeAdded.SetSubject(this);
    //        toBeAdded.ApplyEffect();
    //    }
    //    else
    //        toBeAdded.StackUp();
    //}
    #endregion

    #region UI
    public void SetUI()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        hpBar = (GameObject)Instantiate(Resources.Load("UIPrefabs/Battle/HPSlider_Mob"));
        hpSlider = hpBar.GetComponentInChildren<Slider>();
        hpBar.transform.SetParent(canvas.transform);
        hpBar.GetComponent<HPBar>().SetSubject(this);

        hpBar.SetActive(true);
        damageText.transform.SetParent(canvas.transform);
    }
    #endregion
}
