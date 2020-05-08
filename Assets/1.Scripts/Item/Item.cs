using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item// : IHasEquipmentEffect
{
    List<StatModContinuous> statModContinuousList;
    List<StatModDiscrete> statModDiscreteList;

    public int code { get; set; }
	public string name { get; set; }
	public string icon { get; set; }
	public float attack { get; set; }
	public float defense { get; set; }
	public float health { get; set; }
	public float shiled { get; set; }
	public float critical { get; set; }
	public float attackspeed { get; set; }
	public float penetrate { get; set; }
	//public List<Enchantment> enchantments;
	//public List<EquipmentEffect> equipmentEffects;
    public int value { get; set; }
	public int minProperLevel { get; set; }
	public int maxProperLevel { get; set; }
	public int demandLevel { get; set; }
	public string explain { get; set; }


}
