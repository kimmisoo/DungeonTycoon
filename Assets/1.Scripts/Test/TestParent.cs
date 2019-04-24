using UnityEngine;
using System.Collections;

public class TestParent : MonoBehaviour {

    private int parentInt = 0;
	// Use this for initialization
	public virtual void Start () {
	}

    public virtual void foo()
    {
        Debug.Log("im parent");
    }
	// Update is called once per frame
	void Update () {
	    
	}
}
