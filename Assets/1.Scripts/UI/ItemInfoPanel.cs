using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPanel : MonoBehaviour
{
    string name, modifier, explanation, skill, price;
    bool isSkill = false;
    Text nameText, modifierText, explanationText, skillText, priceText;
    Button purchaseBtn;
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
        if (price == "장착중")
            purchaseBtn.interactable = false;
        else if (price == "이미 구매함")
        {

        }
    }
}
