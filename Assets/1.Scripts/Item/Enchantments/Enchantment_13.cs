using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_13 : Enchantment {

	//가벼움 1
	//가벼움 중복안됨
	//공속 + 10%
	//category == 2
	EquipmentEffect tempEffect;
	const int category = 2;

	public override void OnEquip(Character user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.attackspeedMult += 0.1f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Character user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
}
