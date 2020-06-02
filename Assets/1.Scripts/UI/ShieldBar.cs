using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldBar : MonoBehaviour
{
    public ICombatant subject;
    public Slider slider;
    public Image[] images;
    public Canvas canvas;
    public float fadeInTime = 0.15f;
    public float fadeOutTime = 0.4f;

    public void SetSubject(ICombatant subjectIn)
    {
        subject = subjectIn;
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        //images = slider.GetComponentsInChildren<Image>(true);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, subject.GetPosition());
        if (canvas == null)
            Debug.Log("NULL");
        transform.position = WorldToUISpace(canvas, subject.GetPosition() + new Vector3(0, 0.35f, 0));
        slider.maxValue = subject.GetBattleStat().HealthMax;
        slider.value = subject.GetBattleStat().Shield;
    }

    public Vector3 WorldToUISpace(Canvas parentCanvas, Vector3 worldPos)
    {
        //Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 movePos;

        //Convert the screenpoint to ui rectangle local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out movePos);
        //Convert the local point to world point
        return parentCanvas.transform.TransformPoint(movePos);
    }

    public void Show()
    {
        StartCoroutine(FadeIn());
    }

    public void Hide()
    {
        StartCoroutine(FadeOut());
    }

    public IEnumerator FadeIn()
    {
        float curAlpha = images[0].color.a;
        while (curAlpha < 1.0f - Mathf.Epsilon)
        {
            curAlpha += Time.deltaTime / fadeInTime;
            foreach (Image item in images)
                item.color = new Color(item.color.r, item.color.g, item.color.b, curAlpha);
            yield return null;
        }
    }

    public IEnumerator FadeOut()
    {
        float curAlpha = images[0].color.a;

        while (curAlpha > 0.0f + Mathf.Epsilon)
        {
            curAlpha -= Time.deltaTime / fadeOutTime;
            foreach (Image item in images)
                item.color = new Color(item.color.r, item.color.g, item.color.b, curAlpha);
            yield return null;
        }
    }
}
