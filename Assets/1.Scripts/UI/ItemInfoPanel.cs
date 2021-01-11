using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPanel : MonoBehaviour
{
    string itemName, modifier, explanation, skill, price;
    bool isSkill = false;
    Text nameText, modifierText, explanationText, skillText, priceText;
    PurchaseButton purchaseBtn;
    public SwitchSkillBetweenExplanation skillExplBtn;

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
        nameText.text = name;
    }

    public void SetModifier(string inputModifier)
    {
        modifier = inputModifier;
        modifierText.text = modifier;
    }

    public void SetExplanation(string inputExplanation)
    {
        explanation = inputExplanation;
        explanationText.text = explanation;
    }

    public void SetSkill(string inputSkill)
    {
        skill = inputSkill;
        skillText.text = skill;
    }

    public void SetPrice(string inputPrice)
    {
        price = inputPrice;
        priceText.text = price;
    }
}
