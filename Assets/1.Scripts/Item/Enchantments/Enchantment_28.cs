﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_28 : Enchantment {

	//숨돌리기
	//비전투 상태 && 잃은 체력 30 이상일때 체력 30회복.
	//3번 사용가능
	//성소에서 충전가능
	int count = 3;

	public override void OnEndBattle(Character user, Monster target, Monster[] targets)
	{
		if(user.GetCalculatedHealthMax() - user.health > 30.0f && count > 0)
		{
			user.TakeHeal(30.0f, user);
			count--;
		}
	}

	public override void OnBuildingEnter(Structure structure)
	{
		if(structure.genre == "sanctuary")
		{
			count = 3;
		}
	}
}
