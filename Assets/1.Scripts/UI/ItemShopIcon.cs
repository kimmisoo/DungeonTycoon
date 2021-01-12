using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopIcon : MonoBehaviour
{
    public Button buttonComp;
    public int index;

    private void Awake()
    {
        buttonComp = gameObject.GetComponent<Button>();
        buttonComp.onClick.AddListener(() => func2(index));
        buttonComp.onClick.AddListener(func1);
    }

    public void func1()
    {
        Debug.Log("func1!");
    }
    public void func2(int a)
    {
        Debug.Log("func2! " + a);
    }
}
