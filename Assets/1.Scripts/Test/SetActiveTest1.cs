using UnityEngine;
using System.Collections;

public class SetActiveTest1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
	}
	public void OnEnable()
	{
		Debug.Log("Enabled!" + Time.time);
		StartCoroutine(Destroy());
	}
	public void OnDisable()
	{
		Debug.Log("Disabled!" + Time.time);
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
