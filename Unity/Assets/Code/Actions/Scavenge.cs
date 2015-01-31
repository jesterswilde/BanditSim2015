using UnityEngine;
using System.Collections;

public class Scavenge : Actions {

	[SerializeField]
	Vector2 _foodRange = new Vector2 (0, 0);  
	public Vector2 FoodRange { get { return _foodRange; } set { _foodRange = value; } }
	[SerializeField]
	int _chance = 25; 
	public int SuccessChance { get { return _chance; } set { _chance = Mathf.Clamp (value, 0, 100); } }


	public override void StartGame(Location _theLoc){ //called when the game starts
		base.StartGame (_theLoc); 
	}
	
	public override void Startup (Location _theLoc) //often called in editor
	{
		base.Startup (_theLoc);
		transform.position = _loc.transform.position; 
		transform.parent = _loc.transform; 
		gameObject.name = "ScavengeAction"; 
		_actionName = "Scavenge"; 
	}

	public override void NewDay (Bandit _bandit)
	{
		base.NewDay (_bandit);
		ScavengeForFood (_bandit); 
	}
	void ScavengeForFood(Bandit _bandit){
		_bandit.GatheredFood( _bandit.Crafty + _bandit.DieRoll () ); 
	}
}
