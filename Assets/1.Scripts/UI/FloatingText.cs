using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloatingText : MonoBehaviour
{
    private float moveSpeed;
    private float alphaSpeed;
    private float destroyTime;
    public TextMeshPro text;
    private Color alpha;
    public string msg;
    //public Color atkColor, healColor;

    // Start is called before the first frame update
    void OnEnable()
    {
        moveSpeed = 0.1f;
        alphaSpeed = 2.0f;
        destroyTime = 2.0f;
        //text = GetComponent<TextMeshPro>();

        alpha = text.color;
        text.text = msg;
        Invoke("DestroyObject", destroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0)); // 텍스트 위치

        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed); // 텍스트 알파값
        text.color = alpha;
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    public void InitFloatingText(string msg, Vector3 worldPos)
    {
        transform.position = worldPos; //WorldToUISpace(GameObject.Find("Canvas").GetComponent<Canvas>(), worldPos);
        this.msg = msg;
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
}
