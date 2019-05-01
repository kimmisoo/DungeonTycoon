using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_19 : Enchantment {

	//거치적거림 2
	//거치적거림 중복 x
	//치명타 -15%
	//category == 3

	EquipmentEffect tempEffect;
	const int category = 3;

	public override void OnEquip(Actor user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.criticalChanceMult -= 0.15f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Actor user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
}
