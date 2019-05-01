using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_7 : Enchantment {
	//전격
	//치명타시 상대 2초간 스턴. 60데미지. 쿨타임 16초
	bool isCooldown = false;
	Coroutine cooldown;
	WaitForSeconds interval = new WaitForSeconds(16.0f);

	public override void OnDamage(Actor user, Actor target, Actor[] targets, float damage, bool isCritical)
	{
		if(isCritical == true && isCooldown == false)
		{
			target.TakeDamageFromEnchantment(60.0f, user, this, false, out isHit, out isDead);
			target.TakeStunned(user, this, 2.0f);
			cooldown = StartCoroutine(StunCooldown());
		}
	}
	public override void OnUnequip(Actor user)
	{
		StopCoroutine(cooldown);
	}
	IEnumerator StunCooldown()
	{
		isCooldown = true;
		yield return interval;
		isCooldown = false;
	}
}
