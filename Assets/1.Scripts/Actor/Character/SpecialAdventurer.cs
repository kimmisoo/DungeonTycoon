#define DEBUG_ITEM

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAdventurer : Adventurer
{
    private Item weapon, armor, accessory1, accessory2;

    private const int ACCESSORY_CAPACITY = 2;

    //Skill uniqueSkill;
    public void InitSpecialAdventurer(Stat stat, BattleStat battleStat, RewardStat rewardStat, string name)
    {
        base.InitAdventurer(stat, battleStat, rewardStat);
        //battleStat.ResetBattleStat();
        //uniqueSkill = SkillFactory.CreateSkill(gameObject, name);
        //uniqueSkill.SetOwner(this);
        //uniqueSkill.InitSkill();
        AddSkill(name);
        //AddSkill("LifeTap");

    }

    public void OnEnable()
    {
        base.OnEnable();
        monsterSearchCnt = 0;
        //uniqueSkill.Activate();
        //SetUI();
#if DEBUG_ITEM
        ItemManager.Instance.setItemCategory("Armor");
        ItemManager.Instance.setItemIndex(22);

        Debug.Log("[OnEnable] Def before : " + battleStat.Defence + ", " + "Hp before : " + battleStat.HealthMax);
        EquipArmor(ItemManager.Instance.CreateItem());
        Debug.Log("[OnEnable] Def after : " + battleStat.Defence + ", " + "Hp after : " + battleStat.HealthMax);

        //ItemManager.Instance.setItemCategory("Accessory");
        //ItemManager.Instance.setItemIndex(22);

        //Debug.Log("[OnEnable] Before Atk : " + battleStat.Attack + ", AtkSpd : " + battleStat.AttackSpeed + ", CritChance : " + battleStat.CriticalChance + ", PenFixed : " + battleStat.PenetrationFixed + ", PenMult : " + battleStat.PenetrationMult);
        //EquipAccessory1(ItemManager.Instance.CreateItem());
        //Debug.Log("[OnEnable] After Atk : " + battleStat.Attack + ", AtkSpd : " + battleStat.AttackSpeed + ", CritChance : " + battleStat.CriticalChance + ", PenFixed : " + battleStat.PenetrationFixed + ", PenMult : " + battleStat.PenetrationMult);
#endif
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
    public void EquipWeapon(Item item)
    {
        if (weapon != null)
            weapon.RemoveItemEffects();

        if (item == null)
            weapon = item;
        else if (item.ItemType == ItemType.Weapon)
        {
            weapon = item;
            weapon.SetOwner(this);
            weapon.ApplyItemEffects();
        }
    }

    public void EquipArmor(Item item)
    {
        if (armor != null)
            armor.RemoveItemEffects();

        if (item == null)
            armor = item;
        else if (item.ItemType == ItemType.Armor)
        {
            //Debug.Log("Armor equiped.");
            armor = item;
            armor.SetOwner(this);
            armor.ApplyItemEffects();
        }
    }

    public void EquipAccessory1(Item item)
    {
        if (accessory1 != null)
            accessory1.RemoveItemEffects();

        if (item == null)
            accessory1 = item;
        else if (item.ItemType == ItemType.Accessory)
        {
            accessory1 = item;
            accessory1.SetOwner(this);
            accessory1.ApplyItemEffects();
        }
    }

    public void EquipAccessory2(Item item)
    {
        if (accessory2 != null)
            accessory2.RemoveItemEffects();

        if (item == null)
            accessory2 = item;
        else if (item.ItemType == ItemType.Accessory)
        {
            accessory2 = item;
            accessory2.SetOwner(this);
            accessory2.ApplyItemEffects();
        }
    }

    #endregion

    #region BossBattle
    #endregion
}
