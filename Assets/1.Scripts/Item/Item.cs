using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public Item()
    {
        continuousMods = new List<StatModContinuous>();
        discreteMods = new List<StatModDiscrete>();
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
}
