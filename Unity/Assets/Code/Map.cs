using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class Map : MonoBehaviour {

	public Material roadMaterial; 
	public Material selectedRoadMaterial; 

	Location _selLocation; 
	public Location SelLocation { get {return _selLocation;}}
	Bandit _selBandit; 
	bool _draggingBandit; 
	PanelUI _mapUI; 

	public void SelectLocation(Location _theLoc){
		_selLocation = _theLoc; 
		World.PanelUI.SelectLocation (_selLocation); 
	}
	public void SelectBandit (Bandit _theBandit){
		_selBandit = _theBandit; 
	}
	public void ClearSelection(){
		_selLocation = null; 
	}
	public void UpdateMapUI(){

	}

	void Awake(){
		_mapUI = gameObject.GetComponent<PanelUI> (); 
	}
}
