using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_23 : Enchantment {

	//가벼움2
	//가벼움 중복x
	//공속 +15%
	//category == 2

	EquipmentEffect tempEffect;
	const int category = 2;

	public override void OnEquip(Character user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.attackspeedMult += 0.15f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Character user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}

}
