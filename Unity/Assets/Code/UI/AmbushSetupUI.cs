using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine.UI; 

public class AmbushSetupUI : MonoBehaviour {


	RoadClicked _road; 
	Vector3 _clickPos; 
	public RectTransform listParent; 

	List<GameObject> _routeButtons = new List<GameObject>(); 

	public void Startup(RoadClicked _theRoad, Vector3 _theClickPos){
		_clickPos = _theClickPos; 
		_road = _theRoad; 
		DrawButtons (); 
	}

	public void DrawButtons(){
		ClearList (); 
		_road.Clicked (_clickPos); 
		foreach (CaravanRoute _route in _road.PassingRoutes) {
			_routeButtons.Add(MakeCaravanButton(_route)); 
			Debug.Log("ambush list button"); 
		}
	}
	GameObject MakeCaravanButton(CaravanRoute _route){
		GameObject _buttonParentGO = Instantiate (World.PanelUI.listButton) as GameObject;
		_buttonParentGO.transform.SetParent (listParent); 
		Transform _buttonGO = _buttonParentGO.transform.FindChild ("Button");
		Transform _textGO = _buttonGO.transform.FindChild ("Text"); 

		_textGO.GetComponent<Text> ().text = _route.name + " | ETA: " + (_road.ArrivalTime (_route, _clickPos) / World.world.SecondsInDay).ToString();
		_buttonGO.GetComponent<Button> ().onClick.AddListener (() => SelectRoute (_route)); 

		return _buttonParentGO; 
	}
	void ClearList(){
		foreach (GameObject _go in _routeButtons) {
			Destroy (_go); 
		}
		_routeButtons.Clear (); 
	}
	public void SelectRoute(CaravanRoute _route){
		
	}



}
