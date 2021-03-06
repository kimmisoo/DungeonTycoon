using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherSpAdvSelectionUI : MonoBehaviour
{
    public GameObject spAdvList;
    private Dictionary<string, OtherSpAdvSelectButton> buttons;
    public GameObject otherSpAdvInfoPanel;

    private void Awake()
    {
        buttons = new Dictionary<string, OtherSpAdvSelectButton>();

        for (int i = 0; i < spAdvList.transform.childCount; i++)
        {
            OtherSpAdvSelectButton curBtn = spAdvList.transform.GetChild(i).GetComponent<OtherSpAdvSelectButton>();
            buttons.Add(curBtn.nameKey, curBtn);
            Debug.Log(curBtn.nameKey);
        }
        buttons[GameManager.Instance.GetPlayerSpAdv().GetComponent<SpecialAdventurer>().nameKey].HighlightOn();
        buttons[GameManager.Instance.GetPlayerSpAdv().GetComponent<SpecialAdventurer>().nameKey].gameObject.GetComponent<Button>().interactable = false;
    }

    public void SelectSpAdv(string nameKey)
    {
        int selectedIndex = GameManager.Instance.GetSpAdv(nameKey).GetComponent<SpecialAdventurer>().index;
        otherSpAdvInfoPanel.GetComponent<OtherAdventurerInfo>().SelectSpAdv(selectedIndex);
    }
}
