﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상위클래스 샌드박스 패턴 사용. 
/// </summary>
public abstract class Skill : MonoBehaviour
{
    public ICombatant owner;
    public ICombatant enemy;
    protected Coroutine curCoroutine;
    protected List<ICombatant> targets; // 효과 대상 TFM 위에 있는 적들
    public bool isActive;


    protected const float TICK_TIME = 0.5f;

    public Skill()
    {
        targets = new List<ICombatant>();
        isActive = false;
    }

    public void SetOwner(ICombatant ownerIn)
    {
        owner = ownerIn;
    }

    public void SetEnemy()
    {
        enemy = owner.GetEnemy();
    }


    public abstract void InitSkill();
    /// <summary>
    /// 맞을 때 발생하는 효과
    /// </summary>
    public abstract void OnStruck(float actualDamage, bool isDodged, ICombatant attacker);
    /// <summary>
    /// 때릴 때 발생하는 효과
    /// </summary>
    public abstract void OnAttack(float actualDamage, bool isCrit, bool isDodged);
    /// <summary>
    /// 항시발동 및 일정시간마다 발동. 코루틴 실행하는 메서드. 단위시간은 tickTime. 항시발동은 단위시간마다 체크.
    /// 게임 시작 혹은 아이템 착용시마다 실행.
    /// </summary>
    public void Activate()
    {
        curCoroutine = StartCoroutine(OnAlways());
        isActive = true;
    }
    /// <summary>
    /// Activate()에서 실행한 코루틴 정지시키는 메서드. 모험가 사망 혹은 아이템 해제시마다 실항.
    /// </summary>
    public void Deactivate()
    {
        StopCoroutine(curCoroutine);
        isActive = false;
    }

    public abstract IEnumerator OnAlways();

    public float AdditionalAttack(List<ICombatant> enemies, float damage, float penFixed, float penMult, bool isCrit)
    {
        float totalDamage = 0;

        foreach (ICombatant target in enemies)
        {
               totalDamage += AdditionalAttack(enemy, damage, penFixed, penMult, isCrit);
        }

        return totalDamage;
    }

    public float AdditionalAttack(ICombatant enemy, float damage, float penFixed, float penMult, bool isCrit)
    {
        float actualDamage = 0;

        if (IsHostile(enemy))
            enemy.TakeDamage(owner, damage, penFixed, penMult, isCrit, out actualDamage);

        return actualDamage;
    }

    public void ModifyBattleStatContinuous(ICombatant target, StatType statType, ModType modType, float value)
    {
        target.GetBattleStat().AddStatModContinuous(statType, new StatModContinuous(modType, value));
    }

    public void ModifyBattleStatDiscrete(ICombatant target, StatType statType, ModType modType, int value)
    {
        target.GetBattleStat().AddStatModDiscrete(statType, new StatModDiscrete(modType, value));
    }

    /// <summary>
    /// 적인지 아닌지 판단. 다른 타입(Adventurer<->Monster)이거나 enemy(PvP는 무조건 1:1이므로)일 때만 true
    /// </summary>
    protected bool IsHostile(ICombatant target)
    {
        SetEnemy();

        if (owner is Adventurer)
        {
            if ((target is Monster || target == enemy) && owner.ValidatingEnemy(target))
                return true;
            else
                return false;
        }
        else
        {
            if ((target is Adventurer || target == enemy) && owner.ValidatingEnemy(target))
                return true;
            else
                return false;
        }
    }
}