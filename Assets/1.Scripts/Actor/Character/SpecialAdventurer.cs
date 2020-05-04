using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAdventurer : Adventurer//, IDamagable {
{
    Skill uniqueSkill;
    public void InitSpecialAdventurer(Stat stat, BattleStat battleStat, RewardStat rewardStat, int skillID)
    {
        base.InitAdventurer(stat, battleStat, rewardStat);
        //uniqueSkill = new Skill(skillID);
    }

    public void OnEnable()
    {
        base.OnEnable();
        monsterSearchCnt = 0;

        SetUI();
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
    #region Items
    #endregion

    #region BossBattle
    #endregion
}
