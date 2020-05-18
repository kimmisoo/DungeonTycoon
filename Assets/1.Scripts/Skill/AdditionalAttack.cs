using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuratUniqueSkill : Skill
{
    int attackCnt = 0;
    const int INVOKE_PERIOD = 3;
    const float HEAL_RATE = 0.02f;
    const float ATTACK_RATE = 0.04f;
    GameObject skillEffect;

    public override void InitSkill()
    {
        attackCnt = 0;
        skillEffect = Instantiate((GameObject)Resources.Load("EffectPrefabs/Murat_SkillEffect"));
        skillEffect.transform.SetParent(GameObject.Find("EffectPool").transform);
        SetNameAndExplanation("광휘", "매 3번째 공격마다 최대 체력의 2%를 회복하고 그 2배의 피해를 적에게 줍니다. 적에게 주는 피해는 치명타가 발생하지 않는 대신, 적의 방어력을 무시합니다.");
    }

    public override IEnumerator OnAlways()
    {
        yield return null;
    }

    public override void OnAttack(float actualDamage, bool isCrit, bool isDodged)
    {
        BattleStat myBattleStat = owner.GetBattleStat();

        if (!isDodged && actualDamage != 0)
        {
            targets.Clear();
            SetEnemy();
            targets.Add(enemy);

            attackCnt++;
            if (attackCnt % INVOKE_PERIOD == 0)
            {
                AdditionalAttack(targets, myBattleStat.HealthMax * ATTACK_RATE, 0, 1.0f, false);
                float healed = myBattleStat.Heal(myBattleStat.HealthMax * HEAL_RATE);
                if (healed > 1)
                    owner.DisplayHeal(healed);
                DisplaySkillEffect();
            }
        }
    }

    public void DisplaySkillEffect()
    {
        skillEffect.transform.position = new Vector3(enemy.GetPosition().x * 0.9f + transform.position.x * 0.1f, enemy.GetPosition().y * 0.9f + transform.position.y * 0.1f, enemy.GetPosition().z * 0.5f + transform.position.z * 0.5f);
        skillEffect.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 180f));
        skillEffect.GetComponent<AttackEffect>().StartEffect();
    }
}

public class YeonhwaUniqueSkill : Skill
{
    const float ATTACK_RATE = 1.55f;
    GameObject skillEffect;
    BattleStat myBattleStat;

    public override void InitSkill()
    {
        skillEffect = Instantiate((GameObject)Resources.Load("EffectPrefabs/Yeonhwa_SkillEffect"));
        skillEffect.transform.SetParent(GameObject.Find("EffectPool").transform);
        SetNameAndExplanation("환류", "회피율이 10% 증가합니다. 적의 공격을 회피할 때마다, 공격력의 155%만큼의 피해를 공격자에게 줍니다.");
    }

    public override IEnumerator OnAlways()
    {
        yield return null;
    }

    public override void OnStruck(float actualDamage, bool isDodged, ICombatant attacker)
    {
        targets.Clear();

        if (isDodged)
        {
            myBattleStat = owner.GetBattleStat();
            float calculatedDamage;
            bool isCrit;
            myBattleStat.CalDamage(out calculatedDamage, out isCrit);
            AdditionalAttack(attacker, calculatedDamage, myBattleStat.PenetrationFixed, myBattleStat.PenetrationMult, isCrit);
            DisplaySkillEffect(skillEffect, attacker);
        }
    }
}

public class ThornMailSkill : Skill
{
    const float REFLECTION_RATE = 0.15f;
    public override void InitSkill()
    {
        SetNameAndExplanation("가시갑옷", "공격을 받을 때, 받은 데미지의 15%를 공격자에게 되돌려줍니다.");
    }

    public override void OnStruck(float actualDamage, bool isDodged, ICombatant attacker)
    {
        AdditionalAttack(attacker, actualDamage * REFLECTION_RATE, 0, 1.0f, false);
    }
}

public class RepulsivePowerSkill : Skill
{
    const float REFLECTION_RATE = 0.2f;
    BattleStat myBattleStat;
    StatModContinuous defenceStatMod;
    const float DEF_BONUS_RATE = 0.35f;

    public override void InitSkill()
    {
        SetNameAndExplanation("반발력", "방어력이 35% 증가합니다. 공격을 받을 때마다, 방어력의 20%만큼의 피해를 공격자에게 줍니다.");
        myBattleStat = owner.GetBattleStat();
        defenceStatMod = new StatModContinuous(StatType.Defence, ModType.Mult, DEF_BONUS_RATE);
    }

    public override void OnStruck(float actualDamage, bool isDodged, ICombatant attacker)
    {
        AdditionalAttack(attacker, myBattleStat.Defence * REFLECTION_RATE, 0, 1.0f, false);
    }

    public override void ApplyStatBonuses()
    {
        myBattleStat.AddStatModContinuous(defenceStatMod);
    }

    public override void RemoveStatBonuses()
    {
        myBattleStat.RemoveStatModContinuous(defenceStatMod);
    }
}

public class StaticElectricitySkill : Skill
{
    const float DAMAGE = 4.0f;

    public override void InitSkill()
    {
        SetNameAndExplanation("정전기 발생", "적을 공격할 때마다 4 공격력의 추가 공격을 합니다.");
        SetMyBattleStat();
    }

    public override void OnAttack(float actualDamage, bool isCrit, bool isDodged)
    {
        SetEnemy();
        AdditionalAttack(enemy, DAMAGE, myBattleStat.PenetrationFixed, myBattleStat.PenetrationMult, isCrit);
    }
}

public class ThunderboltSkill : Skill
{
    const float DAMAGE = 60.0f;

    TemporaryEffect defDebuff;
    const float DEF_DEBUFF_VALUE = -10.0f;
    const float DURATION = 3.0f;

    GameObject skillEffect;

    public override void InitSkill()
    {
        SetNameAndExplanation("벼락", "적을 공격할 때 치명타가 발생하면 60의 추가 데미지를 주고 적의 방어력을 10 감소시키는 디버프를 남깁니다. 디버프는 3초간 지속됩니다.");
        defDebuff = new TemporaryEffect(DURATION);
        defDebuff.AddContinuousMod(new StatModContinuous(StatType.Defence, ModType.Fixed, DEF_DEBUFF_VALUE));

        skillEffect = Instantiate((GameObject)Resources.Load("EffectPrefabs/HanaNorm_SkillEffect"));
        skillEffect.transform.SetParent(owner.GetTransform());
    }

    public override void OnAttack(float actualDamage, bool isCrit, bool isDodged)
    {
        if (isCrit)
        {
            SetEnemy();
            SetMyBattleStat();
            AdditionalAttack(enemy, DAMAGE, myBattleStat.PenetrationFixed, myBattleStat.PenetrationMult, false);
            ApplyTemporaryEffect(enemy, defDebuff, false);

            DisplaySkillEffect(skillEffect, enemy, true);
        }
    }
}

public class BlazeSkill : Skill
{
    const float DAMAGE = 12.0f;
    GameObject skillEffect;

    public override void InitSkill()
    {
        SetNameAndExplanation("불꽃", "적을 공격할 때마다, 12의 추가피해를 줍니다.");
        skillEffect = Instantiate((GameObject)Resources.Load("EffectPrefabs/Blaze_SkillEffect"));
        skillEffect.transform.SetParent(owner.GetTransform());
    }

    public override void OnAttack(float actualDamage, bool isCrit, bool isDodged)
    {
        SetEnemy();
        SetMyBattleStat();

        float additionalAttackDmg = DAMAGE;
        bool isAdditionalAttackCrit = myBattleStat.CheckCrit();
        if (isAdditionalAttackCrit)
            additionalAttackDmg *= 2;

        AdditionalAttack(enemy, additionalAttackDmg, myBattleStat.PenetrationFixed, myBattleStat.PenetrationMult, isAdditionalAttackCrit);

        DisplaySkillEffect(skillEffect, enemy, true);
    }
}

public class LavaSkill : Skill
{
    const float DAMAGE = 26.0f;
    GameObject skillEffect;

    public override void InitSkill()
    {
        SetNameAndExplanation("용암", "적을 공격할 때마다, 26의 추가피해를 줍니다.");
        skillEffect = Instantiate((GameObject)Resources.Load("EffectPrefabs/Lava_SkillEffect"));
        skillEffect.transform.SetParent(owner.GetTransform());
    }

    public override void OnAttack(float actualDamage, bool isCrit, bool isDodged)
    {
        SetEnemy();
        SetMyBattleStat();

        float additionalAttackDmg = DAMAGE;
        bool isAdditionalAttackCrit = myBattleStat.CheckCrit();
        if (isAdditionalAttackCrit)
            additionalAttackDmg *= 2;

        AdditionalAttack(enemy, additionalAttackDmg, myBattleStat.PenetrationFixed, myBattleStat.PenetrationMult, isAdditionalAttackCrit);

        DisplaySkillEffect(skillEffect, enemy, true);
    }
}