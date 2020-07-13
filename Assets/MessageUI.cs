using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour
{
    public Button messageBox;
    public Text messageText;

    private bool isFadingOut = false;

    private const float BG_ALPHA_MAX = 0.75f;
    private const float TEXT_ALPHA_MAX = 1.0f;
    private const float ALPHA_MIN = 0.0f;
    private const float fadeTime = 0.3f;
    private const float selfDisapearTime = 3.5f;

    Coroutine fadeOutCoroutine;
    Coroutine timerCoroutine;

    /// <summary>
    /// 화면 정 중앙에 메시지 띄우기
    /// </summary>
    /// <param name="str">띄울 메시지</param>
    public void ShowMessage(string str)
    {
        isFadingOut = false;
        messageBox.interactable = true;
        messageText.text = str;
        StartCoroutine(FadeIn());
        if (fadeOutCoroutine != null)
            StopCoroutine(fadeOutCoroutine);
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(SelfDisapear());
    }
    /// <summary>
    /// 메시지 없애기
    /// </summary>
    public void HideMessage()
    {
        isFadingOut = true;
        messageBox.interactable = false;
        fadeOutCoroutine = StartCoroutine(FadeOut());
    }

    private IEnumerator SelfDisapear()
    {
        float elapsed = 0.0f;

        while(elapsed < selfDisapearTime)
        {
            yield return new WaitForSeconds(SkillConsts.TICK_TIME * 2);
            elapsed += SkillConsts.TICK_TIME * 2;
        }

        if (isFadingOut == false)
            HideMessage();
        timerCoroutine = null;
    }

    private IEnumerator FadeIn()
    {
        messageBox.gameObject.SetActive(true);
        Color msgBoxColor = messageBox.image.color;
        Color textColor = messageText.color;
        float time = 0.0f;
        while(msgBoxColor.a < BG_ALPHA_MAX - Mathf.Epsilon)
        {
            yield return null;
            time += Time.deltaTime/fadeTime;
            msgBoxColor.a = Mathf.Lerp(ALPHA_MIN, BG_ALPHA_MAX, time);
            textColor.a = Mathf.Lerp(ALPHA_MIN, TEXT_ALPHA_MAX, time);
            messageBox.image.color = msgBoxColor;
            messageText.color = textColor;
        }
        msgBoxColor.a = BG_ALPHA_MAX;
    }

    private IEnumerator FadeOut()
    {
        Color msgBoxColor = messageBox.image.color;
        Color textColor = messageText.color;
        float time = 0.0f;
        while (msgBoxColor.a > ALPHA_MIN + Mathf.Epsilon)
        {
            yield return null;
            //도중에 다른 메시지가 들어왔다면 그냥 나감.
            time += Time.deltaTime/fadeTime;
            msgBoxColor.a = Mathf.Lerp(BG_ALPHA_MAX, ALPHA_MIN, time);
            textColor.a = Mathf.Lerp(TEXT_ALPHA_MAX, ALPHA_MIN, time);
            messageBox.image.color = msgBoxColor;
            messageText.color = textColor;
        }

        messageBox.gameObject.SetActive(false);
        fadeOutCoroutine = null;
    }

    
}
