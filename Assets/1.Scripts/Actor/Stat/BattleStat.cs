using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStat
{
	Dictionary<StatType, StatBaseContinuous> battleStatContinuous = new Dictionary<StatType, StatBaseContinuous>();
	Dictionary<StatType, StatBaseDiscrete> battleStatDiscrete = new Dictionary<StatType, StatBaseDiscrete>();

    List<StatModContinuous> shieldModsGC = new List<StatModContinuous>();

    int curExp;
    int nextExp;
    int level;

    public string ownerType = null; // 어떤 모험가의 능력치인지.(데이터 시트 확인용)

    public BattleStat()
    {
        battleStatContinuous.Add(StatType.HealthMax, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.Defence, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.Avoid, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.Attack, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.AttackSpeed, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.CriticalChance, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.CriticalDamage, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.PenetrationFixed, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.PenetrationMult, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.MoveSpeed, new StatBaseContinuous());
        battleStatDiscrete.Add(StatType.AttackRange, new StatBaseDiscrete());

        battleStatContinuous.Add(StatType.Shield, new StatBaseContinuous());

        battleStatContinuous.Add(StatType.Health, new StatBaseContinuous());

        ownerType = null;
        // 수정요망
        curExp = 0;
        NextExp = 150;
        level = 1;  
    }

    public BattleStat(BattleStat input)
    {
        battleStatContinuous.Add(StatType.HealthMax, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.Defence, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.Avoid, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.Attack, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.AttackSpeed, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.CriticalChance, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.CriticalDamage, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.PenetrationFixed, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.PenetrationMult, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.MoveSpeed, new StatBaseContinuous());
        battleStatDiscrete.Add(StatType.AttackRange, new StatBaseDiscrete());

        battleStatContinuous.Add(StatType.Shield, new StatBaseContinuous());

        battleStatContinuous.Add(StatType.Health, new StatBaseContinuous());

        //Debug.Log("input Range : "+input.BaseAttackRange);
        //Debug.Log("input MaxHP : " + input.BaseHealthMax);

        BaseHealthMax = input.BaseHealthMax;
        BaseDefence = input.BaseDefence;
        BaseAvoid = input.BaseAvoid;
        BaseAttack = input.BaseAttack;
        BaseAttackSpeed = input.BaseAttackSpeed;
        BaseCriticalChance = input.BaseCriticalChance;
        BaseCriticalDamage = input.BaseCriticalDamage;
        BasePenetrationFixed = input.BasePenetrationFixed;
        BaseMoveSpeed = input.BaseMoveSpeed;
        BaseAttackRange = input.BaseAttackRange;


        battleStatContinuous[StatType.Health].BaseValue = HealthMax;
        
        // TODO 수정요망
        curExp = 0;
        NextExp = input.NextExp;
        level = input.level;
        ownerType = input.ownerType;
    }

    private void SetBattleStat(BattleStat input)
    {
        BaseHealthMax = input.BaseHealthMax;
        BaseDefence = input.BaseDefence;
        BaseAvoid = input.BaseAvoid;
        BaseAttack = input.BaseAttack;
        BaseAttackSpeed = input.BaseAttackSpeed;
        BaseCriticalChance = input.BaseCriticalChance;
        BaseCriticalDamage = input.BaseCriticalDamage;
        BasePenetrationFixed = input.BasePenetrationFixed;
        BaseMoveSpeed = input.BaseMoveSpeed;
        BaseAttackRange = input.BaseAttackRange;
        BaseShield = input.BaseShield;

        battleStatContinuous[StatType.Health].BaseValue = HealthMax;

        // TODO 수정요망
        NextExp = input.NextExp;
        level = input.level;
    }

    public void ResetBattleStat()
    {
        battleStatContinuous[StatType.HealthMax].ClearStatModList();
        battleStatContinuous[StatType.Defence].ClearStatModList();
        battleStatContinuous[StatType.Avoid].ClearStatModList();
        battleStatContinuous[StatType.Attack].ClearStatModList();
        battleStatContinuous[StatType.AttackSpeed].ClearStatModList();
        battleStatContinuous[StatType.CriticalChance].ClearStatModList();
        battleStatContinuous[StatType.CriticalDamage].ClearStatModList();
        battleStatContinuous[StatType.PenetrationFixed].ClearStatModList();
        battleStatContinuous[StatType.MoveSpeed].ClearStatModList();
        battleStatDiscrete[StatType.AttackRange].ClearStatModList();

        // 현재 체력은 어떻게? 시작하고 버프같은 거 걸릴 수도 있는데?
        battleStatContinuous[StatType.Health].BaseValue = HealthMax;
    }

    public void AddStatModContinuous(StatModContinuous mod)
    {
        if (!battleStatContinuous[mod.StatType].GetModList().Contains(mod))
        {
            battleStatContinuous[mod.StatType].AddStatMod(mod);

            // 아이템 탈착을 통한 글리치 방지
            if (mod.StatType == StatType.HealthMax && Health > HealthMax)
                Health = HealthMax;
        }
    }
    public void RemoveStatModContinuous(StatModContinuous mod)
    {
        if (battleStatContinuous[mod.StatType].GetModList().Contains(mod))
        {
            battleStatContinuous[mod.StatType].RemoveStatMod(mod);

            if (mod.StatType == StatType.HealthMax && Health > HealthMax)
                Health = HealthMax;
        }
    }
    public void AddStatModDiscrete(StatModDiscrete mod)
    {
        if (!battleStatDiscrete[mod.StatType].GetModList().Contains(mod))
            battleStatDiscrete[mod.StatType].AddStatMod(mod);
    }
    public void RemoveStatModDiscrete(StatModDiscrete mod)
    {
        if (battleStatDiscrete[mod.StatType].GetModList().Contains(mod))
            battleStatDiscrete[mod.StatType].RemoveStatMod(mod);
    }
    public bool ContainsStatMod(StatModContinuous mod)
    {
        if (battleStatContinuous[mod.StatType].GetModList().Contains(mod))
            return true;
        else
            return false;
    }
    public bool ContainsStatMod(StatModDiscrete mod)
    {
        if (battleStatDiscrete[mod.StatType].GetModList().Contains(mod))
            return true;
        else
            return false;
    }
    public int Range
    {
        get
        {
            return battleStatDiscrete[StatType.AttackRange].GetCalculatedValue();
        }
    }

    public float HealthMax
    {
        get
        {
            return battleStatContinuous[StatType.HealthMax].GetCalculatedValue();
        }
    }

    public float Defence
    {
        get
        {
            return battleStatContinuous[StatType.Defence].GetCalculatedValue();
        }
    }

    public float Attack
    {
        get
        {
            return battleStatContinuous[StatType.Attack].GetCalculatedValue();
        }
    }

    public float Health
    {
        get
        {
            return battleStatContinuous[StatType.Health].BaseValue;
        }
        set // 수정해야할 수도 있음.
        {
            if(value > HealthMax)
                battleStatContinuous[StatType.Health].BaseValue = HealthMax;
            else if (value < 0)
                battleStatContinuous[StatType.Health].BaseValue = 0;
            else
                battleStatContinuous[StatType.Health].BaseValue = value;
        }
    }

    public float Shield
    {
        get
        {
            return battleStatContinuous[StatType.Shield].GetCalculatedValue();
        }
    }

    
    public float Heal(float healAmount)
    {
        float healed;
        if (healAmount <= MissingHealth)
        {
            healed = healAmount;
            Health += healAmount;
        }
        else
        {
            healed = MissingHealth;
            Health = HealthMax;
        }

        return healed;
    }

    public float PenetrationFixed
    {
        get
        {
            return battleStatContinuous[StatType.PenetrationFixed].GetCalculatedValue();
        }
        set // 수정해야할 수도 있음.
        {
            battleStatContinuous[StatType.PenetrationFixed].BaseValue = value;
        }
    }

    public float PenetrationMult
    {
        get
        {
            return battleStatContinuous[StatType.PenetrationMult].GetCalculatedValue();
        }
    }

    public float AttackSpeed
    {
        get
        {
            return battleStatContinuous[StatType.AttackSpeed].GetCalculatedValue();
        }
    }

    public int CurExp
    {
        get
        {
            return curExp;
        }
        set
        {
            Debug.Assert(value >= 0);

            curExp = value;
            if (CurExp >= NextExp)
            {
                CurExp -= NextExp;
                LevelUp();
            }
        }
    }

    public bool TakeExp(int expAmount)
    {
        Debug.Assert(expAmount >= 0);

        curExp += expAmount;

        if (CurExp >= NextExp)
        {
            CurExp -= NextExp;
            LevelUp();
            return true;
        }
        else
            return false;
    }

    public int NextExp
    {
        get
        {
            return nextExp;
        }
        set
        {
            Debug.Assert(value > 0);
            nextExp = value;
        }
    }

    public int Level
    {
        get
        {
            return level;
        }
        set
        {
            Debug.Assert(value > 0 && value <= 100);

            level = value;
        }
    }

    public bool CheckCrit()
    {
        float randNum = Random.Range(0.0f, 1.0f);
        float critChance = battleStatContinuous[StatType.CriticalChance].GetCalculatedValue();

        if (randNum <= critChance)
            return true;
        else
            return false;
    }

    // 데미지 계산식. 방어력 적용안된 순수 공격력.
    public void CalDamage(out float calculatedDamage, out bool isCrit)
    {
        float damage = battleStatContinuous[StatType.Attack].GetCalculatedValue();
        float randNum = Random.Range(0.0f, 1.0f);
        float critChance = battleStatContinuous[StatType.CriticalChance].GetCalculatedValue();
        float critDmg;

        isCrit = CheckCrit();
        if (isCrit)
        {
            critDmg = battleStatContinuous[StatType.CriticalDamage].GetCalculatedValue(); ;
            damage *= critDmg;
        }

        calculatedDamage = damage;
    }


	public void TakeDamage(float damage, float penFixed, float penMult,
        out float actualDamage, out bool isEvaded) //계산된 데미지로 피해 처리
	{
        if(EvasionAttempt())
        {
            // 회피 애니메이션이나 이펙트 관련 여기 넣을 것.
            // 회피 이펙트만 Monster쪽에서 처리.
            actualDamage = 0;
            isEvaded = true;
            return;
        }
        isEvaded = false;

        float def = Defence;
        def = (def - penFixed);
        if(def>0)
        {
            def = def * (1.0f - penMult); // 적용되는 실질 방어력
        }

        // 피격 애니메이션 및 이펙트 관련 여기 넣을 것.
        actualDamage = (damage / (1.0f + def / 100));
        float remainDamage = TakeDamageShield(actualDamage);
        Health -= remainDamage;

		return;
		//returns true if Dead

		/*if (currentShield + currentHealth - damage <= 0.0f)
		{
			//Dead
			currentShield = 0.0f;
			currentHealth = 0.0f;
			damageTakenSum += damage;
			return true;
		}
		else
		{
			damageTakenSum += damage;
			if (currentShield - damage < 0.0f)
			{
				damage = damage - currentShield;
				currentShield = 0.0f;
				currentHealth = currentHealth - damage;
			}
			else
			{
				currentShield = currentShield - damage;
			}
			return false;
		}*/
	}

    protected bool EvasionAttempt()
    {
        float randNum = Random.Range(0.0f, 1.0f);
        float avoidChance = battleStatContinuous[StatType.Avoid].GetCalculatedValue();

        return randNum <= avoidChance;
    }

    protected void LevelUp()
    {
        Level++;

        if (ownerType == "Adventurer")
            SetBattleStat(GameManager.Instance.GenBattleStat(Level));
        else
            SetBattleStat(GameManager.Instance.GenBattleStat(ownerType, Level));
    }

    #region SetBaseStats
    public float BaseHealthMax
    {
        get
        {
            return battleStatContinuous[StatType.HealthMax].BaseValue;
        }
        set
        {
            battleStatContinuous[StatType.HealthMax].BaseValue = value;
        }
    }

    public float BaseDefence
    {
        get
        {
            return battleStatContinuous[StatType.Defence].BaseValue;
        }
        set
        {
            battleStatContinuous[StatType.Defence].BaseValue = value;
        }
    }

    public float BaseAvoid
    {
        get
        {
            return battleStatContinuous[StatType.Avoid].BaseValue;
        }
        set
        {
            battleStatContinuous[StatType.Avoid].BaseValue = value;
        }
    }

    public float BaseAttack
    {
        get
        {
            return battleStatContinuous[StatType.Attack].BaseValue;
        }
        set
        {
            battleStatContinuous[StatType.Attack].BaseValue = value;
        }
    }

    public float BaseAttackSpeed
    {
        get
        {
            return battleStatContinuous[StatType.AttackSpeed].BaseValue;
        }
        set
        {
            battleStatContinuous[StatType.AttackSpeed].BaseValue = value;
        }
    }

    public float BaseCriticalChance
    {
        get
        {
            return battleStatContinuous[StatType.CriticalChance].BaseValue;
        }
        set
        {
            battleStatContinuous[StatType.CriticalChance].BaseValue = value;
        }
    }

    public float BaseCriticalDamage
    {
        get
        {
           return  battleStatContinuous[StatType.CriticalDamage].BaseValue;
        }
        set
        {
            battleStatContinuous[StatType.CriticalDamage].BaseValue = value;
        }
    }

    public float BasePenetrationFixed
    {
        get
        {
            return battleStatContinuous[StatType.PenetrationFixed].BaseValue;
        }
        set
        {
            battleStatContinuous[StatType.PenetrationFixed].BaseValue = value;
        }
    }

    public float BaseMoveSpeed
    {
        get
        {
            return battleStatContinuous[StatType.MoveSpeed].BaseValue;
        }
        set
        {
            battleStatContinuous[StatType.MoveSpeed].BaseValue = value;
        }
    }

    public int BaseAttackRange
    {
        get
        {
            return battleStatDiscrete[StatType.AttackRange].BaseValue;
        }
        set
        {
            battleStatDiscrete[StatType.AttackRange].BaseValue = value;
        }
    }

    public float BaseShield
    {
        get
        {
            return battleStatContinuous[StatType.Shield].BaseValue;
        }
        set
        {
            battleStatContinuous[StatType.Shield].BaseValue = value;
        }
    }


    public float MissingHealth
    {
        get
        {
            return HealthMax - Health;
        }
    }

    // 방어막 ModList 순회하며 데미지 적용해줌. ModType.Mult는 적용하지 않음. 기획에도 없음.
    private float TakeDamageShield(float damage)
    {
        List<StatModContinuous> shieldMods = battleStatContinuous[StatType.Shield].GetModList();
        float damageRemain = damage;
        shieldModsGC.Clear();

        foreach (StatModContinuous shield in shieldMods)
        {
            if(shield.ModType == ModType.Fixed)
            {
                if(damageRemain >= shield.ModValue)
                {
                    damageRemain -= shield.ModValue;
                    shield.ModValue = 0;
                    shieldModsGC.Add(shield);
                }
                else
                {
                    shield.ModValue -= damageRemain;
                    damageRemain = 0;
                    break;
                }
            }
        }

        foreach(StatModContinuous shieldBroken in shieldModsGC)
        {
            shieldMods.Remove(shieldBroken);
        }

        return damageRemain;
    }
    #endregion
}
