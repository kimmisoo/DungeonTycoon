using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageProgressTest : MonoBehaviour
{
    public Text progressDisplay;
	
	// Update is called once per frame
	void Update ()
    {
        if (GameManager.Instance.isBossPhase)
            progressDisplay.text = "BossArea #" + CombatAreaManager.Instance.BossAreaIndex;
        else
            progressDisplay.text = "HuntingArea #" + CombatAreaManager.Instance.ConqueringHuntingAreaIndex;

    }

    public void AreaConquered()
    {
        CombatArea curArea;

        if(GameManager.Instance.isBossPhase)
            curArea = CombatAreaManager.Instance.FindBossArea();
        else
            curArea = CombatAreaManager.Instance.huntingAreas[CombatAreaManager.Instance.ConqueringHuntingAreaIndex];

        curArea.InvokeAreaConqueredEvent();
    }
}
