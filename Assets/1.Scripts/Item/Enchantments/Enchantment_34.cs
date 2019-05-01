using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_34 : Enchantment {

	//일섬
	//치명타 공격력 +50%

	EquipmentEffect tempEffect;

	public override void OnEquip(Character user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.criticalDamageMult += 0.5f;
		user.AddEquipmentEffect(tempEffect);
	}

	public override void OnUnequip(Character user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
}
