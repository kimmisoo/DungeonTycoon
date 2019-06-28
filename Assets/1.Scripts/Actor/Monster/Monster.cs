using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Monster : Actor {
	
	//Stat 관련 처리. 데미지 처리. 등 ...
	//선공 인식 범위 십자 3칸
	
	int monsterCode;
	TileLayer tileLayer;
	//Coroutine currentCoroutine;
	Direction curDirection = Direction.DownRight;
	MonsterActAI monsterActAI;
	MonsterAttackAI monsterAttackAI;
	MonsterMoveAI monsterMoveAI;
	BattleStatus battleStatus;


	//이동시 Monster Tile만 체크
	public void Awake()
	{
		base.Awake();
		monsterAttackAI = GetComponent<MonsterAttackAI>();
		monsterActAI = GetComponent<MonsterActAI>();
		monsterMoveAI = GetComponent<MonsterMoveAI>();
	}
	
	private void OnEnable()
	{
		tileLayer = GetCurTile().GetLayer();
		moving = StartCoroutine(Moving());
		attacking = StartCoroutine(Attacking());
	}
	private void OnDisable()
	{		
	}
	
	IEnumerator Respawn(ActingResult result)
	{
		yield return null;
	}
	
	public override void Die(Actor Opponent)
	{
		//object 비활성화 및 respawn 준비
	}
	public override void StartBattle(Actor opponent)
	{
		foreach(Enchantment e in enchantmentList)
		{
			e.OnStartBattle(this, opponent, GetAdjacentActor(2).ToArray());
		}
	}
	public override void EndBattle(Actor opponent)
	{
		//전투 끝.(인챈트 호출용))
		foreach(Enchantment e in enchantmentList)
		{
			e.OnEndBattle(this, opponent, GetAdjacentActor(2).ToArray());
		}
	}
	public override void TakeHeal(float heal, Actor from)
	{
		currentHealth = Mathf.Min(GetCalculatedHealthMax(), currentHealth + heal);
	}
	public override void TakeHealFromEnchantment(float heal, Actor from, Enchantment enchantment)
	{
		currentHealth = Mathf.Min(GetCalculatedHealthMax(), currentHealth + heal);
	}
	public override void AddEnchantment(Enchantment enchantment)
	{
		enchantmentList.Add(enchantment);
	}
	public override void RemoveEnchantment(Enchantment enchantment)
	{
		enchantmentList.Remove(enchantment);
	}
	public override void TakeStunned(Actor from, Enchantment enchantment, float during)
	{
		if(GetCalculatedImmunedStun() == true || isStunned == true) // 스턴면역 또는 스턴 중이면
		{
			//아무것도 하지않음
		}
		else
		{
			stunning = StartCoroutine(Stun(during));
		}

	}
	//크리티컬이면 스턴 풀림
	IEnumerator Stun(float duration)
	{
		isStunned = true;
		yield return new WaitForSeconds(duration);
		isStunned = false;
	}

	
	public override void Attacked(Actor from, bool isCritical, out bool isHit)
	{
		float rand = Random.Range(0.0f, 100.0f);
		if(rand < GetCalculatedAvoidMult())
		{
			isHit = false;
			return;
		}
		isHit = true;
		foreach(Enchantment e in enchantmentList)
		{
			e.OnAttacked(this, target, GetAdjacentActor(GetCalculatedAttackRange()).ToArray(), isCritical);
		}
	}
	public override void AttackedFromEnchantment(Actor from, Enchantment enchantment, bool isCritical, out bool isHit)
	{
		float rand = Random.Range(0.0f, 100.0f);
		if (rand < GetCalculatedAvoidMult())
		{
			isHit = false;
			return;
		}
		isHit = true;
	} // 인챈트공격 회피 판정
	public override void TakeDamage(Actor from, bool isCritical, out bool isDead, out float damage)
	{
		//데미지 계산
		damage = GetCalculatedDamage(from, isCritical);
		//sheild 계산
		currentHealth = Mathf.Max(0.0f, currentHealth - damage);
		if(currentHealth <= 0.0f)
		{
			state = State.Dead;
			isDead = true;
			Die(from);
			//Dead Process
			
		}
		else
		{
			isDead = false;
			foreach(Enchantment e in enchantmentList)
			{
				e.OnDamaged(this, from, GetAdjacentActor(GetCalculatedAttackRange()).ToArray(), damage, isCritical);
			}
		}
	}
	public override void TakeDamageFromEnchantment(float damage, Actor from, Enchantment enchantment, bool isCritical, out bool isDead)
	{
		isDead = false;
		//sheild 계산
		currentHealth = Mathf.Max(0.0f, currentHealth - damage);
		if (currentHealth <= 0.0f)
		{
			state = State.Dead;
			isDead = true;
			//Dead Process
		}
		else
		{
			isDead = false;
		}
	}
	
	
	public void SetMonsterCode(int code)
	{
		monsterCode = code;
	}
	public int GetMonsterCode()
	{
		return monsterCode;
	}
	public Character GetCurrentTarget()
	{
		return target;
	}
	

}
