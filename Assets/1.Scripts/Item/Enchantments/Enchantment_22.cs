using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_22 : Enchantment {

	//상흔
	//받은피해 100당(방어적용 x) 공격속도 +0.05% . 최대 20%
	EquipmentEffect tempEffect;


	public override void OnDamaged(Actor user, Actor target, Actor[] targets, float damage, bool isCritical)
	{
		user.RemoveAllEquipmentEffectByParent(this);
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.attackspeedMult += (int)(user.damageTakedSum / 100) * 0.05f;
		if (tempEffect.attackspeedMult >= 20.0f)
			tempEffect.attackspeedMult = 20.0f;
		user.AddEquipmentEffect(tempEffect);
	}
}
