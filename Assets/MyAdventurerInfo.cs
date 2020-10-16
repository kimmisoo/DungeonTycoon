using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MyAdventurerInfo : MonoBehaviour
{
    #region StatInfo
    public Text advName;
    public Text advLevel1;
    public Image advPortrait;

    public GameObject advSpritesHolder;
    Dictionary<string, GameObject> advSprites = new Dictionary<string, GameObject>();
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

    private void Awake()
    {
        FillSpriteDict();
        for (int i = 0; i < advSprites.Keys.Count; i++)
            Debug.Log(advSprites.Keys.ToList()[i]);
    }

    void OnEnable()
    {
        RefreshStatInfo();
        RefreshBattleStatInfo();
        RefreshUniqueSkillInfo();
    }

    private void FillSpriteDict()
    {
        for (int i = 0; i < advSpritesHolder.transform.childCount; i++)
        {
            GameObject temp = advSpritesHolder.transform.GetChild(i).gameObject;
            advSprites.Add(temp.name, temp);
        }
    }


    private void RefreshStatInfo()
    {
        SpecialAdventurer playerSpAdv = GameManager.Instance.GetPlayerSpAdv().GetComponent<SpecialAdventurer>();
        Stat pStat = playerSpAdv.GetStat();
        BattleStat pBattleStat = playerSpAdv.GetBattleStat();

        advName.text = playerSpAdv.stat.actorName;
        advLevel1.text = pBattleStat.Level.ToString();

        string imagePath = "UISprites/Portrait/" + playerSpAdv.nameKey; //키 값이 더 적절해서 키값 사용
        Sprite temp = Resources.Load<Sprite>(imagePath);
        if (temp != null)
            advPortrait.sprite = temp;
        Debug.Log(playerSpAdv.nameKey);
        advSprites[playerSpAdv.nameKey].SetActive(true);
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
        advCriticalDamage.text = string.Format("{0:0}%", pBattleStat.CriticalDamage * 100);
        advAttackRange.text = string.Format("{0:0}칸", pBattleStat.Range);
        advDps.text = string.Format("{0:0.00}", pBattleStat.Attack * pBattleStat.AttackSpeed * ((1 + pBattleStat.CriticalChance) * (pBattleStat.CriticalDamage - 1)));
        advPenetration.text = string.Format("{0:0} | {0:0}%", pBattleStat.PenetrationFixed, pBattleStat.PenetrationMult);
    }

    private void RefreshUniqueSkillInfo()
    {
        SpecialAdventurer playerSpAdv = GameManager.Instance.GetPlayerSpAdv().GetComponent<SpecialAdventurer>();
        Skill uniqueSkill = playerSpAdv.GetSkills()[playerSpAdv.nameKey];
        skillName.text = uniqueSkill.SkillName;
        skillExplanation.text = uniqueSkill.Explanation;

        string imagePath = "Icons/" + uniqueSkill.key; //키 값이 더 적절해서 키값 사용
        Sprite temp = Resources.Load<Sprite>(imagePath);
        if(temp != null)
            skillImage.sprite = temp;
    }
}
