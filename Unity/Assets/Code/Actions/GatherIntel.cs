using UnityEngine;
using System.Collections;

public class GatherIntel : Actions {

	public override void Startup (Location _theLoc) //often called in editor
	{
		base.Startup (_theLoc);
		transform.position = _loc.transform.position; 
		transform.parent = _loc.transform; 
		gameObject.name = "GatherIntel"; 
		_actionName = "GatherIntel"; 
	}
}
