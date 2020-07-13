using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossRaidTimerDisplay : MonoBehaviour
{
    Text timeText;

    private void Start()
    {
        timeText = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update ()
    {
        float remainingTime = SceneConsts.BOSSRAID_PREP_TIME - GameManager.Instance.bossRaidPrepWaitedTime;
        timeText.text = remainingTime.ToString("F2");
	}
}
