using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_54 : Enchantment {

	//중급 보호 마법
	//체력 + 120

	EquipmentEffect tempEffect;
	public override void OnEquip(Actor user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.healthMax += 120.0f;
		user.AddEquipmentEffect(tempEffect);
	}

	public override void OnUnequip(Actor user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
}
