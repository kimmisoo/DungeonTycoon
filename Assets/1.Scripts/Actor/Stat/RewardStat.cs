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
