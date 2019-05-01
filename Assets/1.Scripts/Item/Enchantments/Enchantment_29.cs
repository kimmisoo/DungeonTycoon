using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_29 : Enchantment {


	//무기정비
	//방관 + 15 (고정)

	EquipmentEffect tempEffect;
	public override void OnEquip(Actor user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.penetration = 15.0f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Actor user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
}
