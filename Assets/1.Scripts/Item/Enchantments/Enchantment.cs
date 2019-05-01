using UnityEngine;
using System.Collections;

public class Enchantment : MonoBehaviour, IHasEquipmentEffect{

    /*
     * 매 코루틴 return 마다 호출
     * 장비장착시 호출
     * 장비장착 해제시 호출
     * 공격할때 호출
     * 공격받을때 호출
     * 데미지입혔을때 호출
     * 데미지입었을때 호출
     * 건물들어갔을때 호출
     * 건물나왔을때 호출
     */



    /*
     *Params
     * Character player
     * Character monster
     */
    int itemCode;
    string enchantmentName;
	string enchantmentExplanation;
	public bool isHit; 
	public bool isDead;
    Actor user;
	Item item;
	
    
	// Use this for initialization
	void Start () {
        
	}
    public virtual void OnCoroutine(Actor user, Actor target, Actor[] targets)
    {
        return;
    }
    public virtual void OnEquip(Actor user)
    {
        return;
    }
    public virtual void OnUnequip(Actor user)
    {
        return;
    }
    public virtual void OnStartBattle(Actor user, Actor target, Actor[] targets)
    {
		return;
    }
    public virtual void OnEndBattle(Actor user, Actor target, Actor[] targets)
    {
		return;
    }
    public virtual void OnAttack(Actor user, Actor target, Actor[] targets, bool isCritical) // 공격 시도
    {
        return;
    }
    public virtual void OnAttacked(Actor user, Actor target, Actor[] targets, bool isCritical) //적이 공격시도
    {
        return;
    }
    public virtual void OnDamage(Actor user, Actor target, Actor[] targets, float damage, bool isCritical ) // 데미지 입힘 // Hit
    {
        return;
    }
    public virtual void OnDamaged(Actor user, Actor target, Actor[] targets, float damage, bool isCritical) //데미지 입음 // Hit
    {
        return;
    }
    public virtual void OnBuildingEnter(Structure structure)
    {
        return;
    }
    public virtual void OnBuildingExit(Structure structure)
    {
        return;
    }
	public virtual void SetItem(Item _item)
	{
		item = _item;
	}
	public virtual void OnDead(Actor user, Actor target, Actor[] targets)
	{
		return;
	}
	
}
