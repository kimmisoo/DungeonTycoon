#define DEBUG_ITEM

using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemCategory
{
    Weapon, Armor, Accessory
}

public class ItemManager : MonoBehaviour
{
    private static ItemManager _instance;

    private ICombatant subject;
    private string tempItemCategory;
    private int tempItemIndex;

    // 아이템 정보 읽어오기용
    public JSONNode itemsJson;


    public static ItemManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("ItemManager is null");
                return null;
            }
            else
                return _instance;
        }
    }

    // Use this for initialization
    void Awake()
    {
        _instance = this;
        LoadItemData();
        //structures = new List<Structure>();

    }

    public void LoadItemData()
    {
        TextAsset itemsText = Resources.Load<TextAsset>("Items/Items");
        itemsJson = JSON.Parse(itemsText.text);
    }

    ////setStructureCategory -> setStructureNumber -> instantiateStructure // onClick 이벤트 정적 설정이 파라미터가 한개인 함수만 설정 가능하기 때문에 .. 번거롭더라도~~
    //public void setItemCategory(string itemCategoryIn)
    //{
    //    tempItemCategory = itemCategoryIn;
    //}
    //public void setItemIndex(int itemIndexIn)
    //{
    //    tempItemIndex = itemIndexIn;
    //}
    //public void SetSubject(ICombatant combatantIn)
    //{
    //    subject = combatantIn;
    //}

    public Item CreateItem(string tempItemCategory, int tempItemIndex)
    {
        Item tempItem = new Item();

        switch (tempItemCategory)
        {
            case "Weapon":
                tempItem.ItemType = ItemType.Weapon;

                if (itemsJson[tempItemCategory][tempItemIndex]["Attack"] != null)
                    tempItem.AddStatModContinous(new StatModContinuous(StatType.Attack, ModType.Fixed, itemsJson[tempItemCategory][tempItemIndex]["Attack"].AsFloat));

                if (itemsJson[tempItemCategory][tempItemIndex]["CriticalChance"] != null)
                    tempItem.AddStatModContinous(new StatModContinuous(StatType.CriticalChance, ModType.Fixed, itemsJson[tempItemCategory][tempItemIndex]["CriticalChance"].AsFloat));

                if (itemsJson[tempItemCategory][tempItemIndex]["AttackSpeed"] != null)
                    tempItem.AddStatModContinous(new StatModContinuous(StatType.AttackSpeed, ModType.Mult, itemsJson[tempItemCategory][tempItemIndex]["AttackSpeed"].AsFloat));

                if (itemsJson[tempItemCategory][tempItemIndex]["PenetrationMult"] != null)
                    tempItem.AddStatModContinous(new StatModContinuous(StatType.PenetrationMult, ModType.Fixed, itemsJson[tempItemCategory][tempItemIndex]["PenetrationMult"].AsFloat));
                break;
            case "Armor":
                tempItem.ItemType = ItemType.Armor;

                //Debug.Log("Item Provides Def : " + (itemsJson[tempItemCategory][tempItemIndex]["Defence"].AsFloat));
                //Debug.Log("Item Provides HP : " + (itemsJson[tempItemCategory][tempItemIndex]["Health"].AsFloat));
                if (itemsJson[tempItemCategory][tempItemIndex]["Health"] != null)
                    tempItem.AddStatModContinous(new StatModContinuous(StatType.HealthMax, ModType.Fixed, itemsJson[tempItemCategory][tempItemIndex]["Health"].AsFloat));

                if (itemsJson[tempItemCategory][tempItemIndex]["Defence"] != null)
                    tempItem.AddStatModContinous(new StatModContinuous(StatType.Defence, ModType.Fixed, itemsJson[tempItemCategory][tempItemIndex]["Defence"].AsFloat));
                break;
            case "Accessory":
                tempItem.ItemType = ItemType.Accessory;
                break;
            default:
                break;
        }
        tempItem.Name = itemsJson[tempItemCategory][tempItemIndex]["Name"];
        tempItem.Icon = itemsJson[tempItemCategory][tempItemIndex]["Icon"];
        tempItem.Price = itemsJson[tempItemCategory][tempItemIndex]["Price"].AsInt;
        tempItem.DemandedLevel = itemsJson[tempItemCategory][tempItemIndex]["DemendedLevel"].AsInt;
        tempItem.itemSkillKey = itemsJson[tempItemCategory][tempItemIndex]["ItemSkill"];
        tempItem.Explanation = itemsJson[tempItemCategory][tempItemIndex]["Explanation"];
        tempItem.OptimalLevelLower = itemsJson[tempItemCategory][tempItemIndex]["OptimalLevelLower"].AsInt;
        tempItem.OptimalLevelUpper = itemsJson[tempItemCategory][tempItemIndex]["OptimalLevelUpper"].AsInt;

        // 세이브용
        tempItem.itemCategory = tempItemCategory;
        tempItem.itemNum = tempItemIndex;

        return tempItem;
    }

    public int GetItemCount(ItemCategory category)
    {
        switch (category)
        {
            case ItemCategory.Weapon:
                return itemsJson["Weapon"].Count;
            case ItemCategory.Armor:
                return itemsJson["Armor"].Count;
            case ItemCategory.Accessory:
                return itemsJson["Accessory"].Count;
            default:
                return -1;
        }
    }

    public JSONNode GetItemJSONNode()
    {
        return itemsJson;
    }
}
