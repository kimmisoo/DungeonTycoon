using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpAdvSelectionUI : MonoBehaviour
{
    public int curSelected = -1;
    public Button determineBtn;
    public Button moreInfoBtn;
    public TrainPanel trainPanel;

    public void DetermineSpAdv()
    {
        //Debug.Log("Determined");
        if (curSelected != -1)
        {
            GameManager.Instance.ChooseSpAdv(curSelected);
            //trainPanel.SpAdvSelected();

            trainPanel.OpenSpAdvPanel();
        }
    }

    public void SetCurSelected(string nameKeyIn)
    {
        List<GameObject> spAdvs = GameManager.Instance.specialAdventurers;
        for (int i = 0; i<spAdvs.Count; i++)
        {
            //Debug.Log(nameKeyIn + ", "+i);
            if (spAdvs[i].GetComponent<SpecialAdventurer>().nameKey == nameKeyIn)
                curSelected = i;
        }

        if (curSelected != -1)
        {
            determineBtn.interactable = true;
            moreInfoBtn.interactable = true;
        }
    }
}
