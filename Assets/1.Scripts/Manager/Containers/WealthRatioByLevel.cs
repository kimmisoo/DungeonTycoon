using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WealthRatioByLevel
{
    public int levelMin;
    public int levelMax;

    public List<KeyValuePair<string, float>> wealthRatio;

    public WealthRatioByLevel()
    {
        levelMin = 0;
        levelMax = 100;

        wealthRatio = new List<KeyValuePair<string, float>>();
    }
}