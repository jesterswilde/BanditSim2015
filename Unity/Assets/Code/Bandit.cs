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
	float _health = 10; 
	public int Health { get { return (int)_health; } }
	[SerializeField]
	float _currentHealth = 10; 
	public int CurrentHealth { get { return (int)_currentHealth; } }
	[SerializeField]
	float _DR = 0; 
	public int DR { get { return (int)_DR; } }

	[SerializeField] 
	int _dieRoll = 20; 

	int _level = 0; 
	public int Level { get { return _level; } }
	int _xp = 0; 
	public int XP { get { return _xp; } }
	float _damage = 2; 
	public int Damage{ get {return (int)_damage;}}
	float _agility = 3; 
	public int Agility { get { return (int)_agility; } }
	float _range = 0;
	public int Range { get { return (int)_range; } }
	float _magic = 0; 
	public int Magic { get { return (int)_magic; } }
	float _building = 0; 
	public int Building { get { return (int)_building; } }
	float _cunning = 0; 
	public int Cunning { get { return (int)_cunning; } }
	float _crafty = 0; 
	public int Crafty { get { return (int)_crafty; } }

	float _loyalty; 
	[SerializeField]
	float _foodConsumption = 1; //how much food each dude eats in a day. 
	public float FoodConsumption { get { return _foodConsumption; } }
	bool _starving = false; 
	public bool Starving { get { return _starving; } set { _starving = value; } }
	[SerializeField]
	int _starvingDamage = 2; 
	int _id; 
	public int ID { get { return _id; } }
	bool _isAlive = true; 
	public bool IsAlive { get { return _isAlive; } }
	Items[] _equippedGear = {null,null,null}; 
	Items[] EquippedGear { get { return _equippedGear; } }

	[SerializeField]
	CaravandGuard _guardTarget; 

	[SerializeField]
	Location _currentLocation; // Where the bandit currently is. This may be unnecissary as every location has a list of all bandits who are at it. 
	[SerializeField]
	Actions _currentTask; //whatever they are currently assigned to do

	int _itineraryInt; 
	List<LocNode> _itinerary = new List<LocNode>(); 
	bool _isTraveling = false; 
	Location _destination; 
	int _daysOut; //how many days they have left to be at a location
	[SerializeField]
	float _travelSpeed;

	float _currentSkilBase; //for cumulative checks, this is the number that gets incremented


	List<CaravanRoute>_knownRoutes = new List<CaravanRoute> (); 
	int _foodHaul = 0;

	BanditClass _class; 



	//TIME STUFF =----------------------------------------------------------------------------------
	public void NewDay(){ //they spent the night wherevery they are at. 
		if (_starving) {
			Starve(); 
		}
		if(!_isTraveling && _currentTask != null){ //if they are not traveling, then they should be doing something
			_currentTask.NewDay (this); 
		}
		if (_currentLocation != null) { //count down until they head back to the hideout
			if(_currentLocation.ID != World.HideoutLoc.ID){
				_daysOut -= 1; 
				if(_daysOut <= 0){
					TravelToLocation(World.HideoutLoc,0); 
				}
			}
		}
	}
	public void NextHour(){
		if(!_isTraveling && _currentTask != null){
			_currentTask.NextHour (this); 
		}
	}
	void Starve(){
		_currentHealth -= _starvingDamage; 
		CheckHealth (); 
	}
	public void AssginTask(Actions _action){
		_currentTask = _action;
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

	//SKILLS and shit related to skiiiiiiiiiiiiiiillz ----------------------------------------------------------------------
	public int DieRoll(){
		return Random.Range (0, _dieRoll + 1); 
	}
	public int UseCompoundingSkill(float _skill){
		_currentSkilBase += _skill; 
		return (int)_currentSkilBase + DieRoll (); 
	}
	void ReturnedToHideout(){
		UnloadRouteInfo (); 
		UnloadFood (); 
	}
	public void LearnAboutRoute(CaravanRoute _theRoute){ // called by gather intel
		if (!AlredyKnowAboutRoute(_theRoute)) {
			_knownRoutes.Add(_theRoute); 
			Debug.Log("Learned About A Route"); 
		}
	}
	public bool AlredyKnowAboutRoute(CaravanRoute _theRoute){               //check to see if you already know about the route, if you don't put it in the list
		foreach (CaravanRoute _route in _knownRoutes) {    //the list is cleared whenever the bandit returns to the hideout. 
			if(_theRoute.ID == _route.ID){
				return true; 
			}
		}
		return false; 
	}
	void UnloadRouteInfo(){
		Debug.Log ("Informed durr boss about " + _knownRoutes.Count + " route(s)"); 
		foreach (CaravanRoute _route in _knownRoutes) {
			World.TheHideout.LearAboutRoute(_route); 	
		}
		_knownRoutes.Clear (); 
	}
	public void GatheredFood(int _food){
		_foodHaul += _food; 
	}
	void UnloadFood(){
		World.TheHideout.AddFood (_foodHaul);
		_foodHaul = 0; 
	}


	//COMBAT and death, they go hand n hand ya know ----------------------------------------------------------------------
	public int DoDamage(){ //there will be a better system here eventually
		return (int)_damage + Random.Range(1,6); 
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
		if (_currentLocation != null){
			_currentLocation.BanditLeaves (this);
		}
		_isAlive = false; 
		Destroy (this.gameObject); 
	}


	

	//LEVEL & XP ------------------------------------------------------------------------------------------------------
	public void GainXP(int _xpGained){
		_xp += _xpGained; 
		if (_xp >= (_level + 1) * 100) {
			_xp -= (_level+1)*100;
			LevelUp(); 
		}
	}

	void LevelUp(){
		_level += 1; 
	}







	//TRAVELING -------------------------------------------------------------------------------------------------------------
	void Travel(){ //called for moving around
		if (_itinerary.Count > _itineraryInt) { //if there are still places to go
			Vector3 _direction;
			Vector3 _destination; 
			if(_itinerary.Count > _itineraryInt+1){
				_destination = _itinerary[_itineraryInt+1].transform.position; 
			}
			else{
				_destination = _itinerary[_itineraryInt].transform.position; 
			}
			_direction = (_destination - transform.position).normalized; //this is the direction
			float _maxDistance = Vector3.Distance(transform.position, _destination); //this is the how far away it is
			transform.position += _direction * Mathf.Clamp( _travelSpeed * Time.deltaTime,0, _maxDistance); //move it towards the goal
			if (transform.position == _destination) { //if you are close to your destination
				transform.position = _destination;
				_itineraryInt ++; //you have arrived at the next spot
			}
		}
		else{	
			ArrivedAtLocation(); 
		}
	}
	public void ArrivedAtLocation(){ 
		_currentLocation = _destination; 
		_currentLocation.BanditArrives (this); 
		_isTraveling = false; 
		_itineraryInt = 0; 
		_itinerary.Clear ();
		_destination = null;  
		if (_currentLocation.ID == World.HideoutLoc.ID) {
			ReturnedToHideout(); 
		}
		_currentSkilBase = 0; 
	}
	public void LeaveLocation(){
		_currentLocation.BanditLeaves (this);
		_currentLocation = null; 
		_currentTask = null; 
	}
	void GetItinerary(){ //gets the route the bandit will need to travel
		_isTraveling = true; 
		_itinerary = CaravanRoute.GetRoute (_currentLocation, _destination); 
		_itineraryInt = 0; 
	}
	public void TravelToLocation(Location _loc, int _days){ //The action called to get a bandit to go to a place, including back home
		_destination = _loc;
		GetItinerary (); 
		LeaveLocation (); 
		if(_days != 0){
			_daysOut = _days;
		}
	}



	//STARTUP, everything has got to start somewhere -------------------------------------------------------------------
	void SpawnBandit(){ //every bandit starts at the hideout, also tells the world that the bandits exist. 
		if(World.TheHideout.HireBandit (this)) {
			_currentLocation = World.HideoutLoc; 
			World.HideoutLoc.BanditArrives (this);
			transform.position = World.HideoutLoc.transform.position; 
		}
		else {
			Destroy (this.gameObject); 
		}
	}
	void Start(){
		_id = World.GetID ();
		_name = "Bandit_" + _id.ToString (); 
		SpawnBandit (); 
		_class = new ClassPawn (); 
	}

	void Update(){
		if (_isTraveling) {
			Travel(); 
		}
	}


}
