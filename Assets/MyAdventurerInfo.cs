using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MyAdventurerInfo : MonoBehaviour
{
    #region PlayerSpAdv
    private SpecialAdventurer playerSpAdv;
    private Stat pStat;
    private BattleStat pBattleStat;
    #endregion

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

    #region ItemSlots
    private Dictionary<string, Dictionary<int, GameObject>> itemSlots;
    public GameObject slotsParent;
    #endregion

    private void Awake()
    {
        FillSpriteDict();
        //for (int i = 0; i < advSprites.Keys.Count; i++)
        //    Debug.Log(advSprites.Keys.ToList()[i]);
    }

    void OnEnable()
    {
        LoadPlayerData();
        RefreshStatInfo();
        RefreshBattleStatInfo();
        RefreshUniqueSkillInfo();
        InitItemSlots();
    }

    private void InitItemSlots()
    {
        itemSlots = new Dictionary<string, Dictionary<int, GameObject>>();

        itemSlots.Add("Weapon", new Dictionary<int, GameObject>());
        itemSlots.Add("Armor", new Dictionary<int, GameObject>());
        itemSlots.Add("Accessory1", new Dictionary<int, GameObject>());
        itemSlots.Add("Accessory2", new Dictionary<int, GameObject>());

        itemSlots["Weapon"].Add(-1, slotsParent.transform.GetChild(0).GetChild(0).gameObject);
        itemSlots["Armor"].Add(-1, slotsParent.transform.GetChild(1).GetChild(0).gameObject);
        itemSlots["Accessory1"].Add(-1, slotsParent.transform.GetChild(2).GetChild(0).gameObject);
        itemSlots["Accessory2"].Add(-1, slotsParent.transform.GetChild(3).GetChild(0).gameObject);

        //Debug.Log(itemSlots["Accessory1"][-1].name);
    }

    private void RefreshItemSlots()
    {
        int weaponIdx, armorIdx, acc1Idx, acc2Idx;
        weaponIdx = GameManager.Instance.GetPlayerSpAdvItem("Weapon");
        armorIdx = GameManager.Instance.GetPlayerSpAdvItem("Armor");
        acc1Idx = GameManager.Instance.GetPlayerSpAdvItem("Accessory1");
        acc2Idx = GameManager.Instance.GetPlayerSpAdvItem("Accessory2");


        for (int i = 0; i < itemSlots["Weapon"].Count; i++)
            itemSlots["Weapon"].Values.ToArray()[i].SetActive(false);

        if (itemSlots["Weapon"].ContainsKey(weaponIdx) == false)
            GenSlotIcon();

        itemSlots["Weapon"][weaponIdx].SetActive(true);


        for (int i = 0; i < itemSlots["Armor"].Count; i++)
            itemSlots["Armor"].Values.ToArray()[i].SetActive(false);

        if (itemSlots["Armor"].ContainsKey(armorIdx) == false)
            GenSlotIcon();

        itemSlots["Armor"][armorIdx].SetActive(true);


        for (int i = 0; i < itemSlots["Accessory1"].Count; i++)
            itemSlots["Accessory1"].Values.ToArray()[i].SetActive(false);

        if (itemSlots["Accessory1"].ContainsKey(acc1Idx) == false)
            GenSlotIcon();

        itemSlots["Accessory1"][acc1Idx].SetActive(true);


        for (int i = 0; i < itemSlots["Accessory2"].Count; i++)
            itemSlots["Accessory2"].Values.ToArray()[i].SetActive(false);

        if (itemSlots["Accessory2"].ContainsKey(acc2Idx) == false)
            GenSlotIcon();

        itemSlots["Accessory2"][acc2Idx].SetActive(true);
    }

    private void GenSlotIcon()
    {

    }

    private void FillSpriteDict()
    {
        for (int i = 0; i < advSpritesHolder.transform.childCount; i++)
        {
            GameObject temp = advSpritesHolder.transform.GetChild(i).gameObject;
            advSprites.Add(temp.name, temp);
        }
    }

    private void LoadPlayerData()
    {
        playerSpAdv = GameManager.Instance.GetPlayerSpAdv().GetComponent<SpecialAdventurer>();
        playerSpAdv.GetStat();
        pBattleStat = playerSpAdv.GetBattleStat();
    }


    private void RefreshStatInfo()
    {
        advName.text = playerSpAdv.stat.actorName;
        advLevel1.text = pBattleStat.Level.ToString();

        string imagePath = "UISprites/Portrait/" + playerSpAdv.nameKey; //키 값이 더 적절해서 키값 사용
        Sprite temp = Resources.Load<Sprite>(imagePath);
        if (temp != null)
            advPortrait.sprite = temp;
        //Debug.Log(playerSpAdv.nameKey);
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
        advPenetration.text = string.Format("{0:0} | {0:0}%", pBattleStat.PenetrationFixed, pBattleStat.PenetrationMult * 100);
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
