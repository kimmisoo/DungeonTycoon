using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_28 : Enchantment {

	//숨돌리기
	//비전투 상태 && 잃은 체력 30 이상일때 체력 30회복.
	//3번 사용가능
	//성소에서 충전가능
	int count = 3;

	public override void OnEndBattle(Actor user, Actor target, Actor[] targets)
	{
		if(user.GetCalculatedHealthMax() - user.GetCurrentHealth() > 30.0f && count > 0)
		{
			user.TakeHealFromEnchantment(30.0f, user, this);
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
