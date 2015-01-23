using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class Location : MonoBehaviour {

	/* Locations are mainly accesed through the map. The map representation and the actual place are both counted in this component
	 * This is the parent class. Locations will be things like roads, towns, as well as the hideout. 
	 * 
	 * This script has a specfic editor script for handling inspect shinanengans That is also why all the fields have public getters and setters and are serialized
	 * */

	[SerializeField]
	string _displayName; //what hte player sees for this location
	public string DisplayName{get {return _displayName;} set { _displayName = value; } }
	[SerializeField]
	string[] _actionOptions = {"Ambush","Gather Intel", "Scavenge"}; //the string names for options 
	public string[] Actions { get { return _actionOptions; } }
	[SerializeField]
	int _optionsNum = 1; //how many actions can the player take
	public int OptionsNum { get { return _optionsNum; } set { _optionsNum = value; } }
	[SerializeField]
	List<int> _choicesList = new List<int>(); //the selection for each action in town as integers
	public List<int> ChoiceList { get { return _choicesList; } set { _choicesList = value; } }
	[SerializeField]
	List<Actions> _localActions = new List<Actions> ();  //these are the things that can happen at this location
	public List<Actions> LocalActions { get { return _localActions; } set { _localActions = value; } }

	[SerializeField]
	bool _isRoad = false; //is this a road or not? 
	public bool IsRoad { get { return _isRoad; } set { _isRoad = value; } }
	[SerializeField]
	int _numRoadPoints = 1; //how many points are used to draw the road
	public int NumRoadPoints { get { return _numRoadPoints; } set { _numRoadPoints = value; } }
	[SerializeField]
	List<Transform> _roadPoints = new List<Transform> (); //a list of all the points on the road
	public List<Transform> RoadPoints { get { return _roadPoints; } set { _roadPoints = value; } }

	[SerializeField]
	int _dailyHealthMod = 0; //how much does the player heal or get wounded per day
	public int DailyHealthMod { get { return _dailyHealthMod; }  set{ _dailyHealthMod = value; } }


	public List<Bandit> LocalBandits = new List<Bandit>(); //bandits who are at this location
	//public List<Bandit> LocalBandits { get { return _localBandits; } }
	int _id;
	public int ID { get { return _id; } }

	//route and road variable initialization
	public float routeDistance; //how far to this point in the route this is over-written every time a route is calc
	public bool routeCalculated;  //have we checked this route yet. Also recalculated every time
	Location _previousLocation; //the last place the caravan as at
	public Location PreviousLoc { get { return _previousLocation; } set { _previousLocation = value; } } 
	
	float _roadLength; //how long is the road. Currently it's as teh crow flies from start to end.
	public float RoadLength { get { return _roadLength; } }
	[SerializeField]
	int _numConnectedRoads; //how many roads does this point connect to
	public int numConnectedRoads { get { return _numConnectedRoads; } set { _numConnectedRoads = value; } }
	[SerializeField]
	List<Location> _connectedRoads = new List<Location>(); //A list of all roads that this location touches
	public List<Location> ConnectedRoads { get { return _connectedRoads; } set { _connectedRoads = value; } }
	List<float> _connectedRoadsDistance = new List<float>(); //how far along the road, each of these cities are. Only relevant to roads.
	public List<float> ConnectedRoadDistance { get { return _connectedRoadsDistance; } }
	List<Location> _connectedCities = new List<Location>(); //all cities this road touches. Only relevant to roads
	public List<Location> ConnectedCities { get { return _connectedCities; } }
	List<float> _connectedCitiesDistance = new List<float>();  //how far along the road each of these cities are. 
	public List<float> ConnectedCitiesDistance { get { return _connectedCitiesDistance; } }
	Vector3 _roadDirection; //direction from start of the road to the end. This will also have to be retooled when roads can curve
	LineRenderer _lineRender; 




	public virtual void BanditArrives(Bandit _theBandit){ //just deals with bandits showing up and leaving
		LocalBandits.Add (_theBandit); 
		World.Map.UpdateMapUI (); 
	}
	public virtual void BanditLeaves(Bandit _theBandit){
		LocalBandits.Remove (_theBandit); 
		World.Map.UpdateMapUI (); 
	}
	public bool HasBandits(){
		if (LocalBandits.Count > 0) {
			return true;	
		}
		return false; 
	}


	//effects of being at the location
	public virtual void NewDay(){
		foreach (Bandit _bandit in LocalBandits) {
			DayEffectsOnBandit(_bandit);
			DayOfGathering(_bandit); 
		}
	}
	public void NextHour(){
		
	}
	protected void DayEffectsOnBandit(Bandit _bandit){ //what happens to the bandit when they stay here (hurts them, loyals loyalty, they enjoy it, etc.)
		if (_dailyHealthMod < 0) {
			_bandit.TimeDamage (_dailyHealthMod); 
		}
		if (_dailyHealthMod > 0) {
			_bandit.Rest(_dailyHealthMod); 
		}
	}
	protected void DayOfGathering(Bandit _bandit){ //the benefit of the bandit being here. Eventually I want to have this be a cumulative thing. Multiple bandits
		//working together, or maybe you have a trait that helps. As opposed to now where every bandit just does their own thing. 
	}


	//combat will go here eventually 







	//EditorGUI STuff ------------------------------------------------------------
	//this is mostly about make sure that the list displays properly. I know I spelled large without the R. Too late now. 
	//THer is also probably a way to send in a generic list. Next Time I make something like this I will look into it. 

	public void CheckSmallList(){
		if (_choicesList.Count < _optionsNum) {
			_choicesList.Add (0); 
			CheckSmallList(); 
		}
	}
	public void CheckSmallList(List<Transform> _list, int _total){
		if (_list.Count < _total) {
			_list.Add(null); 
			CheckSmallList(_list,_total); 
		}
	}
	public void CheckLageList(){
		if (_choicesList.Count > _optionsNum) {
			_choicesList.RemoveAt(_choicesList.Count-1);
			CheckLageList(); 
		}
	}
	public void CheckLageList(List<Transform> _list, int _total){
		if (_list.Count > _total) {
			_list.RemoveAt(_list.Count-1);
			CheckLageList(_list, _total); 
		}
	}
	public void CheckSmallList(List<Location> _list, int _total){
		if (_list.Count < _total) {
			_list.Add(null); 
			CheckSmallList(_list,_total); 
		}
	}
	public void CheckLageList(List<Location> _list, int _total){
		if (_list.Count > _total) {
			_list.RemoveAt(_list.Count-1);
			CheckLageList(_list, _total); 
		}
	}
	public void CheckSmallList(List<Actions> _list, int _total){
		if (_list.Count < _total) {
			_list.Add(null); 
			CheckSmallList(_list,_total); 
		}
	}
	public void CheckLageList(List<Actions> _list, int _total){
		if (_list.Count > _total) {
			_list.RemoveAt(_list.Count-1);
			CheckLageList(_list, _total); 
		}
	}

	//Route Calculation stuff -------------------------------------------------------------------
	//this is where things get a little dense. 	

	public void MakeRoadPoints(){
		if(_isRoad){
			for(int i = 0; i < _roadPoints.Count; i++){
				LocNode _theNode; 
				if(_roadPoints[i].GetComponent<LocNode>() ==  null){ //if there isn't a loc node on that point, add one
					_theNode = _roadPoints[i].gameObject.AddComponent<LocNode>(); 
					if(_roadPoints[i].GetComponent<Location>() != null){
						_theNode.Startup(_roadPoints[i].GetComponent<Location>()); //if the locNode is also a location, inform them
					}
				}
				else _theNode = _roadPoints[i].GetComponent<LocNode>();  //if the node was already made, just get it instead
				if( i > 0){
					_theNode.ConnectLocNodes(_roadPoints[i-1].GetComponent<LocNode>()); //add the previous node to this node's list of nodes
				}
			}
		}
	}
	public void GetIDs(){ //does all the startup stuff.
		//CalcRoadLength ();
		//CalcRoadDirection (); 
		//StartupCheck ();
		GetID (); 
		StartGame (); 
		MakeRoadPoints (); 
	}
	/*
	public void ConnectRoadsAndCities(){ //this is called after. Think of it like 'start' and 'awake'
		AttachToRoads (); 
	}
	public void AttachToRoads(){ //goes through all roads and lets them know about this location
		foreach (Location _theRoad in _connectedRoads) {
			_theRoad.FinishAttachingToRoad(this); 
		}
	}
	void FinishAttachingToRoad(Location _theLoc){ //the Roads end of htis, where things are added to lists
		if(!_theLoc.IsRoad){
			_connectedCities.Add (_theLoc); 
		}
		else{ //it is a road
			if(!TheseAreAlreadyConnected(_theLoc)){ // check to see if they are already connected
				_connectedRoads.Add (_theLoc);  //this is really just an error catcher
				_numConnectedRoads = _connectedRoads.Count; 
			}
		}
	}
	public bool TheseAreAlreadyConnected(Location _theRoad){ //checks to see if the roads are already connected (at least from one side)
		bool _connected = false; 
		foreach (Location _checkRoad in _connectedRoads) {
			if(_checkRoad.ID == _theRoad.ID){
				_connected = true; 
			}
		}
		return _connected; 
	}
	public void CalculatePositionsOnRoad(){ //gets every road to be aware of it's distance (from the roads start) to each location it is connected to
		if(_isRoad){
			MakeDistanceListProperLength(_connectedCitiesDistance, _connectedCities); //the list of distances needs to be the same length as the list of locations
			MakeDistanceListProperLength(_connectedRoadsDistance, _connectedRoads); 
			for(int i = 0; i < _connectedCities.Count; i++){ //gets the distance to each city
				_connectedCitiesDistance[i] = Vector3.Distance(_roadPoints[0].position, BMath.PointLineIntersect(_roadPoints[0],_roadPoints[_roadPoints.Count-1]
				                              ,_connectedCities[i].transform));
			}
			for(int i = 0; i < _connectedRoads.Count;i++){ //gets the distance to each road
				_connectedRoadsDistance[i] = Vector3.Distance(_roadPoints[0].position ,BMath.DoubleLineIntersect(_roadPoints[0],_roadPoints[_roadPoints.Count-1],
				                     _connectedRoads[i].RoadPoints[0], _connectedRoads[i].RoadPoints[_connectedRoads[i].RoadPoints.Count-1]));
			}
		}
	}
	public Vector3 TravelDirection(Location _nextLoc){ //tells you where this connects with the chosen points
		//this is called by the caravan when it's traveling
		if (!_isRoad)
						return transform.position; 
		return (GetDistanceOnRoad (_nextLoc) * _roadDirection) + _roadPoints[0].transform.position;  
	}
	public float GetDistanceOnRoad(Location _theLoc){ //you give this a location, it gives you how far away that location is from the start of the orad
		if (_theLoc == null) {
			return 0; 		
		}
		if(_theLoc.IsRoad){
			for(int i = 0; i < _connectedRoads.Count; i++){
				if(_theLoc.ID == _connectedRoads[i].ID){
					return _connectedRoadsDistance[i]; 
				}
			}
		}
		else{
			for(int i = 0; i < _connectedCities.Count; i++){
				if(_theLoc.ID == _connectedCities[i].ID){
					return _connectedCitiesDistance[i]; 
				}
			}
		}
		return -1; 
	}
	void MakeDistanceListProperLength(List<float> _floatList, List<Location> _locList){ //makes sure that your distance list is long enough
		if (_floatList.Count < _locList.Count) { // COuld probably condense this into something that checks and adds to list length. But I like
			_floatList.Add (0); 		//making my functions only do one thing...unless you are a patfhinding function. THen you do lots of things
			MakeDistanceListProperLength(_floatList,_locList); 
		}
	}
	public void CalcRoadLength(){ //gets the legnth of the road. Curently this is as crow flies from start to finish, should modify this
		if(_isRoad){
			_roadLength = Vector3.Distance (_roadPoints [0].position, _roadPoints [_roadPoints.Count - 1].position); 
		}
	}
	void CalcRoadDirection(){ // Gets the driection from start to finish, eventually this will need to be modified to look at each point
		if(_isRoad){
			_roadDirection = (_roadPoints[_roadPoints.Count-1].transform.position - _roadPoints[0].transform.position).normalized;
		}
	}
	void StartupCheck(){ //spit out ledgible errors for designers

	}
	*/


	// ACTIONS STUFF -----------------------------------------------------------------------------------

	void StartGame(){
		foreach (Actions _currentAction in _localActions) {
			_currentAction.StartGame(this); 

		}
	}

	void GetID(){
		_id = World.GetID (); 
	}
	void Awake(){
		GetID (); 
	}


}
