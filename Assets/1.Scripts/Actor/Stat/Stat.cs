using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Race
{
	Human, Elf, Dwarf, Orc, Dog, Cat
}
public enum Wealth
{
	Upper, Middle, Lower
}
public enum Job
{
	Traveler, Adventurer, SpecialAdventurer
}
public class Stat {
	#region BattleStat
	//BattleStatusEnum
	//CurrentHealth, HealthMax, Attack, Defence, reduceDamageMult, Penetration, AvoidMult, criticalChance, cirticalDamage, movespeed, attackspeed, attackRange
	//
	public float healthMax { get; set; } = 1.0f;
	public float currentHealth { get; set; } = 1.0f;
	public float currentShield { get; set; } = 0.0f;
	public float attack { get; set; } = 0.0f;
	public float defence { get; set; } = 0.0f;
	public float reduceDamageMult { get; set; } = 0.0f;
	public float penetration { get; set; } = 0.0f;
	public float avoidMult { get; set; } = 0.0f;
	public float criticalChanceMult { get; set; } = 0.0f;
	public float criticalDamageMult { get; set; } = 0.0f;
	public bool isStunned { get; set; } = false;
	public bool isImmunedStun { get; set; } = false;
	public bool isDebuffed { get; set; } = false;
	public float damageTakenSum { get; set; } = 0.0f;
	public float movespeedMult { get; set; } = 1.0f;
	public float movespeedMultFinal { get; set; } = 1.0f;
	public float attackspeedMult { get; set; } = 1.0f;
	public float attackspeedMultFinal { get; set; } = 1.0f;
	public int attackRange { get; set; } = 1;
	public bool isHitRecent { get; set; } = false;
	public bool isCriticalRecent { get; set; } = false;
	public int invincibleCount { get; set; } = 0;
	#endregion

	#region CommonStat
	public int id
	{
		get; set;
	}
	public Race race
	{
		get; set;
	}
	public Wealth wealth
	{
		get; set;
	}
	public Job job
	{
		get; set;
	}
	public string name
	{
		get; set;
	}
	public string explanation
	{
		get; set;
	}
	public int gender
	{
		get; set;
	}// 0 - male, 1 - female;
	public int gold
	{
		get; set;
	}
	#endregion

	Dictionary<Desire, DesireBase> desireDict = new Dictionary<Desire, DesireBase>();

	//inventory 랑 옵저버?
	List<EquipmentEffect> equipmentEffectList;
	Traveler owner;
	public void Init(int _id, Race _race, Wealth _wealth, string _name, string explanation, int _gender, int _gold)
	{ }
	/*
	public void Init(int _id, Race _race, Wealth _wealth, string _name, string _explanation, int _gender, int _gold,
		float _thirstyTick, float _hungryTick, float _sleepTick, float _tourTick, float _funTick, float _convenienceTick, float _equipmentTick,
		float _tickAllMult, float _tickTime, Traveler _owner)
	{ 
		id = _id;
		race = _race;
		wealth = _wealth;
		name = _name;
		explanation = _explanation;
		gender = _gender;
		gold = _gold;
		desireList = new List<DesireBase>();
		thirsty = 0.0f;
		hungry = 0.0f;
		sleep = 0.0f;
		tour = 0.0f;
		fun = 0.0f;
		convenience = 0.0f;
		equipment = 0.0f;
		health = 0.0f;
		///desire

		thirstyTick = _thirstyTick;
		hungryTick = _hungryTick;
		sleepTick = _sleepTick;
		tourTick = _tourTick;
		funTick = _funTick;
		convenienceTick = _convenienceTick;
		equipmentTick = _equipmentTick;
		///desireTick

		tickAllMult = _tickAllMult;
		tickTime = _tickTime;
		owner = _owner;
	}*/

	#region Desire Tick메소드
		/*
	IEnumerator Ticking()
	{
		float tickTimeOrigin = tickTime;
		float eps = 0.0001f;
		WaitForSeconds wait = new WaitForSeconds(tickTime);

		while (owner.GetState() != State.Dead)
		{
			yield return wait;
			TickDesire();
			if (Mathf.Abs(tickTimeOrigin - tickTime) > eps)
			{
				tickTimeOrigin = tickTime;
				wait = new WaitForSeconds(tickTimeOrigin);
			}
		}

	}

	public void TickDesire()
	{

		thirsty += (thirstyTick * tickAllMult);
		hungry += (hungryTick * tickAllMult);
		sleep += (sleepTick * tickAllMult);
		tour += (tourTick * tickAllMult);
		fun += (funTick * tickAllMult);
		convenience += (convenienceTick * tickAllMult);
		equipment += (equipmentTick * tickAllMult);
		health = (1.0f - (owner.stat.GetCurrentHealth() / owner.stat.GetCalculatedHealthMax())) * 100.0f;
	}
	*/
	#endregion
	
	public Desire GetHighestDesire()
	{
		//thirsty, hungry, sleep, tour, fun, convenience, equipment, health....
		float maxVal = 0.0f;
		Desire max = Desire.Base;
		foreach (KeyValuePair<Desire, DesireBase> kvp in desireDict)
		{
			if (maxVal <= kvp.Value.desireValue)
			{
				maxVal = kvp.Value.desireValue;
				max = kvp.Key;
			}
		}
		return max;
	}
	public DesireBase GetSpecificDesire(Desire desire)
	{
		return desireDict[desire];	
	}

	#region 스탯Get
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
	#endregion
	#region EquipmentEffect 계산
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
	#endregion

	public float GetCalculatedDamage(Stat enemyBattleStatus, bool isCritical) // 상대방으로 부터 받는 데미지 계산
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

	public bool TakeDamageProcess(float damage) //계산된 데미지로 피해 처리
	{
		//returns true if Dead

		if (currentShield + currentHealth - damage <= 0.0f)
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
		}
	}

	public bool GetIsCriticalAttack() // 크리티컬 계산
	{
		if (UnityEngine.Random.Range(0, 100) < GetCalculatedCriticalChance() * 100.0f)
			return true;
		return false;
	}

}
