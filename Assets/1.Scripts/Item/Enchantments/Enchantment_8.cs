using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_8 : Enchantment {

	//균열
	//공격시 적에게 방어력 -6%. 5중첩
	Monster opponent = null;
	int damageCount = 0;
	public override void OnDamage(Character user, Monster target, Monster[] targets, bool isCritical)
	{

		if (opponent == null)
			opponent = target;

		if (target.Equals(opponent))
		{
			//중첩
			damageCount++;
			opponent.enchantmentDefenceMult -= 0.06f;
		}
		else
		{
			opponent.enchantmentDefenceMult += 0.06f * damageCount;
			damageCount = 1;
			opponent = target;
			opponent.enchantmentDefenceMult -= 0.06f;
			
		}
	}
	public override void OnEndBattle(Character user, Monster target, Monster[] targets)
	{
		if(target.gameObject.activeSelf == true)
		{
			target.enchantmentDefenceMult += 0.06f * damageCount;
		}
	}
}
