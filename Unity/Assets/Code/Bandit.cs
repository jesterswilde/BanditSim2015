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
	[SerializeField]
	int _health = 20; 
	public int Health { get { return _health; } }
	[SerializeField]
	int _currentHealth = 20; 
	public int CurrentHealth { get { return _currentHealth; } }
	[SerializeField]
	int _level = 1; 
	int _XP = 0; 
	int _DR = 0; 
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

	CaravandGuard _guardTarget; 

	[SerializeField]
	Location _currentLocation; // Where the bandit currently is. This may be unnecissary as every location has a list of all bandits who are at it. 
	[SerializeField]
	Actions _currentTask; //whatever they are currently assigned to do

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


	//this is the combat section
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
	//Leveling up and XP should go here


	void SpawnBandit(){
		_currentLocation = World.HideoutLoc; 
		World.HideoutLoc.BanditArrives (this); 
		World.TheHideout.AddBandit (this); 
	}
	void Start(){
		SpawnBandit (); 	
		_id = World.GetID ();
		_name = "Bandit_" + _id.ToString (); 
	}

}
