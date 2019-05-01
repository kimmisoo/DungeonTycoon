using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_51 : Enchantment {

	//빙결
	//공격시 4%확률로 대상 1.5초간 스턴
	//쿨타임 12초

	int rand = 0;
	Coroutine cooldown;
	WaitForSeconds interval = new WaitForSeconds(12.0f);
	bool isCooldown = false;
	public override void OnDamage(Actor user, Actor target, Actor[] targets, float damage, bool isCritical)
	{
		rand = UnityEngine.Random.Range(0, 100);
		if(rand < 4 && isCooldown == false)
		{
			target.TakeStunned(user, this, 1.5f);
			isCooldown = true;
			cooldown = StartCoroutine(Cooldown(user));
		}
	}
	public override void OnUnequip(Actor user)
	{
		StopCoroutine(cooldown);
	}
	IEnumerator Cooldown(Actor user)
	{
		yield return interval;
		isCooldown = false;
	}

}
