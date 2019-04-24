using UnityEngine;
using System.Collections;

public class SetActiveTest2 : MonoBehaviour {

    public GameObject reference;
	// Use this for initialization
	void Start () {
        Debug.Log("name = " + reference.name);
        StartCoroutine(wait());
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator wait()
    {
        yield return new WaitForSeconds(4.0f);
        Debug.Log("name = " + reference.name);
    }
}
