using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class World : MonoBehaviour {

	//The global class holds pointers to all Singletons, and universal lists.  (Except bandits they live at the hideout. Might move htem here. 


	public Hideout theHideout; 
	public static Hideout TheHideout; 
	public static Location HideoutLoc; 
	public static World world; 
	public static Camera mainCam; 
	public static Combat combat; 
	public Camera cam; 
	public Map map; 
	public static Map Map; 

	public Object theBanditPrefab;

	public Transform MapParent; 
	public Transform HideoutParent; 

	static int _id = 0 ; 
	//Currently the game works on a day by day basis. This may want to be changed to a finer level of granularity. Hours perhaps? Minutes seems too fine. 
	[SerializeField]
	float _secondsInDay = 10; 
	float _secondsInHour; 
	static int _hour = 1; 
	static int _day = 0; 

	public static int Hour { get { return _hour; } }
	public static int Day { get { return _day; } }
	float _timer = 0;

	static List<Location> _allLocals = new List<Location>(); 

	Controls _currentControls;
	MapControl _mapControls = new MapControl(); 
	HideoutControl _hideoutControls = new HideoutControl(); 

	
	//Time Stuff ----------------------------------------------------------
	void PassTime(){
		_timer += Time.fixedDeltaTime; 
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
			_loc.GetLocationsIDsAndLengths();
		}
		foreach (Location _loc in _allLocals) {
			_loc.AttachToRoads(); 	
		}
		foreach (Location _loc in _allLocals) {
			_loc.CalculatePositionsOnRoad(); 		
		}
	}



	//Caravan and Route stufff -----------------------------------------------------
	public static void ClearRoute(){
		foreach (Location _loc in _allLocals) {
			_loc.routeCalculated = false; 
			_loc.routeDistance = 100000000000; 
			_loc.PreviousLoc = null; 
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
	void FixedUpdate(){
		PassTime (); 
	}
	void Awake(){ 
		//get the static variables
		TheHideout = theHideout; 
		HideoutLoc = theHideout.gameObject.GetComponent<Location> (); 
		world = this; 
		Map = map;
		mainCam = cam; 
		combat = gameObject.GetComponent<Combat> (); 
		_hideoutControls.Startup ();
		_mapControls.Startup ();

		_secondsInHour = _secondsInDay / 24; 

		CollectLocations (); 
		LocationStartup (); 

	}
	void Start(){
		_currentControls = _mapControls; 
	}
	void Update(){
		_currentControls.CheckForClicks (); 
	}



}
