using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_53 : Enchantment {

	//하급 보호 마법
	//체력 + 60

	EquipmentEffect tempEffect;
	public override void OnEquip(Character user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.healthMax += 60.0f;
		user.AddEquipmentEffect(tempEffect);
	}

	public override void OnUnequip(Character user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}

}
