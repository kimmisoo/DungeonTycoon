using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_9 : Enchantment {
	//산탄
	//사정거리 +1
	//(최대사정거리 - 상대와의 거리) * 0.15f 만큼 추가공격력(추가데미지)
	
	int attackMult = 0;
	EquipmentEffect tempEffect;
	public override void OnEquip(Character user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.attackRange += 1;
		user.AddEquipmentEffect(tempEffect);
		//user.enchantmentAttackRange += 1;
	}
	public override void OnUnequip(Character user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
	public override void OnDamage(Character user, Monster target, Monster[] targets, float damage, bool isCritical)
	{
		//공격거리 검증은 Attack 단계에서 ~
		attackMult = user.GetCalculatedAttackRange() - user.GetCurTileForMove().GetDistance(target.GetCurTileForMove());
		if (attackMult == 0)
			return;
		target.TakeDamageFromEnchantment(user.GetCalculatedAttack() * 0.15f, user, this, false, out isHit, out isDead);
		
	}
	public override void OnDead(Character user, Monster target, Monster[] targets)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
}
