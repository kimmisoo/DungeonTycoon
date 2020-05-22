using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldManUniqueSkill : Skill
{
    private const int TICK_MULT = 20;
    private const float HEAL_RATE = 0.05f;

    public override void InitSkill()
    {
        SetNameAndExplanation("숨 고르기", "매 5초마다 잃은 체력의 5%를 회복합니다.");
    }

    public override IEnumerator OnAlways()
    {
        SetMyBattleStat();

        float healAmount = 0;
        while (true)
        {
            yield return new WaitForSeconds(SkillConsts.TICK_TIME * TICK_MULT);
            if (owner.GetSuperState() != SuperState.PassedOut)
            {
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
    StatModContinuous myMod;
    GameObject skillEffect;

    public override void InitSkill()
    {
        SetMyBattleStat();
        myMod = new StatModContinuous(StatType.Attack, ModType.Mult, 2.0f);

        SetNameAndExplanation("선수필승!", "비 전투 상태의 적을 공격할 때 공격력이 200% 증가합니다.");
    }

    public override void BeforeAttack()
    {
        SetEnemy();

        if (enemy.GetSuperState() != SuperState.Battle)
        {
            myBattleStat.AddStatModContinuous(myMod);
            owner.DisplayBuff();
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
}

public class WalUniqueSkill : Skill
{
    StatModContinuous atkMod;
    const float ATK_BONUS_RATE = 0.4f;
    StatModContinuous atkSpdMod;
    const float ATKSPD_PENALTY_RATE = -0.33f;

    StatModContinuous shieldMod;
    GameObject skillEffect;
    TemporaryEffect shieldBuff;
    const float DURATION = 3.0f;
    const float SHIELD_RATE = 0.53f;

    public override void InitSkill()
    {
        myBattleStat = owner.GetBattleStat();
        shieldMod = new StatModContinuous(StatType.Shield, ModType.Fixed, 0);
        shieldBuff = new TemporaryEffect("파비스", DURATION);
        shieldBuff.AddContinuousMod(shieldMod);

        atkMod = new StatModContinuous(StatType.Attack, ModType.Mult, ATK_BONUS_RATE);
        atkSpdMod = new StatModContinuous(StatType.AttackSpeed, ModType.Mult, ATKSPD_PENALTY_RATE);

        skillEffect = Instantiate((GameObject)Resources.Load("EffectPrefabs/ShieldGaining_BuffEffect"));
        skillEffect.transform.SetParent(owner.GetTransform());
        skillEffect.transform.position = owner.GetPosition();

        SetNameAndExplanation("파비스 석궁병", "공격력이 40% 증가하는 대신 공격속도가 33% 감소합니다. 매 사격마다 최대체력의 3%에 해당하는 방어막을 얻습니다. 방어막은 중첩되지 않습니다.");
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
        //owner.RemoveTemporaryEffect(shieldBuff);

        // TemporaryEffect 생성
        shieldMod.ModValue = myBattleStat.HealthMax * SHIELD_RATE;

        Debug.Log("elapsedTime: " + shieldBuff.elapsedTime + ", amount: " + shieldMod.ModValue);
        //owner.AddTemporaryEffect(shieldBuff);
        ApplyTemporaryEffect(owner, shieldBuff, false);
        //shieldTimer = 0;
        DisplaySkillEffect();
    }

    public override void ApplyStatBonuses()
    {
        myBattleStat.AddStatModContinuous(atkMod);
        myBattleStat.AddStatModContinuous(atkSpdMod);
    }

    public override void RemoveStatBonuses()
    {
        myBattleStat.RemoveStatModContinuous(atkMod);
        myBattleStat.RemoveStatModContinuous(atkSpdMod);
    }

    public void DisplaySkillEffect()
    {
        skillEffect.GetComponent<AttackEffect>().StartEffect();
        skillEffect.transform.position = new Vector3(owner.GetPosition().x, owner.GetPosition().y + 0.08f, owner.GetPosition().z);
    }
}

public class CommonSelfBuffSkill : Skill
{
    List<StatModContinuous> continuousMods;
    List<StatModDiscrete> discreteMods;

    public CommonSelfBuffSkill(string name, string explanation)
    {
        SetNameAndExplanation(name, explanation);
    }

    public override void InitSkill()
    {
        myBattleStat = owner.GetBattleStat();
    }

    public override void ApplyStatBonuses()
    {
        foreach (StatModContinuous mod in continuousMods)
            myBattleStat.AddStatModContinuous(mod);
        foreach (StatModDiscrete mod in discreteMods)
            myBattleStat.AddStatModDiscrete(mod);
    }

    public override void RemoveStatBonuses()
    {
        foreach (StatModContinuous mod in continuousMods)
            myBattleStat.RemoveStatModContinuous(mod);
        foreach (StatModDiscrete mod in discreteMods)
            myBattleStat.RemoveStatModDiscrete(mod);
    }

    public void InstantiateLists()
    {
        continuousMods = new List<StatModContinuous>();
        discreteMods = new List<StatModDiscrete>();
    }

    public void AddContinuousMod(StatModContinuous mod)
    {
        continuousMods.Add(mod);
    }

    public void AddDiscreteMod(StatModDiscrete mod)
    {
        discreteMods.Add(mod);
    }
}

public class DamageAbsorbSkill : Skill
{
    const float HEAL_AMOUNT = 15f;

    public override void InitSkill()
    {
        SetNameAndExplanation("데미지 흡수", "공격을 받을 때마다, 체력을 15 회복합니다.");
        myBattleStat = owner.GetBattleStat();
    }

    public override void OnStruck(float actualDamage, bool isDodged, ICombatant attacker)
    {
        float healed = myBattleStat.Heal(HEAL_AMOUNT);
        if (healed >= 1.0f)
            owner.DisplayHeal(healed);
    }
}

public class OverBoostSkill : Skill
{
    const float BONUS_TRIGGER = 0.3f;
    StatModContinuous atkModStat;
    const float ATK_BONUS_RATE = 0.25f;

    public override void InitSkill()
    {
        SetNameAndExplanation("오버부스트", "공격대상의 체력이 30% 이하일 때 공격력이 25% 증가합니다.");
        SetMyBattleStat();

        atkModStat = new StatModContinuous(StatType.Attack, ModType.Mult, ATK_BONUS_RATE);
    }

    public override void BeforeAttack()
    {
        BattleStat enemyBattleStat;
        SetEnemy();
        enemyBattleStat = enemy.GetBattleStat();

        if (enemyBattleStat.Health <= enemyBattleStat.HealthMax * BONUS_TRIGGER)
        {
            myBattleStat.AddStatModContinuous(atkModStat);
            owner.DisplayBuff();
        }
    }

    public override void AfterAttack()
    {
        BattleStat enemyBattleStat;
        SetEnemy();
        enemyBattleStat = enemy.GetBattleStat();

        myBattleStat.RemoveStatModContinuous(atkModStat);
    }
}

public class BlessSkill : Skill
{
    const float SHIELD_RATE = 0.12f;
    const float DURATION = 3.0f;
    GameObject skillEffect;

    public override void InitSkill()
    {
        SetNameAndExplanation("축복", "적을 공격할 때마다 입힌 피해의 12%에 해당하는 방어막을 얻습니다. 방어막은 3초 동안 지속됩니다.");
        SetSkillEffect();
    }

    public override void OnAttack(float actualDamage, bool isCrit, bool isDodged)
    {
        TemporaryEffect shieldBuff = new TemporaryEffect(name, DURATION);
        shieldBuff.AddContinuousMod(new StatModContinuous(StatType.Shield, ModType.Fixed, actualDamage * SHIELD_RATE));
        owner.AddTemporaryEffect(shieldBuff);
        DisplaySkillEffect();
    }

    private void SetSkillEffect()
    {
        skillEffect = Instantiate((GameObject)Resources.Load("EffectPrefabs/ShieldGaining_BuffEffect"));
        skillEffect.transform.SetParent(owner.GetTransform());
        skillEffect.transform.position = owner.GetPosition();
    }

    private void DisplaySkillEffect()
    {
        skillEffect.GetComponent<AttackEffect>().StartEffect();
        skillEffect.transform.position = new Vector3(owner.GetPosition().x, owner.GetPosition().y + 0.08f, owner.GetPosition().z);
    }
}

public class BuckshotSkill : Skill
{
    const float ATTACK_BONUS_RATE = 0.10f;
    StatModContinuous atkStatMod;
    StatModDiscrete rangeStatMod;

    public override void InitSkill()
    {
        SetNameAndExplanation("산탄", "사정거리가 1 증가합니다. 가까이 있는 적을 공격할 때 추가 공격력을 얻습니다.");
        SetMyBattleStat();
        atkStatMod = new StatModContinuous(StatType.Attack, ModType.Mult, 0.0f);
        rangeStatMod = new StatModDiscrete(StatType.AttackRange, ModType.Fixed, 1);
    }

    public override void BeforeAttack()
    {
        SetEnemy();
        int dist = owner.GetCurTileForMove().GetDistance(enemy.GetCurTileForMove());
        atkStatMod.ModValue = (myBattleStat.Range - dist) * ATTACK_BONUS_RATE;
        if (atkStatMod.ModValue > 0)
        {
            owner.DisplayBuff();
            myBattleStat.AddStatModContinuous(atkStatMod);
        }
    }

    public override void AfterAttack()
    {
        myBattleStat.RemoveStatModContinuous(atkStatMod);
    }

    public override void ApplyStatBonuses()
    {
        myBattleStat.AddStatModDiscrete(rangeStatMod);
    }

    public override void RemoveStatBonuses()
    {
        myBattleStat.RemoveStatModDiscrete(rangeStatMod);
    }
}

public class DualWieldSkill : Skill
{
    StatModContinuous atkMod;
    const float ATK_PENALTY = -0.5f;
    StatModContinuous atkSpdMod;
    const float ATKSPD_BONUS = 1.0f;

    public override void InitSkill()
    {
        SetNameAndExplanation("쌍수무기", "공격속도가 100% 증가하지만 공격력이 50% 감소합니다.");
        SetMyBattleStat();
        atkMod = new StatModContinuous(StatType.Attack, ModType.Mult, ATK_PENALTY);
        atkSpdMod = new StatModContinuous(StatType.AttackSpeed, ModType.Mult, ATKSPD_BONUS);
    }

    public override void ApplyStatBonuses()
    {
        myBattleStat.AddStatModContinuous(atkMod);
        myBattleStat.AddStatModContinuous(atkSpdMod);
    }

    public override void RemoveStatBonuses()
    {
        myBattleStat.RemoveStatModContinuous(atkMod);
        myBattleStat.RemoveStatModContinuous(atkSpdMod);
    }
}

public class LifeStealSkill : Skill
{
    const float HEAL_RATE = 0.12f;

    public override void InitSkill()
    {
        SetNameAndExplanation("흡혈", "적을 공격할 때마다, 입힌 데미지의 12%만큼의 체력을 회복합니다.");
        SetMyBattleStat();
    }

    public override void OnAttack(float actualDamage, bool isCrit, bool isDodged)
    {
        float healAmount = actualDamage * HEAL_RATE;
        myBattleStat.Heal(healAmount);
        if (healAmount >= 1)
            owner.DisplayHeal(healAmount);
    }
}

public class ImmerseSkill : Skill
{
    TemporaryEffect atkspdBuff;
    const int TICK_MULT = 8;
    const float DURATION = 3.0f;
    const int STACK_LIMIT = 5;
    const float ATKSPD_BONUS_RATE = 0.05f;

    public override void InitSkill()
    {
        SetNameAndExplanation("몰입", "전투 시작 후 1초가 지날 때마다 공격속도가 5% 증가하는 버프를 얻습니다. 버프는 최대 5번까지 중첩되며, 2초간 지속됩니다.");
        atkspdBuff = new TemporaryEffect(name, DURATION, STACK_LIMIT);
        atkspdBuff.AddContinuousMod(new StatModContinuous(StatType.AttackSpeed, ModType.Mult, ATKSPD_BONUS_RATE));
    }

    public override IEnumerator OnAlways()
    {
        while (true)
        {
            yield return new WaitForSeconds(SkillConsts.TICK_TIME * TICK_MULT);

            if (owner.GetState() == State.Battle)
            {
                ApplyTemporaryEffect(owner, atkspdBuff, false);
                Debug.Log(owner.GetBattleStat().AttackSpeed);
                owner.DisplayBuff();
            }
        }
    }
}

public class RedPotionSkill : Skill
{
    const int TICK_MULT = 12;
    const float HEAL_AMOUNT = 1.0f;

    public override void InitSkill()
    {
        SetNameAndExplanation("빨간 물약", "매 3초마다, 체력을 1 회복합니다.");
        SetMyBattleStat();
    }

    public override IEnumerator OnAlways()
    {
        while (true)
        {
            yield return new WaitForSeconds(SkillConsts.TICK_TIME * TICK_MULT);

            if (owner.GetState() != State.PassedOut)
            {
                float healed = myBattleStat.Heal(HEAL_AMOUNT);

                //if (healed >= 1.0f - Mathf.Epsilon)
                //    owner.DisplayHeal(healed);
            }
        }
    }
}

public class RejuvenateSkill : Skill
{
    const int TICK_MULT = 4;
    const float HEAL_RATE = 0.015f;

    public override void InitSkill()
    {
        SetNameAndExplanation("재생", "매 1초마다, 잃은 체력의 1.5%를 회복합니다.");
        SetMyBattleStat();
    }

    public override IEnumerator OnAlways()
    {
        while (true)
        {
            yield return new WaitForSeconds(SkillConsts.TICK_TIME * TICK_MULT);

            if (owner.GetState() != State.PassedOut)
            {
                float healed = myBattleStat.Heal(myBattleStat.MissingHealth * HEAL_RATE);

                if (healed >= 1.0f - Mathf.Epsilon)
                    owner.DisplayHeal(healed);
            }
        }
    }
}

public class CrisisManagementSkill : Skill
{
    private const float HEALTH_TRIGGER = 0.2f;
    private const float DEF_BONUS = 75.0f;
    private StatModContinuous defBuff;

    public override void InitSkill()
    {
        SetNameAndExplanation("위기 대처", "체력이 20% 이하일 때, 방어력이 75 증가합니다.");
        SetMyBattleStat();
        defBuff = new StatModContinuous(StatType.Defence, ModType.Fixed, DEF_BONUS);
    }

    public override IEnumerator OnAlways()
    {
        while (true)
        {
            yield return new WaitForSeconds(SkillConsts.TICK_TIME);

            if (myBattleStat.Health <= myBattleStat.HealthMax * HEALTH_TRIGGER)
            {
                if (!myBattleStat.ContainsStatMod(defBuff))
                    owner.DisplayBuff();
                myBattleStat.AddStatModContinuous(defBuff);
            }
            else
            {
                myBattleStat.RemoveStatModContinuous(defBuff);
            }
        }
    }
}

public class ExecutionSkill : Skill
{
    private const float ATK_BONUS = 0.5f;
    private const float HEALTH_TRIGGER = 0.2f;
    private StatModContinuous atkMod;

    public override void InitSkill()
    {
        SetNameAndExplanation("처형", "공격 대상의 체력이 20% 이하일 때 공격력이 50% 증가합니다.");
        atkMod = new StatModContinuous(StatType.Attack, ModType.Mult, ATK_BONUS);
        SetMyBattleStat();
    }

    //public override IEnumerator OnAlways()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(SkillConsts.TICK_TIME);

    //        SetEnemy();
    //        if (enemy != null)
    //        {
    //            if (enemy.GetBattleStat().Health <= enemy.GetBattleStat().HealthMax * HEALTH_TRIGGER)
    //            {
    //                if (!myBattleStat.ContainsStatMod(atkMod))
    //                    owner.DisplayBuff();
    //                Debug.Log("before : " + myBattleStat.Attack);
    //                myBattleStat.AddStatModContinuous(atkMod);
    //                Debug.Log("After : " + myBattleStat.Attack);
    //            }
    //            else
    //            {
    //                myBattleStat.RemoveStatModContinuous(atkMod);
    //            }
    //        }
    //    }
    //}

    public override void BeforeAttack()
    {
        SetEnemy();

        if (enemy.GetBattleStat().Health <= enemy.GetBattleStat().HealthMax * HEALTH_TRIGGER)
        {
            owner.DisplayBuff();
            Debug.Log("before : " + myBattleStat.Attack);
            myBattleStat.AddStatModContinuous(atkMod);
            Debug.Log("After : " + myBattleStat.Attack);
        }
    }

    public override void AfterAttack()
    {
        myBattleStat.RemoveStatModContinuous(atkMod);
    }
}

public class SelfDefenceSkill : Skill
{
    private const float ATK_BONUS_RATIO = 0.2f;
    private const int TICK_MULT = 4;
    private StatModContinuous atkMod;
    
    public override void InitSkill()
    {
        SetNameAndExplanation("자기 방어", "방어력의 20%만큼 공격력이 증가합니다.");
        SetMyBattleStat();

        atkMod = new StatModContinuous(StatType.Attack, ModType.Fixed, 0);
    }

    public override IEnumerator OnAlways()
    {
        while(true)
        {
            yield return new WaitForSeconds(SkillConsts.TICK_TIME * TICK_MULT);
            atkMod.ModValue = myBattleStat.Defence * ATK_BONUS_RATIO;
        }
    }

    public override void ApplyStatBonuses()
    {
        myBattleStat.AddStatModContinuous(atkMod);
    }

    public override void RemoveStatBonuses()
    {
        myBattleStat.RemoveStatModContinuous(atkMod);
    }
}

public class ScarSkill : Skill
{
    private const float ATKSPD_BONUS_UNIT = 0.01f;
    private const int MISSING_HEALTH_UNIT = 50;
    private const int ATKSPD_BONUS_LIMIT = 20;
    private StatModContinuous atkspdMod;
    private int multiplier;

    public override void InitSkill()
    {
        SetNameAndExplanation("상흔", "잃은 체력 50당 공격 속도가 1% 증가합니다. 최대치는 20%입니다.");
        SetMyBattleStat();
        atkspdMod = new StatModContinuous(StatType.AttackSpeed, ModType.Mult, 0.0f);
    }

    public override IEnumerator OnAlways()
    {
        while(true)
        {
            yield return new WaitForSeconds(SkillConsts.TICK_TIME);

            multiplier = ((int)myBattleStat.MissingHealth) / MISSING_HEALTH_UNIT;

            if (multiplier > ATKSPD_BONUS_LIMIT)
                multiplier = ATKSPD_BONUS_LIMIT;
            atkspdMod.ModValue = multiplier * ATKSPD_BONUS_UNIT;
        }
    }

    public override void ApplyStatBonuses()
    {
        myBattleStat.AddStatModContinuous(atkspdMod);
    }

    public override void RemoveStatBonuses()
    {
        myBattleStat.RemoveStatModContinuous(atkspdMod);
    }
}

public class LifeTapSkill : Skill
{
    private const float MAXHP_PENALTY_RATE = -0.2f;
    private const float ATK_BONUS_RATIO = 0.03f;
    private const int TICK_MULT = 4;
    private StatModContinuous hpMod;
    private StatModContinuous atkMod;

    public override void InitSkill()
    {
        SetNameAndExplanation("생명력 전환", "최대 체력이 20% 감소합니다. 감소한 최대 체력의 15%만큼 공격력이 증가합니다.");
        SetMyBattleStat();

        hpMod = new StatModContinuous(StatType.HealthMax, ModType.Mult, MAXHP_PENALTY_RATE);
        atkMod = new StatModContinuous(StatType.Attack, ModType.Fixed, 0);
    }

    public override IEnumerator OnAlways()
    {
        while (true)
        {
            yield return new WaitForSeconds(SkillConsts.TICK_TIME * TICK_MULT);
            atkMod.ModValue = myBattleStat.HealthMax * ATK_BONUS_RATIO;
        }
    }

    public override void ApplyStatBonuses()
    {
        myBattleStat.AddStatModContinuous(hpMod);
        myBattleStat.AddStatModContinuous(atkMod);
    }

    public override void RemoveStatBonuses()
    {
        myBattleStat.RemoveStatModContinuous(hpMod);
        myBattleStat.RemoveStatModContinuous(atkMod);
    }
}


//public class MaxiUniqueSkill : Skill
//{
//    StatModContinuous atkMod;
//    const float ATK_BONUS_RATE = 0.4f;
//    StatModContinuous atkSpdMod;
//    const float ATKSPD_PENALTY_RATE = -0.33f;

//    public override void InitSkill()
//    {
//        SetNameAndExplanation("쇼맨십", "공격속도가 200% 증가하지만 공격력은 65% 감소합니다.");
//    }
//}