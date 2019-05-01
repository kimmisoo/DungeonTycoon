using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_14 : Enchantment {

	//거치적거림 1
	//거치적거림 중복 안됨
	//치명타 - 10%
	//category == 3
	EquipmentEffect tempEffect;
	const int category = 3;

	public override void OnEquip(Actor user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.criticalChanceMult -= 0.1f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Actor user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
}
