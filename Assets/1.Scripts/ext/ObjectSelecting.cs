using UnityEngine;
using System.Collections;

public class ObjectSelecting : MonoBehaviour {

	Touch[] touches;
	GameObject selectedObject = null;
   
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
		touches = Input.touches; 
		if (touches.Length == 1) 
		{
            if(touches[0].phase == TouchPhase.Ended)
				GetSelectedObject ();
		}
        
			
	}

	void GetSelectedObject()
	{
		RaycastHit hit;
		Ray ray;
		Vector3 touchPosition = new Vector3(touches[0].position.x, touches[0].position.y, 0);
		ray = Camera.main.ScreenPointToRay (touchPosition);
        
        if (Physics.Raycast (ray, out hit) == true) 
		{
			
            
			if (hit.collider.gameObject != selectedObject) 
			{
				selectedObject = hit.collider.gameObject;
				Debug.Log (selectedObject.name + " is Selected!");
			}
		}
		else
		{
			selectedObject = null;
		}
	}
}
