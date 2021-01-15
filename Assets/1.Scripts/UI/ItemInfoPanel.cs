using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPanel : MonoBehaviour
{
    string itemName, stat, explanation, skillName, skillEffect, price, demandedLevel;
    bool isSkill = false;
    public Text nameText, statText, explanationText, skillNameText, skillEffectText, priceText, demandedLevelText;
    PurchaseButton purchaseBtn;
    public SkillExplSwitchButton skillExplBtn;

    public void ShowInfo()
    {
        isSkill = false;
        //nameText.text = itemName;
        //modifierText.text = modifier;
        //explanationText.text = explanation;
        //skillText.text = skill;
        //priceText.text = price;

        skillExplBtn.SwitchToSkill();

        switch (price)
        {
            case "장착 중":
                purchaseBtn.SetEquiped();
                break;
            case "이미 구매함":
                purchaseBtn.SetOwned();
                break;
            default:
                purchaseBtn.SetNeedPurchase();
                break;
        }
    }

    public void SetName(string inputName)
    {
        itemName = inputName;
        nameText.text = itemName;
    }

    public void SetStat(string inputStat)
    {
        stat = inputStat;
        statText.text = stat;
    }

    public void SetSkillEffect(string inputSkillEffect)
    {
        skillEffect = inputSkillEffect;
        skillEffectText.text = skillEffect;
    }

    public void SetSkillName(string inputSkillName)
    {
        skillName = inputSkillName;
        skillNameText.text = skillName;
    }

    public void SetPrice(string inputPrice)
    {
        price = inputPrice;
        priceText.text = price;
    }

    public void SetDemandedLevel(string inputDemandedLevel)
    {
        demandedLevel = inputDemandedLevel;
        demandedLevelText.text = demandedLevel;
    }

    public void SetOnlyExpl(string inputExpl)
    {
        explanation = inputExpl;
        explanationText.text = explanation;
        skillExplBtn.EnableOnlyExlp();
    }

    public void SetSkillAndExpl(string inputSkillName, string inputSkillEffect, string inputExpl)
    {
        explanation = inputExpl;
        explanationText.text = explanation;

        skillName = inputSkillName;
        skillNameText.text = skillName;

        skillEffect = inputSkillEffect;
        skillEffectText.text = inputSkillEffect; 

        skillExplBtn.EnableBoth();
    }
}
