using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopIcon : MonoBehaviour
{
    private Button buttonComp;
    private int index;

    public void SetIndex(int inputIndex)
    {
        index = inputIndex;
    }

    private void Awake()
    {
        buttonComp = gameObject.GetComponent<Button>();
        buttonComp.onClick.AddListener(ItemIconClicked);
    }

    public void ItemIconClicked()
    {
        UIManager.Instance.itemEquipUI.SelectItem(index);
    }
}
