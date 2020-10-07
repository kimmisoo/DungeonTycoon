//#define DEBUG_ITEM
//#define DEBUG_SPADV_STATE
//#define DEBUG_BOSSPHASE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAdventurer : Adventurer
{
    private Item weapon, armor, accessory1, accessory2;

    public BossArea curBossArea;

    private const int ACCESSORY_CAPACITY = 2;
    private const int BOSSRAID_CALL_PERIOD = 15;
    private int bossRaidCallCnt = 0;
    public bool willBossRaid = false;

    #region SaveLoad
    public string nameKey;
    #endregion

    ////Skill uniqueSkill;
    //public void InitSpecialAdventurer(Stat stat, BattleStat battleStat, RewardStat rewardStat, string name)
    //{
    //    base.InitAdventurer(stat, battleStat, rewardStat);
    //    AddSkill(name);
    //}
    //Skill uniqueSkill;
    public void InitSpecialAdventurer(BattleStat battleStat, RewardStat rewardStat, string name)
    {
        base.InitAdventurer(battleStat, rewardStat);
        AddSkill(name);
		//SetPathFindEventSpecialAdventurer();

	}

    //Skill uniqueSkill;
    public void InitSpecialAdventurer(StatData stat, BattleStat battleStat, RewardStat rewardStat, string name)
    {
        base.InitAdventurer(stat, battleStat, rewardStat);
        AddSkill(name);
		//SetPathFindEventSpecialAdventurer();

	}

    public void OnEnable()
    {
        base.OnEnable();
        monsterSearchCnt = 0;
        //SetPathFindEventSpecialAdventurer();

        // 플레이어가 선택하기 전에는 공용이벤트 일단 구독해놓음.
        GameManager.Instance.BossRaidCallEventHandler += OnBossRaidCall;
#if DEBUG_ITEM
        ItemManager.Instance.setItemCategory("Armor");
        ItemManager.Instance.setItemIndex(22);

        Debug.Log("[OnEnable] Def before : " + battleStat.Defence + ", " + "Hp before : " + battleStat.HealthMax);
        EquipArmor(ItemManager.Instance.CreateItem());
        Debug.Log("[OnEnable] Def after : " + battleStat.Defence + ", " + "Hp after : " + battleStat.HealthMax);

        //ItemManager.Instance.setItemCategory("Accessory");
        //ItemManager.Instance.setItemIndex(22);

        //Debug.Log("[OnEnable] Before Atk : " + battleStat.Attack + ", AtkSpd : " + battleStat.AttackSpeed + ", CritChance : " + battleStat.CriticalChance + ", PenFixed : " + battleStat.PenetrationFixed + ", PenMult : " + battleStat.PenetrationMult);
        //EquipAccessory1(ItemManager.Instance.CreateItem());
        //Debug.Log("[OnEnable] After Atk : " + battleStat.Attack + ", AtkSpd : " + battleStat.AttackSpeed + ", CritChance : " + battleStat.CriticalChance + ", PenFixed : " + battleStat.PenetrationFixed + ", PenMult : " + battleStat.PenetrationMult);
#endif
    }

    public void OnDisable()
    {
        //uniqueSkill.Deactivate();
    }

    #region StateMachine
    protected override void EnterState(State nextState)
    {
        if (willBossRaid)
        {
            superState = SuperState.TeleportToBossArea;
            state = State.TeleportToBossArea;
            curCoroutine = StartCoroutine(TeleportToBossArea());
        }
        else
        {
            switch (nextState)
            {
                case State.Idle:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "Idle");
#endif
                    superState = SuperState.Idle;
                    curCoroutine = StartCoroutine(Idle());
                    //Traveler이므로 무조건 SearchingStructure 부터
                    //이외에 체크할거 있으면 여기서
                    break;
                case State.SolvingDesire_Wandering:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "SolvingDesire_Wandering");
#endif
                    superState = SuperState.SolvingDesire_Wandering;
                    curCoroutine = StartCoroutine(SolvingDesire_Wandering());
                    break;
                case State.SearchingMonster_Wandering:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "SearchingMonster_Wandering");
#endif
                    superState = SuperState.SearchingMonster_Wandering;
                    curCoroutine = StartCoroutine(SearchingMonster_Wandering());
                    break;
                case State.SearchingStructure:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "SearchingStructure");
#endif
                    superState = SuperState.SolvingDesire;
                    curCoroutine = StartCoroutine(SearchingStructure());
                    break;
                case State.PathFinding:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "PathFinding");
#endif
                    curCoroutine = StartCoroutine(PathFinding());
                    break;
                case State.MovingToDestination:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "MovingToDestination");
#endif
                    animator.SetBool("MoveFlg", true);
                    curCoroutine = StartCoroutine(MoveToDestination());
                    break;
                case State.WaitingStructure:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "WaitingStructure");
#endif
                    destinationPlace.Visit(this);
                    break;
                case State.UsingStructure:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "UsingStructure");
#endif
                    //욕구 감소
                    //소지 골드 감소
                    UsingStructure();
                    break;
                case State.SearchingExit:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "SearchingExit");
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
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "SearchingHuntingArea");
#endif
                    superState = SuperState.SearchingHuntingArea;
                    curCoroutine = StartCoroutine(SearchingHuntingArea());
                    break;
                case State.EnteringHuntingArea:
                    superState = SuperState.EnteringHuntingArea;
                    EnteringHuntingArea();
                    break;
                case State.SearchingMonster:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "SearchingMonster");
#endif
                    superState = SuperState.SearchingMonster;
                    curCoroutine = StartCoroutine(SearchingMonster());
                    break;
                case State.ApproachingToEnemy:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "ApproachingToEnemy");
#endif
                    animator.SetBool("MoveFlg", true);
                    curCoroutine = StartCoroutine(ApproachingToEnemy());
                    break;
                case State.InitiatingBattle:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "InitiatingBattle");
#endif
                    superState = SuperState.Battle;
                    InitiatingBattle();
                    break;
                case State.Battle:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "Battle");
#endif
                    ShowBattleUI();
                    curCoroutine = StartCoroutine(Battle());
                    break;
                case State.AfterBattle:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "AfterBattle");
#endif
                    superState = SuperState.AfterBattle;
                    curCoroutine = StartCoroutine(AfterBattle());
                    break;
                case State.ExitingHuntingArea:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "ExitingHuntingArea");
#endif
                    superState = SuperState.ExitingHuntingArea;
                    curCoroutine = StartCoroutine(ExitingHuntingArea());
                    break;
                case State.PassedOut:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "PassedOut");
#endif
                    superState = SuperState.PassedOut;
                    animator.SetTrigger("DeathFlg");
                    PassedOut();
                    break;
                case State.SpontaneousRecovery:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "SpontaneousRecovery");
#endif
                    curCoroutine = StartCoroutine(SpontaneousRecovery());
                    break;
                case State.Rescued:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "Rescued");
#endif
                    curCoroutine = StartCoroutine(Rescued());
                    break;
                // SpAdv 전용
                case State.TeleportToBossArea:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "SearchingBossArea");
#endif
                    superState = SuperState.TeleportToBossArea;
                    curCoroutine = StartCoroutine(TeleportToBossArea());
                    break;
                case State.WaitingOtherSpecialAdvs:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "WaitingOtherSpecialAdvs");
#endif
                    superState = SuperState.WaitingOtherSpecialAdvs;
                    WaitingOtherSpecialAdvs();
                    break;
                case State.StartingSkirmish:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "StartingSkirmish");
#endif
                    superState = SuperState.Skirmish;
                    curCoroutine = StartCoroutine(StartingSkirmish());
                    break;
                case State.MatchWon:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "MatchWon");
#endif
                    MatchWon();
                    break;
                case State.WaitingOtherMatch:
                    animator.SetBool("isCelebrating", true);
                    //WaitingOtherMatch();
                    break;
                case State.SkirmishDefeated:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "SkirmishDefeated");
#endif
                    superState = SuperState.SkirmishDefeated;
                    animator.SetTrigger("DeathFlg");
                    SkirmishDefeated();
                    break;
                case State.SkirmishWon:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "SkirmishWon");
#endif
                    animator.SetBool("isCelebrating", true);
                    superState = SuperState.SkirmishWon;
                    curCoroutine = StartCoroutine(SkirmishWon());
                    break;
                case State.EnteringBossArea:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "EnteringBossArea");
#endif
                    superState = SuperState.EnteringBossArea;
                    EnteringBossArea();
                    break;
                case State.StartingBossBattle:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "StartingBossBattle");
#endif
                    superState = SuperState.BossBattle;
                    StartingBossBattle();
                    break;
                case State.BossBattleWon:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "BossBattleWon");
#endif
                    superState = SuperState.BossBattleWon;
                    BossBattleWon();
                    break;
                case State.BailOut:
#if DEBUG_SPADV_STATE
                    Debug.Log(name + " : " + "BailOut");
#endif
                    animator.SetTrigger("DeathFlg");
                    superState = SuperState.BailOut;
                    curCoroutine = StartCoroutine(BailOut());
                    break;
                case State.None:
                    curState = State.Idle;
                    break;
            }
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
                HideBattleUI();
                break;
            case State.AfterBattle:
                break;
            case State.ExitingHuntingArea:
                break;
            case State.PassedOut:
                break;
            case State.SpontaneousRecovery:
                HideBattleUI();
                animator.SetTrigger("ResurrectionFlg");
                break;
            case State.Rescued:
                break;
            // SpAdv 전용
            case State.TeleportToBossArea:
                break;
            case State.StartingSkirmish:
                break;
            case State.MatchWon:
                break;
            case State.WaitingOtherMatch:
                animator.SetBool("isCelebrating", false);
                break;
            case State.SkirmishDefeated:
                animator.SetTrigger("ResurrectionFlg");
                break;
            case State.SkirmishWon:
                animator.SetBool("isCelebrating", false);
                break;
            case State.EnteringBossArea:
                break;
            case State.StartingBossBattle:
                break;
            case State.BossBattleWon:
                break;
            case State.BailOut:
                animator.SetTrigger("ResurrectionFlg");
                break;
            case State.None:
                break;
        }
    }
    #endregion

    #region Items
    public void EquipWeapon(Item item)
    {
        if (weapon != null)
            weapon.RemoveItemEffects();

        if (item == null)
            weapon = item;
        else if (item.ItemType == ItemType.Weapon)
        {
            weapon = item;
            weapon.SetOwner(this);
            weapon.ApplyItemEffects();
        }
    }

    public void EquipArmor(Item item)
    {
        if (armor != null)
            armor.RemoveItemEffects();

        if (item == null)
            armor = item;
        else if (item.ItemType == ItemType.Armor)
        {
            //Debug.Log("Armor equiped.");
            armor = item;
            armor.SetOwner(this);
            armor.ApplyItemEffects();
        }
    }

    public void EquipAccessory1(Item item)
    {
        if (accessory1 != null)
            accessory1.RemoveItemEffects();

        if (item == null)
            accessory1 = item;
        else if (item.ItemType == ItemType.Accessory)
        {
            accessory1 = item;
            accessory1.SetOwner(this);
            accessory1.ApplyItemEffects();
        }
    }

    public void EquipAccessory2(Item item)
    {
        if (accessory2 != null)
            accessory2.RemoveItemEffects();

        if (item == null)
            accessory2 = item;
        else if (item.ItemType == ItemType.Accessory)
        {
            accessory2 = item;
            accessory2.SetOwner(this);
            accessory2.ApplyItemEffects();
        }
    }

    #endregion

    #region Exclusive Contract
    public void SignExclusiveContract()
    {
        // 공통 이벤트에서 구독 해제
        GameManager.Instance.BossRaidCallEventHandler -= OnBossRaidCall;
        GameManager.Instance.PlayerAcceptedRaidEventHandler += OnPlayerRaidOrder;
        GameManager.Instance.PlayerRefusedRaidEventHandler += OnPlayerRaidRefusal;
    }
    #endregion

    #region BossBattle
    public void OnBossRaidCall()
    {
        if (!IsParticipatedBossRaid())
        {
            if (CombatAreaManager.Instance.FindBossArea().ChallengeLevel <= battleStat.Level)
            {
                //StopCurActivities();
                //curState = State.TeleportToBossArea;
                //StartCoroutine(ParticipateInRaid());
                ScheduleToBossRaid();
            }
            else
                GameManager.Instance.SpAdvResponsed(false, this);
        }
    }

    public void OnPlayerRaidOrder()
    {
        //StopCurActivities();
        //curState = State.TeleportToBossArea;
        //StartCoroutine(ParticipateInRaid());
        ScheduleToBossRaid();
    }

    public void ScheduleToBossRaid()
    {
        willBossRaid = true;
        if (curState == State.MovingToDestination)
            InterruptMoving();
    }

    public void OnPlayerRaidRefusal()
    {
        GameManager.Instance.SpAdvResponsed(false, this);
    }

    private IEnumerator ParticipateInRaid()
    {
        // 전투 종료까지 기다리기.
        yield return null;
        //while (superState == SuperState.Battle)
        //{
        //    yield return new WaitForSeconds(SkillConsts.TICK_TIME);
        //}
        while (curState == State.PathFinding)
        {
            yield return new WaitForSeconds(SkillConsts.TICK_TIME);
        }

        ResetCurHuntingArea();
        ResetBattleParams();
        StopCurActivities();
        ResetDestinations();

        //Debug.Log(stat.name + " 보스 레이드 참여.");
        curState = State.TeleportToBossArea;
    }

    private void InterruptMoving()
    {
        //ResetCurHuntingArea();
        //ResetBattleParams();
        StopCurActivities();
        //ResetDestinations();

        curState = State.TeleportToBossArea;
    }

    //private void TeleportToBossArea(Place bossArea)
    //{
    //    Vector3 originPos = transform.position;
    //    // 탈출
    //    curTile = bossArea.GetEntrance();
    //    curTileForMove = curTile.GetChild((Random.Range(0, 4)));

    //    // 텔레포트 이펙트 및 실제 위치 이동
    //    AlignPositionToCurTileForMove();
    //    Vector3 destPos = transform.position;

    //    DisplayTeleportEffect(originPos, destPos);
    //}

    private bool IsParticipatedBossRaid()
    {
        // 그 외의 SuperState는 이후 상황에서만 나오니 제외.
        return willBossRaid || superState == SuperState.TeleportToBossArea || superState == SuperState.WaitingOtherSpecialAdvs;
    }
    #endregion

    #region State Implements
    protected override IEnumerator PathFinding()
    {
        yield return curSubCoroutine = StartCoroutine(pathFinder.Moves(curTile, destinationTile));

        if(pathFinder.PathFinded)
        {
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
                case SuperState.BossBattle:
                    curState = State.ApproachingToEnemy;
                    break;
                case SuperState.Skirmish:
                    curState = State.ApproachingToEnemy;
                    break;
                default: //SearchingHuntingArea, EnteringHuntingArea, ExitingDungeon, ExitingHuntingArea, SearchingMonster_Wandering, SolvingDesire_Wandering, SolvingDesire, EnteringBossArea
                    curState = State.MovingToDestination;
                    break;
            }
        }
        else
        {
            switch (superState)
            {
                case SuperState.SearchingMonster:
                    curState = State.SearchingMonster;
                    break;
                case SuperState.Battle:
                    curState = State.PathFinding;
                    break;
                case SuperState.BossBattle:
                    curState = State.PathFinding;
                    break;
                case SuperState.Skirmish:
                    curState = State.PathFinding;
                    break;
                default:
                    curState = State.Idle;
                    break;
            }
        }
    }
	public void PathFindSuccessSpecialAdventurer()
	{
		StartCoroutine(CoroutinePathFindSuccessSpecialAdventurer());
	}
	IEnumerator CoroutinePathFindSuccessSpecialAdventurer()
	{
        Debug.Log("PFSuccess Coroutine Start : " + stat.actorName);
		yield return null;
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
			case SuperState.BossBattle:
				curState = State.ApproachingToEnemy;
				break;
			case SuperState.Skirmish:
				curState = State.ApproachingToEnemy;
				break;
			default: //SearchingHuntingArea, EnteringHuntingArea, ExitingDungeon, ExitingHuntingArea, SearchingMonster_Wandering, SolvingDesire_Wandering, SolvingDesire, EnteringBossArea
				curState = State.MovingToDestination;
				break;
		}
	}
	public void PathFindFailSpecialAdventurer()
	{
		StartCoroutine(CoroutinePathFindFailSpecialAdventurer());
	}
	IEnumerator CoroutinePathFindFailSpecialAdventurer()
	{
		yield return null;
	}
	public void SetPathFindEventSpecialAdventurer()
	{
        //Debug.Log(gameObject.GetInstanceID());
        Debug.Log("[SetPathFindEventSpecialAdventurer]");
        pathFinder.SetNotifyEvent(PathFindSuccessSpecialAdventurer, PathFindFailSpecialAdventurer);
	}
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
				StartCoroutine(CheckStructure());
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
            case SuperState.TeleportToBossArea:
                curState = State.WaitingOtherSpecialAdvs;
                break;
            case SuperState.Skirmish:
                curState = State.ApproachingToEnemy;
                break;
            case SuperState.EnteringBossArea:
                curState = State.StartingBossBattle;
                break;
        }
    }

    protected override IEnumerator Charge(List<TileForMove> tileForMoveWay)
    {
        Direction dir = Direction.DownLeft;

        Vector3 dirVector;
        float distance, sum = 0.0f;
        int walkCnt = 0;
        const int posCheckRate = 4;

#if DEBUG_CHARGE
        Debug.Log("적: " + enemy + ", 목적지: " + destinationTile);
#endif
        // 적이 이미 누웠다면
        if (!ValidatingEnemy(enemy))
        {
            switch (superState)
            {
                case SuperState.Battle:
                    curState = State.AfterBattle;
                    break;
                case SuperState.Skirmish:
                    curState = State.MatchWon;
                    break;
                case SuperState.BossBattle:
                    curState = State.BossBattleWon;
                    break;
                default:
                    curState = State.AfterBattle;
                    break;
            }
            yield break;
        }
        // 레인지 검사. 적이 공격 범위 안으로 들어왔을 때.
        if (CheckInRange())
        {
            //switch (superState)
            //{
            //    case SuperState.SearchingMonster:
            //        curState = State.InitiatingBattle;
            //        yield break;
            //    case SuperState.Skirmish:
            //        curState = State.StartingSkirmish;
            //        yield break;
            //    case SuperState.BossBattle:
            //        curState = State.StartingBossBattle;
            //        yield break;
            //}

            curState = State.Battle;
            //curState = State.InitiatingBattle;
            yield break;
        }

        // PathFinder에서 받은 경로대로 이동
        for (int i = 0; i < tileForMoveWay.Count - 1; i++)
        {
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
                switch (superState)
                {
                    case SuperState.Battle:
                        curState = State.AfterBattle;
                        break;
                    case SuperState.Skirmish:
                        curState = State.MatchWon;
                        break;
                    case SuperState.BossBattle:
                        curState = State.BossBattleWon;
                        break;
                    default:
                        curState = State.AfterBattle;
                        break;
                }
                yield break;
            }
            // 레인지 검사. 적이 공격 범위 안으로 들어왔을 때.
            if (CheckInRange())
            {
                //switch (superState)
                //{
                //    case SuperState.SearchingMonster:
                //        curState = State.InitiatingBattle;
                //        break;
                //    case SuperState.Skirmish:
                //        curState = State.StartingSkirmish;
                //        break;
                //    case SuperState.BossBattle:
                //        curState = State.StartingBossBattle;
                //        break;
                //}
                curState = State.Battle;
                //curState = State.InitiatingBattle;
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

    protected override IEnumerator SearchingHuntingArea()
    {
        yield return null;
        // (이 모험가의 level <= 사냥터의 maxLevel)인 사냥터 중 maxLevel이 가장 낮은 걸 찾음.
        destinationPlace = CombatAreaManager.Instance.FindHuntingAreaSpAdv(battleStat.Level);

        if (destinationPlace == null)
            curState = State.SearchingExit;
        else
        {
            destinationTile = destinationPlace.GetEntrance();
            // TODO: 이거 destination TileForMove에 뭐 집어넣게 바꿔야
            curState = State.PathFinding;
        }
    }

    protected override IEnumerator Battle()
    {
        //enemy.healthBelowZeroEvent += OnEnemyHealthBelowZero;
        while (ValidatingEnemy(enemy))
        {
            if (CheckInRange())
                yield return curSubCoroutine = StartCoroutine(Attack());
            else
            {
                switch(superState)
                {
                    case SuperState.Battle:
                        curState = State.InitiatingBattle;
                        break;
                    case SuperState.BossBattle:
                        curState = State.StartingBossBattle;
                        break;
                    case SuperState.Skirmish:
                        curState = State.StartingSkirmish;
                        break;
                }
                //curState = State.InitiatingBattle;
                yield break;
            }
        }

        switch (superState)
        {
            case SuperState.Battle:
                curState = State.AfterBattle;
                break;
            case SuperState.Skirmish:
                curState = State.MatchWon;
                break;
            case SuperState.BossBattle:
                curState = State.BossBattleWon;
                break;
            default:
                curState = State.AfterBattle;
                break;
        }
    }

    protected override IEnumerator AfterBattle()
    {
        yield return new WaitForSeconds(2.0f);

        bossRaidCallCnt++;

        if (bossRaidCallCnt % BOSSRAID_CALL_PERIOD == 0 && // 10번에 1번 체크
            GameManager.Instance.isBossPhase && GameManager.Instance.canCallBossRaid && // 보스페이즈이며, 보스레이드 콜 할 수 있는 상태이며
            GameManager.Instance.playerSpAdvIndex != index && // 플레이어 캐릭터가 아니며
            Level >= CombatAreaManager.Instance.FindBossArea().ChallengeLevel) // 적정 레벨을 만족할 때
        {
            ScheduleToBossRaid();
            //Debug.Log("bossRaidCall : " + stat.actorName);
            GameManager.Instance.AICalledBossRaid(); // 보스레이드 신청
            curState = State.SearchingMonster;
        }
        else if (curHuntingArea != null && ((battleStat.Health < battleStat.HealthMax / 4) || // 체력이 25%미만이거나
            (Level > curHuntingArea.LevelMax && curHuntingArea.index < CombatAreaManager.Instance.ConqueringHuntingAreaIndex))) // 레벨 제한을 넘겼고, 갈 수 있는 다른 사냥터가 열렸다면
            curState = State.ExitingHuntingArea; // 사냥터에서 퇴장
        else
            curState = State.SearchingMonster;
    }

    private IEnumerator TeleportToBossArea()
    {
        yield return null;
        
        destinationPlace = CombatAreaManager.Instance.FindBossArea();

        Vector3 originPos = transform.position;
        // 순간이동
        curTile = destinationPlace.GetEntrance();
        curTileForMove = curTile.GetChild((Random.Range(0, 4)));

        // 텔레포트 이펙트 및 실제 위치 이동
        AlignPositionToCurTileForMove();
        Vector3 destPos = transform.position;

        DisplayTeleportEffect(originPos, destPos);

        yield return new WaitForSeconds(SkillConsts.TICK_TIME * 4);

        willBossRaid = false;

        curState = State.WaitingOtherSpecialAdvs;

        //if (destinationPlace == null)
        //    curState = State.SearchingExit;
        //else
        //{
        //    destinationTile = destinationPlace.GetEntrance();
        //    // TODO: 이거 destination TileForMove에 뭐 집어넣게 바꿔야
        //    curState = State.PathFinding;
        //}
    }

    private void WaitingOtherSpecialAdvs()
    {
        // 일단 체력 회복
        HealFullHealth(false);
        willBossRaid = false;
        
        // 도착하면 응답 완료했다고 매니저에 알림
        GameManager.Instance.SpAdvResponsed(true, this);
    }

    private IEnumerator StartingSkirmish()
    {
        // 여기서 위치도 정해주는게 좋다.
        yield return new WaitForSeconds(SkillConsts.TICK_TIME * 4);

        if (enemy == null)
        {   
            curState = State.MatchWon;
        }
        else
        {
            SetDestinationTowardEnemy();

            // 적이 공격 범위 안에 있다면 바로 전투.
            if (CheckInRange())
                curState = State.Battle;
            else
                curState = State.PathFinding;
        }
    }

    private void MatchWon()
    {
        GameManager.Instance.ReportMatchWon(this);

        curState = State.WaitingOtherMatch;
    }

    private void WaitingOtherMatch()
    {
        animator.SetTrigger("WinFlg");
    }

    private void SkirmishDefeated()
    {
        GameManager.Instance.ReportMatchDefeated(this);
    }

    private IEnumerator SkirmishWon()
    {
        yield return new WaitForSeconds(1.0f);
        // (이 모험가의 level <= 사냥터의 maxLevel)인 사냥터 중 maxLevel이 가장 낮은 걸 찾음.
        destinationPlace = CombatAreaManager.Instance.FindBossArea();

        // TODO : 이거 바로 내부로 들어가게 수정해야함.
        destinationTile = destinationPlace.GetEntrance();
        // TODO: 이거 destination TileForMove에 뭐 집어넣게 바꿔야
        curState = State.PathFinding;
    }

    private void EnteringBossArea()
    {
        // (이 모험가의 level <= 사냥터의 maxLevel)인 사냥터 중 maxLevel이 가장 낮은 걸 찾음.
        destinationPlace = CombatAreaManager.Instance.FindBossArea();

        curBossArea = destinationPlace as BossArea;
        curBossArea.EnterAdventurer(this.gameObject);

        destinationPlace = null; // 사용 후에는 비워주기.

        destinationTileForMove = curBossArea.FindNearestBlank(curTileForMove);
        destinationTile = destinationTileForMove.GetParent();

        curState = State.PathFinding;
    }

    private void StartingBossBattle()
    {
        enemy = curBossArea.FindNearestMonster(this); // 가장 가까운 몬스터 찾기.

        SetDestinationTowardEnemy();

        // 적이 공격 범위 안에 있다면 바로 전투.
        if (CheckInRange())
            curState = State.Battle;
        else
            curState = State.PathFinding;
    }

    public override bool TakeDamage(ICombatant attacker, float damage, float penFixed, float penMult, bool isCrit, out float actualDamage) // 데미지 받기. 이펙트 처리를 위해 isCrit도 받음.
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
        //Debug.Log("방어력 : " + battleStat.Defence);
        // 조건?
        if (battleStat.Health <= 0)
        {
            HealthBelowZeroNotify(this, attacker);
            StopCurActivities();

            switch (superState)
            {
                case SuperState.Skirmish:
                    curState = State.SkirmishDefeated;
                    break;
                case SuperState.BossBattle:
                    curState = State.BailOut;
                    break;
                default:
                    curState = State.PassedOut;
                    break;
            }
        }
        else if (IsInBattle() == false)
        {
            StopCurActivities();
            animator.SetTrigger("DamageFlg");
            enemy = attacker;
            curState = State.InitiatingBattle;
        }

        return !isDodged;
    }

    private void BossBattleWon()
    {
        // 승리
        // 보상 받기
        GameManager.Instance.ReportBossBattleWon(this);

        curState = State.Idle;
    }

    private IEnumerator BailOut()
    {
        // 체력 소량 회복
        battleStat.Heal(battleStat.HealthMax * 0.1f);

        Vector3 originPos = transform.position;
        // 탈출
        curTile = curBossArea.GetEntrance();
        curTileForMove = curTile.GetChild(0);

        // 텔레포트 이펙트 및 실제 위치 이동
        AlignPositionToCurTileForMove();
        Vector3 destPos = transform.position;

        DisplayTeleportEffect(originPos, destPos);

        GameManager.Instance.ReportBossBattleDefeat();

        yield return new WaitForSeconds(SceneConsts.BAILOUT_RESURRECT_TIME);

        curState = State.Idle;
    }

    private void DisplayTeleportEffect(Vector3 pos1, Vector3 pos2)
    {
        GameObject effect1 = Instantiate((GameObject)Resources.Load("EffectPrefabs/Default_TeleportEffect"));
        effect1.transform.position = pos1;
        GameObject effect2 = Instantiate((GameObject)Resources.Load("EffectPrefabs/Default_TeleportEffect"));
        effect2.transform.position = pos2;

        effect1.GetComponent<AttackEffect>().StartEffect();
        Destroy(effect1, SkillConsts.EFFECT_DESTROY_DELAY);
        effect2.GetComponent<AttackEffect>().StartEffect();
        Destroy(effect2, SkillConsts.EFFECT_DESTROY_DELAY);
    }
    #endregion

    #region SaveLoad
    //public override CombatantType GetCombatantType()
    //{
    //    return CombatantType.SpecialAdventurer;
    //}
    public Item GetWeapon()
    {
        return weapon;
    }
    public Item GetArmor()
    {
        return armor;
    }
    public Item GetAccessory1()
    {
        return accessory1;
    }
    public Item GetAccessory2()
    {
        return accessory2;
    }
    //public override ActorType GetActorType()
    //{
    //    return ActorType.SpecialAdventurer;
    //}
    public override ActorType GetActorType()
    {
        return ActorType.SpecialAdventurer;
    }
    #endregion
}