using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatModContinuous
{
    public StatModContinuous(StatType statType, ModType modType, float value)
    {
        StatType = statType;
        ModType = modType;
        ModValue = value;
    }

    public StatModContinuous(StatModContinuous statMod)
    {
        StatType = statMod.StatType;
        ModType = statMod.ModType;
        ModValue = statMod.ModValue;
    }

    public float ModValue { get; set; } = 0.0f;
    public ModType ModType { get; set; } = ModType.Fixed;
    public StatType StatType { get; set; }
}
