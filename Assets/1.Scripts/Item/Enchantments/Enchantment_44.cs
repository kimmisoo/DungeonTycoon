using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_44 : Enchantment {

	//급소포착2
	//급소포착 중복 x
	//치명타 +20%
	//category == 5

	EquipmentEffect tempEffect;
	const int category = 5;

	public override void OnEquip(Actor user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.criticalChanceMult += 0.20f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Actor user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
}
