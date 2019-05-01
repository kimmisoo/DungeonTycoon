using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_31 : Enchantment {

	//짐꾼
	//이동속도 +25
	//최대체력 + 4%
	// 2분이내 해제 불가

	EquipmentEffect tempEffect;

	public override void OnEquip(Character user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.movespeedMult += 0.25f;
		tempEffect.healthMaxMult += 0.04f;
		user.AddEquipmentEffect(tempEffect);
	}
	public override void OnUnequip(Character user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}

}
