using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Character : Actor, IMove, IUseStructure
{
	//건물입장.
	//골드 등~

	
	protected Animator animator;
	protected SpriteRenderer[] spriteRenderers;

	protected PathFinder moveto;
	
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



	

	public override void Die(Actor Opponent)
	{
		//사망 처리.
		//상대방 경험치?
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
	public State GetState()
	{
		return state;
	}


}
