﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_61 : Enchantment {

	//소형방패
	//방어력 +20

	EquipmentEffect tempEffect;

	public override void OnEquip(Character user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.defence += 20.0f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Character user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
}
