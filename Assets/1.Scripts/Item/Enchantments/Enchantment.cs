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
	public bool isCriticalAttack; // 내가 적에게
	public bool isCriticalAttacked; // 적이 나에게
	public bool isHit; // 내가 적에게
	public bool isHits; // 적이 나에게
	public bool isDead;
    Character user;
	Item item;
    
    
	// Use this for initialization
	void Start () {
        
	}
    public virtual void OnCoroutine(Character user, Monster target, Monster[] targets)
    {
        return;
    }
    public virtual void OnEquip(Character user)
    {
        return;
    }
    public virtual void OnUnequip(Character user)
    {
        return;
    }
    public virtual void OnStartBattle(Character user, Monster target, Monster[] targets)
    {
		return;
    }
    public virtual void OnEndBattle(Character user, Monster target, Monster[] targets)
    {
		return;
    }
    public virtual void OnAttack(Character user, Monster target, Monster[] targets, bool isCritical) // 공격 시도
    {
        return;
    }
    public virtual void OnAttacked(Character user, Monster target, Monster[] targets, bool isCritical) //적이 공격시도
    {
        return;
    }
    public virtual void OnDamage(Character user, Monster target, Monster[] targets, float damage, bool isCritical ) // 데미지 입힘 // Hit
    {
        return;
    }
    public virtual void OnDamaged(Character user, Monster target, Monster[] targets, float damage, bool isCritical) //데미지 입음 // Hit
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
	public virtual void OnDead(Character user, Monster target, Monster[] targets)
	{
		return;
	}
	
}
