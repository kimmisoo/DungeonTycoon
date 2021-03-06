using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OtherAdventurerInfo : MonoBehaviour
{
    #region PlayerSpAdv
    private SpecialAdventurer selectedSpAdv;
    private Stat pStat;
    private BattleStat advBattleStat;
    private int selectedSpAdvIdx;
    #endregion

    #region ExpInfo
    public Slider expBar;
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
    private Dictionary<string, GameObject> itemSlots;
    private Dictionary<string, Dictionary<int, GameObject>> slotIcons;
    public GameObject slotsParent;
    #endregion

    private void Awake()
    {
        FillSpriteDict();
        //for (int i = 0; i < advSprites.Keys.Count; i++)
        //    Debug.Log(advSprites.Keys.ToList()[i]);
        InitItemSlots();
    }

    private void Start()
    {

    }

    void OnEnable()
    {
        LoadPlayerData();
        RefreshStatInfo();
        RefreshBattleStatInfo();
        RefreshUniqueSkillInfo();
        RefreshItemSlots();
    }

    private void Update()
    {
        RefreshBattleStatInfo();
        RefreshExpBar();
    }

    private void InitItemSlots()
    {
        itemSlots = new Dictionary<string, GameObject>();
        itemSlots.Add("Weapon", slotsParent.transform.GetChild(0).gameObject);
        itemSlots.Add("Armor", slotsParent.transform.GetChild(1).gameObject);
        itemSlots.Add("Accessory1", slotsParent.transform.GetChild(2).gameObject);
        itemSlots.Add("Accessory2", slotsParent.transform.GetChild(3).gameObject);

        slotIcons = new Dictionary<string, Dictionary<int, GameObject>>();

        slotIcons.Add("Weapon", new Dictionary<int, GameObject>());
        slotIcons.Add("Armor", new Dictionary<int, GameObject>());
        slotIcons.Add("Accessory1", new Dictionary<int, GameObject>());
        slotIcons.Add("Accessory2", new Dictionary<int, GameObject>());

        slotIcons["Weapon"].Add(-1, slotsParent.transform.GetChild(0).GetChild(0).gameObject);
        slotIcons["Armor"].Add(-1, slotsParent.transform.GetChild(1).GetChild(0).gameObject);
        slotIcons["Accessory1"].Add(-1, slotsParent.transform.GetChild(2).GetChild(0).gameObject);
        slotIcons["Accessory2"].Add(-1, slotsParent.transform.GetChild(3).GetChild(0).gameObject);

        //Debug.Log(itemSlots["Accessory1"][-1].name);
    }

    public void SelectSpAdv(int index)
    {
        selectedSpAdvIdx = index;
    }

    private void RefreshItemSlots()
    {
        int weaponIdx, armorIdx, acc1Idx, acc2Idx;
        weaponIdx = GameManager.Instance.GetSpAdvItemIndex(selectedSpAdvIdx, "Weapon");
        armorIdx = GameManager.Instance.GetSpAdvItemIndex(selectedSpAdvIdx, "Armor");
        acc1Idx = GameManager.Instance.GetSpAdvItemIndex(selectedSpAdvIdx, "Accessory1");
        acc2Idx = GameManager.Instance.GetSpAdvItemIndex(selectedSpAdvIdx, "Accessory2");


        for (int i = 0; i < slotIcons["Weapon"].Count; i++)
            slotIcons["Weapon"].Values.ToArray()[i].SetActive(false);

        if (slotIcons["Weapon"].ContainsKey(weaponIdx) == false)
            GenSlotIcon("Weapon", weaponIdx);

        slotIcons["Weapon"][weaponIdx].SetActive(true);


        for (int i = 0; i < slotIcons["Armor"].Count; i++)
            slotIcons["Armor"].Values.ToArray()[i].SetActive(false);

        if (slotIcons["Armor"].ContainsKey(armorIdx) == false)
            GenSlotIcon("Armor", armorIdx);

        slotIcons["Armor"][armorIdx].SetActive(true);


        for (int i = 0; i < slotIcons["Accessory1"].Count; i++)
            slotIcons["Accessory1"].Values.ToArray()[i].SetActive(false);

        if (slotIcons["Accessory1"].ContainsKey(acc1Idx) == false)
            GenSlotIcon("Accessory1", acc1Idx);

        slotIcons["Accessory1"][acc1Idx].SetActive(true);


        for (int i = 0; i < slotIcons["Accessory2"].Count; i++)
            slotIcons["Accessory2"].Values.ToArray()[i].SetActive(false);

        if (slotIcons["Accessory2"].ContainsKey(acc2Idx) == false)
            GenSlotIcon("Accessory2", acc2Idx);

        slotIcons["Accessory2"][acc2Idx].SetActive(true);
    }

    private void GenSlotIcon(string slot, int index)
    {
        ItemListPanel listPanel = UIManager.Instance.itemEquipUI.listPanel;
        string category;

        if (slot == "Accessory1" || slot == "Accessory2")
            category = "Accessory";
        else
            category = slot;

        GameObject newIcon = Instantiate<GameObject>(listPanel.GetComponent<ItemListPanel>().GetItemIconByCategoryAndIndex(category, index));
        slotIcons[slot].Add(index, newIcon);

        //Debug.Log(newIcon.name);

        newIcon.transform.SetParent(itemSlots[slot].transform);

        newIcon.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        newIcon.transform.localPosition = new Vector3(0, 0, 0);

        newIcon.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        //Debug.Log(newIcon.GetComponent<RectTransform>().anchorMin.x + ", " + newIcon.GetComponent<RectTransform>().anchorMin.y);
        newIcon.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        //Debug.Log(newIcon.GetComponent<RectTransform>().anchorMax.x + ", " + newIcon.GetComponent<RectTransform>().anchorMax.y);

        //newIcon.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        //newIcon.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
        //newIcon.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
        //newIcon.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);

        newIcon.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        newIcon.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        newIcon.transform.SetAsFirstSibling();
        //newIcon.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
    }

    private void FillSpriteDict()
    {
        for (int i = 0; i < advSpritesHolder.transform.childCount; i++)
        {
            GameObject temp = advSpritesHolder.transform.GetChild(i).gameObject;
            advSprites.Add(temp.name, temp);
        }
        Debug.Log("FSD");
    }

    private void LoadPlayerData()
    {
        selectedSpAdv = GameManager.Instance.GetSpAdv(selectedSpAdvIdx).GetComponent<SpecialAdventurer>();
        selectedSpAdv.GetStat();
        advBattleStat = selectedSpAdv.GetBattleStat();
    }


    private void RefreshStatInfo()
    {
        advName.text = selectedSpAdv.stat.actorName;
        advLevel1.text = advBattleStat.Level.ToString();

        string imagePath = "UISprites/Portrait/" + selectedSpAdv.nameKey; //키 값이 더 적절해서 키값 사용
        Sprite temp = Resources.Load<Sprite>(imagePath);
        if (temp != null)
            advPortrait.sprite = temp;
        //Debug.Log(playerSpAdv.nameKey);
        for(int i = 0; i < advSprites.Count; i++)
            advSprites.Values.ToList()[i].SetActive(false);
        advSprites[selectedSpAdv.nameKey].SetActive(true);
    }

    private void RefreshBattleStatInfo()
    {
        selectedSpAdv = GameManager.Instance.GetSpAdv(selectedSpAdvIdx).GetComponent<SpecialAdventurer>();
        BattleStat advBattleStat = selectedSpAdv.GetBattleStat();
        advLevel2.text = advBattleStat.Level.ToString();
        advHealth.text = string.Format("{0:0} / {1:0}", advBattleStat.Health, advBattleStat.HealthMax);
        advAttack.text = string.Format("{0:0}", advBattleStat.Attack);
        advDefence.text = string.Format("{0:0}", advBattleStat.Defence);
        advAttackSpeed.text = string.Format("{0:0.0000}", advBattleStat.AttackSpeed);
        advMoveSpeed.text = string.Format("{0:0}%", advBattleStat.MoveSpeed * 100);
        advCriticalChance.text = string.Format("{0:0.00}%", advBattleStat.CriticalChance * 100);
        advCriticalDamage.text = string.Format("{0:0}%", advBattleStat.CriticalDamage * 100);
        advAttackRange.text = string.Format("{0:0}칸", advBattleStat.Range);
        advDps.text = string.Format("{0:0.00}", advBattleStat.Attack * advBattleStat.AttackSpeed * ((1 + advBattleStat.CriticalChance) * (advBattleStat.CriticalDamage - 1)));
        advPenetration.text = string.Format("{0:0} | {0:0}%", advBattleStat.PenetrationFixed, advBattleStat.PenetrationMult * 100);
    }

    private void RefreshUniqueSkillInfo()
    {
        SpecialAdventurer playerSpAdv = GameManager.Instance.GetSpAdv(selectedSpAdvIdx).GetComponent<SpecialAdventurer>();
        Skill uniqueSkill = playerSpAdv.GetSkills()[playerSpAdv.nameKey];
        skillName.text = uniqueSkill.SkillName;
        skillExplanation.text = uniqueSkill.Explanation;

        string imagePath = "Icons/" + uniqueSkill.key; //키 값이 더 적절해서 키값 사용
        Sprite temp = Resources.Load<Sprite>(imagePath);
        if (temp != null)
            skillImage.sprite = temp;
    }

    private void RefreshExpBar()
    {
        selectedSpAdv = GameManager.Instance.GetSpAdv(selectedSpAdvIdx).GetComponent<SpecialAdventurer>();
        BattleStat advBattleStat = selectedSpAdv.GetBattleStat();
        expBar.value = (float)advBattleStat.CurExp / advBattleStat.NextExp;
    }
}
