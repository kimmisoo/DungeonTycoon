using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossRaidUI : MonoBehaviour
{
    public Button bossChallengeBtn;
    public Sprite enabledBossChallengeBtnSprite;
    public Sprite disabledBossChallengeBtnSprite;

    public GameObject bossRaidPanel;

    public GameObject raidState;
    public Text participatingText;
    public Text bossRaidStateText;

    public Button challengeRefusalBtn;
    public GameObject raidPrepTimer;

    public enum BossRaidUIState
    { BeforeApplication, WaitingRetryTimer, WaitingPlayerResponse, PlayerParticipating, PlayerNotParticipating, NotBossPhase }

    private BossRaidUIState _state;
    public BossRaidUIState State
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;
            switch (_state)
            {
                case BossRaidUIState.BeforeApplication:
                    BeforeApplication();
                    break;
                case BossRaidUIState.WaitingRetryTimer:
                    WaitingRetryTimer();
                    break;
                case BossRaidUIState.WaitingPlayerResponse:
                    WaitingPlayerResponse();
                    break;
                case BossRaidUIState.PlayerParticipating:
                    PlayerParticipating();
                    break;
                case BossRaidUIState.PlayerNotParticipating:
                    PlayerNotParticipating();
                    break;
                case BossRaidUIState.NotBossPhase:
                    NotBossPhase();
                    break;

            }
        }
    }

    private void Awake()
    {
        NotBossPhase();
    }

    // 도전 버튼
    private void ShowBossChallengeBtn()
    {
        bossChallengeBtn.gameObject.SetActive(true);
    }
    private void DisableChallengeBtn()
    {
        bossChallengeBtn.gameObject.GetComponent<Image>().sprite = disabledBossChallengeBtnSprite;
        bossChallengeBtn.transform.GetChild(1).GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        bossChallengeBtn.interactable = false;
    }
    // 활성화 비활성화.
    private void EnableBossChallengeBtn()
    {
        bossChallengeBtn.gameObject.GetComponent<Image>().sprite = enabledBossChallengeBtnSprite;
        bossChallengeBtn.transform.GetChild(1).GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        bossChallengeBtn.interactable = true;
    }
    private void HideBossChallengeBtn()
    {
        bossChallengeBtn.gameObject.SetActive(false);
    }

    // 레이드 패널
    private void ShowRaidPanel()
    {
        bossRaidPanel.SetActive(true);
    }
    private void HideBossRaidPanel()
    {
        bossRaidPanel.SetActive(false);
    }

    // 레이드 현황
    private void ShowRaidState()
    {
        raidState.SetActive(true);
    }
    private void HideRaidState()
    {
        raidState.SetActive(false);
    }
    private void ShowParticipatingText()
    {
        participatingText.gameObject.SetActive(true);
    }
    private void HideParticipatingText()
    {
        participatingText.gameObject.SetActive(false);
    }
    public void SetRaidStateText(string str)
    {
        bossRaidStateText.text = str;
    }

    // 타이머 전체
    private void ShowRaidPrepTimer()
    {
        raidPrepTimer.SetActive(true);
    }
    private void HideRaidPrepTimer()
    {
        raidPrepTimer.SetActive(false);
    }

    private void ShowChallengeRefusalBtn()
    {
        challengeRefusalBtn.gameObject.SetActive(true);
    }
    private void HideChallengeRefusalBtn()
    {
        challengeRefusalBtn.gameObject.SetActive(false);
    }
    private void EnableChallengeRefusalBtn()
    {
        challengeRefusalBtn.interactable = true;
    }
    private void DisableChallengeRefusalBtn()
    {
        challengeRefusalBtn.interactable = false;
    }

    // 캡슐화 메서드
    private void BeforeApplication()
    {
        ShowBossChallengeBtn();
        EnableBossChallengeBtn();
        HideBossRaidPanel();
    }
    private void WaitingRetryTimer()
    {
        ShowBossChallengeBtn();
        DisableChallengeBtn();
        HideBossRaidPanel();
    }
    private void WaitingPlayerResponse()
    {
        ShowBossChallengeBtn();
        EnableBossChallengeBtn();

        ShowRaidPanel();
        HideRaidState();
        ShowRaidPrepTimer();
    }
    private void PlayerParticipating()
    {
        ShowBossChallengeBtn();
        DisableChallengeBtn();

        ShowRaidPanel();
        //DisableChallengeRefusalBtn();
        ShowParticipatingText();
        ShowRaidState();
        HideRaidPrepTimer();
    }
    public void PlayerNotParticipating()
    {
        ShowBossChallengeBtn();
        DisableChallengeBtn();

        ShowRaidPanel();
        //DisableChallengeRefusalBtn();
        HideParticipatingText();
        ShowRaidState();
        HideRaidPrepTimer();
    }

    public void NotBossPhase()
    {
        HideBossChallengeBtn();
        HideBossRaidPanel();
    }

    #region SaveLoad
    public string GetRaidStateText()
    {
        return bossRaidStateText.text;
    }
    #endregion
}
