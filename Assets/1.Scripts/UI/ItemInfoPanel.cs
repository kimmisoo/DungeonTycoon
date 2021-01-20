using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPanel : MonoBehaviour
{
    string itemName, stat, explanation, skillName, skillEffect, price, demandedLevel;
    bool isSkill = false;
    public Text nameText, statText, explanationText, skillNameText, skillEffectText, priceText, demandedLevelText;
    public Image demandedLevelBG, contentCover;
    PurchaseButton purchaseBtn;
    public SkillExplSwitchButton skillExplBtn;
    private BattleStat pBattleStat;

    private void OnEnable()
    {
        pBattleStat = GameManager.Instance.GetPlayerSpAdv().GetComponent<SpecialAdventurer>().GetBattleStat(); // 배틀스탯 받기
        HideContent();
    }

    public void HideContent()
    {
        contentCover.gameObject.SetActive(true);
    }

    public void RevealContent()
    {
        contentCover.gameObject.SetActive(false);
    }

    public void ShowInfo()
    {
        //RevealContent();
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

        DemandedLevelCheck();
    }

    private void DemandedLevelCheck()
    {
        if (pBattleStat.Level >= int.Parse(demandedLevel))
        {
            demandedLevelBG.color = new Color(0.4868455f, 0.7921569f, 0.1019608f);
            Debug.Log("can");
        }
        else
        {
            demandedLevelBG.color = new Color(1.0f, 0.6039216f, 0.1764706f);
            Debug.Log("can't");
        }
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
