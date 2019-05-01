using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_8 : Enchantment {

	//균열
	//공격시 적에게 방어력 -6%. 5중첩
	Monster opponent = null;
	int damageCount = 0;
	EquipmentEffect tempEffect;
	public override void OnDamage(Character user, Monster target, Monster[] targets, float damage, bool isCritical)
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
	
	public override void OnEndBattle(Character user, Monster target, Monster[] targets)
	{
		if(target.gameObject.activeSelf == true)
		{
			//target.enchantmentDefenceMult += 0.06f * damageCount;
			target.RemoveAllEquipmentEffectByParent(this);
		}
	}
}
