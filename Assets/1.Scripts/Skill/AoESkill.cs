using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AoESkill : Skill
{
    protected List<TileForMove> effectedAreas; // 매 공격시, 효과 대상 TFM
    protected List<Coverage> coverages; // 스킬별 고유의 효과지역 모양. {0, 0}이 타겟 좌표, y축이 중심축(소유자와 타겟의).

    protected class Coverage // 0, 0인 
    {
        public int x, y;

        public Coverage(int xIn, int yIn)
        {
            x = xIn;
            y = yIn;
        }
    }
    protected enum CentralAxis { X, Y, InverseX, InverseY }

    /// <summary>
    /// 효과범위를 저장하는 메서드. 하위 클래스에서 구현해주면 됨.
    /// </summary>
    public abstract void SetCoverage();

    public AoESkill()
    {
        //targets = new List<ICombatant>();
        effectedAreas = new List<TileForMove>();
        coverages = new List<Coverage>();
        SetCoverage();
    }

    /// <summary>
    /// 효과받는 타일을 구해서 effectedAreas에 Add
    /// </summary>
    /// <param name="mainTarget">효과범위의 중심이 되는 Actor</param>
    protected void GetArea(ICombatant mainTarget)
    {
        TileLayer tileLayer = GameManager.Instance.GetTileLayer(0);
        TileForMove mainTargetPos = mainTarget.GetCurTileForMove();

        effectedAreas.Clear();

        CentralAxis centralAxis = GetCentralAxis(owner.GetCurTileForMove(), mainTarget.GetCurTileForMove());

        switch (centralAxis)
        {
            case CentralAxis.Y:
                foreach (Coverage item in coverages)
                    effectedAreas.Add(tileLayer.GetTileForMove(mainTargetPos.GetX() + item.x, mainTargetPos.GetY() + item.y));
                break;
            case CentralAxis.InverseY:
                foreach (Coverage item in coverages)
                    effectedAreas.Add(tileLayer.GetTileForMove(mainTargetPos.GetX() - item.x, mainTargetPos.GetY() - item.y));
                break;
            case CentralAxis.X:
                foreach (Coverage item in coverages)
                    effectedAreas.Add(tileLayer.GetTileForMove(mainTargetPos.GetX() + item.y, mainTargetPos.GetY() + item.x));
                break;
            case CentralAxis.InverseX:
                foreach (Coverage item in coverages)
                    effectedAreas.Add(tileLayer.GetTileForMove(mainTargetPos.GetX() - item.y, mainTargetPos.GetY() - item.x));
                break;
        }
    }

    // 효과범위의 회전을 결정하기 위한 메서드.
    protected CentralAxis GetCentralAxis(TileForMove myPos, TileForMove targetPos)
    {
        int xDisplacement = targetPos.GetX() - myPos.GetX();
        int yDisplacement = targetPos.GetY() - myPos.GetY();

        if (Mathf.Abs(yDisplacement) >= Mathf.Abs(xDisplacement))
        {
            if (yDisplacement >= 0)
                return CentralAxis.Y;
            else
                return CentralAxis.InverseY;
        }
        else
        {
            if (xDisplacement >= 0)
                return CentralAxis.X;
            else
                return CentralAxis.InverseX;
        }
    }

    /// <summary>
    /// 효과 범위 내의 ICombatant 중 적을 targets에 저장.
    /// </summary>
    protected void GetTargets()
    {
        List<Actor> recentActors;
        targets.Clear();

        foreach (TileForMove tfm in effectedAreas)
        {
            if (tfm != null)
            {
                recentActors = tfm.GetRecentActors();
                foreach (Actor actor in recentActors)
                {
                    if (actor is ICombatant)
                        targets.Add(actor as ICombatant);
                }
            }
        }
    }

    /// <summary>
    /// 효과 범위 내의 적을 찾아서 targets에 저장해놓음. GetArea()와 GetTargets()의 캡슐화 메서드
    /// </summary>
    /// <param name="mainTarget">효과범위 중심이 되는 대상</param>
    protected void FindEnemies(ICombatant mainTarget)
    {
        GetArea(mainTarget);
        GetTargets();
    }
}


public class HanaUniqueSkill : AoESkill
{
    float totalDmg;
    const float RATE_SINGLE = 0.15f;
    const float RATE_NORM = 0.1f;
    const float RATE_CHARGED = 1.7f;
    const float CHARGED_TRIGGER = 0.12f;
    const float TICK_MULT = 8;
    GameObject normEffects;
    GameObject chargedEffect;

    const float DEBUFF_RATE = -0.15f;
    const float DURATION = 4.0f;
    TemporaryEffect defDebuff;

    public HanaUniqueSkill()
    {
        totalDmg = 0;
        //SetCoverage();
        defDebuff = new TemporaryEffect("감전", DURATION);
        defDebuff.AddContinuousMod(new StatModContinuous(StatType.Defence, ModType.Mult, DEBUFF_RATE));
        ///defDebuff.AddContinuousMod(new StatModContinuous(StatType.Defence, ModType.Fixed, 10.0f));
    }

    public override void InitSkill()
    {
        normEffects = Instantiate((GameObject)Resources.Load("EffectPrefabs/HanaNorm_SkillEffect"));
        normEffects.transform.SetParent(owner.GetTransform());
        //        normEffects.transform.position = new Vector3(0, 0, 0);
        normEffects.transform.position = owner.GetPosition();
        //normEffects.GetComponent<ToggleEffect>().OffEffect();
        myBattleStat = owner.GetBattleStat();
        chargedEffect = Instantiate((GameObject)Resources.Load("EffectPrefabs/HanaCharged_SkillEffect"));

        SetNameAndExplanation("축전", "매 1초마다 주위 1칸 내의 모든 적에게 공격력의 10%(대상이 하나일 때는 15%)만큼 피해를 줍니다. 준 피해의 양이 하나 최대체력의 12%가 될 때마다 주위 1칸 내의 모든 적에게 공격력의 170% 피해를 주고 방어력을 15% 감소시키는 디버프를 남깁니다. 디버프는 4초간 지속됩니다.");
    }

    public override IEnumerator OnAlways()
    {
        while (true)
        {
            yield return new WaitForSeconds(TICK_MULT * SkillConsts.TICK_TIME);
            FindEnemies(owner);

            normEffects.GetComponent<AttackEffect>().StopEffect();

            if (owner.GetSuperState() == SuperState.Battle)
            {
                float dmg;
                bool isCrit;

                myBattleStat.CalDamage(out dmg, out isCrit);

                if (targets.Count == 0)
                {
                    // 효과범위 내에 아무도 없을 때는 아무거도 안함.
                }
                else if (totalDmg >= myBattleStat.HealthMax * CHARGED_TRIGGER)
                {
                    ChargedAttack(dmg, isCrit);
                }
                else
                {
                    NormAttack(dmg, isCrit);
                }
            }
        }
    }

    private void ChargedAttack(float damage, bool isCrit)
    {
        // 차지됐을 때의 광역데미지
        float dmg = damage;
        dmg *= RATE_CHARGED;

        AdditionalAttack(targets, dmg, myBattleStat.PenetrationFixed, myBattleStat.PenetrationMult, isCrit);
        totalDmg -= myBattleStat.HealthMax * CHARGED_TRIGGER;

        ApplyTemporaryEffect(targets, defDebuff, true);

        DisplayChargedEffect();
    }

    private void NormAttack(float damage, bool isCrit)
    {
        float dmg = damage;

        //Debug.Log("cnt: " + effectedAreas.Count);
        //foreach (TileForMove item in effectedAreas)
        //{
        //    Debug.Log("[" + item.GetX() + ", " + item.GetY() + "], Actor Cnt: " + item.GetRecentActors().Count);
        //}
        //Debug.Log("-----------");

        //Debug.Log("target Cnt: " + targets.Count);

        if (targets.Count == 1)
            dmg *= RATE_SINGLE;
        else
            dmg *= RATE_NORM;

        totalDmg += AdditionalAttack(targets, dmg, myBattleStat.PenetrationFixed, myBattleStat.PenetrationMult, isCrit);

        normEffects.GetComponent<AttackEffect>().StartEffect();
    }

    public override void SetCoverage()
    {
        coverages.Add(new Coverage(1, 0));
        coverages.Add(new Coverage(-1, 0));
        coverages.Add(new Coverage(0, 1));
        coverages.Add(new Coverage(0, -1));
        coverages.Add(new Coverage(0, 0));
    }

    public void DisplayChargedEffect()
    {
        chargedEffect.GetComponent<AttackEffect>().StartEffect();
        chargedEffect.transform.position = new Vector3(owner.GetPosition().x, owner.GetPosition().y + 0.2f, owner.GetPosition().z);
    }
}

public class IrisUniqueSkill : AoESkill
{
    private const float RATE_SINGLE = 2.4f;
    private const float RATE_NORM = 2.1f;
    private const float INVOKE_PERIOD = 7;
    private int attackCnt = 6;

    GameObject skillEffect;

    public IrisUniqueSkill()
    {
        attackCnt = 0;
        //SetCoverage();
    }

    public override void InitSkill()
    {
        skillEffect = Instantiate((GameObject)Resources.Load("EffectPrefabs/Iris_SkillEffect"));
        skillEffect.transform.SetParent(owner.GetTransform());
        myBattleStat = owner.GetBattleStat();

        SetNameAndExplanation("빙화", "전투 개시 시, 그 후 7번째 공격마다 얼음 마법을 시전하여 공격 대상과 그 주위 1칸 모든 적에게 공격력의 210%(대상이 하나일 때는 240%)의 피해를 줍니다.");
        //        normEffects.transform.position = new Vector3(0, 0, 0);
    }

    public override void OnAttack(float actualDamage, bool isCrit, bool isDodged)
    {
        attackCnt++;

        if (attackCnt % INVOKE_PERIOD == 0)
        {
            SetEnemy();
            FindEnemies(enemy);
            if (targets.Count == 1)
                AdditionalAttack(targets, actualDamage * RATE_SINGLE, myBattleStat.PenetrationFixed, myBattleStat.PenetrationMult, isCrit);
            else
                AdditionalAttack(targets, actualDamage * RATE_NORM, myBattleStat.PenetrationFixed, myBattleStat.PenetrationMult, isCrit);

            DisplaySkillEffect(skillEffect, enemy, false);
            skillEffect.transform.position = new Vector3(enemy.GetPosition().x, enemy.GetPosition().y * 0.9f + transform.position.y * 0.1f - 0.13f, enemy.GetPosition().z * 0.5f + transform.position.z * 0.5f);
        }
    }

    public override IEnumerator OnAlways()
    {
        yield return null;
    }

    public override void SetCoverage()
    {
        coverages.Add(new Coverage(1, 0));
        coverages.Add(new Coverage(-1, 0));
        coverages.Add(new Coverage(0, 1));
        coverages.Add(new Coverage(0, -1));
        coverages.Add(new Coverage(0, 0));
    }
}

public class SweepSkill : AoESkill
{
    const float ATTACK_RATE = 0.35f;

    public SweepSkill()
    {
        //SetCoverage();
    }
    public override void InitSkill()
    {
        SetNameAndExplanation("휩쓸기", "적을 공격할 때마다 공격 대상 양 옆의 적에게 공격력의 35%만큼 피해를 줍니다.");
        myBattleStat = owner.GetBattleStat();
    }

    public override void OnAttack(float actualDamage, bool isCrit, bool isDodged)
    {
        SetEnemy();
        FindEnemies(enemy);



        AdditionalAttack(targets, actualDamage * ATTACK_RATE, myBattleStat.PenetrationFixed, myBattleStat.PenetrationMult, isCrit);
    }

    public override void SetCoverage()
    {
        coverages.Add(new Coverage(1, 0));
        coverages.Add(new Coverage(-1, 0));
    }
}

public class AmplifySkill : AoESkill
{
    const float ATTACK_RATE = 0.15f;
    public override void InitSkill()
    {
        SetNameAndExplanation("증폭", "적을 공격할 때마다, 공격대상 주위 1칸 내의 모든 적에게 공격력의 15%만큼 피해를 입힙니다.");
        SetMyBattleStat();
        //SetCoverage();
    }

    public override void OnAttack(float actualDamage, bool isCrit, bool isDodged)
    {
        SetEnemy();
        FindEnemies(enemy);

        AdditionalAttack(targets, actualDamage * ATTACK_RATE, myBattleStat.PenetrationFixed, myBattleStat.PenetrationMult, isCrit);
    }

    public override void SetCoverage()
    {
        coverages.Add(new Coverage(1, 0));
        coverages.Add(new Coverage(-1, 0));
        coverages.Add(new Coverage(0, 1));
        coverages.Add(new Coverage(0, -1));
        coverages.Add(new Coverage(0, 0));
    }
}

public class ShockWaveSkill : AoESkill
{
    const float DAMAGE = 10.0f;

    public override void InitSkill()
    {
        SetNameAndExplanation("충격파 생성", "적을 공격할 때마다, 주위 1칸 내의 모든 적에게 10만큼의 피해를 입힙니다.");
        SetMyBattleStat();
        //SetCoverage();
    }

    public override void OnAttack(float actualDamage, bool isCrit, bool isDodged)
    {
        SetEnemy();
        FindEnemies(owner);

        AdditionalAttack(targets, DAMAGE, myBattleStat.PenetrationFixed, myBattleStat.PenetrationMult, isCrit);
    }

    public override void SetCoverage()
    {
        coverages.Add(new Coverage(1, 0));
        coverages.Add(new Coverage(-1, 0));
        coverages.Add(new Coverage(0, 1));
        coverages.Add(new Coverage(0, -1));
        coverages.Add(new Coverage(0, 0));
    }
}

public class FlameAuraSkill : AoESkill
{
    const float ATTACK_RATE = 0.08f;
    const int TICK_MULT = 4;
    GameObject skillEffect;
    string prefabRoot = "EffectPrefabs/FlameAura_SkillEffect";

    public override void InitSkill()
    {
        SetNameAndExplanation("화염 오라", "매 1초마다, 주위 1칸 내의 모든 적에게 공격력의 8%만큼의 피해를 입힙니다.");
        //SetCoverage();
        SetMyBattleStat();

        skillEffect = Instantiate((GameObject)Resources.Load("EffectPrefabs/FlameAura_SkillEffect"));
    }

    public override void SetCoverage()
    {
        coverages.Add(new Coverage(1, 0));
        coverages.Add(new Coverage(-1, 0));
        coverages.Add(new Coverage(0, 1));
        coverages.Add(new Coverage(0, -1));
        coverages.Add(new Coverage(0, 0));
    }

    public override IEnumerator OnAlways()
    {
        while(true)
        {
            yield return new WaitForSeconds(SkillConsts.TICK_TIME * TICK_MULT);
            if (owner.GetSuperState() == SuperState.Battle)
            {
                FindEnemies(owner);

                if (targets.Count != 0)
                {
                    float dmg;
                    bool isCrit;
                    myBattleStat.CalDamage(out dmg, out isCrit);

                    AdditionalAttack(targets, dmg * ATTACK_RATE, myBattleStat.PenetrationFixed, myBattleStat.PenetrationMult, isCrit);
                }
                //DisplaySkillEffect(skillEffect, targets[0], false);
                DisplaySkillEffect(skillEffect, targets, false);
            }
        }
    }
}

public class DivineProtectionSkill : AoESkill
{
    private const int TICK_MULT = 4;
    private const float DAMAGE = 12;
    private const float DEF_PENALTY = -15;
    private const float DURATION = 2.0f;
    private TemporaryEffect defDebuff;
    private GameObject skillEffect;

    public override void InitSkill()
    {
        SetNameAndExplanation("신의 가호", "매 1초마다, 주위 1칸 내의 모든 적에게 12만큼의 피해를 주고 방어력을 15 감소시키는 디버프를 남깁니다. 피해는 적의 방어력을 무시하며, 디버프는 2초 동안 지속됩니다.");
        //SetCoverage();
        SetMyBattleStat();
        defDebuff = new TemporaryEffect("눈부심", DURATION);
        defDebuff.AddContinuousMod(new StatModContinuous(StatType.Defence, ModType.Fixed, DEF_PENALTY));

        skillEffect = Instantiate((GameObject)Resources.Load("EffectPrefabs/DivineProtection_SkillEffect"));
        skillEffect.transform.SetParent(owner.GetTransform());
        skillEffect.transform.position = owner.GetPosition() + new Vector3(0, -0.11f, 0);
    }

    public override void SetCoverage()
    {
        coverages.Add(new Coverage(1, 0));
        coverages.Add(new Coverage(-1, 0));
        coverages.Add(new Coverage(0, 1));
        coverages.Add(new Coverage(0, -1));
        coverages.Add(new Coverage(0, 0));
    }

    public override IEnumerator OnAlways()
    {
        while(true)
        {
            yield return new WaitForSeconds(SkillConsts.TICK_TIME * TICK_MULT);
            if (owner.GetSuperState() == SuperState.Battle)
            {
                FindEnemies(owner);
                AdditionalAttack(targets, DAMAGE, 0.0f, 1.0f, false);

                ApplyTemporaryEffect(targets, defDebuff, true);
                skillEffect.GetComponent<AttackEffect>().StartEffect();
            }
        }
    }
}

public class SiphonStrengthSkill : AoESkill
{
    TemporaryEffect siphonStrengthBuff;
    StatModContinuous atkMod;
    TemporaryEffect siphonStrengthDebuff;
    float SIPHON_RATE = 0.1f;
    float DURATION = 0.5f;

    GameObject skillEffect;

    public override void InitSkill()
    {
        SetNameAndExplanation("힘 흡수", "주위 1칸 내의 모든 적 공격력의 10%를 흡수합니다.");
        SetMyBattleStat();

        siphonStrengthDebuff = new TemporaryEffect("빠져나간 힘", DURATION);

        siphonStrengthBuff = new TemporaryEffect("흡수한 힘", DURATION);
        atkMod = new StatModContinuous(StatType.Attack, ModType.Fixed, 0.0f);
        siphonStrengthBuff.AddContinuousMod(atkMod);

        skillEffect = Instantiate((GameObject)Resources.Load("EffectPrefabs/SiphonStrength_SkillEffect"));
        skillEffect.transform.SetParent(owner.GetTransform());
        skillEffect.transform.position = owner.GetPosition() + new Vector3(0, -0.11f, 0);
        //SetCoverage();
    }

    public override void SetCoverage()
    {
        coverages.Add(new Coverage(1, 0));
        coverages.Add(new Coverage(-1, 0));
        coverages.Add(new Coverage(0, 1));
        coverages.Add(new Coverage(0, -1));
        coverages.Add(new Coverage(0, 0));
    }

    public override IEnumerator OnAlways()
    {
        while(true)
        {
            yield return new WaitForSeconds(SkillConsts.TICK_TIME);
            FindEnemies(owner);

            float modValueBefore = atkMod.ModValue;
            atkMod.ModValue = SiphonFromArea();
            if(atkMod.ModValue > 0.0f + Mathf.Epsilon)
            {
                //owner.DisplayBuff();

                Debug.Log("공격력 : " + myBattleStat.Attack + ", 베이스 : " + myBattleStat.BaseAttack);
                ApplyTemporaryEffect(owner, siphonStrengthBuff, false);

                if(modValueBefore <= 0.0f + Mathf.Epsilon)
                    skillEffect.GetComponent<AttackEffect>().StartEffect();
            }
        }
    }

    private float SiphonFromArea()
    {
        float totalSiphonedAtk = 0;

        foreach (ICombatant target in targets)
        {
            if(IsHostile(target))
            {
                float siphonedAtk = target.GetBattleStat().Attack * SIPHON_RATE;
                totalSiphonedAtk += siphonedAtk;
                TemporaryEffect temp = new TemporaryEffect(siphonStrengthDebuff);
                temp.AddContinuousMod(new StatModContinuous(StatType.Attack, ModType.Fixed, siphonedAtk * -1));

                ApplyTemporaryEffect(target, temp, false);
            }
        }

        return totalSiphonedAtk;
    }
}

