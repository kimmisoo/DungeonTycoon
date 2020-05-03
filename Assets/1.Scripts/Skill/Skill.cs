using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill
{
    public ICombatant owner;
    public GameObject effect;
    public ICombatant enemy;

    public Skill(ICombatant ownerIn)
    {
        owner = ownerIn;
    }

    public abstract void OnStruck();
    public abstract void OnAttack();
    public abstract void Activate();

    public void AdditionalAttack(ICombatant[] enemies, float damage, float penFixed, float penMult, bool isCrit)
    {
        foreach (ICombatant target in enemies)
            target.TakeDamage(owner, damage, penFixed, penMult, isCrit);
    }

    public void ModifyBattleStatContinuous(ICombatant target, StatType statType, ModType modType, float value)
    {
        target.GetBattleStat().AddStatModContinuous(statType, new StatModContinuous(modType, value));
    }

    public void ModifyBattleStatDiscrete(ICombatant target, StatType statType, ModType modType, int value)
    {
        target.GetBattleStat().AddStatModDiscrete(statType, new StatModDiscrete(modType, value));
    }
}
