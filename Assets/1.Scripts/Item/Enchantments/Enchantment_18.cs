using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_18 : Enchantment {

	//화염오라
	//주위 1칸 적에게 1초마다 공격력 8%로 공격

	Coroutine flameAura;
	WaitForSeconds interval = new WaitForSeconds(1.0f);

	public override void OnEquip(Character user)
	{
		flameAura = StartCoroutine(FlameAura(user));
	}

	public override void OnUnequip(Character user)
	{
		StopCoroutine(flameAura);
	}

	IEnumerator FlameAura(Character user)
	{

		while (true)
		{
			yield return interval;
			foreach (Actor a in user.GetAdjacentActor(1))
			{
				if (a is Monster)
				{
					a.TakeDamageFromEnchantment(user.GetCalculatedAttack() * 0.08f, user, this, false, out isHit, out isDead);
				}
			}
		}
	}
}
