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

    public void OnEnable()
    {
        AdjustHeight();
    }

    public void GetContents()
    {
        JSONNode jsonNode = ItemManager.Instance.GetItemJSONNode();
        GameObject itemIcon = Resources.Load("UIPrefabs/TrainUI/ItemIcon");

        for(int i = 0; i<jsonNode["Weapon"].Count; i++)
        {

        }
    }

    // 그리드 레이아웃에서 필요한 정보 가져오는 메서드
    public void GetGridLayoutAttribute()
    {
        //rowHeight = gridLayoutGroup.cellSize.y;
        //colNum = gridLayoutGroup.
        colNum = Mathf.FloorToInt(gameObject.GetComponent<RectTransform>().rect.width / gridLayoutGroup.cellSize.x);
        rowNum = Mathf.CeilToInt(gameObject.transform.childCount / colNum); 
    }

    // 내용물 양에 따라 크기 조절
    public void AdjustHeight()
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
}
