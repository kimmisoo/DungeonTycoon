using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotButton : MonoBehaviour
{
    public Image highlightEffect;

    public void HighlightOn()
    {
        highlightEffect.gameObject.SetActive(true);
    }

    public void HighlightOff()
    {
        highlightEffect.gameObject.SetActive(false);
    }
}
