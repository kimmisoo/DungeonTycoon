using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Monster : Actor {
	
	//선공 인식 범위 십자 3칸
	//
	int monsterCode;
	SpriteRenderer spriteRenderer;
	Animator animator;
	Character target;
	WaitForSeconds searchInterval = new WaitForSeconds(3.0f);
	ActingResult actingResult = new ActingResult();
	int turn = 0;
	//이동시 Monster Tile만 체크

	void Start () {
		base.Start();
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();		
	}
	private void OnEnable()
	{
		StartCoroutine(Acting());
	}
	private void OnDisable()
	{
		
	}
	IEnumerator Acting()
	{
		//대기중(3초 1칸)
		//주변 모험가 체크
		//모험가로 이동 or 추격
		//전투
		//사망 or 승리
		
		while(true)
		{
			yield return StartCoroutine(EnemySearch(actingResult));
			if(actingResult.isFoundEnemy == true)
			{
				actingResult.isFoundEnemy = false;
				break;
			}
		}

		yield return null;
	}
	IEnumerator EnemySearch(ActingResult result)
	{
		while (true)
		{
			target = null;
			foreach (Actor a in GetAdjacentActor(3)) // Character 찾기 ? adventurer or Special Adventurer
			{
				
			}
		}
	}
	IEnumerator Chase()
	{

	}
	IEnumerator Battle()
	{

	}

	public override void Die(Actor Opponent)
	{
		
	}
	public override void TakeHeal(float heal, Actor from)
	{

	}
	public override void TakeHealFromEnchantment(float heal, Actor from, Enchantment enchantment)
	{

	}
	public override void AddEnchantment(Enchantment enchantment)
	{

	}
	public override void RemoveEnchantment(Enchantment enchantment)
	{

	}
	public override void TakeStunned(Actor from, Enchantment enchantment, float during)
	{
		
	}
	public override void TakeDamage(Actor from, bool isCritical, out bool isHit, out bool isDead)
	{
		isHit = false;
		isDead = false;
	}
	public override void TakeDamageFromEnchantment(float damage, Actor from, Enchantment enchantment, bool isCritical, out bool isHit, out bool isDead)
	{
		isHit = false;
		isDead = false;
	}
	public void Wander()
	{

	}
}
