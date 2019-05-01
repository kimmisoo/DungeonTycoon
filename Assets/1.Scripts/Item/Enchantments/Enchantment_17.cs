using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_17 : Enchantment {


	//강화외골격
	//공격력 + 10%
	EquipmentEffect tempEffect;
	
	public override void OnEquip(Actor user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.attackMult += 0.1f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Actor user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}

}
