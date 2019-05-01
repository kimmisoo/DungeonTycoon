using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_15 : Enchantment {


	//움직이기편함 1
	//움직이기편함 중복 x
	//치명타 +12%
	//category == 4

	EquipmentEffect tempEffect;
	const int category = 4;

	public override void OnEquip(Character user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.criticalChanceMult += 0.12f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Character user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
}
