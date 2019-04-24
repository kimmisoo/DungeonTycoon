using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Markers : MonoBehaviour, ISelectHandler, IPointerClickHandler {
	public string type = "";
	public int key = -1;
	public int valueIndex = -1;
	public Vector3 position;
	public Dialogs dialog;
	//vector 에 따라 마커 생성
	//리스트
	public GameObject smallMarker;

	public void OnSelect(BaseEventData d)
	{
		Debug.Log("markerSelected");

		if (smallMarker != null)
		{
			Debug.Log("markerSelected2");
			StartCoroutine(HighlightMarker());
			dialog.de.selectedMarkers = this;
			
		}
	}
	
	public void OnPointerClick(PointerEventData d)
	{
		if (smallMarker != null)
		{
			Debug.Log("markerSelected2");
			StartCoroutine(HighlightMarker());
			dialog.de.selectedMarkers = this;
		}
	}
	public void SetDialog(Dialogs _dialog)
	{
		dialog = _dialog;
	}
	public void OnEnable()
	{
		if (smallMarker != null)
			smallMarker.SetActive(true);
	}
	public void OnDisable()
	{
		if (smallMarker != null)
			smallMarker.SetActive(false);
	}
	public void OnDestoy()
	{
		Destroy(smallMarker);
	}
	IEnumerator HighlightMarker()
	{		
		Image img = smallMarker.GetComponent<Image>();
		Color origin = new Color(img.color.r, img.color.g, img.color.b, img.color.a);
		img.color = new Color(1.0f, 1.0f, 1.0f, origin.a);
		yield return null;
		if (img != null)
		{
			while (img.color.r + img.color.g + img.color.b >= origin.r + origin.b + origin.g)
			{
				yield return new WaitForSeconds(0.01f); ;
				img.color = new Color(img.color.r - 0.02f, img.color.g - 0.02f, img.color.b - 0.02f, img.color.a);
			}
			if(img != null)
				img.color = origin;
		}
	}
	public void SetSmallMarker(GameObject _smallMarker)
	{
		smallMarker = _smallMarker;
	}
	
}
