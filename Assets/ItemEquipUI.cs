#define DEBUG_ITEM_INFO_UI

using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCondition
{
    None, Purchased, Equipped
}

public class ItemEquipUI : MonoBehaviour
{
    JSONNode itemJSON = null;

    private string selectedSlot = null;
    private int selectedIndex = -1;
    private string selectedCategory = null;

    public ItemListPanel listPanel;
    public ItemInfoPanel infoPanel;
    public GameObject itemSlotsParent;
    
    private Dictionary<string, List<ItemCondition>> itemStorage; // 아이템 보유 및 장착 현황
    private Dictionary<string, int> curEquipped; //` 장착중인 아이템 인덱스. -1은 빈칸
    private Dictionary<string, GameObject> itemSlots;
    private Dictionary<string, Dictionary<int, GameObject>> slotIcons; // 첫 번째 키 카테고리, 두 번째 키 인덱스
    
    private void Awake()
    {
        curEquipped = new Dictionary<string, int>();

        curEquipped.Add("Weapon", -1);
        curEquipped.Add("Armor", -1);
        curEquipped.Add("Accessory1", -1);
        curEquipped.Add("Accessory2", -1);

        itemSlots = new Dictionary<string, GameObject>();
        itemSlots.Add("Weapon", itemSlotsParent.transform.GetChild(0).gameObject);
        itemSlots.Add("Armor", itemSlotsParent.transform.GetChild(1).gameObject);
        itemSlots.Add("Accessory1", itemSlotsParent.transform.GetChild(2).gameObject);
        itemSlots.Add("Accessory2", itemSlotsParent.transform.GetChild(3).gameObject);
        //Debug.Log(itemSlots["Accessory1"].name);

        slotIcons = new Dictionary<string, Dictionary<int, GameObject>>();
        slotIcons.Add("Weapon", new Dictionary<int, GameObject>());
        slotIcons["Weapon"].Add(-1, itemSlotsParent.transform.GetChild(0).GetChild(0).gameObject);
        slotIcons.Add("Armor", new Dictionary<int, GameObject>());
        slotIcons["Armor"].Add(-1, itemSlotsParent.transform.GetChild(1).GetChild(0).gameObject);
        slotIcons.Add("Accessory", new Dictionary<int, GameObject>());
        slotIcons["Accessory"].Add(-1, itemSlotsParent.transform.GetChild(2).GetChild(0).gameObject);
        //slotIcons.Add("Weapon", new Dictionary<int, GameObject>());
    }

    private void Start()
    {
        UIManager.Instance.itemEquipUI = this;
        GetItemData();
        CreateItemStorage();
    }

    private void CreateItemStorage() // 아이템 보유 및 장착 현황 초기화
    {
        JSONNode jsonNode = ItemManager.Instance.GetItemJSONNode();
        //GameObject itemIcon = (GameObject)Resources.Load("UIPrefabs/TrainUI/ItemIcon_8");
        itemStorage = new Dictionary<string, List<ItemCondition>>();

        List<ItemCondition> newList = new List<ItemCondition>(jsonNode["Weapon"].Count);
        for (int i = 0; i < jsonNode["Weapon"].Count; i++)
            newList.Add(ItemCondition.None);
        itemStorage.Add("Weapon", newList);

        newList = new List<ItemCondition>(jsonNode["Armor"].Count);
        for (int i = 0; i < jsonNode["Armor"].Count; i++)
            newList.Add(ItemCondition.None);
        itemStorage.Add("Armor", newList);

        newList = new List<ItemCondition>(jsonNode["Accessory"].Count);
        for (int i = 0; i < jsonNode["Accessory"].Count; i++)
            newList.Add(ItemCondition.None);
        itemStorage.Add("Accessory", newList);
    }

    private void OnEnable()
    {
        
    }

    public int GetCurEquipped(string slot)
    {
        return curEquipped[slot];
    }

    public void SetCurEquipped(string slot, int inputIndex)
    {
        curEquipped[slot] = inputIndex;
    }

    public void SelectSlot(string inputSlot) // 모험가 아이템슬롯 선택 메서드
    {
        if (selectedSlot != inputSlot)
        {
            selectedSlot = inputSlot;

            if (selectedSlot == "Accessory1" || selectedSlot == "Accessory2")
                selectedCategory = "Accessory";
            else
                selectedCategory = selectedSlot;
        }

        for(int i = 0; i<itemSlots.Count; i++)
            itemSlots[itemSlots.Keys.ToArray()[i]].GetComponent<ItemSlotButton>().HighlightOff();

        itemSlots[selectedSlot].GetComponent<ItemSlotButton>().HighlightOn();
    }

    public void GetItemData() // JSONNode Get
    {
        itemJSON = ItemManager.Instance.GetItemJSONNode();
    }

    public void SelectItem(int inputIndex) // 아이템 선택 메서드(카테고리는 SelectSlot에서 먼저 선택해야 함)
    {
        selectedIndex = inputIndex;

        if (itemJSON[selectedCategory][selectedIndex]["PenetrationMult"].AsFloat == 0)
            Debug.Log(itemJSON[selectedCategory][selectedIndex]["PenetrationMult"].AsFloat);

        RefreshItemInfo();
        infoPanel.RevealContent();
    }

    private void RefreshItemInfo()
    {
        infoPanel.SetName(itemJSON[selectedCategory][selectedIndex]["Name"]);
        //infoPanel.SetExplanation(itemJSON[selectedItemCategory][selectedItemIndex]["Explanation"]);

        if (itemJSON[selectedCategory][selectedIndex]["ItemSkill"] == null)
        {
            infoPanel.SetOnlyExpl(itemJSON[selectedCategory][selectedIndex]["Explanation"]);
            Debug.Log("Skill X");
        }
        else
        {
            string skillName, skillEffect;
            SkillFactory.GetNameAndExplanation(itemJSON[selectedCategory][selectedIndex]["ItemSkill"], out skillName, out skillEffect);
            infoPanel.SetSkillAndExpl(skillName, skillEffect, itemJSON[selectedCategory][selectedIndex]["Explanation"]);
            Debug.Log("Skill O");
        }

        infoPanel.SetStat(MakeStatString());

        if(Mathf.Abs(itemJSON[selectedCategory][selectedIndex]["DemendedLevel"].AsFloat) <= 0.0f + Mathf.Epsilon)
        {
            infoPanel.SetDemandedLevel("1");
        }
        else
        {
            infoPanel.SetDemandedLevel(itemJSON[selectedCategory][selectedIndex]["DemendedLevel"]);
        }

        //CheckPurchaseConditions();
        infoPanel.SetPrice(itemJSON[selectedCategory][selectedIndex]["Price"].AsInt);
        infoPanel.SetSelectedItemCondition(itemStorage[selectedCategory][selectedIndex]);
        infoPanel.CheckPurchaseConditions();
    }

    private string MakeStatString()
    {
        string resultStr = "";
        int statCnt = 0;

        switch(selectedCategory)
        {
            case "Weapon":
                if (Mathf.Abs(itemJSON[selectedCategory][selectedIndex]["Attack"].AsFloat) > 0.0f + Mathf.Epsilon)
                {
                    resultStr += "공격력+" + itemJSON[selectedCategory][selectedIndex]["Attack"];
                    statCnt++;
                }
                if(Mathf.Abs(itemJSON[selectedCategory][selectedIndex]["CriticalChance"].AsFloat) > 0.0f + Mathf.Epsilon)
                {
                    if (statCnt != 0)
                        resultStr += ", ";
                    resultStr += "치명타 확률+" + itemJSON[selectedCategory][selectedIndex]["CriticalChance"].AsFloat * 100 + "%";
                    statCnt++;
                }
                if(Mathf.Abs(itemJSON[selectedCategory][selectedIndex]["AttackSpeed"].AsFloat) > 0.0f + Mathf.Epsilon)
                {
                    if (statCnt != 0)
                        resultStr += ", ";
                    resultStr += "공격속도+" + itemJSON[selectedCategory][selectedIndex]["AttackSpeed"].AsFloat * 100 + "%";
                    statCnt++;
                }
                if (Mathf.Abs(itemJSON[selectedCategory][selectedIndex]["PenetrationMult"].AsFloat) > 0.0f + Mathf.Epsilon)
                {
                    if (statCnt != 0)
                        resultStr += ", ";
                    resultStr += "방어구 관통력+" + itemJSON[selectedCategory][selectedIndex]["PenetrationMult"].AsFloat * 100 + "%";
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

    //private void CheckPurchaseConditions()
    //{
    //    // 장착중인지 어떻게 Check? 이거는 이름 비교하면 됨.
    //    // 구매했는지 어떻게 Check? 컬렉션을 가지고 있어야겠다.
        
    //}

    public void EquipItem()
    {
        if(infoPanel.GetIsPurchase())
            GameManager.Instance.AddGold(-itemJSON[selectedCategory][selectedIndex]["Price"].AsInt);

        if(curEquipped[selectedSlot] != -1)
            itemStorage[selectedCategory][curEquipped[selectedSlot]] = ItemCondition.Purchased;

        itemStorage[selectedCategory][selectedIndex] = ItemCondition.Equipped;
        

    }

    public void SlotIconChange()
    {
        for(int i = 0; i<slotIcons[selectedCategory].Count; i++)
            slotIcons[selectedCategory].Values.ToArray()[i].SetActive(false);

        if (slotIcons[selectedCategory].ContainsKey(selectedIndex) == false)
            GenSlotIcon();

        slotIcons[selectedCategory][selectedIndex].SetActive(true);
    }

    private void GenSlotIcon()
    {
        GameObject newIcon = Instantiate<GameObject>(listPanel.GetComponent<ItemListPanel>().GetItemIconByIndex(selectedIndex));
        slotIcons[selectedCategory].Add(selectedIndex, newIcon);
    }
}