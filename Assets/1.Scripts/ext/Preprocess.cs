using UnityEngine;
using System.Collections;

public class Preprocess : MonoBehaviour {
    float x = 0.0f;
    float dest = 1.0f;
    void Awake()
    {
		/*
        #if RESTRICT_FRAMERATE
            //프레임제한
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
            //프레임 제한
        #endif*/
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;
    }
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
