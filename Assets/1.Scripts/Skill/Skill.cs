//#define DEBUG_TEMP_EFFECT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillConsts
{
    public const float TICK_TIME = 0.25f;
    public const float EFFECT_DESTROY_DELAY = 3.0f;
}
/// <summary>
/// 상위클래스 샌드박스 패턴 사용. 
/// </summary>
public abstract class Skill : MonoBehaviour
{
    public ICombatant owner;
    protected BattleStat myBattleStat;
    public ICombatant enemy;
    protected Coroutine curCoroutine;
    protected List<ICombatant> targets; // 효과 대상 TFM 위에 있는 적들
    public bool isActive;
    public string key; // 스킬 생성하고 데이터 받을 때 사용하는 키

    public string Name // 스킬 이름
    {
        get { return name; }
    }
    protected string name;
    public string Explanation // 스킬 설명
    {
        get { return explanation; }
    }
    protected string explanation;
    protected Sprite icon;

    //protected const float TICK_TIME = 0.25f;

    public Skill()
    {
        targets = new List<ICombatant>();
        isActive = false;
    }

    public void SetOwner(ICombatant ownerIn)
    {
        owner = ownerIn;
    }

    public void SetEnemy()
    {
        enemy = owner.GetEnemy();
    }

    protected void SetMyBattleStat()
    {
        myBattleStat = owner.GetBattleStat();
    }


    public abstract void InitSkill();
    /// <summary>
    /// 맞을 때 발생하는 효과
    /// </summary>
    public virtual void OnStruck(float actualDamage, bool isDodged, ICombatant attacker) { }

    /// <summary>
    /// 때리기 직전 발생하는 효과. 주로 능력치 버프류.
    /// </summary>
    public virtual void BeforeAttack() { }
    /// <summary>
    /// 때릴 때 발생하는 효과
    /// </summary>
    public virtual void OnAttack(float actualDamage, bool isCrit, bool isDodged) { }
    /// <summary>
    /// 때린 후 발생하는 효과. 주로 능력치 버프를 해제.
    /// </summary>
    public virtual void AfterAttack() { }
    /// <summary>
    /// 항시발동 및 일정시간마다 발동. 코루틴 실행하는 메서드. 단위시간은 tickTime. 항시발동은 단위시간마다 체크.
    /// 게임 시작 혹은 아이템 착용시마다 실행.
    /// </summary>
    public void Activate()
    {
        ApplyStatBonuses();
        curCoroutine = StartCoroutine(OnAlways());
        isActive = true;
    }
    /// <summary>
    /// Activate()에서 실행한 코루틴 정지시키는 메서드. 모험가 사망 혹은 아이템 해제시마다 실행.
    /// </summary>
    public void Deactivate()
    {
        RemoveStatBonuses();
        StopCoroutine(curCoroutine);
        isActive = false;
    }

    public virtual void ApplyStatBonuses() { }
    public virtual void RemoveStatBonuses() { }

    public virtual IEnumerator OnAlways()
    {
        yield return null;
    }

    public float AdditionalAttack(List<ICombatant> targets, float damage, float penFixed, float penMult, bool isCrit)
    {
        float totalDamage = 0;

        //Debug.Log("enemies Cnt : " + enemies.Count);
        foreach (ICombatant target in targets)
        {
            //Debug.Log("is Monster : " + (target is Monster));
            if (IsHostile(target))
                totalDamage += AdditionalAttack(target, damage, penFixed, penMult, isCrit);
        }

        return totalDamage;
    }

    public float AdditionalAttack(ICombatant target, float damage, float penFixed, float penMult, bool isCrit)
    {
        float actualDamage = 0;

        target.TakeDamage(owner, damage, penFixed, penMult, isCrit, out actualDamage);

        return actualDamage;
    }

    public void ApplyTemporaryEffect(List<ICombatant> targets, TemporaryEffect effect, bool toHostile, bool createNew = true)
    {
        foreach(ICombatant target in targets)
        {
            if(toHostile && IsHostile(target))
                ApplyTemporaryEffect(target, effect, createNew);
            else if(!toHostile && !IsHostile(target))
                ApplyTemporaryEffect(target, effect, createNew);
        }
    }

    /// <summary>
    /// 대상에게 버프 or 디버프를 적용
    /// </summary>
    /// <param name="target">대상</param>
    /// <param name="effect">버프나 디버프의 프로토타입</param>
    /// <param name="createNew">전달된 그대로 적용할 건지, 새 개체를 생성해서 적용할 건지. 중첩가능한 스킬은 false</param>
    public void ApplyTemporaryEffect(ICombatant target, TemporaryEffect effect, bool createNew = true)
    {
#if DEBUG_TEMP_EFFECT
        Debug.Log("적: " + target);
#endif
        if (createNew)
            target.AddTemporaryEffect(new TemporaryEffect(effect));
        else
            target.AddTemporaryEffect(effect);
    }

    /// <summary>
    /// 적인지 아닌지 판단. 다른 타입(Adventurer<->Monster)이거나 enemy(PvP는 무조건 1:1이므로)일 때만 true
    /// </summary>
    protected bool IsHostile(ICombatant target)
    {
        SetEnemy();

        if (owner is Adventurer)
        {
            if ((target is Monster || target == enemy) && owner.ValidatingEnemy(target))
                return true;
            else
                return false;
        }
        else
        {
            if ((target is Adventurer || target == enemy) && owner.ValidatingEnemy(target))
                return true;
            else
                return false;
        }
    }

    protected void DisplaySkillEffect(GameObject skillEffect, List<ICombatant> targets, bool randomRotation = true, bool toHostile = true)
    {
        GameObject temp;
        foreach (ICombatant target in targets)
        {
            if (toHostile && IsHostile(target))
            {
                temp = Instantiate(skillEffect);
                DisplaySkillEffect(temp, target, randomRotation);
                Destroy(temp, SkillConsts.EFFECT_DESTROY_DELAY);
            }
            else if (!toHostile && !IsHostile(target))
            {
                temp = Instantiate(skillEffect);
                DisplaySkillEffect(Instantiate(skillEffect), target, randomRotation);
                Destroy(temp, SkillConsts.EFFECT_DESTROY_DELAY);
            }
        }
        
        //foreach (ICombatant target in targets)
        //{
        //    if (toHostile && IsHostile(target))
        //        DisplaySkillEffect(Instantiate((GameObject)Resources.Load(prefabRoot)), target, randomRotation);
        //    else if (!toHostile && !IsHostile(target))
        //        DisplaySkillEffect(Instantiate((GameObject)Resources.Load(prefabRoot)), target, randomRotation);
        //}
    }

    protected void DisplaySkillEffect(GameObject skillEffect, ICombatant target, bool randomRotation = true)
    {
        skillEffect.transform.position = new Vector3(target.GetPosition().x * 0.9f + transform.position.x * 0.1f, target.GetPosition().y * 0.9f + transform.position.y * 0.1f, target.GetPosition().z * 0.5f + transform.position.z * 0.5f);
        if (randomRotation)
            skillEffect.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
        skillEffect.GetComponent<AttackEffect>().StartEffect();
    }

    //public void SetNameAndExplanation(string nameIn, string explanationIn)
    //{
    //    name = nameIn;
    //    explanation = explanationIn;
    //    // 아마 여기서 아이콘 설정은 이름에 따라 Resource.Load 해주면 될 거 같음
    //    // 일단 스킬 아이콘은 리소스도 없고 하니 스킵
    //}

    public void SetNameAndExplanation(string key)
    {
        this.key = key;

        SkillFactory.GetNameAndExplanation(key, out name, out explanation);
        // 아마 여기서 아이콘 설정은 이름에 따라 Resource.Load 해주면 될 거 같음
        // 일단 스킬 아이콘은 리소스도 없고 하니 스킵
    }
}