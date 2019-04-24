using UnityEngine;
using System.Collections;

public class UIObject : MonoBehaviour {

    private Vector3 _originLocalPosition = Vector3.zero;
    private Vector3 _originLocalScale = Vector3.zero;
    
    private RectTransform _rectTransform;
    private RectTransform _parent;


    public virtual void Awake()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        parent = rectTransform.parent.GetComponent<RectTransform>();
        originLocalPosition = rectTransform.localPosition;
        originLocalScale = rectTransform.sizeDelta;
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
	public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    
    public RectTransform parent
    {
        get
        {
            return _parent;
        }
        set
        {
            _parent = value;
        }
    }
    public RectTransform rectTransform
    {
        get
        {
            return _rectTransform;
        }
        set
        {
            _rectTransform = value;
        }
    }

    public Vector3 originLocalPosition
    {
        get
        {
            return _originLocalPosition;
        }
        set
        {
            _originLocalPosition = value;
        }
    }
    public Vector3 originLocalScale
    {
        get
        {
            return _originLocalScale;
        }
        set
        {
            _originLocalScale = value;
        }
    }
}
