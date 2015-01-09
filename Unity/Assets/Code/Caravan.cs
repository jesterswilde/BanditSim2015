using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class Caravan : MonoBehaviour {

	[SerializeField]
	string _displayName; 
	public string DisplayName { get { return _displayName; } set { _displayName = value; } }
	[SerializeField]
	string _description; 
	public string Description { get { return _description; } set { _description = value; } }
	[SerializeField]
	int _foodMin = 0;
	public int FoodMin { get { return _foodMin; } set { _foodMin = value; } }
	[SerializeField]
	int _foodMax = 0;
	public int FoodMax { get { return _foodMax; } set { _foodMax = value; } }
	[SerializeField]
	int _moneyMin = 0;
	public int MoneyMin { get { return _moneyMin; } set { _moneyMin = value; } }
	[SerializeField]
	int _moneyMax = 0; 
	public int MoneyMax { get { return _moneyMax; } set { _moneyMax = value; } }
	[SerializeField]
	int _banditsMin = 0; 
	public int BanditsMin { get { return _banditsMin; } set { _banditsMin = value; } }
	[SerializeField]
	int _banditsMax = 0; 
	public int BanditsMax { get { return _banditsMax; } set { _banditsMax = value; } }

	[SerializeField]
	int _numGuard = 1; //how many differetn types of guards there are
	public int NumGuard { get { return _numGuard; } set { _numGuard = value; } }
	[SerializeField]
	List<Object> _guards = new List<Object>(); //the different prefabs of guards tha are there
	public List<Object> Guards { get { return _guards; } set { _guards = value; } }
	[SerializeField]
	List<Vector2> _guardRange = new List<Vector2>(); //how many guards of each type can be spawned
	public List<Vector2> GuardRange { get { return _guardRange; } set { _guardRange = value; } }

	public List<CaravandGuard> ActiveGuards = new List<CaravandGuard>(); //the Guards that the caravan has
	//public List<CaravandGuard> ActiveGuards { get { return _activeGuards; } }

	public List<Location> _itinerary = new List<Location>(); 
	[SerializeField]
	int _travelSpeed;
	public int TravelSpeed {get { return _travelSpeed; } set { _travelSpeed = value; }}

	int _hasFoughtAt; 
	public int HasFoughtAt{ get { return _hasFoughtAt; } set { _hasFoughtAt = value; } }

	void SpawnGuards(){ 
		for(int i = 0; i < _numGuard; i++){
			for(int s = 0; s < Random.Range(_guardRange[i].x,_guardRange[i].y); s++){
				GameObject _theGuardGO =  Instantiate(_guards[i]) as GameObject;
				CaravandGuard _theGuard = _theGuardGO.GetComponent<CaravandGuard>(); 
				_theGuardGO.transform.parent = this.transform;
				ActiveGuards.Add(_theGuard); //put the guard in the active guard list
				_theGuard.Startup(this);  //lets the guard known that htey are part of this caravan
			}
		}
	}
	public void GuardDied(CaravandGuard _theGuard){
		ActiveGuards.Remove (_theGuard); 
	}
	public void CaravanDefeated(){
		Debug.Log ("The men defending the caravan, just trying to feed their families, have all been slain."); 
		World.TheHideout.Money += Random.Range (_moneyMin, _moneyMax); 
		World.TheHideout.Food += Random.Range (_foodMin, _foodMax); 
		for(int i = 0 ; i < Random.Range(_banditsMin,_banditsMax); i++){ 
			World.MakeBandit(); 
		}
		Destroy (this.gameObject); 
	}
	public void CaravanVictorious(){
		Debug.Log ("The bandits were defeated, you get nothing. NOTHING.");
	}
	public void CheckSmallList(List<Object> _theList){
		if (_theList.Count < _numGuard) {
			_theList.Add (null); 
			CheckSmallList(_theList); 
		}
	}
	public void CheckSmallList(List<Vector2> _theList){
		if (_theList.Count < _numGuard) {
			_theList.Add (Vector2.zero); 
			CheckSmallList(_theList); 
		}
	}
	public void CheckLageList(List<Object> _theList){
		if (_theList.Count > _numGuard) {
			_theList.RemoveAt(_theList.Count-1);
			CheckLageList(_theList); 
		}
	}
	public void CheckLageList(List<Vector2> _theList){
		if (_theList.Count > _numGuard) {
			_theList.RemoveAt(_theList.Count-1);
			CheckLageList(_theList); 
		}
	}
	public void SpawnCaravan(List<Location> _theLocs){ //makes the caravand and places it at the start location
		_itinerary = _theLocs; 
		transform.position = _itinerary [0].transform.position; 
		_itinerary.Remove (_itinerary [0]); 
	}
	void Travel(){
		if (_itinerary.Count > 0) { //if there are still places to go
			Vector3 _direction;
			Vector3 _destination; 
			if(_itinerary.Count > 1){
				_destination = _itinerary[0].TravelDirection(_itinerary[1]); 
			}
			else{
				_destination = _itinerary[0].transform.position; 
			}
			_direction = (_destination - transform.position).normalized; //this is the direction
			float _maxDistance = Vector3.Distance(transform.position, _destination); //this is the how far away it is
			transform.position += _direction * Mathf.Clamp(_travelSpeed*Time.deltaTime,0, _maxDistance); //move it towards the goal
			if (transform.position == _destination) { //if you are close to your destination
				transform.position = _destination;
				_itinerary.Remove(_itinerary[0]); //you have arrived at the next spot
			}
		}
	}
	void Update(){
		Travel (); 
	}
	void Start(){
		SpawnGuards (); 
	}
}
