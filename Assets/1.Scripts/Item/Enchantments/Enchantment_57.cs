using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_57 : Enchantment {

	//묵직함
	//방어구관통 + 20%

	EquipmentEffect tempEffect;
	public override void OnEquip(Actor user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.penetrationMult += 0.2f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Actor user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
}
