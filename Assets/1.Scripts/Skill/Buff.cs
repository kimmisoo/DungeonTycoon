using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldManUniqueSkill : Skill
{
    private const int TICK_MULT = 20;
    private const float HEAL_RATE = 0.05f;

    public override void InitSkill()
    { }

    public override IEnumerator OnAlways()
    {
        BattleStat myBattleStat = owner.GetBattleStat();
        float healAmount = 0;
        while (true)
        {
            if (owner.GetSuperState() != SuperState.PassedOut)
            {
                yield return new WaitForSeconds(TICK_TIME * TICK_MULT);
                healAmount = myBattleStat.MissingHealth * 0.05f;
                myBattleStat.Heal(healAmount);

                if (healAmount > 1)
                    DisplayHeal(healAmount);
            }
        }
    }

    public override void BeforeAttack() { }
    public override void OnAttack(float actualDamage, bool isCrit, bool isDodged) { }
    public override void AfterAttack() { }
    public override void OnStruck(float actualDamage, bool isDodged, ICombatant Attacker) { }

    public void DisplayHeal(float healAmount)
    {
        owner.DisplayHeal(healAmount);
    }
}

public class NyangUniqueSkill : Skill
{
    BattleStat myBattleStat;
    StatModContinuous myMod;
    GameObject skillEffect;

    public override void InitSkill()
    {
        myBattleStat = owner.GetBattleStat();
        myMod = new StatModContinuous(ModType.Mult, 2.0f);
        skillEffect = Instantiate((GameObject)Resources.Load("EffectPrefabs/Default_BuffEffect"));
        skillEffect.transform.SetParent(owner.GetTransform());
        skillEffect.transform.position = owner.GetPosition();
    }

    public override void BeforeAttack()
    {
        SetEnemy();

        if (enemy.GetSuperState() != SuperState.Battle)
        {
            myBattleStat.AddStatModContinuous(StatType.Attack, myMod);
            DisplaySkillEffect();

        }
    }

    public override void AfterAttack()
    {
        myBattleStat.RemoveStatModContinuous(StatType.Attack, myMod);
    }

    public override IEnumerator OnAlways()
    {
        yield return null;
    }

    public override void OnAttack(float actualDamage, bool isCrit, bool isDodged) { }
    public override void OnStruck(float actualDamage, bool isDodged, ICombatant attacker) { }

    public void DisplaySkillEffect()
    {
        skillEffect.GetComponent<AttackEffect>().StartEffect();
        skillEffect.transform.position = new Vector3(owner.GetPosition().x, owner.GetPosition().y + 0.08f, owner.GetPosition().z);
    }
}