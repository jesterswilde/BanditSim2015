using UnityEngine;
using System.Collections;

public class Controls  {

	protected Camera _cam;  

	public virtual void Startup(){
		_cam = World.mainCam; 
	}
	public virtual void CheckForClicks(){
		if (Input.GetMouseButtonDown (0)) {
			LeftClick(); 	
		}
		if (Input.GetMouseButton (1)) {
			RightClick(); 	
		}
	}
	public virtual void Movement(){
		
	}
	protected virtual void LeftClick(){

	}
	protected virtual void RightClick(){
		
	}
}



public class MapControl : Controls {
	protected override void LeftClick ()
	{
		Ray _ray = _cam.ScreenPointToRay (Input.mousePosition); 
		RaycastHit _hit; 
		if(Physics.Raycast(_ray,out _hit,100f)){
			if(_hit.collider.gameObject.layer == 8) //The ray hit a location
			{
				Debug.Log("clicked on a buildling"); 
				World.Map.SelectLocation(_hit.collider.transform.parent.gameObject.GetComponent<Location>()); 
			}
		}
	}
}
public class HideoutControl : Controls{
	
}
