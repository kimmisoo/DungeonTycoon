using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_33 : Enchantment {

	//급소포착 1
	//급소포착 중복 x
	//치명타확률 +10%
	//category == 5


	EquipmentEffect tempEffect;
	const int category = 5;

	public override void OnEquip(Character user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.criticalChanceMult += 0.10f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Character user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}

}
