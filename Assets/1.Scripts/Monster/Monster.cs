using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Monster : Actor {

	SpriteRenderer spriteRenderer;
	Animator animator;
	List<EquipmentEffect> equipmentEffectList;
	List<Enchantment> enchantmentList;
	int turn = 0;
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		equipmentEffectList = new List<EquipmentEffect>();
		enchantmentList = new List<Enchantment>();
	}
	private void OnEnable()
	{
		
		StartCoroutine(Acting());
	}
	
	IEnumerator Acting()
	{
		yield return null;
	}
	
	public void GetEnchantmentEffect()
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
	
	
}
