using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDownFix : MonoBehaviour
{

    public string sortingLayer = "UI"; //Or what ever layer you want it to be

    public void OnDropDownClicked()
    {
        Transform droplist = transform.Find("Dropdown List");

        if (droplist != null)
        {
            droplist.GetComponent<Canvas>().sortingLayerName = sortingLayer;
        }
    }
}