using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEquipUI : MonoBehaviour
{
    private string selectedSlot = null;
    private string selectedItem = null;
    public ItemListPanel listPanel;
    JSONNode itemJSON = null;

    private void Start()
    {
        GetItemData();
    }

    public void SelectSlot(string inputSlot)
    {
        if (selectedSlot != inputSlot)
        {
            selectedSlot = inputSlot;

            if (selectedSlot == "Accessory1" || selectedSlot == "Accessory2")
            {
                listPanel.ShowItemListByCategory("Accessory");
            }
            else
                listPanel.ShowItemListByCategory(selectedSlot);
        }
    }

    public void GetItemData()
    {
        itemJSON = ItemManager.Instance.GetItemJSONNode();
    }

    public void SelectItem(string inputItem)
    {
        selectedItem = inputItem;
    }

    private void RefreshItemInfo()
    {

    }
}