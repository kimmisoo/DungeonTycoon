using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
public enum ItemType
{
    Weapon, Armor, Accessory
}

public class Item
{
    private List<StatModContinuous> continuousMods;
    private List<StatModDiscrete> discreteMods;

    public string itemSkillKey;

    private SpecialAdventurer owner;
    private BattleStat ownerBattleStat;

    public ItemType ItemType { get; set; }
    public int Code { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }

    public int Price { get; set; }
    public int OptimalLevelLower { get; set; }
    public int OptimalLevelUpper { get; set; }
    public int DemandedLevel { get; set; }
    public string Explanation { get; set; }
    public string BonusText { get; set; } // UI용 보너스 텍스트
    Sprite itemImage=null;
    // 세이브용
    public int itemIndex;
    public string itemCategory;

    public Item()
    {
        continuousMods = new List<StatModContinuous>();
        discreteMods = new List<StatModDiscrete>();
		itemImage = Resources.Load<Sprite>("Items/weapons_image/001");
    }

    public string SkillName
    {
        get
        {
            string tempName, tempExplanation;

            SkillFactory.GetNameAndExplanation(itemSkillKey, out tempName, out tempExplanation);

            return tempName;
        }
    }

    public string SkillExplanation
    {
        get
        {
            string tempName, tempExplanation;

            SkillFactory.GetNameAndExplanation(itemSkillKey, out tempName, out tempExplanation);

            return tempExplanation;
        }
    }

    public void SetOwner(SpecialAdventurer spAdv)
    {
        owner = spAdv;
        ownerBattleStat = spAdv.GetBattleStat();
    }

    public void ApplyItemEffects()
    {
        foreach (StatModContinuous mod in continuousMods)
            ownerBattleStat.AddStatModContinuous(mod);
        foreach (StatModDiscrete mod in discreteMods)
            ownerBattleStat.AddStatModDiscrete(mod);

        //Debug.Log(itemSkillKey);
        //Debug.Log("ItemSkillKey : " + itemSkillKey);
        if (itemSkillKey != null)
            owner.AddSkill(itemSkillKey);
        //    itemSkills.Add(SkillFactory.CreateSkill(owner, skillName));
    }

    public void RemoveItemEffects()
    {
        foreach (StatModContinuous mod in continuousMods)
            ownerBattleStat.RemoveStatModContinuous(mod);
        foreach (StatModDiscrete mod in discreteMods)
            ownerBattleStat.RemoveStatModDiscrete(mod);

        if (itemSkillKey != null)
            owner.RemoveSkill(itemSkillKey);
    }

    public void AddStatModContinous(StatModContinuous statMod)
    {
        //Debug.Log("[AddStatModContinous] " + statMod.StatType + " : " + statMod.ModValue);
        continuousMods.Add(statMod);
    }

    public void AddStatModDiscrete(StatModDiscrete statMod)
    {
        discreteMods.Add(statMod);
    }
	public string GetItemStatAsString()
	{
		StringBuilder sb = new StringBuilder();
		foreach(StatModContinuous smc in continuousMods)
		{

			sb.Append(smc.StatType.ToString());
			sb.Append(" : ");
			sb.Append(smc.ModValue.ToString());
			sb.Append("\n");
		}
		foreach(StatModDiscrete sdc in discreteMods)
		{
			sb.Append(sdc.StatType.ToString());
			sb.Append(" : ");
			sb.Append(sdc.ModValue.ToString());
			sb.Append("\n");
		}
		sb.Append(SkillName);
		sb.Append(" : ");
		sb.Append(SkillExplanation);
		return sb.ToString();
	}
	public Sprite GetItemImage()
	{
		return itemImage;
	}
}
