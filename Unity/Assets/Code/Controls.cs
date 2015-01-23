using UnityEngine;
using System.Collections;

public class Controls  {

	protected Camera _cam;  
	protected Transform _camTran; 

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
	public virtual void Hover(){

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
				World.Map.SelectLocation(_hit.collider.GetComponent<Location>()); 
			}
		}
	}
}
public class HideoutControl : Controls{
	float speed = 10; 
	public override void Startup ()
	{
		base.Startup ();
		_camTran = World.world.HideoutParent; 
	}
	protected override void LeftClick ()
	{
		World.bManager.Click (); 
		Debug.Log ("clicked"); 
	}
	public override void Hover ()
	{
		World.bManager.Hover (); 
	}
	public override void Movement ()
	{
		if(Input.GetKey(KeyCode.D)){
			_camTran.Translate (new Vector3 (speed*Time.deltaTime,0,0),Space.World); 
		}
		if(Input.GetKey(KeyCode.A)){
			_camTran.Translate (new Vector3 (speed*Time.deltaTime*-1,0,0),Space.World); 
		}
		if(Input.GetKey(KeyCode.W)){
			_camTran.Translate (new Vector3 (0,0,speed*Time.deltaTime),Space.World); 
		}
		if(Input.GetKey(KeyCode.S)){
			_camTran.Translate (new Vector3 (0,0,speed*Time.deltaTime*-1),Space.World); 
		}
		float _scroll = Input.GetAxis ("Mouse ScrollWheel");
		if( _scroll != 0 ){
			_camTran.Translate ( new Vector3(0,Time.deltaTime*_scroll*400,0),Space.World);
		}
		if(Input.GetKey(KeyCode.G)){
			Application.LoadLevel(Application.loadedLevel); 
		}
	}
}
