using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStatus {


	public float healthMax { get; set; }
	public float currentHealth { get; set; }
	public float currentShield { get; set; }
	public float attack { get; set; }
	public float defence { get; set; }
	public float reduceDamageMult { get; set; }
	public float penetration { get; set; }
	public float avoidMult { get; set; }
	public float criticalChanceMult { get; set; }
	public float criticalDamageMult { get; set; }
	public bool isStunned { get; set; }
	public bool isImmunedStun { get; set; }
	public bool isDebuffed { get; set; }
	public float damageTakedSum { get; set; }
	public float movespeedMult { get; set; }
	public float movespeedMultFinal { get; set; } = 1.0f;
	public float attackspeedMult { get; set; }
	public float attackspeedMultFinal { get; set; } = 1.0f;
	public int attackRange { get; set; } = 1;
	public bool isHitRecent { get; set; } = false;
	public bool isCriticalRecent { get; set; } = false;
	public int invincibleCount { get; set; } = 0;
	//inventory 랑 옵저버?
	List<EquipmentEffect> equipmentEffectList;

	//
	public float GetCalculatedDamage(BattleStatus enemyBattleStatus, bool isCritical)
	{
		//return 0.0f;
		if (isCritical == true)
		{
			return enemyBattleStatus.GetCalculatedAttack() * enemyBattleStatus.GetCalculatedCriticalDamage() * 100.0f /
				(100.0f + (Mathf.Max(0.0f, GetCalculatedDefence() - enemyBattleStatus.GetCalculatedFixedPenetration()) * (2.0f - enemyBattleStatus.GetCalculatedRatioPenetration())));
		}
		else
		{
			return enemyBattleStatus.GetCalculatedAttack() * 100.0f /
				(100.0f + (Mathf.Max(0.0f, GetCalculatedDefence() - enemyBattleStatus.GetCalculatedFixedPenetration()) * (2.0f - enemyBattleStatus.GetCalculatedRatioPenetration())));
		}
	}
	public float GetCurrentHealth()
	{
		return currentHealth;
	}
	public float GetCurrentShield()
	{
		return currentShield;
	}
	public float GetHealthMax()
	{
		return healthMax;
	}
	public float GetCalculatedHealthMax()
	{
		return ((healthMax + GetHealthMaxFromEquipmentEffect()) * (1.0f + GetHealthMaxMultFromEquipmentEffect()) * (1.0f + GetHealthMaxMultFinalFromEquipmentEffect()));
	}
	public void SetHealthMax(float _healthMax)
	{
		healthMax = _healthMax;
	}

	public float GetCalculatedAttack()
	{
		//return 0.0f;
		//실제 공식에따라 ~~~ 공격력 리턴
		return ((attack + GetAttackFromEquipmentEffect()) * (1.0f + GetAttackMultFromEquipmentEffect()) * (1.0f + GetAttackMultFinalFromEquipmentEffect()));

	}
	public float GetCalculatedDefence()
	{
		return ((defence + GetDefenceFromEquipmentEffect()) * (1.0f + GetDefenceMultFromEquipmentEffect()) * (1.0f + GetDefenceMultFinalFromEquipmentEffect()));
	}
	public float GetCalculatedFixedPenetration()
	{
		return (penetration + GetPenetrationFromEquipmentEffect());
	}
	public float GetCalculatedRatioPenetration()
	{
		return (1.0f + GetPenetrationMultFromEquipmentEffect()) * (1.0f + GetPenetrationMultFinalFromEquipmentEffect()) >= 2.0f ?
				2.0f :
			   (1.0f + GetPenetrationMultFromEquipmentEffect()) * (1.0f + GetPenetrationMultFinalFromEquipmentEffect());
	}
	public float GetCalculatedAvoidMult()
	{
		return (avoidMult * (1.0f + GetAvoidMultFromEquipmentEffect()) * (1.0f + GetAvoidMultFinalFromEquipmentEffect()));
	}
	public float GetCalculatedCriticalChance()
	{
		return (criticalChanceMult * (1.0f + GetCriticalChanceMultFromEquipmentEffect()) * (1.0f + GetCriticalChanceMultFinalFromEquipmentEffect()));
	}
	public float GetCalculatedCriticalDamage()
	{
		return (criticalDamageMult * (1.0f + GetCriticalDamageMultFromEquipmentEffect()) * (1.0f + GetCriticalDamageMultFinalFromEquipmentEffect()));
	}
	public float GetCalculatedMovespeed()
	{
		return (movespeedMult * (1.0f + GetMovespeedMultFromEquipmentEffect()) * (1.0f + GetMovespeedMultFinalFromEquipmentEffect()));
	}
	public float GetCalculatedAttackspeed()
	{
		return (attackspeedMult * (1.0f + GetAttackspeedMultFromEquipmentEffect()) * (1.0f + GetAttackspeedMultFinalFromEquipmentEffect()));
	}
	public int GetCalculatedAttackRange()
	{
		return attackRange + GetAttackRangeFromEquipmentEffect();
	}
	public int GetCalculatedInvincibleCount()
	{
		return invincibleCount + GetInvincibleCountFromEquipmentEffect();
	}
	public bool GetCalculatedImmunedStun()
	{
		return GetImmunedStunFromEquipmentEffect();
	}

	public float GetHealthMaxFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.healthMax;
		}
		return sum;
	}
	public float GetHealthMaxMultFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.healthMaxMult;
		}
		return sum;
	}
	public float GetHealthMaxMultFinalFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.healthMaxMultFinal;
		}
		return sum;
	}
	public float GetShieldFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.shield;
		}
		return sum;
	}
	public float GetShieldMultFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.shieldMult;
		}
		return sum;
	}
	public float GetShieldMultFinalFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.shieldMultFinal;
		}
		return sum;
	}
	public float GetAttackFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.attack;
		}
		return sum;
	}
	public float GetAttackMultFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.attackMult;
		}
		return sum;
	}
	public float GetAttackMultFinalFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.attackMultFinal;
		}
		return sum;
	}
	public float GetDefenceFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.defence;
		}
		return sum;
	}
	public float GetDefenceMultFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.defenceMult;
		}
		return sum;
	}
	public float GetDefenceMultFinalFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.defenceMultFinal;
		}
		return sum;
	}
	public float GetPenetrationFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.penetration;
		}
		return sum;
	}
	public float GetPenetrationMultFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.penetrationMult;
		}
		return sum;
	}
	public float GetPenetrationMultFinalFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.penetrationMultFinal;
		}
		return sum;
	}
	public float GetAvoidMultFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.avoidMult;
		}
		return sum;
	}
	public float GetAvoidMultFinalFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.avoidMultFinal;
		}
		return sum;
	}
	public float GetCriticalChanceMultFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.criticalChanceMult;
		}
		return sum;
	}
	public float GetCriticalChanceMultFinalFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.criticalChanceMultFinal;
		}
		return sum;
	}
	public float GetCriticalDamageMultFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.criticalDamageMult;
		}
		return sum;
	}
	public float GetCriticalDamageMultFinalFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.criticalDamageMultFinal;
		}
		return sum;
	}
	public float GetMovespeedMultFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.movespeedMult;
		}
		return sum;
	}
	public float GetMovespeedMultFinalFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.movespeedMultFinal;
		}
		return sum;
	}
	public float GetAttackspeedMultFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.attackspeedMult;
		}
		return sum;
	}
	public float GetAttackspeedMultFinalFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.attackspeedMultFinal;
		}
		return sum;
	}
	public int GetAttackRangeFromEquipmentEffect()
	{
		int sum = 0;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.attackRange;
		}
		return sum;
	}
	public int GetInvincibleCountFromEquipmentEffect()
	{
		int sum = 0;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.invincibleCount;
		}
		return sum;
	}
	public bool GetImmunedStunFromEquipmentEffect()
	{
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			if (e.immunedStun == true)
			{
				return true;
			}
		}
		return false;
	}
	public bool TakeDamageProcess(float damage)
	{
		//returns true if Dead

		if(currentShield + currentHealth - damage <= 0.0f)
		{
			//Dead
			currentShield = 0.0f;
			currentHealth = 0.0f;
			damageTakedSum += damage;
			return true;
		}
		else
		{
			damageTakedSum += damage;
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
		}
	}
}
