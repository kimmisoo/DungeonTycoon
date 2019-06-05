﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_62 : Enchantment {

	//중형방패
	//방어력 +30

	EquipmentEffect tempEffect;

	public override void OnEquip(Actor user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.defence += 30.0f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Actor user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
}