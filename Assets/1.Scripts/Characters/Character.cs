using UnityEngine;
using System.Collections;

public class Character : Actor {

	public enum State
	{ Idle, Move, Hunt, Indoor }


	protected TileMap map;
	protected Animator animator;
	protected SpriteRenderer[] spriteRenderers;
	
	protected Moveto moveto;
	protected State curState = State.Idle;
	public Desire desire = new Desire();

    

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



	public override void TakeDamage(float damage, Actor from)
	{
		//공격우선순위설정
		//데미지 처리
	}
	public override void TakeDamageFromEnchantment(float damage, Actor from, Enchantment enchantment)
	{
		//공격우선순위설정
		//데미지 처리
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
	public override void AddEnchantment(Enchantment enchantment)
	{
		//인챈트 
	}
	public override void RemoveEnchantment(Enchantment enchantment)
	{
		//인챈트 제거
	}

	protected virtual void Start()
    {
		
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
