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
                yield return new WaitForSeconds(SkillConsts.TICK_TIME * TICK_MULT);
                healAmount = myBattleStat.MissingHealth * 0.05f;
                myBattleStat.Heal(healAmount);

                if (healAmount > 1)
                    DisplayHeal(healAmount);
            }
        }
    }

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
        myMod = new StatModContinuous(StatType.Attack, ModType.Mult, 2.0f);
        skillEffect = Instantiate((GameObject)Resources.Load("EffectPrefabs/Default_BuffEffect"));
        skillEffect.transform.SetParent(owner.GetTransform());
        skillEffect.transform.position = owner.GetPosition();
    }

    public override void BeforeAttack()
    {
        SetEnemy();

        if (enemy.GetSuperState() != SuperState.Battle)
        {
            myBattleStat.AddStatModContinuous(myMod);
            DisplaySkillEffect();

        }
    }

    public override void AfterAttack()
    {
        myBattleStat.RemoveStatModContinuous(myMod);
    }

    public override IEnumerator OnAlways()
    {
        yield return null;
    }

    public void DisplaySkillEffect()
    {
        skillEffect.GetComponent<AttackEffect>().StartEffect();
        skillEffect.transform.position = new Vector3(owner.GetPosition().x, owner.GetPosition().y + 0.08f, owner.GetPosition().z);
    }
}

public class WalUniqueSkill : Skill
{
    BattleStat myBattleStat;
    StatModContinuous myMod;
    GameObject skillEffect;
    float shieldTimer;
    TemporaryEffect shieldBuff;
    const float DURATION = 3.0f;
    const float SHIELD_RATE = 0.23f;

    public override void InitSkill()
    {
        myBattleStat = owner.GetBattleStat();
        myMod = new StatModContinuous(StatType.Shield, ModType.Fixed, 0);
        shieldBuff = new TemporaryEffect(DURATION);
        shieldTimer = -1;

        skillEffect = Instantiate((GameObject)Resources.Load("EffectPrefabs/ShieldGaining_BuffEffect"));
        skillEffect.transform.SetParent(owner.GetTransform());
        skillEffect.transform.position = owner.GetPosition();
    }

    public override IEnumerator OnAlways()
    {
        //while (true)
        //{
        //    yield return new WaitForSeconds(SkillConsts.TICK_TIME);

        //    if (shieldTimer >= 0 - Mathf.Epsilon)
        //    {
        //        shieldTimer += SkillConsts.TICK_TIME;

        //        if (shieldTimer >= DURATION)
        //        {
        //            shieldTimer = -1;
        //            myBattleStat.RemoveStatModContinuous(myMod);
        //        }
        //    }
        //}
        yield return null;
    }

    public override void OnAttack(float actualDamage, bool isCrit, bool isDodged)
    {
        owner.RemoveTemporaryEffect(shieldBuff);

        // TemporaryEffect 생성
        myMod.ModValue = myBattleStat.HealthMax * SHIELD_RATE;
        StatModContinuous tempMod = new StatModContinuous(myMod);
        shieldBuff = new TemporaryEffect(DURATION);
        shieldBuff.AddContinuousMod(myMod);

        owner.AddTemporaryEffect(shieldBuff);
        //shieldTimer = 0;
        DisplaySkillEffect();
    }

    public void DisplaySkillEffect()
    {
        skillEffect.GetComponent<AttackEffect>().StartEffect();
        skillEffect.transform.position = new Vector3(owner.GetPosition().x, owner.GetPosition().y + 0.08f, owner.GetPosition().z);
    }
}