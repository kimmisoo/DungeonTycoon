using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


public class ThreadTest : MonoBehaviour {

    int a = 0;
    float starts = 0;
    float ends = 0;
    string name = "";
    // Use this for initialization
    void Start () {
        name = gameObject.name;

	}
	
	
    public void click()
    {
        StartCoroutine(_click());
        //StartCoroutine(work2());
    }
    
    public void Update()
    {
        
    }
    public IEnumerator _click()
    {
        yield return null;
        
        

        ThreadStart ts = new ThreadStart(work);
        Thread t = new Thread(ts);
        t.Start();
        t.Join();
        Debug.Log("work ended! - " + name);           
        
    }
    public void work()
    {
        
        int x = 0;
        for(int i = 0; i<5000; i++)
        {
            x += 1;
            Debug.Log(name + " : " + x);
        }
        a = 1;
    }
}
