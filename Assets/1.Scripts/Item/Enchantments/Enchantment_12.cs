using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_12 : Enchantment {

	//무거움 1
	//무거움 중복 안됨
	//공속 - 10%
	//무거움 Category == 1;
	EquipmentEffect tempEffect;
	const int category = 1;

	public override void OnEquip(Actor user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.attackspeedMult -= 0.1f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Actor user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
}
