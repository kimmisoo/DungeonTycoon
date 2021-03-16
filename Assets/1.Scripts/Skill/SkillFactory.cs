using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillFactory
{
    public static Skill CreateSkill(GameObject go, string skillName)
    {
        CommonSelfBuffSkill tempCommonBuff;
        Skill skill;

        // 여기부터. 스킬 이름, 설명 세팅 바꿔줘야함.

        switch (skillName)
        {
            case "OldMan":
                skill = go.AddComponent<OldManUniqueSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "Hana":
                skill = go.AddComponent<HanaUniqueSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "Murat":
                skill = go.AddComponent<MuratUniqueSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "Yeonhwa":
                skill = go.AddComponent<YeonhwaUniqueSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "Iris":
                skill = go.AddComponent<IrisUniqueSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "Nyang":
                skill = go.AddComponent<NyangUniqueSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "Wal":
                skill = go.AddComponent<WalUniqueSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "Maxi":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.Attack, ModType.Mult, -0.65f));
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.AttackSpeed, ModType.Mult, 2.0f));
                return tempCommonBuff;
            case "ThornMail":
                skill = go.AddComponent<ThornMailSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "DamageAbsorb":
                skill = go.AddComponent<DamageAbsorbSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "RepulsivePower":
                skill = go.AddComponent<RepulsivePowerSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "OverBoost":
                skill = go.AddComponent<OverBoostSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "Crack":
                skill = go.AddComponent<CrackSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "Sweep":
                skill = go.AddComponent<SweepSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "Bless":
                skill = go.AddComponent<BlessSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "Buckshot":
                skill = go.AddComponent<BuckshotSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "StaticElectricity":
                skill = go.AddComponent<StaticElectricitySkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "DualWield":
                skill = go.AddComponent<DualWieldSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "Thunderbolt":
                skill = go.AddComponent<ThunderboltSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "Amplify":
                skill = go.AddComponent<AmplifySkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "ShockWave":
                skill = go.AddComponent<ShockWaveSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "LifeSteal":
                skill = go.AddComponent<LifeStealSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "Blaze":
                skill = go.AddComponent<BlazeSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "Lava":
                skill = go.AddComponent<LavaSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "Immerse":
                skill = go.AddComponent<ImmerseSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "FlameAura":
                skill = go.AddComponent<FlameAuraSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "RedPotion":
                skill = go.AddComponent<RedPotionSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "Rejuvenate":
                skill = go.AddComponent<RejuvenateSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "DivineProtection":
                skill = go.AddComponent<DivineProtectionSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "CrisisManagement":
                skill = go.AddComponent<CrisisManagementSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "Execution":
                skill = go.AddComponent<ExecutionSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "MirrorImage":
                skill = go.AddComponent<MirrorImage>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "SiphonStrength":
                skill = go.AddComponent<SiphonStrengthSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "SelfDefence":
                skill = go.AddComponent<SelfDefenceSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "Scar":
                skill = go.AddComponent<ScarSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "LifeTap":
                skill = go.AddComponent<LifeTapSkill>();
                skill.SetNameAndExplanation(skillName);
                return skill;
            case "HeavyWeight":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.AttackSpeed, ModType.Mult, -0.1f));
                return tempCommonBuff;
            case "LightWeight":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.AttackSpeed, ModType.Mult, 0.1f));
                return tempCommonBuff;
            case "Uncomfortable":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.CriticalChance, ModType.Fixed, -0.1f));
                return tempCommonBuff;
            case "Comfortable":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.CriticalChance, ModType.Fixed, 0.1f));
                return tempCommonBuff;
            case "VeryLightWeight":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.AttackSpeed, ModType.Mult, 0.15f));
                return tempCommonBuff;
            case "PoweredExoskeleton":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.Attack, ModType.Mult, 0.1f));
                return tempCommonBuff;
            case "VeryUncomfortable":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.CriticalChance, ModType.Fixed, -0.15f));
                return tempCommonBuff;
            case "VeryComfortable":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.CriticalChance, ModType.Fixed, 0.15f));
                return tempCommonBuff;
            case "VeryLightAndComfortable":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.CriticalChance, ModType.Fixed, 0.15f));
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.AttackSpeed, ModType.Mult, 0.15f));
                return tempCommonBuff;
            case "WeaponMaintenance":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.PenetrationFixed, ModType.Fixed, 15.0f));
                return tempCommonBuff;
            case "CatchVitalPoint":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.CriticalChance, ModType.Fixed, 0.1f));
                return tempCommonBuff;
            case "JustOneSlash":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.CriticalDamage, ModType.Fixed, 0.1f));
                return tempCommonBuff;
            case "AdditionalArmor":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.Defence, ModType.Mult, 0.3f));
                return tempCommonBuff;
            case "Accelerate":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.AttackSpeed, ModType.Mult, 0.1f));
                return tempCommonBuff;
            case "SwordAura":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddDiscreteMod(new StatModDiscrete(StatType.AttackRange, ModType.Fixed, 1));
                return tempCommonBuff;
            case "CatchVitalPoint2":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.CriticalChance, ModType.Fixed, 0.2f));
                return tempCommonBuff;
            case "Accelerate2":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.AttackSpeed, ModType.Mult, 0.2f));
                return tempCommonBuff;
            case "BlessedByFortuna":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.Avoid, ModType.Fixed, 0.1f));
                return tempCommonBuff;
            case "LesserPretectionSpell":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.HealthMax, ModType.Fixed, 60.0f));
                return tempCommonBuff;
            case "StandardPretectionSpell":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.HealthMax, ModType.Fixed, 150.0f));
                return tempCommonBuff;
            case "GreaterPretectionSpell":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.HealthMax, ModType.Mult, 0.12f));
                return tempCommonBuff;
            case "HeavyBlow":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.PenetrationMult, ModType.Fixed, 0.2f));
                return tempCommonBuff;
            case "SharpendBlade":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.PenetrationMult, ModType.Fixed, 0.3f));
                return tempCommonBuff;
            case "ArcaneBlade":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.PenetrationMult, ModType.Fixed, 0.4f));
                return tempCommonBuff;
            case "SmallShield":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.Defence, ModType.Fixed, 20.0f));
                return tempCommonBuff;
            case "MediumShield":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.Defence, ModType.Fixed, 30.0f));
                return tempCommonBuff;
            case "LargeShield":
                tempCommonBuff = go.AddComponent<CommonSelfBuffSkill>();
                tempCommonBuff.InstantiateLists();
                tempCommonBuff.SetNameAndExplanation(skillName);
                tempCommonBuff.AddContinuousMod(new StatModContinuous(StatType.Defence, ModType.Fixed, 40.0f));
                return tempCommonBuff;
            default:
                return null;
        }
    }

    public static void GetNameAndExplanation(string key, out string name, out string explanation)
    {
        switch (key)
        {
            case "OldMan":
                name = "숨 고르기";
                explanation = "매 5초마다 잃은 체력의 5%를 회복합니다.";
                break;
            case "Hana":
                name = "축전";
                explanation = "매 1초마다 주위 1칸 내의 모든 적에게 공격력의 10%(대상이 하나일 때는 15%)만큼 피해를 줍니다. 준 피해의 양이 하나 최대체력의 12%가 될 때마다 주위 1칸 내의 모든 적에게 공격력의 170% 피해를 주고 방어력을 15% 감소시키는 디버프를 남깁니다. 디버프는 4초간 지속됩니다.";
                break;
            case "Murat":
                name = "광휘";
                explanation = "매 3번째 공격마다 최대 체력의 2%를 회복하고 그 2배의 피해를 적에게 줍니다. 적에게 주는 피해는 치명타가 발생하지 않는 대신, 적의 방어력을 무시합니다.";
                break;
            case "Yeonhwa":
                name = "환류";
                explanation = "회피율이 10% 증가합니다. 적의 공격을 회피할 때마다, 공격력의 155%만큼의 피해를 공격자에게 줍니다.";
                break;
            case "Iris":
                name = "빙화";
                explanation = "전투 개시 시, 그 후 7번째 공격마다 얼음 마법을 시전하여 공격 대상과 그 주위 1칸 모든 적에게 공격력의 210%(대상이 하나일 때는 240%)의 피해를 줍니다.";
                break;
            case "Nyang":
                name = "선수필승!";
                explanation = "비 전투 상태의 적을 공격할 때 공격력이 300% 증가합니다.";
                break;
            case "Wal":
                name = "파비스 석궁병";
                explanation = "공격력이 40% 증가하는 대신 공격속도가 33% 감소합니다. 매 사격마다 최대체력의 3%에 해당하는 방어막을 얻습니다. 방어막은 중첩되지 않습니다.";
                break;
            case "Maxi":
                name = "쇼맨십";
                explanation = "공격속도가 200% 증가하지만 공격력은 65% 감소합니다.";
                break;
            case "ThornMail":
                name = "가시갑옷";
                explanation = "공격을 받을 때, 받은 데미지의 15%를 공격자에게 되돌려줍니다.";
                break;
            case "DamageAbsorb":
                name = "데미지 흡수";
                explanation = "공격을 받을 때마다, 체력을 15 회복합니다.";
                break;
            case "RepulsivePower":
                name = "반발력";
                explanation = "방어력이 35% 증가합니다. 공격을 받을 때마다, 방어력의 20%만큼의 피해를 공격자에게 줍니다.";
                break;
            case "OverBoost":
                name = "오버부스트";
                explanation = "공격대상의 체력이 30% 이하일 때 공격력이 25% 증가합니다.";
                break;
            case "Crack":
                name = "균열";
                explanation = "적을 공격할 때마다 방어력을 6% 감소시키는 디버프를 남깁니다. 디버프는 최대 5번까지 중첩되며 2초간 지속됩니다.";
                break;
            case "Sweep":
                name = "휩쓸기";
                explanation = "적을 공격할 때마다 공격 대상 양 옆의 적에게 공격력의 35%만큼 피해를 줍니다.";
                break;
            case "Bless":
                name = "축복";
                explanation = "적을 공격할 때마다 입힌 피해의 12%에 해당하는 방어막을 얻습니다. 방어막은 3초 동안 지속됩니다.";
                break;
            case "Buckshot":
                name = "산탄";
                explanation = "사정거리가 1 증가합니다. 가까이 있는 적을 공격할 때 추가 공격력을 얻습니다.";
                break;
            case "StaticElectricity":
                name = "정전기 발생";
                explanation = "적을 공격할 때마다, 4의 추가 피해를 줍니다.";
                break;
            case "DualWield":
                name = "쌍수무기";
                explanation = "공격속도가 100% 증가하지만 공격력이 50% 감소합니다.";
                break;
            case "Thunderbolt":
                name = "벼락";
                explanation = "적을 공격할 때 치명타가 발생하면, 60의 추가 피해를 주고 적의 방어력을 10 감소시키는 디버프를 남깁니다. 디버프는 3초간 지속됩니다.";
                break;
            case "Amplify":
                name = "증폭";
                explanation = "적을 공격할 때마다, 공격대상 주위 1칸 내의 모든 적에게 공격력의 15%만큼 피해를 입힙니다.";
                break;
            case "ShockWave":
                name = "충격파 생성";
                explanation = "적을 공격할 때마다, 주위 1칸 내의 모든 적에게 10만큼의 피해를 입힙니다.";
                break;
            case "LifeSteal":
                name = "흡혈";
                explanation = "적을 공격할 때마다, 입힌 데미지의 12%만큼의 체력을 회복합니다.";
                break;
            case "Blaze":
                name = "불꽃";
                explanation = "적을 공격할 때마다, 12의 추가 피해를 줍니다.";
                break;
            case "Lava":
                name = "용암";
                explanation = "적을 공격할 때마다, 26의 추가 피해를 줍니다.";
                break;
            case "Immerse":
                name = "몰입";
                explanation = "전투 시작 후 1초가 지날 때마다 공격속도가 5% 증가하는 버프를 얻습니다. 버프는 최대 5번까지 중첩되며, 2초간 지속됩니다.";
                break;
            case "FlameAura":
                name = "화염 오라";
                explanation = "매 1초마다, 주위 1칸 내의 모든 적에게 공격력의 8%만큼의 피해를 입힙니다.";
                break;
            case "RedPotion":
                name = "빨간 물약";
                explanation = "매 3초마다, 체력을 1 회복합니다.";
                break;
            case "Rejuvenate":
                name = "재생";
                explanation = "매 1초마다, 잃은 체력의 1.5%를 회복합니다.";
                break;
            case "DivineProtection":
                name = "신의 가호";
                explanation = "매 1초마다, 주위 1칸 내의 모든 적에게 12만큼의 피해를 주고 방어력을 15 감소시키는 디버프를 남깁니다. 피해는 적의 방어력을 무시하며, 디버프는 2초 동안 지속됩니다.";
                break;
            case "CrisisManagement":
                name = "위기 대처";
                explanation = "체력이 20% 이하일 때, 방어력이 75 증가합니다.";
                break;
            case "Execution":
                name = "처형";
                explanation = "공격 대상의 체력이 20% 이하일 때 공격력이 50% 증가합니다.";
                break;
            case "MirrorImage":
                name = "분신";
                explanation = "적을 공격할 때 치명타가 발생하면, 공격력의 50%만큼 추가 피해를 줍니다.";
                break;
            case "SiphonStrength":
                name = "힘 흡수";
                explanation = "주위 1칸 내의 모든 적 공격력의 10%를 흡수합니다.";
                break;
            case "SelfDefence":
                name = "자기 방어";
                explanation = "방어력의 20%만큼 공격력이 증가합니다.";
                break;
            case "Scar":
                name = "상흔";
                explanation = "잃은 체력 50당 공격 속도가 1% 증가합니다. 최대치는 20%입니다.";
                break;
            case "LifeTap":
                name = "생명력 전환";
                explanation = "최대 체력이 20% 감소합니다. 감소한 최대 체력의 15%만큼 공격력이 증가합니다.";
                break;
            case "HeavyWeight":
                name = "무거움";
                explanation = "최대 체력이 20% 감소합니다. 감소한 최대 체력의 15%만큼 공격력이 증가합니다.";
                break;
            case "LightWeight":
                name = "가벼움";
                explanation = "공격 속도가 10% 증가합니다.";
                break;
            case "Uncomfortable":
                name = "걸리적거림";
                explanation = "치명타 확률이 10% 감소합니다.";
                break;
            case "Comfortable":
                name = "움직이기 편함";
                explanation = "치명타 확률이 10% 증가합니다.";
                break;
            case "VeryLightWeight":
                name = "매우 가벼움";
                explanation = "공격 속도가 15% 증가합니다.";
                break;
            case "PoweredExoskeleton":
                name = "강화 외골격";
                explanation = "공격력이 10% 증가합니다.";
                break;
            case "VeryUncomfortable":
                name = "매우 걸리적거림";
                explanation = "치명타 확률이 15% 감소합니다.";
                break;
            case "VeryComfortable":
                name = "움직이기 매우 편함";
                explanation = "치명타 확률이 15% 증가합니다.";
                break;
            case "VeryLightAndComfortable":
                name = "매우 가볍고 편함";
                explanation = "공격 속도가 15%, 치명타 확률이 15% 증가합니다.";
                break;
            case "WeaponMaintenance":
                name = "무기 정비";
                explanation = "방어구 관통력이 15 증가합니다.";
                break;
            case "CatchVitalPoint":
                name = "급소 포착";
                explanation = "치명타 확률이 10% 증가합니다.";
                break;
            case "JustOneSlash":
                name = "일섬";
                explanation = "치명타 공격력이 50% 증가합니다.";
                break;
            case "AdditionalArmor":
                name = "추가 장갑";
                explanation = "방어력이 30% 증가합니다.";
                break;
            case "Accelerate":
                name = "가속";
                explanation = "공격 속도가 10% 증가합니다.";
                break;
            case "SwordAura":
                name = "검기";
                explanation = "사정거리가 1 증가합니다.";
                break;
            case "CatchVitalPoint2":
                name = "급소 포착 II";
                explanation = "치명타 확률이 20% 증가합니다.";
                break;
            case "Accelerate2":
                name = "가속 II";
                explanation = "공격 속도가 20% 증가합니다.";
                break;
            case "BlessedByFortuna":
                name = "포르투나의 축복";
                explanation = "회피율이 10% 증가합니다.";
                break;
            case "LesserPretectionSpell":
                name = "하급 보호 마법";
                explanation = "체력이 60 증가합니다.";
                break;
            case "StandardPretectionSpell":
                name = "중급 보호 마법";
                explanation = "체력이 150 증가합니다.";
                break;
            case "GreaterPretectionSpell":
                name = "상급 보호 마법";
                explanation = "체력이 12% 증가합니다.";
                break;
            case "HeavyBlow":
                name = "묵직함";
                explanation = "방어구 관통력이 20% 증가합니다.";
                break;
            case "SharpendBlade":
                name = "날카로운 날";
                explanation = "방어구 관통력이 30% 증가합니다.";
                break;
            case "ArcaneBlade":
                name = "비전 칼날";
                explanation = "방어구 관통력이 40% 증가합니다.";
                break;
            case "SmallShield":
                name = "소형 방패";
                explanation = "방어력이 20 증가합니다.";
                break;
            case "MediumShield":
                name = "중형 방패";
                explanation = "방어력이 30 증가합니다.";
                break;
            case "LargeShield":
                name = "대형 방패";
                explanation = "방어력이 40 증가합니다.";
                break;
            default:
                name = null;
                explanation = null;
                break;
        }
    }
}
