using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListContentsController : MonoBehaviour
{
    private int rowNum, colNum;
    //private float rowHeight, colWidth;
    public GridLayoutGroup gridLayoutGroup;

    public void OnEnable()
    {
        AdjustHeight();
    }

    public void GetGridAttribute()
    {
        //rowHeight = gridLayoutGroup.cellSize.y;
        //colNum = gridLayoutGroup.
        colNum = Mathf.FloorToInt(gameObject.GetComponent<RectTransform>().rect.width / gridLayoutGroup.cellSize.x);
        rowNum = Mathf.CeilToInt(gameObject.transform.childCount / colNum); 
    }

    public void AdjustHeight()
    {
        GetGridAttribute();

        float x, y, width, height;
        x = gameObject.GetComponent<RectTransform>().rect.x;
        y = gameObject.GetComponent<RectTransform>().rect.y;
        width = gameObject.GetComponent<RectTransform>().rect.width;
        height = rowNum * gridLayoutGroup.cellSize.y;
        gameObject.GetComponent<RectTransform>().rect.Set(x, y, width, height);
    }
}
