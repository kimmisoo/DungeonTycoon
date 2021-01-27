using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPanel : MonoBehaviour
{
    private string itemName, stat, explanation, skillName, skillEffect, demandedLevel;
    private int price;
    private bool isSkill = false;
    public Text nameText, statText, explanationText, skillNameText, skillEffectText, priceText, demandedLevelText;
    public Image demandedLevelBG, contentCover;
    public PurchaseButton purchaseBtn;
    public SkillExplSwitchButton skillExplBtn;
    private BattleStat pBattleStat;
    private bool isPurchase = false;
    private ItemCondition selectedItemCondition;

    private void OnEnable()
    {
        //GameManager.Instance.ChooseSpAdv(0);
        pBattleStat = GameManager.Instance.GetPlayerSpAdv().GetComponent<SpecialAdventurer>().GetBattleStat(); // 배틀스탯 받기
        HideContent();

        //SetPrice(100 + " G");
        //SetSelectedItemCondition(ItemCondition.None);
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

        //switch (price)
        //{
        //    case "장착 중":
        //        purchaseBtn.SetEquipped();
        //        break;
        //    case "이미 구매함":
        //        purchaseBtn.SetPurchased();
        //        break;
        //    default:
        //        purchaseBtn.SetNeedPurchase();
        //        break;
        //}
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

    public void SetPrice(int inputPrice)
    {
        price = inputPrice;
        priceText.text = price + " G";
    }

    public void SetDemandedLevel(string inputDemandedLevel)
    {
        demandedLevel = inputDemandedLevel;
        demandedLevelText.text = demandedLevel;

        //DemandedLevelCheck();
    }

    // 요구 레벨 체크해서 장착가능하면 true 반환
    private bool DemandedLevelCheck()
    {
        //Debug.Log(pBattleStat);
        //Debug.Log(demandedLevel);
        if (pBattleStat.Level >= int.Parse(demandedLevel))
        {
            demandedLevelBG.color = new Color(0.4868455f, 0.7921569f, 0.1019608f);
            return true;
        }
        else
        {
            demandedLevelBG.color = new Color(1.0f, 0.6039216f, 0.1764706f);
            return false;
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

    public void SetSelectedItemCondition(ItemCondition condition)
    {
        selectedItemCondition = condition;
    }

    public void CheckPurchaseConditions()
    {
        switch (selectedItemCondition)
        {
            case ItemCondition.Equipped:
                priceText.text = "장착 중";
                isPurchase = false;
                purchaseBtn.SetEquipped();
                break;
            case ItemCondition.Purchased:
                priceText.text = "이미 구매함";
                isPurchase = false;
                purchaseBtn.SetPurchased();
                break;
            case ItemCondition.None:
                isPurchase = true;
                purchaseBtn.SetNeedPurchase();
                break;
            default:
                break;
        }

        if (DemandedLevelCheck() == false)
            purchaseBtn.SetNotInteractable();
        if (GameManager.Instance.GetPlayerGold() < price)
            purchaseBtn.SetNotInteractable();
    }


    public bool GetIsPurchase()
    {
        return isPurchase;
    }
}
