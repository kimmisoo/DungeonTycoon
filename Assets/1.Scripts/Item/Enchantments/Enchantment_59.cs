using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_59 : Enchantment {

	//이중관통
	//방어구관통 + 40%

	EquipmentEffect tempEffect;
	public override void OnEquip(Character user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.penetrationMult += 0.4f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Character user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
}
