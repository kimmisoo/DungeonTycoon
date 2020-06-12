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
        if (GameManager.Instance.IsBossPhase)
            progressDisplay.text = "BossArea #" + CombatAreaManager.Instance.BossAreaIndex;
        else
            progressDisplay.text = "PublicHuntingArea #" + CombatAreaManager.Instance.PublicHuntingAreaIndex;

    }

    public void AreaConquered()
    {
        CombatArea curArea;

        if(GameManager.Instance.IsBossPhase)
            curArea = CombatAreaManager.Instance.FindBossArea();
        else
            curArea = CombatAreaManager.Instance.GetHuntingAreas()[CombatAreaManager.Instance.ConqueringHuntingAreaIndex];

        curArea.InvokeAreaConqueredEvent();
    }
}
