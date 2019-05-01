using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_48 : Enchantment {


	//포르투나의 축복
	//포르투나의 축복 중복 x
	//회피율 + 10%
	//category == 7

	EquipmentEffect tempEffect;
	const int category = 7;

	public override void OnEquip(Character user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.avoidMult += 0.10f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Character user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}

}
