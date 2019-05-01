using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_9 : Enchantment {
	//산탄
	//사정거리 +1
	//(최대사정거리 - 상대와의 거리) * 0.15f 만큼 추가공격력(추가데미지)
	
	int attackMult = 0;
	EquipmentEffect tempEffect;
	public override void OnEquip(Actor user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.attackRange += 1;
		user.AddEquipmentEffect(tempEffect);
		//user.enchantmentAttackRange += 1;
	}
	public override void OnUnequip(Actor user)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
	public override void OnDamage(Actor user, Actor target, Actor[] targets, float damage, bool isCritical)
	{
		//공격거리 검증은 Attack 단계에서 ~
		attackMult = user.GetCalculatedAttackRange() - user.GetCurTileForMove().GetDistance(target.GetCurTileForMove());
		if (attackMult == 0)
			return;
		target.TakeDamageFromEnchantment(user.GetCalculatedAttack() * 0.15f, user, this, false, out isHit, out isDead);
		
	}
	public override void OnDead(Actor user, Actor target, Actor[] targets)
	{
		user.RemoveAllEquipmentEffectByParent(this);
	}
}
