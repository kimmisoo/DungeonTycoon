using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_42 : Enchantment {

	//검기
	//range + 1

	EquipmentEffect tempEffect;

	public override void OnEquip(Character user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.attackRange += 1;
		user.AddEquipmentEffect(tempEffect);
	}

	public override void OnUnequip(Character user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}

}
