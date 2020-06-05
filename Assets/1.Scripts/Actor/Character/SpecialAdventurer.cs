//#define DEBUG_ITEM
//#define DEBUG_SPADV_STATE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAdventurer : Adventurer
{
    private Item weapon, armor, accessory1, accessory2;

    private const int ACCESSORY_CAPACITY = 2;

    //Skill uniqueSkill;
    public void InitSpecialAdventurer(Stat stat, BattleStat battleStat, RewardStat rewardStat, string name)
    {
        base.InitAdventurer(stat, battleStat, rewardStat);
        AddSkill(name);
    }

    public void OnEnable()
    {
        base.OnEnable();
        monsterSearchCnt = 0;
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
        switch (nextState)
        {
            case State.Idle:
#if DEBUG_SPADV_STATE
                Debug.Log("Idle");
#endif
                superState = SuperState.Idle;
                Idle();
                //Traveler이므로 무조건 SearchingStructure 부터
                //이외에 체크할거 있으면 여기서
                break;
            case State.SolvingDesire_Wandering:
#if DEBUG_SPADV_STATE
                Debug.Log("SolvingDesire_Wandering");
#endif
                superState = SuperState.SolvingDesire_Wandering;
                curCoroutine = StartCoroutine(SolvingDesire_Wandering());
                break;
            case State.SearchingMonster_Wandering:
#if DEBUG_SPADV_STATE
                Debug.Log("SearchingMonster_Wandering");
#endif
                superState = SuperState.SearchingMonster_Wandering;
                curCoroutine = StartCoroutine(SearchingMonster_Wandering());
                break;
            case State.SearchingStructure:
#if DEBUG_SPADV_STATE
                Debug.Log("SearchingStructure");
#endif
                superState = SuperState.SolvingDesire;
                curCoroutine = StartCoroutine(SearchingStructure());
                break;
            case State.PathFinding:
#if DEBUG_SPADV_STATE
                Debug.Log("PathFinding");
#endif
                curCoroutine = StartCoroutine(PathFinding());
                break;
            case State.MovingToDestination:
#if DEBUG_SPADV_STATE
                Debug.Log("MovingToDestination");
#endif
                animator.SetBool("MoveFlg", true);
                curCoroutine = StartCoroutine(MoveToDestination());
                break;
            case State.WaitingStructure:
#if DEBUG_SPADV_STATE
                Debug.Log("WaitingStructure");
#endif
                destinationPlace.Visit(this);
                break;
            case State.UsingStructure:
#if DEBUG_SPADV_STATE
                Debug.Log("UsingStructure");
#endif
                //욕구 감소
                //소지 골드 감소
                UsingStructure();
                break;
            case State.SearchingExit:
#if DEBUG_SPADV_STATE
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
#if DEBUG_SPADV_STATE
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
#if DEBUG_SPADV_STATE
                Debug.Log("SearchingMonster");
#endif
                superState = SuperState.SearchingMonster;
                curCoroutine = StartCoroutine(SearchingMonster());
                break;
            case State.ApproachingToEnemy:
#if DEBUG_SPADV_STATE
                Debug.Log("ApproachingToEnemy");
#endif
                animator.SetBool("MoveFlg", true);
                curCoroutine = StartCoroutine(ApproachingToEnemy());
                break;
            case State.InitiatingBattle:
#if DEBUG_SPADV_STATE
                Debug.Log("InitiatingBattle");
#endif
                superState = SuperState.Battle;
                InitiatingBattle();
                break;
            case State.Battle:
#if DEBUG_SPADV_STATE
                Debug.Log("Battle");
#endif
                ShowBattleUI();
                curCoroutine = StartCoroutine(Battle());
                break;
            case State.AfterBattle:
#if DEBUG_SPADV_STATE
                Debug.Log("AfterBattle");
#endif
                superState = SuperState.AfterBattle;
                curCoroutine = StartCoroutine(AfterBattle());
                break;
            case State.ExitingHuntingArea:
#if DEBUG_SPADV_STATE
                Debug.Log("ExitingHuntingArea");
#endif
                superState = SuperState.ExitingHuntingArea;
                curCoroutine = StartCoroutine(ExitingHuntingArea());
                break;
            case State.PassedOut:
#if DEBUG_SPADV_STATE
                Debug.Log("PassedOut");
#endif
                superState = SuperState.PassedOut;
                animator.SetTrigger("DeathFlg");
                PassedOut();
                break;
            case State.SpontaneousRecovery:
#if DEBUG_SPADV_STATE
                Debug.Log("SpontaneousRecovery");
#endif
                curCoroutine = StartCoroutine(SpontaneousRecovery());
                break;
            case State.Rescued:
#if DEBUG_SPADV_STATE
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

    #region BossBattle
    public void OnBossRaidCall()
    {
        StopCurActivities();
        // 전투상태였다면?
        //if()
        //    StartCoroutine(ParticipateInRaid());
    }

    public void OnPlayerRaidOrder()
    {
        StopCurActivities();

        StartCoroutine(ParticipateInRaid());
    }

    private IEnumerator ParticipateInRaid()
    {
        // 전투 종료까지 기다리기.
        while(superState == SuperState.Battle )
        {
            yield return new WaitForSeconds(SkillConsts.TICK_TIME);
        }

        StopCurActivities();

        
    }
    #endregion
}
