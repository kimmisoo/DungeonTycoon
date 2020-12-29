using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEquipUI : MonoBehaviour
{
    private string selectedSlot = null;
    public ItemListPanel listPanel;

    public void SelectItemSlot(string inputSlot)
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
}