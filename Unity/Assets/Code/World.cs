using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class World : MonoBehaviour {

	//The global class holds pointers to all Singletons, and universal lists.  (Except bandits they live at the hideout. Might move htem here. 
	/* This is a big document so we are laying out a table of ocntents
	 * 
	 * 		1 - Variable Decleration 
	 * 		2 - Time 
	 * 		3 - Location 
	 * 		4 - Controls
	 * 		5 - Caravan / Route
	 * 		6 - Bandit 
	 * 		7 - Book Keeping / Start & Update
	 * 
	 * */

	public Hideout theHideout; 
	public static Hideout TheHideout; 
	public static Location HideoutLoc; 
	public static World world; 
	public static Camera mainCam; 
	public static Combat combat; 
	public Camera cam; 
	public Map map; 
	public static Map Map; 
	public BuildingManager buildingManager; 
	public static BuildingManager bManager; 
	public PanelUI thePanelUI; 
	public static PanelUI PanelUI; 


	public Object theBanditPrefab;

	public Transform MapParent; 
	public Transform HideoutParent; 

	static int _id = 0 ; 
	//Currently the game works on a day by day basis. This may want to be changed to a finer level of granularity. Hours perhaps? Minutes seems too fine. 
	[SerializeField]
	float _secondsInDay = 10; 
	public float SecondsInDay { get { return _secondsInDay; } }
	float _secondsInHour; 
	static int _hour = 1; 
	static int _day = 0; 

	public static int Hour { get { return _hour; } }
	public static int Day { get { return _day; } }
	float _timer = 0;

	static List<Location> _allLocals = new List<Location>(); 
	LocNode[] _theNodes;
	CaravanRoute[] _allRoutes; 

	Controls _currentControls;
	MapControl _mapControls = new MapControl(); 
	HideoutControl _hideoutControls = new HideoutControl(); 
	bool _mapMode = false;
	public bool _inModeTransition = false;
	bool _canTransition = true; 
	public float roadWdith = .03f; 

	
	//Time Stuff ----------------------------------------------------------
	void PassTime(){
		_timer += Time.deltaTime; 
		if (_timer > _secondsInHour) {
			_timer -= _secondsInHour;
			_hour += 1; 
			NextHour(); 
		}
		if (_hour > 24) {
			_hour = 1; 
			_day +=1;
			NewDay(); 
		}
	}
	void NextHour(){
		foreach (CaravanRoute _theRoute in _allRoutes) {
			_theRoute.NextHour(); 
		}
		TheHideout.NextHour (); 
		foreach (Location _local in _allLocals) {
			_local.NextHour(); 
		}
	}
	void NewDay(){
		TheHideout.NewDay (); 
		foreach (Location _local in _allLocals) {
			_local.NewDay(); 
		}
	}


	//Location Stuff ----------------------------------------------------------------------------------------
	void CollectLocations(){ //gets every location
		Location[] _allTheLocations = FindObjectsOfType<Location>(); 
		foreach (Location _loc in _allTheLocations) {
			_allLocals.Add (_loc); 		
		}
	}
	void LocationStartup(){
		foreach (Location _loc in _allLocals) {
			_loc.GetIDs();
		}
	}
	void NodeStartup(){
		_theNodes = FindObjectsOfType<LocNode> (); 
		foreach (LocNode _node in _theNodes) {
			_node.DrawRoads(roadWdith); 
		}
	}
	void CaravanRouteStartup(){
		foreach (CaravanRoute _route in _allRoutes) {
			_route.Startups(); 
		}
	}

	//Controls Stuff ---------------------------------------------------------------------------

	void TransitionTimer(){
		_canTransition = true; 
	}
	public void TransitionControls(){
		if (_mapMode && !_inModeTransition && _canTransition) { //begin transition to hideout mode
			_mapMode = false;
			_inModeTransition = true;
			mainCam.transform.parent = null; 
			_currentControls.LeaveMode(); 
		}
		if (!_mapMode && _inModeTransition && _canTransition) { //transitioning to hideout mode
			mainCam.transform.position = Vector3.Lerp(mainCam.transform.position,HideoutParent.position, Time.deltaTime * 5); 
			mainCam.transform.rotation = Quaternion.Lerp (mainCam.transform.rotation, HideoutParent.rotation, Time.deltaTime * 10); 
			if( Vector2.Distance(mainCam.transform.position,HideoutParent.position) < .001f){ //camera transition complete
				mainCam.transform.position = HideoutParent.position; 
				mainCam.transform.rotation = HideoutParent.rotation;
				mainCam.transform.parent = HideoutParent; 
				_inModeTransition = false; 
				_currentControls = _hideoutControls; 
				_currentControls.EnterMode(); 
				_canTransition = false; 
				Invoke ("TransitionTimer",.5f); 
			}
		}
		if (!_mapMode && !_inModeTransition && _canTransition) {
			_mapMode = true;
			_inModeTransition = true; 
			mainCam.transform.parent = null; 
			_currentControls.LeaveMode(); 
		}
		if (_mapMode && _inModeTransition && _canTransition) { //transitioning to Map mode
			mainCam.transform.position = Vector3.Lerp(mainCam.transform.position,MapParent.position, Time.deltaTime * 5); 
			mainCam.transform.rotation = Quaternion.Lerp (mainCam.transform.rotation, MapParent.rotation, Time.deltaTime*10); 
			if(Vector2.Distance(mainCam.transform.position,MapParent.position) < .001f){ //camera transition complete
				mainCam.transform.position = MapParent.position; 
				mainCam.transform.rotation = MapParent.rotation; 
				mainCam.transform.parent = MapParent; 
				_inModeTransition = false; 
				_currentControls = _mapControls;
				_currentControls.EnterMode(); 
				_canTransition = false; 
				Invoke ("TransitionTimer",.5f); 
			}
		}
	}


	//Caravan and Route stufff -----------------------------------------------------
	public static void ClearRoute(){
		foreach (LocNode _loc in world._theNodes ) {
			_loc.AlreadyCalculated = false; 
			_loc.RouteDistance = 100000000000;
		}
	}



	//bandit stuff-------------------------------------------------
	public static void MakeBandit(){
		Instantiate (World.world.theBanditPrefab);
	}


	//book keepig and start up stuff ------------------------------------------------------------
	public static int GetID(){
		_id ++;
		return _id; 
	}
	void Awake(){ 
		//get the static variables
		TheHideout = theHideout; 
		HideoutLoc = theHideout.gameObject.GetComponent<Location> (); 
		world = this; 
		Map = map;
		mainCam = cam; 
		bManager = buildingManager; 
		combat = gameObject.GetComponent<Combat> (); 
		_hideoutControls.Startup ();
		_mapControls.Startup ();
		PanelUI = thePanelUI; 
		_allRoutes = FindObjectsOfType<CaravanRoute> (); 

		_secondsInHour = _secondsInDay / 24; 

		CollectLocations (); 
		LocationStartup (); 
		NodeStartup (); 


	}
	void Start(){
		_currentControls = _hideoutControls; 
		CaravanRouteStartup (); 
	}
	void Update(){
		PassTime (); 
		_currentControls.CheckForClicks (); 
		_currentControls.Hover (); 
		_currentControls.Movement (); 
		if(Input.GetKeyDown(KeyCode.M)){
			TransitionControls(); 
		}
		if (_inModeTransition) {
			TransitionControls(); 
		}
	}



}

/* You exercised for the first time in a while today. (1/13/2015). The feelings of regret were getting unbearable. Hopefully you've found some respite from them.
 * If you are having trouble, you should try writing notes down on what you want to do each morning, on the nights prior. It is working so far.
 * 
 * You had grand ideas on how to push this module (module 2) for the FX TD class to the next level. YOu are poor and don't have the software but decided
 * to take the concpets and apply them with lower end tools as well as to FX. You major failing is that you need to package them. Make them (and yourself)
 * sellable. CUrrently nobody is buying. Don't let the small library of stuff to show stop you from putting it out there. It's better than nothing. 
 * 
 * Your curry tasted good. 
 */
