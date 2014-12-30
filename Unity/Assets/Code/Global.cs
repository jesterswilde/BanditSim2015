using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class Global : MonoBehaviour {

	public Hideout theHideout; 
	public static Hideout TheHideout; 
	public static Location HideoutLoc; 
	public static Global global; 
	[SerializeField]
	int _secondsInDay = 10; 
	static int _day = 0; 
	public static int Day { get { return _day; } }
	float _timer = 0;

	List<Location> _allLocals = new List<Location>(); 

	public void AddLocal(Location _newLocal){
		_allLocals.Add (_newLocal); 
	}
	
	void Awake(){ 
		//get the static variables
		TheHideout = theHideout; 
		HideoutLoc = theHideout.gameObject.GetComponent<Location> (); 
		global = this; 
	}
	void PassTime(){
		_timer += Time.fixedDeltaTime; 
		if (_timer > _secondsInDay) {
			_timer -= _secondsInDay;
			_day +=1; 
			NewDay(); 
			Debug.Log(_day); 
		}
	}
	void NewDay(){
		TheHideout.NewDay (); 
		foreach (Location _local in _allLocals) {
			_local.NewDay(); 
		}
	}

	void FixedUpdate(){
		PassTime (); 
	}
}
