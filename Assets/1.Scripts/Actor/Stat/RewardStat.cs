using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardStat
{
    int rewardGold;
    int rewardExp;

    public RewardStat() { }

    public RewardStat(RewardStat input)
    {
        Gold = input.Gold;
        Exp = input.Exp;
    }

    public void SetRewardStatToLevel(int level)
    {
        Exp = level * 50;
        Gold = 5 * (Mathf.RoundToInt(level * 0.9f) + 20);
    }

    public int Gold
    {
        get
        {
            return rewardGold;
        }
        set
        {
            rewardGold = value;
        }
    }

    public int Exp
    {
        get
        {
            return rewardExp;
        }
        set
        {
            rewardExp = value;
        }
    }
}
