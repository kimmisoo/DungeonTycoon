using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyAdventurerInfo : MonoBehaviour
{
    #region StatInfo
    public Text advName;
    public Text advLevel1;
    #endregion

    #region BattleStatInfo
    public Text advLevel2;
    public Text advHealth;
    public Text advAttack;
    public Text advDefence;
    public Text advAttackSpeed;
    public Text advMoveSpeed;
    public Text advCriticalChance;
    public Text advCriticalDamage;
    public Text advAttackRange;
    public Text advDps;
    public Text advPenetration;
    #endregion

    #region UniqueSkillInfo
    public Text skillName;
    public Text skillExplanation;
    public Image skillImage;
    #endregion

    void OnEnable()
    {
        RefreshStatInfo();
        RefreshBattleStatInfo();
        RefreshUniqueSkillInfo();
    }



    private void RefreshStatInfo()
    {
        SpecialAdventurer playerSpAdv = GameManager.Instance.GetPlayerSpAdv().GetComponent<SpecialAdventurer>();
        Stat pStat = playerSpAdv.GetStat();
        BattleStat pBattleStat = playerSpAdv.GetBattleStat();

        advName.text = playerSpAdv.stat.actorName;
        advLevel1.text = pBattleStat.Level.ToString();
    }

    private void RefreshBattleStatInfo()
    {
        SpecialAdventurer playerSpAdv = GameManager.Instance.GetPlayerSpAdv().GetComponent<SpecialAdventurer>();
        BattleStat pBattleStat = playerSpAdv.GetBattleStat();
        advLevel2.text = pBattleStat.Level.ToString();
        advHealth.text = string.Format("{0:0} / {1:0}", pBattleStat.Health, pBattleStat.HealthMax);
        advAttack.text = string.Format("{0:0}", pBattleStat.Attack);
        advDefence.text = string.Format("{0:0}", pBattleStat.Defence);
        advAttackSpeed.text = string.Format("{0:0.0000}", pBattleStat.AttackSpeed);
        advMoveSpeed.text = string.Format("{0:0}%", pBattleStat.MoveSpeed * 100);
        advCriticalChance.text = string.Format("{0:0.00}%", pBattleStat.CriticalChance * 100);
        advCriticalDamage.text = string.Format("{0:0}%", pBattleStat.CriticalDamage *100);
        advAttackRange.text = string.Format("{0:0}칸", pBattleStat.Range);
        advDps.text = string.Format("{0:0.00}", pBattleStat.Attack * pBattleStat.AttackSpeed * ((1+pBattleStat.CriticalChance) * (pBattleStat.CriticalDamage -1)));
        advPenetration.text = string.Format("{0:0} | {0:0}%", pBattleStat.PenetrationFixed, pBattleStat.PenetrationMult);
    }

    private void RefreshUniqueSkillInfo()
    {
        SpecialAdventurer playerSpAdv = GameManager.Instance.GetPlayerSpAdv().GetComponent<SpecialAdventurer>();
        Skill uniqueSkill = playerSpAdv.GetSkills()[playerSpAdv.nameKey];
        skillName.text = uniqueSkill.Name;
        skillExplanation.text = uniqueSkill.Explanation;
        //skillImage;
}
}
