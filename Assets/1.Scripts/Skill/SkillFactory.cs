using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillFactory
{
    public static Skill CreateSkill(GameObject go, string skillName)
    {
        switch (skillName)
        {
            case "Hana":
                return go.AddComponent<HanaUniqueSkill>();
            default:
                return null;
        }
    }
}
