using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemComparisonPanel : MonoBehaviour
{
    public Text levelValue;
    public Text healthValue, healthVariance;
    public Text defenceValue, defenceVariance;
    public Text moveSpeedValue, moveSpeedVariance;
    public Text rangeValue, rangeVariance;
    public Text penMultValue, penMultVariance;
    public Text dpsValue, dpsVariance;
    public Text attackValue, attackVariance;
    public Text attackSpeedValue, attackSpeedVariance;
    public Text critChanceValue, critChanceVariance;
    public Text critDamageValue, critDamageVariance;
    public Text penFixedValue, penFixedVariance;

    private readonly Color increaseColor = new Color(0.5764706f, 1.0f, 0), decreaseColor = new Color(1.0f, 0.7254902f, 0.1372549f);
    private readonly string increaseSymbol = "▲", decreaseSymbol = "▼";

    private SpecialAdventurer dummyBefore, dummyAfter;

    private void OnEnable() // SpAdv선택해야 Enable 되므로문제의 여지가 없음.
    {
        dummyBefore = GameManager.Instance.GetDummyBefore().GetComponent<SpecialAdventurer>();
        dummyAfter = GameManager.Instance.GetDummyAfter().GetComponent<SpecialAdventurer>();
    }

    public void RefreshComparisonPanel()
    {
        BattleStat beforeStat = dummyBefore.GetBattleStat();
        BattleStat afterStat = dummyAfter.GetBattleStat();

        float varianceTemp;

        levelValue.text = beforeStat.Level.ToString();


        healthValue.text = Mathf.RoundToInt(afterStat.HealthMax).ToString();
        varianceTemp = afterStat.HealthMax - beforeStat.HealthMax;
        if (varianceTemp > 0 + Mathf.Epsilon)
        {
            healthVariance.color = increaseColor;
            healthVariance.text = increaseSymbol + Mathf.Abs(Mathf.RoundToInt(varianceTemp));
        }
        else if (varianceTemp < 0 - Mathf.Epsilon)
        {
            healthVariance.color = decreaseColor;
            healthVariance.text = decreaseSymbol + Mathf.Abs(Mathf.RoundToInt(varianceTemp));
        }
        else
            healthVariance.text = "";


        defenceValue.text = Mathf.RoundToInt(afterStat.Defence).ToString();
        varianceTemp = afterStat.Defence - beforeStat.Defence;
        if (varianceTemp > 0 + Mathf.Epsilon)
        {
            defenceVariance.color = increaseColor;
            defenceVariance.text = increaseSymbol + Mathf.Abs(Mathf.RoundToInt(varianceTemp));
        }
        else if (varianceTemp < 0 - Mathf.Epsilon)
        {
            defenceVariance.color = decreaseColor;
            defenceVariance.text = decreaseSymbol + Mathf.Abs(Mathf.RoundToInt(varianceTemp));
        }
        else
            defenceVariance.text = "";

        moveSpeedValue.text = Mathf.RoundToInt(afterStat.MoveSpeed).ToString()+"%";
        varianceTemp = afterStat.MoveSpeed - beforeStat.MoveSpeed;
        if (varianceTemp > 0 + Mathf.Epsilon)
        {
            moveSpeedVariance.color = increaseColor;
            moveSpeedVariance.text = increaseSymbol + Mathf.Abs(Mathf.RoundToInt(varianceTemp));
        }
        else if (varianceTemp < 0 - Mathf.Epsilon)
        {
            moveSpeedVariance.color = decreaseColor;
            moveSpeedVariance.text = decreaseSymbol + Mathf.Abs(Mathf.RoundToInt(varianceTemp));
        }
        else
            moveSpeedVariance.text = "";

        rangeValue.text = Mathf.RoundToInt(afterStat.Range).ToString()+"칸";
        varianceTemp = afterStat.Range - beforeStat.Range;
        if (varianceTemp > 0 + Mathf.Epsilon)
        {
            rangeVariance.color = increaseColor;
            rangeVariance.text = increaseSymbol + Mathf.Abs(Mathf.RoundToInt(varianceTemp));
        }
        else if (varianceTemp < 0 - Mathf.Epsilon)
        {
            rangeVariance.color = decreaseColor;
            rangeVariance.text = decreaseSymbol + Mathf.Abs(Mathf.RoundToInt(varianceTemp));
        }
        else
            rangeVariance.text = "";


        penMultValue.text = Mathf.RoundToInt(afterStat.PenetrationMult * 100).ToString() + "%";
        varianceTemp = afterStat.PenetrationMult - beforeStat.PenetrationMult;
        if (varianceTemp > 0 + Mathf.Epsilon)
        {
            penMultVariance.color = increaseColor;
            penMultVariance.text = increaseSymbol + Mathf.Abs(Mathf.RoundToInt(varianceTemp * 100));
        }
        else if (varianceTemp < 0 - Mathf.Epsilon)
        {
            penMultVariance.color = decreaseColor;
            penMultVariance.text = decreaseSymbol + Mathf.Abs(Mathf.RoundToInt(varianceTemp * 100));
        }
        else
            penMultVariance.text = "";


        float dpsBefore = beforeStat.Attack * beforeStat.AttackSpeed * ((1 + beforeStat.CriticalChance) * (beforeStat.CriticalDamage - 1));
        float dpsAfter = afterStat.Attack * afterStat.AttackSpeed * ((1 + afterStat.CriticalChance) * (afterStat.CriticalDamage - 1));
        
        dpsValue.text = string.Format("{0:0.00}", dpsAfter);
        varianceTemp = dpsAfter - dpsBefore;
        string dpsVarianceStr = string.Format("{0:0.00}", Mathf.Abs(dpsAfter-dpsBefore));
        if (varianceTemp > 0 + Mathf.Epsilon)
        {
            dpsVariance.color = increaseColor;
            dpsVariance.text = increaseSymbol + dpsVarianceStr;
        }
        else if (varianceTemp < 0 - Mathf.Epsilon)
        {
            dpsVariance.color = decreaseColor;
            dpsVariance.text = decreaseSymbol + dpsVarianceStr;
        }
        else
            dpsVariance.text = "";


        attackValue.text = Mathf.RoundToInt(afterStat.Attack).ToString();
        varianceTemp = afterStat.Attack - beforeStat.Attack;
        if (varianceTemp > 0 + Mathf.Epsilon)
        {
            attackVariance.color = increaseColor;
            attackVariance.text = increaseSymbol + Mathf.Abs(Mathf.RoundToInt(varianceTemp));
        }
        else if (varianceTemp < 0 - Mathf.Epsilon)
        {
            attackVariance.color = decreaseColor;
            attackVariance.text = decreaseSymbol + Mathf.Abs(Mathf.RoundToInt(varianceTemp));
        }
        else
            attackVariance.text = "";

        attackSpeedValue.text = string.Format("{0:0.00}", afterStat.AttackSpeed);
        varianceTemp = afterStat.AttackSpeed - beforeStat.AttackSpeed;
        if (varianceTemp > 0 + Mathf.Epsilon)
        {
            attackSpeedVariance.color = increaseColor;
            attackSpeedVariance.text = increaseSymbol + string.Format("{0:0.00}", Mathf.Abs(varianceTemp));
        }
        else if (varianceTemp < 0 - Mathf.Epsilon)
        {
            attackSpeedVariance.color = decreaseColor;
            attackSpeedVariance.text = decreaseSymbol + string.Format("{0:0.00}", Mathf.Abs(varianceTemp));
        }
        else
            attackSpeedVariance.text = "";

        critChanceValue.text = string.Format("{0:0}%", afterStat.CriticalChance * 100);
        varianceTemp = afterStat.CriticalChance - beforeStat.CriticalChance;
        if (varianceTemp > 0 + Mathf.Epsilon)
        {
            critChanceVariance.color = increaseColor;
            critChanceVariance.text = increaseSymbol + string.Format("{0:0}", Mathf.Abs(varianceTemp * 100));
        }
        else if (varianceTemp < 0 - Mathf.Epsilon)
        {
            critChanceVariance.color = decreaseColor;
            critChanceVariance.text = decreaseSymbol + string.Format("{0:0}", Mathf.Abs(varianceTemp * 100));
        }
        else
            critChanceVariance.text = "";

        critDamageValue.text = string.Format("{0:0}%", afterStat.CriticalDamage * 100);
        varianceTemp = afterStat.CriticalDamage - beforeStat.CriticalDamage;
        if (varianceTemp > 0 + Mathf.Epsilon)
        {
            critDamageVariance.color = increaseColor;
            critDamageVariance.text = increaseSymbol + string.Format("{0:0}", Mathf.Abs(varianceTemp * 100));
        }
        else if (varianceTemp < 0 - Mathf.Epsilon)
        {
            critDamageVariance.color = decreaseColor;
            critDamageVariance.text = decreaseSymbol + string.Format("{0:0}", Mathf.Abs(varianceTemp * 100));
        }
        else
            critDamageVariance.text = "";

        penFixedValue.text = Mathf.RoundToInt(afterStat.PenetrationFixed).ToString();
        varianceTemp = afterStat.PenetrationFixed - beforeStat.PenetrationFixed;
        if (varianceTemp > 0 + Mathf.Epsilon)
        {
            penFixedVariance.color = increaseColor;
            penFixedVariance.text = increaseSymbol + Mathf.Abs(Mathf.RoundToInt(varianceTemp));
        }
        else if (varianceTemp < 0 - Mathf.Epsilon)
        {
            penFixedVariance.color = decreaseColor;
            penFixedVariance.text = decreaseSymbol + Mathf.Abs(Mathf.RoundToInt(varianceTemp));
        }
        else
            penFixedVariance.text = "";
    }
}
