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

    public void SelectItem(GameObject clickedIcon)
    {
        //selectedItemName = inputItem;
        selectedItemIndex = clickedIcon.transform.GetSiblingIndex();
        Debug.Log(selectedItemCategory + " [" + selectedItemCategory + "] Selected");
    }

    private void RefreshItemInfo()
    {
        //infoPanel.SetName(selectedItemName);
        //infoPanel.SetExplanation(itemJSON[selectedItemCategory][selectedItemName]["explanation"]);
    }
}