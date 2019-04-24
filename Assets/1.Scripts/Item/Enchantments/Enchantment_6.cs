﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_6 : Enchantment {
	//몰입
	//전투시작후 3초마다 공속 5% 추가. 5중첩
	Coroutine immerse;
	int immerseCount = 0;
	WaitForSeconds interval = new WaitForSeconds(3.0f);

	public override void OnStartBattle(Character user, Monster target, Monster[] targets)
	{
		immerse = StartCoroutine(Immerse(user));
	}
	public override void OnEndBattle(Character user, Monster target, Monster[] targets)
	{
		StopCoroutine(immerse);
		user.enchantmentAttackspeedMult -= 0.05f * immerseCount;
		immerseCount = 0;
	}

	IEnumerator Immerse(Character user)
	{
		yield return interval;
		if (immerseCount < 5) // 총 누적 5번
		{
			immerseCount++;
			user.enchantmentAttackspeedMult += 0.05f;
		}

	}
}
