using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListContentsPanelController : MonoBehaviour
{
    private int rowNum, colNum;
    //private float rowHeight, colWidth;
    public GridLayoutGroup gridLayoutGroup;

    void Start()
    {
        GetContents();
        //RefreshGridLayout();
        AdjustHeight();
    }

    void OnEnable()
    {
        AdjustHeight();
    }

    private void GetContents()
    {
        JSONNode jsonNode = ItemManager.Instance.GetItemJSONNode();
        //GameObject itemIcon = (GameObject)Resources.Load("UIPrefabs/TrainUI/ItemIcon_8");

        for(int i = 0; i<jsonNode["Weapon"].Count; i++)
        {
            string iconPath = "UIPrefabs/TrainUI/ItemIcons/Weapons/";
            iconPath += jsonNode["Weapon"][i]["Name"];
            Debug.Log(iconPath);
            GameObject itemIcon = (GameObject)Resources.Load(iconPath);

            GameObject newIcon = Instantiate<GameObject>(itemIcon);

            GameObject iconImage = newIcon.transform.GetChild(1).gameObject;

            newIcon.transform.SetParent(gameObject.transform);
            newIcon.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            newIcon.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    // 그리드 레이아웃에서 필요한 정보 가져오는 메서드
    private void GetGridLayoutAttribute()
    {
        //rowHeight = gridLayoutGroup.cellSize.y;
        //colNum = gridLayoutGroup.
        colNum = Mathf.FloorToInt(gameObject.GetComponent<RectTransform>().rect.width / gridLayoutGroup.cellSize.x);
        rowNum = Mathf.CeilToInt((float)gameObject.transform.childCount / colNum); 
    }

    // 내용물 양에 따라 크기 조절
    private void AdjustHeight()
    {
        GetGridLayoutAttribute();

        float x, y, width, height;
        x = gameObject.GetComponent<RectTransform>().rect.x;
        y = gameObject.GetComponent<RectTransform>().rect.y;
        width = gameObject.GetComponent<RectTransform>().rect.width;
        height = rowNum * gridLayoutGroup.cellSize.y;
        //gameObject.GetComponent<RectTransform>().rect.Set(x, y, width, height);
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        //Debug.Log("AdjustingHeight " + height + "\n" + "new: " + gameObject.GetComponent<RectTransform>().rect.height);
    }

    //private void RefreshGridLayout()
    //{
    //    LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
    //    //gameObject.GetComponent<GridLayoutGroup>()
    //}
}
