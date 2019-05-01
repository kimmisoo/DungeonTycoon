using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_45 : Enchantment {

	//가속2
	//가속 중복 x
	//공격속도 +20%
	//category == 6

	EquipmentEffect tempEffect;
	const int category = 6;

	public override void OnEquip(Character user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.attackspeedMult += 0.20f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Character user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}

}
