using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Intro : MonoBehaviour {

	public Image logo;
	float alpha = 0.0f;
	// Use this for initialization
	void Start () {
        StartCoroutine(CountSeconds());
	}
	
	

	IEnumerator StartLogoMove()
	{
		yield return null;
		while(alpha <= 0.95f)
		{
			yield return new WaitForSeconds(0.05f);
			alpha += 0.04f;
			logo.color = new Color(1.0f, 1.0f, 1.0f, alpha);
		}
	}

	IEnumerator CountSeconds()
	{
        yield return StartCoroutine(StartLogoMove());
        yield return new WaitForSeconds(1.0f);
		
		Application.LoadLevel("0");
	}
	
}
