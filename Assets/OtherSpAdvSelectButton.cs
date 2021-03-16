using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherSpAdvSelectButton : MonoBehaviour
{
    public Image highlight;
    public string nameKey;

    public void HighlightOn()
    {
        highlight.gameObject.SetActive(true);
    }

    public void HighlightOff()
    {
        highlight.gameObject.SetActive(false);
    }
}
