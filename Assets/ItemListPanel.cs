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

    void Start()
    {
        GameObject loadedPrefab = (GameObject)Resources.Load("UIPrefabs/TrainUI/ListContentsPanel");

        MakeItemListByCategory("Weapon", loadedPrefab);
        MakeItemListByCategory("Armor", loadedPrefab);
        MakeItemListByCategory("Accessory", loadedPrefab);
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
        AdjustHeight(newObject);
        itemList.Add(itemCategory, newObject);
        LoadContents(newObject, itemCategory);
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
            Debug.Log(iconPath);
            GameObject itemIcon = (GameObject)Resources.Load(iconPath);

            GameObject newIcon = Instantiate<GameObject>(itemIcon);

            GameObject iconImage = newIcon.transform.GetChild(1).gameObject;

            newIcon.transform.SetParent(itemList[itemCategory].transform);
            newIcon.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            newIcon.transform.localPosition = new Vector3(0, 0, 0);
        }
        //JSONNode jsonNode = ItemManager.Instance.GetItemJSONNode();
        ////GameObject itemIcon = (GameObject)Resources.Load("UIPrefabs/TrainUI/ItemIcon_8");

        //for (int i = 0; i<jsonNode[itemCategory].Count; i++)
        //{
        //    string iconPath = "UIPrefabs/TrainUI/ItemIcons/" + itemCategory + "/";
        //    iconPath += jsonNode[itemCategory][i]["Name"];
        //    Debug.Log(iconPath);
        //    GameObject itemIcon = (GameObject)Resources.Load(iconPath);

        //    GameObject newIcon = Instantiate<GameObject>(itemIcon);

        //    GameObject iconImage = newIcon.transform.GetChild(1).gameObject;

        //    newIcon.transform.SetParent(gameObject.transform);
        //    newIcon.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        //    newIcon.transform.localPosition = new Vector3(0, 0, 0);
        //}
    }

    // 그리드 레이아웃에서 필요한 정보 가져오는 메서드
    //private void GetGridLayoutAttribute()
    //{
    //    //rowHeight = gridLayoutGroup.cellSize.y;
    //    //colNum = gridLayoutGroup.
    //    colNum = Mathf.FloorToInt(gameObject.GetComponent<RectTransform>().rect.width / gridLayoutGroup.cellSize.x);
    //    rowNum = Mathf.CeilToInt((float)gameObject.transform.childCount / colNum);
    //}

    // 내용물 양에 따라 크기 조절
    private void AdjustHeight(GameObject listObject)
    {
        GridLayoutGroup gridLayoutGroup = listObject.GetComponent<GridLayoutGroup>();
        //GetGridLayoutAttribute();
        int colNum = Mathf.FloorToInt(listObject.GetComponent<RectTransform>().rect.width / gridLayoutGroup.cellSize.x);
        int rowNum = Mathf.CeilToInt((float)listObject.transform.childCount / colNum);

        float x, y, width, height;
        x = listObject.GetComponent<RectTransform>().rect.x;
        y = listObject.GetComponent<RectTransform>().rect.y;
        width = listObject.GetComponent<RectTransform>().rect.width;
        height = rowNum * gridLayoutGroup.cellSize.y;
        //gameObject.GetComponent<RectTransform>().rect.Set(x, y, width, height);
        listObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        //Debug.Log("AdjustingHeight " + height + "\n" + "new: " + gameObject.GetComponent<RectTransform>().rect.height);
    }

    //private void RefreshGridLayout()
    //{
    //    LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
    //    //gameObject.GetComponent<GridLayoutGroup>()
    //}
}
