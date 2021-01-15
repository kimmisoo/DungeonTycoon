using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEquipUI : MonoBehaviour
{
    private string selectedItemSlot = null;
    //private string selectedItemName = null;
    private int selectedItemIndex = -1;
    private string selectedItemCategory = null;
    public ItemListPanel listPanel;
    public ItemInfoPanel infoPanel;
    JSONNode itemJSON = null;

    private void Start()
    {
        UIManager.Instance.itemEquipUI = this;
        GetItemData();
    }

    public void SelectSlot(string inputSlot)
    {
        if (selectedItemSlot != inputSlot)
        {
            selectedItemSlot = inputSlot;

            if (selectedItemSlot == "Accessory1" || selectedItemSlot == "Accessory2")
                selectedItemCategory = "Accessory";
            else
                selectedItemCategory = selectedItemSlot;
        }
    }

    public void GetItemData()
    {
        itemJSON = ItemManager.Instance.GetItemJSONNode();
    }

    public void SelectItem(int inputIndex)
    {
        //selectedItemName = inputItem;
        selectedItemIndex = inputIndex;
        //if (itemJSON[selectedItemCategory][selectedItemIndex]["PenetrationMult"] == null)
        //    Debug.Log("NULL!");
        //string testStr = itemJSON[selectedItemCategory][selectedItemIndex]["PenetrationMult"];
        
        if (itemJSON[selectedItemCategory][selectedItemIndex]["PenetrationMult"].AsFloat == 0)
            Debug.Log(itemJSON[selectedItemCategory][selectedItemIndex]["PenetrationMult"].AsFloat);
        //if(itemJSON[selectedItemCategory][selectedItemIndex]["PenetrationMult"] == "null")
        //Debug.Log("O");
        RefreshItemInfo();
    }

    private void RefreshItemInfo()
    {
        infoPanel.SetName(itemJSON[selectedItemCategory][selectedItemIndex]["Name"]);
        //infoPanel.SetExplanation(itemJSON[selectedItemCategory][selectedItemIndex]["Explanation"]);

        if (itemJSON[selectedItemCategory][selectedItemIndex]["ItemSkill"] == null)
        {
            infoPanel.SetOnlyExpl(itemJSON[selectedItemCategory][selectedItemIndex]["Explanation"]);
            Debug.Log("Skill X");
        }
        else
        {
            string skillName, skillEffect;
            SkillFactory.GetNameAndExplanation(itemJSON[selectedItemCategory][selectedItemIndex]["ItemSkill"], out skillName, out skillEffect);
            infoPanel.SetSkillAndExpl(skillName, skillEffect, itemJSON[selectedItemCategory][selectedItemIndex]["Explanation"]);
            Debug.Log("Skill O");
        }

        infoPanel.SetStat(MakeStatString());
    }

    private string MakeStatString()
    {
        string resultStr = "";
        int statCnt = 0;

        switch(selectedItemCategory)
        {
            case "Weapon":
                if (Mathf.Abs(itemJSON[selectedItemCategory][selectedItemIndex]["Attack"].AsFloat) > 0.0f + Mathf.Epsilon)
                {
                    resultStr += "공격력+" + itemJSON[selectedItemCategory][selectedItemIndex]["Attack"];
                    statCnt++;
                }
                if(Mathf.Abs(itemJSON[selectedItemCategory][selectedItemIndex]["CriticalChance"].AsFloat) > 0.0f + Mathf.Epsilon)
                {
                    if (statCnt != 0)
                        resultStr += ", ";
                    resultStr += "치명타 확률+" + itemJSON[selectedItemCategory][selectedItemIndex]["CriticalChance"].AsFloat * 100 + "%";
                    statCnt++;
                }
                if(Mathf.Abs(itemJSON[selectedItemCategory][selectedItemIndex]["AttackSpeed"].AsFloat) > 0.0f + Mathf.Epsilon)
                {
                    if (statCnt != 0)
                        resultStr += ", ";
                    resultStr += "공격속도+" + itemJSON[selectedItemCategory][selectedItemIndex]["AttackSpeed"].AsFloat * 100 + "%";
                    statCnt++;
                }
                if (Mathf.Abs(itemJSON[selectedItemCategory][selectedItemIndex]["PenetrationMult"].AsFloat) > 0.0f + Mathf.Epsilon)
                {
                    if (statCnt != 0)
                        resultStr += ", ";
                    resultStr += "방어구 관통력+" + itemJSON[selectedItemCategory][selectedItemIndex]["PenetrationMult"].AsFloat * 100 + "%";
                    statCnt++;
                }
                break;
            case "Armor":
                break;
            case "Accessory":
                break;
            default:
                resultStr = null;
                break;
        }

        return resultStr;
    }
}