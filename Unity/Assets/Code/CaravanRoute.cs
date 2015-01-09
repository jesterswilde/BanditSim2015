using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CaravanRoute : MonoBehaviour {

	public Location startCity; 
	public Location endCity;

	public float spawnRate; 
	public Object caravanPrefab; 

	List<Location> _placesToCheck = new List<Location>(); 

	public List<Location> GetRoute(){
		World.ClearRoute (); 
		List<Location> theRoute = new List<Location> (); 

		startCity.routeDistance = 0; 
		startCity.routeCalculated = true; 
		_placesToCheck.Clear (); 
		_placesToCheck.Add (startCity);
		CheckNeighbors (startCity, endCity, 10000000000); //calculate all route distances

		bool _returnedHome = false; 
		theRoute.Add (endCity); 
		Location _nextLoc = endCity; 
		while (!_returnedHome) { //keep building the route until we get home
			_nextLoc = CompileRoute (_nextLoc); 
			theRoute.Add (_nextLoc);
			if(_nextLoc.ID == startCity.ID){
				_returnedHome = true; 
			}
		}
		theRoute.Reverse();
		return theRoute; 
	}

	void CheckNeighbors(Location _currentLoc, Location _destination, float _fullRouteDistance){
		if (_fullRouteDistance < _currentLoc.routeDistance) { //if we found our destination and there is no way there is a shorter parth, we are done here.
			return; 		
		}
		if (_placesToCheck.Count > 0) { //there are more places to go through
			for(int i = 0; i < _currentLoc.ConnectedCities.Count; i++){ //We gonna go through all the cities
				if(_currentLoc.ConnectedCities[i].routeCalculated == false){ //if this city hasn't been calculated before
					_currentLoc.ConnectedCities[i].routeCalculated = true;
					_placesToCheck.Add (_currentLoc.ConnectedCities[i]);  // Make sure it does get calculated
				}     
				if(_currentLoc.IsRoad){
					if(_currentLoc.ConnectedCities[i].routeDistance > 
					   _currentLoc.routeDistance + Mathf.Abs (_currentLoc.GetDistanceOnRoad(_currentLoc.PreviousLoc) - _currentLoc.GetDistanceOnRoad(_currentLoc.ConnectedCities[i]))){
						_currentLoc.ConnectedCities[i].routeDistance =  _currentLoc.routeDistance + //this is a long bit of code, I know
							Mathf.Abs (_currentLoc.GetDistanceOnRoad(_currentLoc.PreviousLoc) - _currentLoc.GetDistanceOnRoad(_currentLoc.ConnectedCities[i])); 
								//if your currently calculated route is larger than the one I'm presenting you now, use this new distance
						_currentLoc.ConnectedCities[i].PreviousLoc = _currentLoc; 
					}
				}
				else{
					if(_currentLoc.ConnectedCities[i].routeDistance > _currentLoc.routeDistance){
						_currentLoc.ConnectedCities[i].routeDistance = _currentLoc.routeDistance;
						_currentLoc.ConnectedCities[i].PreviousLoc = _currentLoc; 
					}
				}
			}
			for(int i = 0; i < _currentLoc.ConnectedRoads.Count; i++){ //same thing as above, but with roads
				if(_currentLoc.ConnectedRoads[i].routeCalculated == false){
					_currentLoc.ConnectedRoads[i].routeCalculated = true; 
					_placesToCheck.Add (_currentLoc.ConnectedRoads[i]); 
				}
				if(_currentLoc.IsRoad){
					if(_currentLoc.ConnectedRoads[i].routeDistance > 
					   _currentLoc.routeDistance + Mathf.Abs (_currentLoc.GetDistanceOnRoad(_currentLoc.PreviousLoc) - _currentLoc.GetDistanceOnRoad(_currentLoc.ConnectedRoads[i]))){
						_currentLoc.ConnectedRoads[i].routeDistance =  _currentLoc.routeDistance + //this is a long bit of code, I know
							Mathf.Abs (_currentLoc.GetDistanceOnRoad(_currentLoc.PreviousLoc) - _currentLoc.GetDistanceOnRoad(_currentLoc.ConnectedRoads[i]));
						//if your currently calculated route is larger than the one I'm presenting you now, use this new distance
						_currentLoc.ConnectedRoads[i].PreviousLoc = _currentLoc; 
					}
				}
				else{
					if(_currentLoc.ConnectedRoads[i].routeDistance > _currentLoc.routeDistance){
						_currentLoc.ConnectedRoads[i].routeDistance = _currentLoc.routeDistance;
						_currentLoc.ConnectedRoads[i].PreviousLoc = _currentLoc; 
					}
				}
			}
		}
		_placesToCheck.Remove (_currentLoc); 
		if (_currentLoc.ID == _destination.ID) {
			_fullRouteDistance = _currentLoc.routeDistance; 
		}
		if (_placesToCheck.Count > 0) { //find the shortest current route, and check that one
			int index = 0; 
			float _shortRoute = 1000000; 
			for(int i = 0; i < _placesToCheck.Count; i ++){
				if(_placesToCheck[i].routeDistance < _shortRoute){
					index = i; 
					_shortRoute = _placesToCheck[i].routeDistance; 
				}
			}
			CheckNeighbors (_placesToCheck[index],_destination, _fullRouteDistance); 
		}
	}
	Location CompileRoute (Location _here){
		bool _isRoad = false; 
		int _index = 0;
		float _shortestRoute = 100000000; 
		for(int i = 0; i < _here.ConnectedCities.Count;i++){
			if(_here.ConnectedCities[i].routeDistance < _shortestRoute){
				_index = i;
				_isRoad = false; 
				_shortestRoute = _here.ConnectedCities[i].routeDistance; 
			}
		}
		for(int i = 0; i < _here.ConnectedRoads.Count;i++){
			if(_here.ConnectedRoads[i].routeDistance < _shortestRoute){
				_index = i; 
				_isRoad = true;
				_shortestRoute = _here.ConnectedRoads[i].routeDistance; 
			}
		}
		if (_isRoad) {
			return _here.ConnectedRoads[_index];	
		}
		else return _here.ConnectedCities[_index]; 
	}

	void Start(){

		InvokeRepeating ("SpawnCaravan",0,spawnRate); 

	}
	void SpawnCaravan(){
		GameObject _theCaravan = Instantiate (caravanPrefab) as GameObject; 
		_theCaravan.GetComponent<Caravan> ().SpawnCaravan (GetRoute ()); 
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
