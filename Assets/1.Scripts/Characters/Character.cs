using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Character : Actor {
	//건물입장.
	//골드 등~

	protected TileMap map;
	protected Animator animator;
	protected SpriteRenderer[] spriteRenderers;
	
	protected Moveto moveto;
	protected State curState = State.Idle;
	public Desire desire = new Desire();

	List<Enchantment> enchantmentList = new List<Enchantment>();
	List<EquipmentEffect> equipmentEffectList = new List<EquipmentEffect>();

    

	//공통 Attribute
	public string names
	{
		get; set;
	}
	public string race
	{
		get; set;
	}
	public int gender
	{
		get; set;
	}
	public int job
	{
		get; set;
	}
	public float moveSpeed
	{
		get; set;
	}
	//end of 공통 Attribute



	public override void TakeDamage(Actor from, bool isCritical, out bool isDead, out float damage)
	{
		damage = 0.0f;
		isDead = false;
		
	}
	public override void TakeDamageFromEnchantment(float damage, Actor from, Enchantment enchantment, bool isCritical, out bool isDead)
	{
		isDead = false;
		
	}

	public override void Die(Actor Opponent)
	{
		//사망 처리.
		//상대방 경험치?
	}
	public override void TakeHeal(float heal, Actor from)
	{
		//힐 처리.
	}
	public override void TakeHealFromEnchantment(float heal, Actor from, Enchantment enchantment)
	{
		//힐 처리
	}
	public override void TakeStunned(Actor from, Enchantment enchantment, float during)
	{
		
	}
	public override void AddEnchantment(Enchantment enchantment)
	{
		//인챈트 
	}
	public override void RemoveEnchantment(Enchantment enchantment)
	{
		//인챈트 제거
	}

	protected virtual void Activate()
	{

	}
	protected virtual void Deactivate()
	{

	}
	protected virtual IEnumerator Acting()
	{
		yield return null;
	}
	protected virtual void Init()
	{
		map = GameManager.Instance.GetMap();
		moveto = GetComponent<Moveto>();
		animator = GetComponent<Animator>();
		spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
	}
    public State GetCurState()
    {
        return curState;
    }
	
	
}
