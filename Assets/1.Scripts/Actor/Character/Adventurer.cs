using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adventurer : Traveler//, IDamagable {
{ 
	/*
	public BattleStatus battleStatus
	{
		get
		{
			return _battleStatus;
		}
	}
	protected BattleStatus _battleStatus;
	WaitForSeconds idleInterval;


	public void Awake()
	{
		_battleStatus = GetComponent<BattleStatus>();
	}
	public void Start()
	{
		idleInterval = new WaitForSeconds(1.0f);
		tileLayer = GetCurTile().GetLayer();
		direction = Direction.DownRight;
	}
	

	#region IDamagable
	public void SetBattleStatus(BattleStatus bs)
	{
		_battleStatus = bs;
	}

	public bool Attack(IDamagable enemy)
	{
		if (enemy != null && (enemy as Adventurer).state != State.Dead)
		{
			float dealtDamage = 0.0f;
			bool isCritical = false;
			bool isHit = false;
			bool isDead = false;
			isCritical = battleStatus.GetIsCriticalAttack();

			enemy.Attacked(this, isCritical, out isHit);
			foreach (Enchantment e in enchantmentList)
			{
				e.OnAttack(this, enemy as Actor, GetAdjacentActor(battleStatus.GetCalculatedAttackRange()), isCritical);
			}
			if (isHit == true)
			{
				enemy.TakeDamage(this, isCritical, out isDead, out dealtDamage);
				foreach (Enchantment e in enchantmentList)
				{
					e.OnDamage(this, enemy as Actor, GetAdjacentActor(2), dealtDamage, isCritical);
				}
			}
			if (isDead)
			{
				EndBattle(enemy);
				state = State.Idle;
			}
			return isHit;
		}
		return false;
	}

	public void Attacked(IDamagable from, bool isCritical, out bool isHit)
	{
		if (Random.Range(0.0f, 1.0f) < battleStatus.GetCalculatedAvoidMult())
			isHit = false;
		else
			isHit = true;
		foreach (Enchantment e in enchantmentList)
		{
			e.OnAttacked(this, from as Actor, GetAdjacentActor(2), isCritical);
		}
		return;
	}

	public void AttackedFromEnchantment(IDamagable from, Enchantment enchantment, bool isCritical, out bool isHit)
	{
		if (Random.Range(0.0f, 1.0f) < battleStatus.GetCalculatedAvoidMult())
			isHit = false;
		else
			isHit = true;
		return;
	}

	public void TakeDamage(IDamagable from, bool isCritical, out bool isDead, out float damage)
	{
		float calculatedDamage = battleStatus.GetCalculatedDamage(from.battleStatus, isCritical);
		damage = calculatedDamage;
		isDead = battleStatus.TakeDamageProcess(calculatedDamage);
		foreach (Enchantment e in enchantmentList)
		{
			e.OnDamaged(this, from as Actor, GetAdjacentActor(2), damage, isCritical);
		}
		if (isDead)
		{
			EndBattle(from);
			Die(from);
			state = State.Dead;
			foreach (Enchantment e in enchantmentList)
			{
				e.OnEndBattle(this, from as Actor, GetAdjacentActor(2));
				e.OnDead(this, from as Actor, GetAdjacentActor(2));
			}
		}

	}

	public void TakeDamageFromEnchantment(float damage, IDamagable from, Enchantment enchantment, bool isCritical, out bool isDead)
	{
		isDead = battleStatus.TakeDamageProcess(damage);
		if (isDead)
		{
			EndBattle(from);

		}
	}

	public void TakeHeal(float heal, IDamagable from)
	{

	}
	public void TakeHealFromEnchantment(float heal, IDamagable from, Enchantment enchantment)
	{

	}

	public void StartBattle(IDamagable Opponent)
	{
		foreach (Enchantment e in enchantmentList)
		{
			e.OnStartBattle(this, Opponent as Actor, GetAdjacentActor(2));
		}

	}

	public void EndBattle(IDamagable Opponent)
	{
		foreach (Enchantment e in enchantmentList)
		{
			e.OnEndBattle(this, Opponent as Actor, GetAdjacentActor(2));
		}

	}

	public void AddEnchantment(Enchantment enchantment)
	{
		enchantmentList.Add(enchantment);
	}

	public void RemoveEnchantmnet(Enchantment enchantment)
	{
		enchantmentList.Remove(enchantment);
	}

	public void TakeStunned(IDamagable from, Enchantment enchantment, float during)
	{

	}

	public bool IsAttackable(IDamagable enemy)
	{
		if (!(enemy is Adventurer))
			return false;
		if (enemy != null && (enemy as Adventurer).GetCurTile().GetMonsterArea() && GetDistanceFromOtherActorForMove(enemy as Adventurer) <= battleStatus.GetCalculatedAttackRange())
			return true;
		return false;
	}

	public IEnumerator Stun(float duration)
	{
		
		yield return null;
	}

	#endregion*/

}
