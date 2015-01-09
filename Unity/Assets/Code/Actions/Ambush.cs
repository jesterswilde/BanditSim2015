using UnityEngine;
using System.Collections;

public class Ambush : Actions {

	[SerializeField]
	AmbushZone _ambushZone; 
	

	public override void StartGame(Location _theLoc){ //called when the game starts
		base.StartGame (_theLoc); 
	}

	public override void Startup (Location _theLoc) //often called in editor
	{
		base.Startup (_theLoc);
		transform.position = _loc.transform.position; 
		transform.parent = _loc.transform; 
		gameObject.name = "AmbushAction"; 
		_actionName = "Ambush"; 
		CreateAmbushZone (); 
	}

	public void CreateAmbushZone(){ //if you can ambush at aplace, this is how ambush zone detection will be handled. 
		if (gameObject.GetComponent<AmbushZone> () == null) {
			_ambushZone = gameObject.AddComponent<AmbushZone>();
			_ambushZone.Startup (this); 
			BoxCollider _collider = gameObject.AddComponent<BoxCollider> (); 
			_collider.isTrigger = true; 
		}
	}
	public override void Cleanup ()
	{
		DestroyImmediate (_ambushZone); 
		base.Cleanup ();
	}
	public void AndTheCageComesDown(Caravan _caravan){ //with the japanese fighting spiders
		if(_caravan.HasFoughtAt != _loc.ID){ //if they haven't fought at this location before
			_caravan.HasFoughtAt = _loc.ID; 
			if(_loc.HasBandits()){
				World.combat.StartCombat (_loc, _caravan); 
			}
		}
	}
}
