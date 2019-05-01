using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_55 : Enchantment {

	//하급 보호 마법
	//체력 + 12%

	EquipmentEffect tempEffect;
	public override void OnEquip(Actor user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.healthMaxMult += 0.12f;
		user.AddEquipmentEffect(tempEffect);
	}

	public override void OnUnequip(Actor user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
}
