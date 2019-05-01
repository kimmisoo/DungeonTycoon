using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_63 : Enchantment {

	//대형방패
	//방어력 +40

	EquipmentEffect tempEffect;

	public override void OnEquip(Character user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.defence += 40.0f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Character user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
}
