using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuratUniqueSkill : Skill
{
    int attackCnt = 0;
    const int INVOKE_PERIOD = 3;
    const float HEAL_MULT = 0.02f;
    const float ATTACK_MULT = 0.04f;
    GameObject skillEffect;

    public override void InitSkill()
    {
        attackCnt = 0;
        skillEffect = Instantiate((GameObject)Resources.Load("EffectPrefabs/Murat_SkillEffect"));
        skillEffect.transform.SetParent(GameObject.Find("EffectPool").transform);
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
                AdditionalAttack(targets, myBattleStat.HealthMax * ATTACK_MULT, 0, 1.0f, false);
                float healed = myBattleStat.Heal(myBattleStat.HealthMax * HEAL_MULT);
                if (healed > 1)
                    owner.DisplayHeal(healed);
                DisplaySkillEffect();
            }
        }
    }

    public override void OnStruck(float actualDamage, bool isCrit, bool isDodged)
    {    }

    public void DisplaySkillEffect()
    {
        skillEffect.transform.position = new Vector3(enemy.GetPosition().x * 0.9f + transform.position.x * 0.1f, enemy.GetPosition().y * 0.9f + transform.position.y * 0.1f, enemy.GetPosition().z * 0.5f + transform.position.z * 0.5f);
        skillEffect.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 180f));
        skillEffect.GetComponent<AttackEffect>().StartEffect();
    }
}

