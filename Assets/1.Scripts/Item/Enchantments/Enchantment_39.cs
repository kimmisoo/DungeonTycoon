using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_39 : Enchantment {

	//반작용
	//방어력 + 35%
	//데미지 입을시 상대에게 20% 방어력만큼 데미지
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
	public override void OnDamaged(Character user, Monster target, Monster[] targets, float damage, bool isCritical)
	{
		target.TakeDamageFromEnchantment(user.GetCalculatedDefence() * 0.2f, user, this, false, out isHit, out isDead);
	}
}
