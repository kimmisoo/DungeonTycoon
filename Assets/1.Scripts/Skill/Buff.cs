using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldManUniqueSkill : Skill
{
    private const int TICK_MULT = 10;
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

    public override void OnAttack(float actualDamage, bool isCrit, bool isDodged)
    {
        
    }

    public override void OnStruck(float actualDamage, bool isDodged, ICombatant Attacker)
    {
        
    }

    public void DisplayHeal(float healAmount)
    {
        owner.DisplayHeal(healAmount);
    }
}
