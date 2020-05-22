using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillFactory
{
    public static Skill CreateSkill(GameObject go, string skillName)
    {
        CommonSelfBuffSkill temp;

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
                temp = go.AddComponent<CommonSelfBuffSkill>();
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
            case "SiphonStrength":
                return go.AddComponent<SiphonStrengthSkill>();
            case "SelfDefence":
                return go.AddComponent<SelfDefenceSkill>();
            case "Scar":
                return go.AddComponent<ScarSkill>();
            case "HeavyWeight":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("무거움", "공격 속도가 10% 감소합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.AttackSpeed, ModType.Mult, -0.1f));
                return temp;
            case "LightWeight":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("가벼움", "공격 속도가 10% 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.AttackSpeed, ModType.Mult, 0.1f));
                return temp;
            case "Uncomfortable":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("걸리적거림", "치명타 확률이 10% 감소합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.CriticalChance, ModType.Fixed, -0.1f));
                return temp;
            case "Comfortable":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("움직이기 편함", "치명타 확률이 10% 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.CriticalChance, ModType.Fixed, 0.1f));
                return temp;
            case "VeryLightWeight":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("매우 가벼움", "공격 속도가 15% 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.AttackSpeed, ModType.Mult, 0.15f));
                return temp;
            case "PoweredExoskeleton":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("강화 외골격", "공격력이 10% 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.Attack, ModType.Mult, 0.1f));
                return temp;
            case "VeryUncomfortable":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("매우 걸리적거림", "치명타 확률이 15% 감소합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.CriticalChance, ModType.Fixed, -0.15f));
                return temp;
            case "VeryComfortable":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("움직이기 매우 편함", "치명타 확률이 15% 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.CriticalChance, ModType.Fixed, 0.15f));
                return temp;
            case "VeryLightAndComfortable":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("매우 가볍고 편함", "공격 속도가 15%, 치명타 확률이 15% 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.CriticalChance, ModType.Fixed, 0.15f));
                temp.AddContinuousMod(new StatModContinuous(StatType.AttackSpeed, ModType.Mult, 0.15f));
                return temp;
            case "WeaponMaintenance":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("무기 정비", "방어구 관통력이 15 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.PenetrationFixed, ModType.Fixed, 15.0f));
                return temp;
            case "CatchVitalPoint":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("급소 포착", "치명타 확률이 10% 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.CriticalChance, ModType.Fixed, 0.1f));
                return temp;
            case "JustOneSlash":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("일섬", "치명타 공격력이 50% 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.CriticalDamage, ModType.Fixed, 0.1f));
                return temp;
            case "AdditionalArmor":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("추가 장갑", "방어력이 30% 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.Defence, ModType.Mult, 0.3f));
                return temp;
            case "Accelerate":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("가속", "공격 속도가 10% 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.AttackSpeed, ModType.Mult, 0.1f));
                return temp;
            case "SwordAura":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("검기", "사정거리가 1 증가합니다.");
                temp.AddDiscreteMod(new StatModDiscrete(StatType.AttackRange, ModType.Fixed, 1));
                return temp;
            case "CatchVitalPoint2":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("급소 포착 II", "치명타 확률이 20% 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.CriticalChance, ModType.Fixed, 0.2f));
                return temp;
            case "Accelerate2":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("가속 II", "공격 속도가 20% 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.AttackSpeed, ModType.Mult, 0.2f));
                return temp;
            case "BlessedByFortuna":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("포르투나의 축복", "회피율이 10% 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.Avoid, ModType.Fixed, 0.1f));
                return temp;
            case "LesserPretectionSpell":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("하급 보호 마법", "체력이 60 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.HealthMax, ModType.Fixed, 60.0f));
                return temp;
            case "StandardPretectionSpell":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("중급 보호 마법", "체력이 150 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.HealthMax, ModType.Fixed, 150.0f));
                return temp;
            case "GreaterPretectionSpell":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("상급 보호 마법", "체력이 12% 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.HealthMax, ModType.Mult, 0.12f));
                return temp;
            case "HeavyBlow":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("묵직함", "방어구 관통력이 20% 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.PenetrationMult, ModType.Fixed, 0.2f));
                return temp;
            case "SharpendBlade":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("날카로운 날", "방어구 관통력이 30% 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.PenetrationMult, ModType.Fixed, 0.3f));
                return temp;
            case "ArcaneBlade":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("비전 칼날", "방어구 관통력이 40% 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.PenetrationMult, ModType.Fixed, 0.4f));
                return temp;
            case "SmallShield":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("소형 방패", "방어력이 20 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.Defence, ModType.Fixed, 20.0f));
                return temp;
            case "MediumShield":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("중형 방패", "방어력이 30 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.Defence, ModType.Fixed, 30.0f));
                return temp;
            case "LargeShield":
                temp = go.AddComponent<CommonSelfBuffSkill>();
                temp.InstantiateLists();
                temp.SetNameAndExplanation("대형 방패", "방어력이 40 증가합니다.");
                temp.AddContinuousMod(new StatModContinuous(StatType.Defence, ModType.Fixed, 40.0f));
                return temp;
            default:
                return null;
        }
    }
}
