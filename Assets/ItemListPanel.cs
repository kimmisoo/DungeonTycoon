#define DEBUG_TRAIN_UI

using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemListPanel : MonoBehaviour
{
    //private int rowNum, colNum;
    //private float rowHeight, colWidth;
    public Dictionary<string, GameObject> itemList = new Dictionary<string, GameObject>();
    public ScrollRect scrollRect;
    string selectedCategory = null;

    private void Awake()
    {
        GameObject loadedPrefab = (GameObject)Resources.Load("UIPrefabs/TrainUI/ListContentsPanel");
        scrollRect = GetComponent<ScrollRect>();

        MakeItemListByCategory("Weapon", loadedPrefab);
        MakeItemListByCategory("Armor", loadedPrefab);
    }

    void Start()
    {
        //GameObject loadedPrefab = (GameObject)Resources.Load("UIPrefabs/TrainUI/ListContentsPanel");
        //scrollRect = GetComponent<ScrollRect>();

        //#if DEBUG_TRAIN_UI
        //MakeItemListByCategory("Weapon", loadedPrefab);
        //MakeItemListByCategory("Armor", loadedPrefab);
        //MakeItemListByCategory("Accessory", loadedPrefab);
        //scrollRect.content = itemList["Weapon"].GetComponent<RectTransform>();
        //scrollRect.content = itemList["Armor"].GetComponent<RectTransform>();
        //#endif
    }

    void OnEnable()
    {
        
    }

    // 카테고리 별로 리스트 만드는 메서드
    private void MakeItemListByCategory(string itemCategory, GameObject loadedPrefab)
    {
        string prefabName = "ListContentsPanel ";

        // 부모가 될 게임오브젝트 Instantiate
        GameObject newObject = Instantiate<GameObject>(loadedPrefab);
        newObject.transform.SetParent(gameObject.transform);
        newObject.name = prefabName + itemCategory;

        itemList.Add(itemCategory, newObject);
        LoadContents(newObject, itemCategory);
        AdjustHeight(newObject);

        //newObject.transform.localPosition = new Vector3(0, -875, 0);
        newObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        
    }

    public void SelectCategory(string category)
    {
        selectedCategory = category;
        ShowItemListByCategory();
    }

    // 아이템 카테고리에 맞는 리스트 보이는 메서드
    public void ShowItemListByCategory()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList.Values.ToList()[i].SetActive(false);
        }
        if (selectedCategory == null)
            return;
        itemList[selectedCategory].SetActive(true);
        scrollRect.content = itemList[selectedCategory].GetComponent<RectTransform>();
    }

    // 아이템별 아이템 프리팹 불러와서 패널에 집어넣는 메서드
    private void LoadContents(GameObject parent, string itemCategory)
    {
        JSONNode jsonNode = ItemManager.Instance.GetItemJSONNode();
        //GameObject itemIcon = (GameObject)Resources.Load("UIPrefabs/TrainUI/ItemIcon_8");

        // 일단 빈 칸 생성 index 0은 빈 칸인 거 기억!
        GameObject emptyPrefab = Resources.Load<GameObject>("UIPrefabs/TrainUI/ItemIcons/Empty/List_Empty");
        GameObject emptyIcon = Instantiate(emptyPrefab);

        emptyIcon.GetComponent<ItemShopIcon>().SetIndex(-1);

        emptyIcon.transform.SetParent(itemList[itemCategory].transform);
        emptyIcon.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        emptyIcon.transform.localPosition = new Vector3(0, 0, 0);

        for (int i = 0; i < jsonNode[itemCategory].Count; i++)
        {
            string iconPath = "UIPrefabs/TrainUI/ItemIcons/" + itemCategory + "/";
            iconPath += jsonNode[itemCategory][i]["Name"];
            //Debug.Log(iconPath);
            GameObject itemIcon = (GameObject)Resources.Load(iconPath);
            Debug.Log(iconPath);
            GameObject newIcon = Instantiate<GameObject>(itemIcon);
            
            //GameObject iconImage = newIcon.transform.GetChild(1).gameObject;

            newIcon.GetComponent<ItemShopIcon>().SetIndex(i);

            newIcon.transform.SetParent(itemList[itemCategory].transform);
            newIcon.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            newIcon.transform.localPosition = new Vector3(0, 0, 0);
            //Debug.Log(newIcon.name + ", " + itemList[itemCategory].transform.GetChild(i).name);
            //Debug.Log(i + ", " + newIcon.transform.GetSiblingIndex());
        }

       // Debug.Log(itemList["Weapon"].transform.GetChild(0));
    }

    // 내용물 양에 따라 크기 조절
    private void AdjustHeight(GameObject listObject)
    {
        GridLayoutGroup gridLayoutGroup = listObject.GetComponent<GridLayoutGroup>();
        //GetGridLayoutAttribute();
        int colNum = Mathf.FloorToInt(listObject.GetComponent<RectTransform>().rect.width / gridLayoutGroup.cellSize.x);
        int rowNum = Mathf.CeilToInt((float)listObject.transform.childCount / colNum);

        float x, y, width, height;
        width = listObject.GetComponent<RectTransform>().rect.width;
        height = rowNum * gridLayoutGroup.cellSize.y;

        x = listObject.GetComponent<RectTransform>().rect.x;
        y = -height/2;

        listObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        listObject.GetComponent<RectTransform>().localPosition = new Vector3(0, y, 0);

    }
    //private void RefreshGridLayout()
    //{
    //    LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
    //    //gameObject.GetComponent<GridLayoutGroup>()
    //}

    public GameObject GetItemIconByIndex(int index)
    {
        // 빈 칸 때문에 +1한 인덱스에 있는 오브젝트 리턴
        return itemList[selectedCategory].transform.GetChild(index + 1).gameObject;        
    }

    public GameObject GetItemIconByCategoryAndIndex(string category, int index)
    {
        return itemList[category].transform.GetChild(index + 1).gameObject;
    }
}