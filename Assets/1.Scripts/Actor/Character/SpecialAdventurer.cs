using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAdventurer : Adventurer, IDamagable {

	
	public void Awake()
	{
		
		base.Awake();
		//BattleStatus GetComponent;;
	}

	//+ item 추가만~~

	/* -> Special Adventurer
	public EquipmentEffect[] GetSameCategoryEffects(int category)
	{
		List<EquipmentEffect> tempList = new List<EquipmentEffect>();
		foreach(EquipmentEffect e in equipmentEffectList)
		{
			if (e.category != -1)
				tempList.Add(e);
		}
		return tempList.ToArray();
	}*/

}
