using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchSkillBetweenExplanation : MonoBehaviour
{
    public int curSide = 0;
    public Text[] buttonText = new Text[2];
    public GameObject[] contents = new GameObject[2];

    public void SwapContent()
    {
        buttonText[curSide % 2].gameObject.SetActive(false);
        contents[curSide % 2].gameObject.SetActive(false);
        curSide++;
        buttonText[curSide % 2].gameObject.SetActive(true);
        contents[curSide % 2].gameObject.SetActive(true);
    }
}
