using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseButton : MonoBehaviour
{
    private Text buttonText;
    private Image buttonImage;
    private Button buttonComp;
    

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
        //SetPurchased();
    }

    public void SetPurchased()
    {
        buttonText.text = "장착";
        buttonComp.interactable = true;
        buttonImage.sprite = Resources.Load<Sprite>("UISprites/Button/UI_Button_Standard_White");
    }

    public void SetNeedPurchase()
    {
        buttonText.text = "구매";
        buttonComp.interactable = true;
        buttonImage.sprite = Resources.Load<Sprite>("UISprites/Button/OrangeButton");
    }

    public void SetEquipped()
    {
        buttonText.text = "장착 중";
        buttonComp.interactable = false;
        buttonImage.sprite = Resources.Load<Sprite>("UISprites/Button/UI_Button_Standard_White");
    }

    public void SetDisarm(bool isSame)
    {
        buttonText.text = "장착 해제";
        buttonComp.interactable = !isSame;
        buttonImage.sprite = Resources.Load<Sprite>("UISprites/Button/UI_Button_Standard_White");
    }

    public void SetNotInteractable()
    {
        buttonComp.interactable = false;
    }
}
