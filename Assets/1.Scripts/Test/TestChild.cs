using UnityEngine;
using System.Collections;

public class TestChild : TestParent {

	// Use this for initialization
	void Start () {
        
	}
    public void ImChild()
    {
        
    }
    public override void foo()
    {
        Debug.Log("immmmmmm CHILD!");   
    }

    // Update is called once per frame
    void Update () {
	
	}
}
