using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherSpAdvSelectionUI : MonoBehaviour
{
    public GameObject spAdvList;
    private Dictionary<string, OtherSpAdvSelectButton> buttons;

    private void Awake()
    {
        buttons = new Dictionary<string, OtherSpAdvSelectButton>();

        for (int i = 0; i < spAdvList.transform.childCount; i++)
        {
            OtherSpAdvSelectButton curBtn = spAdvList.transform.GetChild(i).GetComponent<OtherSpAdvSelectButton>();
            buttons.Add(curBtn.nameKey, curBtn);
        }
        buttons[GameManager.Instance.GetPlayerSpAdv().GetComponent<SpecialAdventurer>().nameKey].HighlightOn();
    }

    // Use this for initialization
    void Start ()
    {
		
	}
}
