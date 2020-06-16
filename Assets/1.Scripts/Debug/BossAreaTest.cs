using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossAreaTest : MonoBehaviour
{
    public Button playerOrderBossRaidBtn;
    public bool isBossPhase = false;

    private void Update()
    {
        playerOrderBossRaidBtn.gameObject.SetActive(GameManager.Instance.isBossPhase);
    }

    public void BossPhaseStarted()
    {
        // 게임매니저에서 보스 페이즈 시작하는 메서드
        GameManager.Instance.OnBossAreaConquerStarted();

        playerOrderBossRaidBtn.gameObject.SetActive(GameManager.Instance.isBossPhase);
    }

    public void PlayerOrderdBossRaid()
    {
        GameManager.Instance.PlayerCalledBossRaid();
    }
}
