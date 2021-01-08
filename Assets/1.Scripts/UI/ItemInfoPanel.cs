using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPanel : MonoBehaviour
{
    string name, modifier, explanation, skill, price;
    bool isSkill = false;
    Text nameText, modifierText, explanationText, skillText, priceText;
    PurchaseButton purchaseBtn;
    public SwitchSkillBetweenExplanation skillExplBtn;

    public void ShowInfo()
    {
        isSkill = false;
        nameText.text = name;
        modifierText.text = modifier;
        explanationText.text = explanation;
        skillText.text = skill;
        priceText.text = price;

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
}
