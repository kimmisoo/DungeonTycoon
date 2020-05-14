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

        if(!isDodged && actualDamage != 0)
        {
            targets.Clear();
            SetEnemy();
            targets.Add(enemy);

            attackCnt++;
            if(attackCnt % INVOKE_PERIOD == 0)
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

        if(isDodged)
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