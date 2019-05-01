using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_58 : Enchantment {

	//날카로운 날
	//방어구관통 + 30%

	EquipmentEffect tempEffect;
	public override void OnEquip(Actor user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.penetrationMult += 0.3f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Actor user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
}
