using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ItemType
{
    Weapon, Armor, Accessory
}

public class Item// : IHasEquipmentEffect
{
    List<StatModContinuous> continuousMods;
    List<StatModDiscrete> discreteMods;

    public List<string> itemSkillNames;

    ICombatant owner;
    BattleStat ownerBattleStat;

    public ItemType ItemType { get; set; }
    public int Code { get; set; }
	public string Name { get; set; }
	public string Icon { get; set; }

    public int Price { get; set; }
	public int OptimalLevelLower { get; set; }
	public int OptimalLevelMax { get; set; }
	public int DemandedLevel { get; set; }
	public string Explanation { get; set; }

    public void ApplyItemEffects()
    {
        foreach (StatModContinuous mod in continuousMods)
            ownerBattleStat.AddStatModContinuous(mod);
        foreach (StatModDiscrete mod in discreteMods)
            ownerBattleStat.AddStatModDiscrete(mod);

        foreach (string skillName in itemSkillNames)
            owner.AddSkill(skillName);
        //    itemSkills.Add(SkillFactory.CreateSkill(owner, skillName));
    }

    public void RemoveItemEffects()
    {
        foreach (StatModContinuous mod in continuousMods)
            ownerBattleStat.RemoveStatModContinuous(mod);
        foreach (StatModDiscrete mod in discreteMods)
            ownerBattleStat.RemoveStatModDiscrete(mod);

        foreach (string skillName in itemSkillNames)
            owner.RemoveSkill(skillName);
    }
}
