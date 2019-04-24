﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_4 : Enchantment {
	//힘 흡수
	//주변 1칸 적의 공격력 - 10%씩, 주변 적의 수 * 10% 만큼 자신의 공격력 증가.
	Coroutine nearMonsterCheck;
	WaitForSeconds interval = new WaitForSeconds(5.0f);
	List<Actor> near = new List<Actor>();
	const float amount = 0.1f;
	public override void OnEquip(Character user)
	{
		nearMonsterCheck = StartCoroutine(NearMonsterCheck(user));
	}
	public override void OnCoroutine(Character user, Monster target, Monster[] targets)
	{
		//쓰일 일 없을듯
	}
	public override void OnUnequip(Character user)
	{
		StopCoroutine(nearMonsterCheck);
	}
	IEnumerator NearMonsterCheck(Actor user)
	{

		int nearCount = 0;
		//near에 인근 몬스터 추가
		while (true)
		{
			near = user.GetAdjacentActor(1);
			nearCount = 0;
			foreach (Actor a in near)
			{
				if (a is Monster)
				{
					a.enchantmentAttackMult -= 0.1f;
					nearCount++;
				}
			}
			user.enchantmentAttackMult += amount * nearCount;
			///버프, 디버프 추가

			yield return interval;

			foreach(Actor a in near)
			{
				if (a is Monster)
					a.enchantmentAttackMult += 0.1f;
			}
			user.enchantmentAttackMult -= amount * nearCount;
			///버프, 디버프 해제
		}
	}
}
