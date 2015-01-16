using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class Bandit : MonoBehaviour {

	/*The bandit is your main unit. They are simple and expendable. 
	Currently they don't have loyalty or hunger. Each 

	 */
	[SerializeField]
	string _name; 
	public string Name { get { return _name; } set { _name = value; } }
	bool isFemale; 

	[SerializeField]
	int _health = 20; 
	public int Health { get { return _health; } }
	[SerializeField]
	int _currentHealth = 20; 
	public int CurrentHealth { get { return _currentHealth; } }
	[SerializeField]
	int _DR = 0; 
	public int DR { get { return _DR; } }

	int _level = 0; 
	public int Level { get { return _level; } }
	int _xp = 0; 
	public int XP { get { return _xp; } }
	float _agility = 0; 
	public float Agility { get { return _agility; } }
	float _range = 0;
	public float Range { get { return _range; } }
	float _magic = 0; 
	public float Magic { get { return _magic; } }
	float _building = 0; 
	public float Building { get { return _building; } }
	float _cunning = 0; 
	public float Cunning { get { return _cunning; } }
	float _crafty = 0; 
	public float Crafty { get { return _crafty; } }

	float _loyalty; 
	[SerializeField]
	float _foodConsumption = 1; //how much food each dude eats in a day. 
	public float FoodConsumption { get { return _foodConsumption; } }
	bool _starving = false; 
	public bool Starving { get { return _starving; } set { _starving = value; } }
	[SerializeField]
	int _starvingDamage = 2; 
	int _id; 
	bool _isAlive = true; 
	public bool IsAlive { get { return _isAlive; } }
	Items[] _equippedGear = {null,null,null}; 
	Items[] EquippedGear { get { return _equippedGear; } }

	CaravandGuard _guardTarget; 

	[SerializeField]
	Location _currentLocation; // Where the bandit currently is. This may be unnecissary as every location has a list of all bandits who are at it. 
	[SerializeField]
	Actions _currentTask; //whatever they are currently assigned to do




	//TIME STUFF =----------------------------------------------------------------------------------
	public void NewDay(){ //they spent the night wherevery they are at. 
		if (_starving) {
			Starve(); 
		}
		_currentTask.NewDay (this); 
	}
	public void NextHour(){
		_currentTask.NextHour (this); 
	}
	void Starve(){
		_currentHealth -= _starvingDamage; 
		CheckHealth (); 
	}
	public void AssginTask(Actions _action){
		_currentTask = _action;
	}
	public void ArrivedAtLocation(Location _loc){
		_currentLocation = _loc; 
		_loc.BanditArrives (this); 
	}
	public void LeaveLocation(){
		_currentLocation.BanditLeaves (this);
	}
	public void TravelToLocation(Location _loc){ //does both leaving and arriving actions. Should work in travel time at some point
		Debug.Log (World.Map.SelLocation); 
		LeaveLocation (); 
		ArrivedAtLocation (_loc); 
	}
	
	public void TimeDamage(int _damage){ //damage from being exhausted, dr doesnot affect
		_currentHealth -= _damage; 
		CheckHealth (); 
	}
	public void Rest(int _healing){
		if (!_starving) {
			_currentHealth += _healing;
			if(_currentHealth > _health){
				_currentHealth = _health; 
			}
		}
	}




	//COMBAT and death, they go hand n hand ya know ----------------------------------------------------------------------
	public int DoDamage(){ //there will be a better system here eventually
		return _level + Random.Range(1,6); 
	}
	public void TakeDamage(int _damage){
		_currentHealth -= Mathf.Clamp (_damage - _DR, 0, 100000); 
		Debug.Log(_name +" took " + Mathf.Clamp (_damage - _DR, 0, 100000)  + " to the face, has "+ _currentHealth +" remaining"); 
		CheckHealth (); 
	}
	public void AcquireTarget(List<CaravandGuard> _guards){
		if(_guards.Count > 0 ){
			_guardTarget = _guards[Random.Range(0,_guards.Count)]; 
		}
	}
	public void Attack(List<CaravandGuard> _guards){
		if(_isAlive){
			if (_guardTarget == null || !_guardTarget.IsAlive) //if you don't have a target, get one
							AcquireTarget (_guards); 
			if (_guardTarget == null || !_guardTarget.IsAlive) //if you still don't have a target, it's because none are available. 
							return; 
			_guardTarget.TakeDamage (DoDamage ()); //deal damage
		}
	}
	void CheckHealth(){
		if (_currentHealth <= 0) {
			Die(); 		
		}
	}
	public void Die(){
		Debug.Log (_name + " Has died"); 
		World.TheHideout.RemoveBandit (this); 
		_currentLocation.BanditLeaves (this); 
		_isAlive = false; 
		Destroy (this.gameObject); 
	}


	

	//LEVEL & XP ------------------------------------------------------------------------------------------------------
	void GainXP(int _xpGained){
		_xp += _xpGained; 
		if (_xp >= (_level + 1) * 100) {
			_xp -= (_level+1)*100;
			LevelUp(); 
		}
	}

	void LevelUp(){
		_level += 1; 
	}







	//STARTUP, everything has got to start somewhere -------------------------------------------------------------------
	void SpawnBandit(){
		_currentLocation = World.HideoutLoc; 
		World.HideoutLoc.BanditArrives (this); 
		World.TheHideout.AddBandit (this); 
	}
	void Start(){
		_id = World.GetID ();
		_name = "Bandit_" + _id.ToString (); 
		SpawnBandit (); 
	}

}
