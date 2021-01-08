using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseButton : MonoBehaviour
{
    bool isOwned = false;
    Text buttonText;
    Image buttonImage;
    Button buttonComp;

    private void Awake()
    {
        buttonText = transform.GetComponentInChildren<Text>();
        buttonComp = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
    }

    private void Start()
    {
        //SetEquiped();
        //SetOwned();
    }

    public void SetOwned()
    {
        buttonText.text = "장착";
        isOwned = true;
        buttonComp.interactable = true;
        buttonImage.sprite = Resources.Load<Sprite>("UISprites/Button/SkyButtonPressed");
    }

    public void SetNeedPurchase()
    {
        buttonText.text = "구매";
        isOwned = true;
        buttonComp.interactable = true;
        buttonImage.sprite = Resources.Load<Sprite>("UISprites/Button/OrangeButton");
    }

    public void SetEquiped()
    {
        buttonText.text = "장착 중";
        isOwned = true;
        buttonComp.interactable = false;
        buttonImage.sprite = Resources.Load<Sprite>("UISprites/Button/OrangeButton");
    }
}
