using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_35 : Enchantment {

	//추가장갑
	//방어력 + 30%

	EquipmentEffect tempEffect;

	public override void OnEquip(Character user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.defenceMult += 0.3f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Character user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
}
