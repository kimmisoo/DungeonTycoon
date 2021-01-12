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

    void Start()
    {
        GameObject loadedPrefab = (GameObject)Resources.Load("UIPrefabs/TrainUI/ListContentsPanel");
        scrollRect = GetComponent<ScrollRect>();

        #if DEBUG_TRAIN_UI
        MakeItemListByCategory("Weapon", loadedPrefab);
        //MakeItemListByCategory("Armor", loadedPrefab);
        //MakeItemListByCategory("Accessory", loadedPrefab);
        scrollRect.content = itemList["Weapon"].GetComponent<RectTransform>();
        #endif
    }

    void OnEnable()
    {
        
    }

    private void MakeItemListByCategory(string itemCategory, GameObject loadedPrefab)
    {
        string prefabName = "ListContentsPanel ";

        GameObject newObject = Instantiate<GameObject>(loadedPrefab);
        newObject.transform.SetParent(gameObject.transform);
        newObject.name = prefabName + itemCategory;
        
        itemList.Add(itemCategory, newObject);
        LoadContents(newObject, itemCategory);
        AdjustHeight(newObject);

        //newObject.transform.localPosition = new Vector3(0, -875, 0);
        newObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        
    }

    // 아이템 카테고리에 맞는 리스트 보이는 메서드
    public void ShowItemListByCategory(string itemCategory)
    {
        //GetContents(itemList[selectedCategory], selectedCategory);
        //AdjustHeight();
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList.Values.ToList()[i].SetActive(false);
        }
        itemList[itemCategory].SetActive(true);
        scrollRect.content = itemList[itemCategory].GetComponent<RectTransform>();
    }

    // 아이템별 아이템 프리팹 불러와서 패널에 집어넣는 메서드
    private void LoadContents(GameObject parent, string itemCategory)
    {
        JSONNode jsonNode = ItemManager.Instance.GetItemJSONNode();
        //GameObject itemIcon = (GameObject)Resources.Load("UIPrefabs/TrainUI/ItemIcon_8");

        for (int i = 0; i < jsonNode[itemCategory].Count; i++)
        {
            string iconPath = "UIPrefabs/TrainUI/ItemIcons/" + itemCategory + "/";
            iconPath += jsonNode[itemCategory][i]["Name"];
            //Debug.Log(iconPath);
            GameObject itemIcon = (GameObject)Resources.Load(iconPath);

            GameObject newIcon = Instantiate<GameObject>(itemIcon);

            GameObject iconImage = newIcon.transform.GetChild(1).gameObject;

            newIcon.transform.SetParent(itemList[itemCategory].transform);
            newIcon.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            newIcon.transform.localPosition = new Vector3(0, 0, 0);
            //Debug.Log(newIcon.name + ", " + itemList[itemCategory].transform.GetChild(i).name);
            //Debug.Log(i + ", " + newIcon.transform.GetSiblingIndex());
        }
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
}