using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillFactory
{
    public static Skill CreateSkill(GameObject go, string skillName)
    {
        switch (skillName)
        {
            case "OldMan":
                return go.AddComponent<OldManUniqueSkill>();
            case "Hana":
                return go.AddComponent<HanaUniqueSkill>();
            case "Murat":
                return go.AddComponent<MuratUniqueSkill>();
            case "Yeonhwa":
                return go.AddComponent<YeonhwaUniqueSkill>();
            case "Iris":
                return go.AddComponent<IrisUniqueSkill>();
            case "Nyang":
                return go.AddComponent<NyangUniqueSkill>();
            case "Wal":
                return go.AddComponent<WalUniqueSkill>();
            default:
                return null;
        }
    }
}
