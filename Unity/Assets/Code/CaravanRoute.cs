using UnityEngine;
using System; 
using System.Collections;
using System.Collections.Generic;

public class CaravanRoute : MonoBehaviour, IComparable<CaravanRoute> {

	public Location startCity; 
	public Location endCity;

	[SerializeField]
	int _spawnRate;
	int _counter; 
	[SerializeField]
	float _speed; 
	public float Speed { get { return _speed; } }
	float _totalRouteDistance; 
	public UnityEngine.Object caravanPrefab; 

	[SerializeField]
	int _dc; 
	public int DC{ get { return _dc; } }

	int _id; 
	public int ID { get { return _id; } }

	List<LocNode> _theRoute = new List<LocNode> (); 
	List<float> _arrivalTimes = new List<float>(); 



	void CalculateTraveTime(){ ///use the speed and distance to make a list of equal length to the route. Where each node has a corrolating time from departure
		_arrivalTimes.Clear ();
		_arrivalTimes.Add (0); 
		for (int i = 1; i < _theRoute.Count; i++) {
			_arrivalTimes.Add(_arrivalTimes[i-1] + Vector3.Distance(_theRoute[i].transform.position,_theRoute[i-1].transform.position)/_speed);
		}
	}

	public float ArrivalTime(LocNode _node){ //if given a node, will tell you when you arrive there
		for (int i = 0; i < _theRoute.Count; i ++) {
			if(_theRoute[i].ID == _node.ID){
				return _arrivalTimes[i] + NextCaravanETA(); 
			}
		}
		return -1; 
	}
	public int NextCaravanETA(){
		return (_spawnRate - _counter); 
	}
	void ConnectToNodes(){
		foreach (LocNode _node in _theRoute) {
			_node.KnowAboutRoute (this); 
		}
	}
	public static float CalcTotalDistance(List<LocNode> _theList){
		float _distance = 0;
		for (int i = 0; i <  _theList.Count; i++) {
			if(i != 0){
				_distance += Vector3.Distance(_theList[i].transform.position, _theList[i-1].transform.position);
			}
		}
		return _distance; 
	}

	public int CompareTo(CaravanRoute _otherRoute){ //fuffilling interface stuff so the list can be sorted.
		if (_otherRoute.DC > _dc)
						return -1; 
		if (_otherRoute.DC == _dc)
						return 0; 
		return 1; 
	}

	public bool ContainsNode(LocNode _theNode){
		foreach (LocNode _otherNode in _theRoute) {
			if(_otherNode.ID == _theNode.ID)
				return true;
		}
		return false; 
	}
	public LocNode FirstLocation(LocNode _locA, LocNode _locB){
		for (int i = 0; i < _theRoute.Count; i++) {
			if(_theRoute[i].ID == _locA.ID) return _locA; 
			if(_theRoute[i].ID == _locB.ID) return _locB; 
		}
		return null; 
	}

	//UI SHiznit ------------------------------------------------------------------------------------------
	bool _currentState = false; 


	public void HighlightRoute(bool _onOff){
		if(_currentState != _onOff){
			_currentState = !_currentState; 
			for (int i = 0; i < _theRoute.Count; i++) {
				if(i > 0 ){
					_theRoute[i].HighlightRoad(_theRoute[i-1],_onOff); 
				}
				if(i < _theRoute.Count-1){
					_theRoute[i].HighlightRoad(_theRoute[i+1],_onOff);  
				}
			}
		}
	}



	//Route pathfinding -------------------------------------------------------------------------------------

	public static List<LocNode> GetRoute(Location _startCity, Location _endCity ){
		LocNode startNode = _startCity.GetComponent<LocNode> ();
		LocNode endNode = _endCity.GetComponent<LocNode> (); 
		List<LocNode> _placesToCheck = new List<LocNode>(); 
		World.ClearRoute (); 
		List<LocNode> theRoute = new List<LocNode> (); 

		startNode.RouteDistance = 0; 
		startNode.AlreadyCalculated  = true; 
		_placesToCheck.Clear (); 
		_placesToCheck.Add (startNode); 
		CheckNeighbors (startNode,endNode,1000000,_placesToCheck); //calculate all route distances

		bool _returnedHome = false; 
		theRoute.Add (endNode);  
		LocNode _nextLoc = endNode;
		int _iter = 0; 
		while (!_returnedHome ) { //keep building the route until we get home
			_nextLoc = CompileRoute (_nextLoc); 
			theRoute.Add (_nextLoc);
			if(_nextLoc.ID == startNode.ID){
				_returnedHome = true; 
			}
			_iter++; 
			if(_iter > 60){
				Debug.Log("We are erroring out"); 
				_returnedHome = true;
			}
		}
		theRoute.Reverse();
		return theRoute; 
	}

	static void CheckNeighbors(LocNode _currentLoc, LocNode _destination, float _fullRouteDistance, List<LocNode> _placesToCheck){
		if (_fullRouteDistance < _currentLoc.RouteDistance) { //if we found our destination and there is no way there is a shorter parth, we are done here.
			return; 
		}
		if (_placesToCheck.Count > 0) { //there are more places to go through
			for(int i = 0; i < _currentLoc.ConnectedNodes.Count; i++){ //We gonna go through all the cities
				if(_currentLoc.ConnectedNodes[i].AlreadyCalculated == false){ //if this city hasn't been calculated before
					_currentLoc.ConnectedNodes[i].AlreadyCalculated = true;
					_placesToCheck.Add (_currentLoc.ConnectedNodes[i]);  // Make sure it does get calculated
				}
				if(_currentLoc.RouteDistance + _currentLoc.LocDistance[i] < _currentLoc.ConnectedNodes[i].RouteDistance){
					_currentLoc.ConnectedNodes[i].RouteDistance = _currentLoc.RouteDistance + _currentLoc.LocDistance[i];
				}
			}
		}
		_placesToCheck.Remove (_currentLoc); 
		if (_currentLoc.ID == _destination.ID) {
			_fullRouteDistance = _currentLoc.RouteDistance; 
		}
		if (_placesToCheck.Count > 0) { //find the shortest current route, and check that one
			int index = 0; 
			float _shortRoute = 1000000; 
			for(int i = 0; i < _placesToCheck.Count; i ++){
				if(_placesToCheck[i].RouteDistance < _shortRoute){
					index = i; 
					_shortRoute = _placesToCheck[i].RouteDistance; 
				}
			}
			CheckNeighbors (_placesToCheck[index],_destination, _fullRouteDistance, _placesToCheck); 
		}
	}

	static LocNode CompileRoute (LocNode _here){
		if(_here == null) return null;
		int _index = 0;
		float _shortestRoute = 100000000; 
		for(int i = 0; i < _here.ConnectedNodes.Count;i++){
			if(_here.ConnectedNodes[i].RouteDistance < _shortestRoute){
				_index = i;
				_shortestRoute = _here.ConnectedNodes[i].RouteDistance; 
			}
		}
		return _here.ConnectedNodes[_index]; 
	}

	//STARTUP AND BOOK KEEPING --------------------------------------------------------------------------------

	public void Startups(){
		_theRoute = GetRoute (startCity, endCity); 
		_totalRouteDistance = CalcTotalDistance (_theRoute); 
		CalculateTraveTime ();
		ConnectToNodes(); 
	}
	public void NextHour(){
		_counter ++; 
		if (_counter >= _spawnRate) {
			_counter = 0; 
			SpawnCaravan(); 
		}
	}


	void SpawnCaravan(){
		if(_theRoute.Count > 0){
			GameObject _theCaravan = Instantiate (caravanPrefab) as GameObject; 
			if(_theCaravan.GetComponent<Caravan>()!= null){
				_theCaravan.GetComponent<Caravan> ().SpawnCaravan (_theRoute, _speed); 
			}
			else Debug.Log("Error! You a stupid Ho! The route " + this.name +" is spawning a caravan "+ _theCaravan.name +" But that is not actually a caravan. It has no caravan route"); 
		}
	}

	void Start(){
		_id = World.GetID (); 
	}
	/*
	 * 
	public List<Location> FindPath(Location _destination){ //path finding algorithm
		Debug.Log ("finding path from " +player.CurentLocation.DisplayName+ " to " + _destination.DisplayName); 
		_placesToCheck.Clear (); 
		List<Location> theRoute = new List<Location> (); 
		foreach (Location _loc in allLocations) {
			_loc.RouteDistance = 100000; //set all distance to ~infinite	
			_loc.RouteChecked = false;
		}
		player.CurentLocation.RouteDistance = 0; //make the starting point
		_placesToCheck.Add (player.CurentLocation); //we start where you are
		CheckNeighborRoute (player.CurentLocation, _destination);  //go through the neighbors till you find where you are going

		bool _returnedHome = false;
		Location _nextLocal = _destination; 
		theRoute.Add (_destination); 
		while (_returnedHome == false) { //until we get back
			_nextLocal = CompileRoute(_nextLocal); //build the route
			if(_nextLocal.ID != player.CurentLocation.ID){ //we aren't back hom eyet
				theRoute.Add (_nextLocal); 
			}
			else{ //we found our way back home
				_returnedHome = true; 
			}
		}
		theRoute.Reverse(); 
		return theRoute; 
	}

	void CheckNeighborRoute(Location _currentLocation, Location _targetLocation){ //iterates through locations until we find the destination
		if(_placesToCheck.Count > 0){
			for (int i = 0; i < _currentLocation.nearbyLocals.Count; i++) {
				if(_currentLocation.nearbyLocals[i].RouteChecked == false){ //you haven't been there yet
					_currentLocation.nearbyLocals[i].RouteChecked = true; //so check it
					_placesToCheck.Add(_currentLocation.nearbyLocals[i]); //and visit it next
				}
				if(_currentLocation.nearbyLocals[i].RouteDistance > _currentLocation.RouteDistance + _currentLocation.LocationDistances[i]){ //optimum route
					_currentLocation.nearbyLocals[i].RouteDistance = _currentLocation.RouteDistance + _currentLocation.LocationDistances[i]; 
				}
			}
			_placesToCheck.Remove(_currentLocation);  //we've sorted this place, no need to check it again
			/*if(_currentLocation.ID == _targetLocation.ID){ //if it was the destination end the loop
				return; 
			}
	if(_placesToCheck.Count>0){
		CheckNeighborRoute(_placesToCheck[NextNeighborToCheck()],_targetLocation); //continue
	}
}
}
int NextNeighborToCheck(){ //goes through the list and finds the shortest distance we've traveled yet
	float _shortestDistance = 10000000; 
	int theIndex = 0; 
	for (int i = 0; i < _placesToCheck.Count; i++) { //go through all the locations
		if(_placesToCheck[i].RouteDistance < _shortestDistance){ //pick the one which has the least distance on it
			_shortestDistance = _placesToCheck[i].RouteDistance; 
			theIndex = i; 
		}
	}
	return theIndex; 
}
Location CompileRoute(Location _here){
	int _theIndex = 0;
	float _shortestDistance = 100000; 
	for(int i = 0 ; i < _here.nearbyLocals.Count;i++){
		if(_here.nearbyLocals[i].RouteDistance < _shortestDistance){
			_theIndex = i; 
			_shortestDistance = _here.nearbyLocals[i].RouteDistance; 
		}
	}
	return _here.nearbyLocals [_theIndex]; 
}
*/
}
