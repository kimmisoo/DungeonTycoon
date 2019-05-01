using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_49 : Enchantment {

	//일렉트릭브레스
	//공격시 5%확률로 2칸내 적 2개체 300데미지
	int randNum = 0;
	List<Actor> near;
	public override void OnDamage(Actor user, Actor target, Actor[] targets, float damage, bool isCritical)
	{
		randNum = UnityEngine.Random.Range(0, 100);
		if(randNum < 5)
		{
			near = user.GetAdjacentActor(2);
			Shuffle(near);
			int count = 0;
			foreach(Actor a in near)
			{
				if(a is Monster)
				{
					a.TakeDamageFromEnchantment(300.0f, user, this, false, out isHit, out isDead);
					count++;
					if (count >= 2)
						break;
				}
			}
		}
	}

	void Shuffle(List<Actor> list)
	{
		int rand;
		Actor temp;
		for(int i=0; i<list.Count; i++)
		{
			rand = UnityEngine.Random.Range(0, list.Count);
			temp = list[i];
			list[i] = list[rand];
			list[rand] = temp;
		}
	}

}
