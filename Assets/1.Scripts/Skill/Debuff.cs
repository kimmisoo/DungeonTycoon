using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public abstract class DebuffSkill : Skill
//{
//    protected TemporaryEffect debuffPrototype;
//    GameObject debuffEffect;

//    protected void SetDebuffEffect()
//    {
//        debuffEffect = Instantiate((GameObject)Resources.Load("EffectPrefabs/Default_DebuffEffect"));
//        debuffEffect.transform.SetParent(owner.GetTransform());
//        debuffEffect.transform.position = owner.GetPosition();
//    }

//    protected void DisplayDebuffEffect(ICombatant target)
//    {
//        Vector3 targetPos = target.GetPosition();

//        debuffEffect.GetComponent<AttackEffect>().StartEffect();
//        debuffEffect.transform.position = new Vector3(owner.GetPosition().x, owner.GetPosition().y + 0.08f, owner.GetPosition().z);
//    }
//}

public class CrackSkill : Skill
{
    const float DEF_PENALTY_RATE = -0.06f;
    TemporaryEffect defDebuff;
    StatModContinuous defMod;
    const int STACK_LIMIT = 5;

    public override void InitSkill()
    {
        //SetNameAndExplanation("균열", "적을 공격할 때마다 방어력을 6% 감소시키는 디버프를 남깁니다. 디버프는 최대 5번까지 중첩되며 2초간 지속됩니다.");
        defDebuff = new TemporaryEffect(skillName, 2, STACK_LIMIT);
        defMod = new StatModContinuous(StatType.Defence, ModType.Mult, DEF_PENALTY_RATE);
        defDebuff.AddContinuousMod(defMod);
    }

    public override void OnAttack(float actualDamage, bool isCrit, bool isDodged)
    {
        SetEnemy();

        ApplyTemporaryEffect(enemy, defDebuff, false);
        enemy.DisplayDebuff();
    }
}