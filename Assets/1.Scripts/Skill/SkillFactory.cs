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
            case "Maxi":
                CommonSelfBuffSkill temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("쇼맨십", "공격속도가 200% 증가하지만 공격력은 65% 감소합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.Attack, ModType.Mult, -0.65f));
                temp.AddContinuousMod(new StatModContinuous(StatType.AttackSpeed, ModType.Mult, 2.0f));
                return temp;
            case "ThornMail":
                return go.AddComponent<ThornMailSkill>();
            case "DamageAbsorb":
                return go.AddComponent<DamageAbsorbSkill>();
            case "RepulsivePower":
                return go.AddComponent<RepulsivePowerSkill>();
            case "OverBoost":
                return go.AddComponent<OverBoostSkill>();
            case "Crack":
                return go.AddComponent<CrackSkill>();
            case "Sweep":
                return go.AddComponent<SweepSkill>();
            case "Bless":
                return go.AddComponent<BlessSkill>();
            case "Buckshot":
                return go.AddComponent<BuckshotSkill>();
            case "StaticElectricity":
                return go.AddComponent<StaticElectricitySkill>();
            case "DualWield":
                return go.AddComponent<DualWieldSkill>();
            case "Thunderbolt":
                return go.AddComponent<ThunderboltSkill>();
            case "Amplify":
                return go.AddComponent<AmplifySkill>();
            case "ShockWave":
                return go.AddComponent<ShockWaveSkill>();
            case "LifeSteal":
                return go.AddComponent<LifeStealSkill>();
            case "Blaze":
                return go.AddComponent<BlazeSkill>();
            case "Lava":
                return go.AddComponent<LavaSkill>();
            case "Immerse":
                return go.AddComponent<ImmerseSkill>();
            case "FlameAura":
                return go.AddComponent<FlameAuraSkill>();
            case "RedPotion":
                return go.AddComponent<RedPotionSkill>();
            case "Rejuvenate":
                return go.AddComponent<RejuvenateSkill>();
            case "DivineProtection":
                return go.AddComponent<DivineProtectionSkill>();
            case "CrisisManagement":
                return go.AddComponent<CrisisManagementSkill>();
            case "Execution":
                return go.AddComponent<ExecutionSkill>();
            case "MirrorImage":
                return go.AddComponent<MirrorImage>();
            default:
                return null;
        }
    }
}
