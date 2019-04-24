using UnityEngine;
using System.Collections;

public class Monster : Actor {

	SpriteRenderer spriteRenderer;
	Animator animator;
	int turn = 0;
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
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

	public override void TakeDamage(float damage, float penetration, Actor from)
	{

	}
	public override void TakeDamageFromEnchantment(float damage, float penetration, Actor from, Enchantment enchantment)
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
	public override void GetStunned(Actor from, Enchantment enchantment, float during)
	{
		
	}
}
