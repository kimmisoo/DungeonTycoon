using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryEffect
{
    List<StatModContinuous> continuousMods;
    List<StatModDiscrete> discreteMods;
    ICombatant subject;
    BattleStat subjectBattleStat;

    public readonly float DURATION; // -1이면 영구.
    public float elapsedTime;

    public TemporaryEffect(float duration = -1)
    {
        continuousMods = new List<StatModContinuous>();
        discreteMods = new List<StatModDiscrete>();

        DURATION = duration;
        elapsedTime = 0;
    }

    public TemporaryEffect(TemporaryEffect tempEffect)
    {
        continuousMods = new List<StatModContinuous>();
        discreteMods = new List<StatModDiscrete>();

        DURATION = tempEffect.DURATION;
        elapsedTime = 0;

        subject = tempEffect.subject;
        subjectBattleStat = tempEffect.subjectBattleStat;

        foreach (StatModContinuous statMod in tempEffect.continuousMods)
            continuousMods.Add(new StatModContinuous(statMod));
        foreach (StatModDiscrete statMod in tempEffect.discreteMods)
            discreteMods.Add(new StatModDiscrete(statMod));

        //Debug.Log("StatType: " + continuousMods[0].StatType + ", Value: " + continuousMods[0].ModValue);
    }

    public void SetSubject(ICombatant subjectIn)
    {
        subject = subjectIn;
        subjectBattleStat = subject.GetBattleStat();
    }

    public void AddContinuousMod(StatModContinuous statMod)
    {
        continuousMods.Add(statMod);
    }

    public void AddDiscreteMod(StatModDiscrete statMod)
    {
        discreteMods.Add(statMod);
    }

    /// <summary>
    /// 버프/디버프 등을 Refresh하고 지속시간이 다 되었으면 효과 취소.
    /// </summary>
    /// <param name="deltaTime">이전 refresh부터 흐른 시간. 기본값 TICK_TIME</param>
    /// <returns>만료되었는지 여부. true면 제거해줘야 함.</returns>
    public bool Refresh(float deltaTime = SkillConsts.TICK_TIME)
    {
        if (DURATION >= 0)
        {
            //Debug.Log("Elapsed : " + elapsedTime + ",  Duration : " + DURATION);
            elapsedTime += deltaTime;
            if (elapsedTime >= DURATION)
                return true; //만료되었는지 여부.
        }
        return false;
    }

    public void ApplyEffect()
    {
        elapsedTime = 0;

        foreach(StatModContinuous statMod in continuousMods)
        {
            subjectBattleStat.AddStatModContinuous(statMod);
        }

        foreach (StatModDiscrete statMod in discreteMods)
        {
            subjectBattleStat.AddStatModDiscrete(statMod);
        }
    }

    public void RemoveEffect()
    {
        foreach (StatModContinuous statMod in continuousMods)
        {
            subjectBattleStat.RemoveStatModContinuous(statMod);
        }

        foreach (StatModDiscrete statMod in discreteMods)
        {
            subjectBattleStat.RemoveStatModDiscrete(statMod);
        }
    }
}
