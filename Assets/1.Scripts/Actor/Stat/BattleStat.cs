using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStat {

	Dictionary<StatType, StatBaseContinuous> battleStatContinuous = new Dictionary<StatType, StatBaseContinuous>();
	Dictionary<StatType, StatBaseDiscrete> battleStatDiscrete = new Dictionary<StatType, StatBaseDiscrete>();

    int curExp;
    int nextExp;
    int level;

    public BattleStat()
    {
        battleStatContinuous.Add(StatType.HealthMax, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.Defence, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.Attack, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.AttackSpeed, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.CriticalChance, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.CriticalDamage, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.PenetrationFixed, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.MoveSpeed, new StatBaseContinuous());
        battleStatDiscrete.Add(StatType.AttackRange, new StatBaseDiscrete());

        battleStatContinuous.Add(StatType.Health, new StatBaseContinuous());

        // 수정요망
        curExp = 0;
        NextExp = 150;
        level = 1;  
    }

    public BattleStat(BattleStat input)
    {
        battleStatContinuous.Add(StatType.HealthMax, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.Defence, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.Attack, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.AttackSpeed, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.CriticalChance, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.CriticalDamage, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.PenetrationFixed, new StatBaseContinuous());
        battleStatContinuous.Add(StatType.MoveSpeed, new StatBaseContinuous());
        battleStatDiscrete.Add(StatType.AttackRange, new StatBaseDiscrete());

        battleStatContinuous.Add(StatType.Health, new StatBaseContinuous());

        Debug.Log("input: "+input.BaseRange);

        BaseHealthMax = input.BaseHealthMax;
        BaseDefence = input.BaseDefence;
        BaseAttack = input.BaseAttack;
        BaseAttackSpeed = input.BaseAttackSpeed;
        BaseCriticalChance = input.BaseCriticalChance;
        BaseCriticalDamage = input.BaseCriticalDamage;
        BasePenetrationFixed = input.BasePenetrationFixed;
        BaseMoveSpeed = input.BaseMoveSpeed;
        BaseRange = input.BaseRange;

        // 수정요망
        curExp = 0;
        NextExp = 150;
        level = 1;
    }

    public void ResetBattleStat()
    {
        battleStatContinuous[StatType.HealthMax].ClearStatModList();
        battleStatContinuous[StatType.Defence].ClearStatModList();
        battleStatContinuous[StatType.Attack].ClearStatModList();
        battleStatContinuous[StatType.AttackSpeed].ClearStatModList();
        battleStatContinuous[StatType.CriticalChance].ClearStatModList();
        battleStatContinuous[StatType.CriticalDamage].ClearStatModList();
        battleStatContinuous[StatType.PenetrationFixed].ClearStatModList();
        battleStatContinuous[StatType.MoveSpeed].ClearStatModList();
        battleStatDiscrete[StatType.AttackRange].ClearStatModList();

        // 현재 체력은 어떻게? 시작하고 버프같은 거 걸릴 수도 있는데?
        battleStatContinuous[StatType.Health].baseValue = HealthMax;
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

    public float Health
    {
        get
        {
            return battleStatContinuous[StatType.Health].baseValue;
        }
        set // 수정해야할 수도 있음.
        {
            battleStatContinuous[StatType.Health].baseValue = value;
        }
    }

    public float PenetrationFixed
    {
        get
        {
            return battleStatContinuous[StatType.PenetrationFixed].GetCalculatedValue();
        }
        set // 수정해야할 수도 있음.
        {
            battleStatContinuous[StatType.PenetrationFixed].baseValue = value;
        }
    }

    public float PenetrationMult
    {
        get
        {
            return battleStatContinuous[StatType.PenetrationMult].GetCalculatedValue();
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


    // 데미지 계산식. 방어력 적용안된 순수 공격력.
    public void CalDamage(out float calculatedDamage, out bool isCrit)
    {
        float damage = battleStatContinuous[StatType.Attack].GetCalculatedValue();
        float randNum = Random.Range(0.0f, 1.0f);
        float critChance = battleStatContinuous[StatType.CriticalChance].GetCalculatedValue();
        float critDmg;

        isCrit = false;
        if (randNum<= critChance)
        {
            isCrit = true;
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
            def = def * (1 - penMult); // 적용되는 실질 방어력
        }

        // 피격 애니메이션 및 이펙트 관련 여기 넣을 것.
        actualDamage = (damage / (1 + def / 100));
        Health -= actualDamage;

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
        // 다음 레벨 테이블 불러와서 배틀스탯에 저장.
        // 애니메이션 트리거.
    }

    #region SetBaseStats
    public float BaseHealthMax
    {
        get
        {
            return battleStatContinuous[StatType.HealthMax].baseValue;
        }
        set
        {
            battleStatContinuous[StatType.HealthMax].baseValue = value;
        }
    }

    public float BaseDefence
    {
        get
        {
            return battleStatContinuous[StatType.Defence].baseValue;
        }
        set
        {
            battleStatContinuous[StatType.Defence].baseValue = value;
        }
    }

    public float BaseAttack
    {
        get
        {
            return battleStatContinuous[StatType.Attack].baseValue;
        }
        set
        {
            battleStatContinuous[StatType.Attack].baseValue = value;
        }
    }

    public float BaseAttackSpeed
    {
        get
        {
            return battleStatContinuous[StatType.AttackSpeed].baseValue;
        }
        set
        {
            battleStatContinuous[StatType.AttackSpeed].baseValue = value;
        }
    }

    public float BaseCriticalChance
    {
        get
        {
            return battleStatContinuous[StatType.CriticalChance].baseValue;
        }
        set
        {
            battleStatContinuous[StatType.CriticalChance].baseValue = value;
        }
    }

    public float BaseCriticalDamage
    {
        get
        {
           return  battleStatContinuous[StatType.CriticalDamage].baseValue;
        }
        set
        {
            battleStatContinuous[StatType.CriticalDamage].baseValue = value;
        }
    }

    public float BasePenetrationFixed
    {
        get
        {
            return battleStatContinuous[StatType.PenetrationFixed].baseValue;
        }
        set
        {
            battleStatContinuous[StatType.PenetrationFixed].baseValue = value;
        }
    }

    public float BaseMoveSpeed
    {
        get
        {
            return battleStatContinuous[StatType.MoveSpeed].baseValue;
        }
        set
        {
            battleStatContinuous[StatType.MoveSpeed].baseValue = value;
        }
    }

    public int BaseRange
    {
        get
        {
            return battleStatDiscrete[StatType.AttackRange].baseValue;
        }
        set
        {
            battleStatDiscrete[StatType.AttackRange].baseValue = value;
        }
    }
    #endregion
}
