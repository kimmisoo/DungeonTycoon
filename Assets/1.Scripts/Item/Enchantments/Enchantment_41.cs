using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_41 : Enchantment {

	//가속 1
	//가속 중복 x // 가벼움 중복 o
	//공속 + 10%
	//category == 6

	EquipmentEffect tempEffect;
	const int category = 6;

	public override void OnEquip(Actor user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.attackspeedMult += 0.10f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Actor user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}


}
