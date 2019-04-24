using UnityEngine;
using System.Collections;

public class TempBatch : MonoBehaviour {

	public GameObject monster;
	public GameObject tailor;
	TileLayer l1;
	// Use this for initialization
	void Start () {
		l1 = GameManager.Instance.GetMap().GetLayer(0).GetComponent<TileLayer>();
		StartCoroutine(Batch());
	}
	
	IEnumerator Batch()
	{
		yield return null;
		tailor.transform.position = l1.GetTileAsComponent(24, 2).transform.position;
		monster.transform.position = l1.GetTileAsComponent(13, 26).transform.position;
	}
}
