using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_6 : Enchantment {
	//몰입
	//전투시작후 3초마다 공속 5% 추가. 5중첩
	Coroutine immerse;
	int immerseCount = 0;
	WaitForSeconds interval = new WaitForSeconds(3.0f);
	EquipmentEffect tempEffect;

	public override void OnStartBattle(Character user, Monster target, Monster[] targets)
	{
		immerse = StartCoroutine(Immerse(user));
	}
	public override void OnEndBattle(Character user, Monster target, Monster[] targets)
	{
		StopCoroutine(immerse);
		user.RemoveAllEquipmentEffectByParent(this);
		immerseCount = 0;
	}

	IEnumerator Immerse(Character user)
	{
		while (true)
		{
			yield return interval;
			if (immerseCount < 5) // 총 누적 5번
			{
				tempEffect = new EquipmentEffect(this, user);
				tempEffect.attackspeedMult += 0.1f;

				immerseCount++;
				user.AddEquipmentEffect(tempEffect);
			}
		}

	}
}
