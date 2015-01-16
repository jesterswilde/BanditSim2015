using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class Map : MonoBehaviour {

	public Material roadMaterial; 

	Location _selLocation; 
	public Location SelLocation { get {return _selLocation;}}
	Bandit _selBandit; 
	bool _draggingBandit; 
	PanelUI _mapUI; 

	public void SelectLocation(Location _theLoc){
		_selLocation = _theLoc; 
		_mapUI.UpdateBanditButtons (); 
	}
	public void SelectBandit (Bandit _theBandit){
		_selBandit = _theBandit; 
	}
	public void ClearSelection(){
		_selLocation = null; 
	}
	public void UpdateMapUI(){
		/*
		_mapUI.UpdateBanditButtons (); 
		_mapUI.UpdateInfoPanel (); 
		*/
	}

	void Awake(){
		_mapUI = gameObject.GetComponent<PanelUI> (); 
	}
}
