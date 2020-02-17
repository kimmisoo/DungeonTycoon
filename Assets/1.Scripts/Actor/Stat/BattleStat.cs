using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStat {

	Dictionary<StatType, StatBaseContinuous> battleStatContinuous = new Dictionary<StatType, StatBaseContinuous>();
	Dictionary<StatType, StatBaseDiscrete> battleStatDiscrete = new Dictionary<StatType, StatBaseDiscrete>();
	
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
    }

    public float PenetrationMult
    {
        get
        {
            return battleStatContinuous[StatType.PenetrationMult].GetCalculatedValue();
        }
    }


    // 데미지 계산식. 방어력 적용안된 순수 공격력.
    public float CalDamage()
    {
        float damage = battleStatContinuous[StatType.Attack].GetCalculatedValue();
        float randNum = Random.Range(0.0f, 1.0f);
        float critChance = battleStatContinuous[StatType.CriticalChance].GetCalculatedValue();
        float critDmg;

        if (randNum<= critChance)
        {
            critDmg = battleStatContinuous[StatType.CriticalDamage].GetCalculatedValue(); ;
            damage *= critDmg;
        }

        return damage;
    }


	public void TakeDamage(float damage, float penFixed, float penMult) //계산된 데미지로 피해 처리
	{
        if(EvasionAttempt())
        {
            // 회피 애니메이션이나 이펙트 관련 여기 넣을 것.
            damage = 0;
            return;
        }

        float def = battleStatContinuous[StatType.Defence].GetCalculatedValue();
        def = (def - penFixed);
        if(def>0)
        {
            def = def * (1 - penMult); // 적용되는 실질 방어력
        }

        // 피격 애니메이션 및 이펙트 관련 여기 넣을 것.
        Health -= (damage / (1 + def/100));

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

    private bool EvasionAttempt()
    {
        float randNum = Random.Range(0.0f, 1.0f);
        float avoidChance = battleStatContinuous[StatType.Avoid].GetCalculatedValue();

        return randNum <= avoidChance;
    }
}
