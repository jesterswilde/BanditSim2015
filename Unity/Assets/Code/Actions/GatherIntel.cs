using UnityEngine;
using System.Collections;

public class GatherIntel : Actions {

	public override void Startup (Location _theLoc) //often called in editor
	{
		base.Startup (_theLoc);
		transform.position = _loc.transform.position; 
		transform.parent = _loc.transform; 
		gameObject.name ="Gather Intel"; 
		_actionName = "Gather Intel"; 
	}

	public override void NewDay (Bandit _bandit)
	{
		FindOutAboutCaravans (_bandit); 
	}

	void FindOutAboutCaravans(Bandit _bandit){
		int _skillCheck = _bandit.UseCompoundingSkill (_bandit.Cunning);
		Debug.Log (_bandit.Name + " made a roll of " + _skillCheck); 
		CaravanRoute _bestRoute = null;
		foreach (CaravanRoute _route in _loc.PassingRoutes) {
			Debug.Log (_route.name + " Roll check " + _skillCheck + " | " + _route.DC); 
			if(_skillCheck > _route.DC){ //if you pass the DC to learn about the route
				if(!_bandit.AlredyKnowAboutRoute(_route) && !World.TheHideout.AlreadyKnowAboutRoute(_route)){ //and neither the bandit, nor the hideout know about it
					_bestRoute = _route; 
				}
			}
		}
		if (_bestRoute != null) {
			_bandit.LearnAboutRoute (_bestRoute); 
			Debug.Log("Learned about a route"); 
		} 
		else{
			Debug.Log ("Learned nothing"); 
		}
	}
}
