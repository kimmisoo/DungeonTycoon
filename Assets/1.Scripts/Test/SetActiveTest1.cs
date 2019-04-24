using UnityEngine;
using System.Collections;

public class SetActiveTest1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(Destroy());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(2.0f);
        gameObject.SetActive(false);
    }

}
