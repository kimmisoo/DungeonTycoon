using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAdventurer : Adventurer//, IDamagable {
{
    //Skill uniqueSkill;
    public void InitSpecialAdventurer(Stat stat, BattleStat battleStat, RewardStat rewardStat, string name)
    {
        base.InitAdventurer(stat, battleStat, rewardStat);
        //battleStat.ResetBattleStat();
        //uniqueSkill = SkillFactory.CreateSkill(gameObject, name);
        //uniqueSkill.SetOwner(this);
        //uniqueSkill.InitSkill();
        AddSkill(SkillFactory.CreateSkill(gameObject, name));
        AddSkill(SkillFactory.CreateSkill(gameObject, "Lava"));
    }

    public void OnEnable()
    {
        base.OnEnable();
        monsterSearchCnt = 0;
        //uniqueSkill.Activate();
        //SetUI();
    }

    public void OnDisable()
    {
        //uniqueSkill.Deactivate();
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
