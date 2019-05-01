using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_8 : Enchantment {

	//균열
	//공격시 적에게 방어력 -6%. 5중첩
	Actor opponent = null;
	int damageCount = 0;
	EquipmentEffect tempEffect;
	public override void OnDamage(Actor user, Actor target, Actor[] targets, float damage, bool isCritical)
	{
		
		if (opponent == null)
			opponent = target;

		if (target.Equals(opponent))
		{
			//중첩
			damageCount++;
			tempEffect = new EquipmentEffect(this, user);
			tempEffect.defenceMult -= 0.06f;
			opponent.AddEquipmentEffect(tempEffect);
		}
		else
		{
			opponent.RemoveAllEquipmentEffectByParent(this);
			damageCount = 1;
			opponent = target;
			tempEffect = new EquipmentEffect(this, user);
			tempEffect.defenceMult -= 0.06f;
			opponent.AddEquipmentEffect(tempEffect);
			

		}
	}
	
	public override void OnEndBattle(Actor user, Actor target, Actor[] targets)
	{
		if(target.gameObject.activeSelf == true)
		{
			//target.enchantmentDefenceMult += 0.06f * damageCount;
			target.RemoveAllEquipmentEffectByParent(this);
		}
	}
}
