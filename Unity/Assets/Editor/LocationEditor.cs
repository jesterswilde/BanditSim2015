using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using UnityEditor; 

//this script modified the inspector view for the location component

[CustomEditor(typeof(Location))]
public class LocationEditor : Editor {

	[SerializeField]
	public List<bool> showActionsList = new List<bool>(); 

	bool showActions = true; 
	bool showRoad = true; 
	bool showRoadPoints = false; 
	bool showRoadConnections = false; 
	bool showRoadConnectionList = false; 
	public override void OnInspectorGUI ()
	{
		Location _locEdit = (Location)target; //casts the script so we can use it
		EnsureListSize (showActionsList, _locEdit.OptionsNum); 
		_locEdit.CheckSmallList (); //makes sure that the choiceList length is appropriate
		_locEdit.CheckLageList (); 
		_locEdit.CheckSmallList (_locEdit.RoadPoints, _locEdit.NumRoadPoints);
		_locEdit.CheckLageList (_locEdit.RoadPoints, _locEdit.NumRoadPoints); 

		_locEdit.DisplayName = EditorGUILayout.TextField("Location Name:" ,_locEdit.DisplayName);

		//ACTIONS ======================================================================================
		showActions = EditorGUILayout.Foldout (showActions, "Possible Actions"); 
		if(showActions){
			_locEdit.CheckSmallList(_locEdit.LocalActions,_locEdit.OptionsNum);
			_locEdit.CheckLageList (_locEdit.LocalActions,_locEdit.OptionsNum); 
			_locEdit.OptionsNum = EditorGUILayout.IntField ("Number Of Actions", _locEdit.OptionsNum); 
			for (int i = 0; i < _locEdit.OptionsNum; i++) {
				_locEdit.ChoiceList[i] = EditorGUILayout.Popup(_locEdit.ChoiceList[i],_locEdit.Actions,GUILayout.Width (150)); 
				DisplayActionOptions(i,_locEdit); 
			}
			EditorGUILayout.Separator(); 
		}
		_locEdit.DailyHealthMod = EditorGUILayout.IntField ("Damage/Healing:", _locEdit.DailyHealthMod);

		// ROAD POINTS ====================================================================================
		showRoad = EditorGUILayout.Foldout (showRoad, "Road"); 
		if(showRoad){ 
			EditorGUI.indentLevel ++; 
			_locEdit.IsRoad =  EditorGUILayout.Toggle("Is This A Road?" ,_locEdit.IsRoad);
			if (_locEdit.IsRoad) { //if this is a road
				if(_locEdit.RoadLength != 0){
					EditorGUILayout.LabelField ("Road Length: " + _locEdit.RoadLength.ToString()); 
				}
				if(GUILayout.Button("Calculate Road Distance",GUILayout.Width (200))){
					_locEdit.CalcRoadLength(); 
				}
				_locEdit.NumRoadPoints = EditorGUILayout.IntField("Number of Points", _locEdit.NumRoadPoints); 
				EditorGUI.indentLevel ++; 
				showRoadPoints = EditorGUILayout.Foldout(showRoadPoints,"The Points"); 
				if(showRoadPoints){
					for(int i = 0; i < _locEdit.NumRoadPoints; i++){
						_locEdit.RoadPoints[i] = EditorGUILayout.ObjectField(_locEdit.RoadPoints[i],typeof(UnityEngine.Transform),true) as Transform; 
					}
				} 
				EditorGUI.indentLevel -= 1; 
			}
			EditorGUILayout.LabelField(_locEdit.routeDistance.ToString());
			EditorGUI.indentLevel -= 1; 
		}

		//CONNECTED ROADS ================================================================================
		showRoadConnections = EditorGUILayout.Foldout (showRoadConnections, "Connected Roads");
		if (showRoadConnections) {
			_locEdit.CheckSmallList(_locEdit.ConnectedRoads, _locEdit.numConnectedRoads);
			_locEdit.CheckLageList (_locEdit.ConnectedRoads, _locEdit.numConnectedRoads); 
			EditorGUI.indentLevel ++;
			_locEdit.numConnectedRoads = EditorGUILayout.IntField("Number of Roads", _locEdit.numConnectedRoads); 
			showRoadConnectionList = EditorGUILayout.Foldout(showRoadConnectionList, "Road List");
			if(showRoadConnectionList){
				EditorGUI.indentLevel ++; 
				for(int i = 0; i < _locEdit.numConnectedRoads; i++){
					_locEdit.ConnectedRoads[i] = (Location)EditorGUILayout.ObjectField( _locEdit.ConnectedRoads[i],typeof(Location),true);
				}
				EditorGUI.indentLevel -=1; 
			}
			EditorGUI.indentLevel -=1; 
		}
	}


	void EnsureListSize(List<bool> _list, int _total){
		CheckSmallList (_list, _total); 
		CheckLargeList (_list, _total); 
	}
	public void CheckSmallList(List<bool> _list, int _total){
		if (_list.Count < _total) {
			_list.Add(true); 
			CheckSmallList(_list,_total); 
		}
	}
	public void CheckLargeList(List<bool> _list, int _total){
		if (_list.Count > _total) {
			_list.RemoveAt(_list.Count-1);
			CheckLargeList(_list, _total); 
		}
	}


	// This section may get bloated. I should consider putting these GUI elements into the actions themselves.

	void DisplayActionOptions(int i, Location _locEdit){
		if (_locEdit.Actions [_locEdit.ChoiceList [i]] == "Ambush") {  //Ambush ----------------------------------
			if(_locEdit.LocalActions[i] != null){
				if(!_locEdit.LocalActions[i].IsSame("Ambush")){
					_locEdit.LocalActions[i].Cleanup(); 
					GameObject _temGO = new GameObject(); 
					_locEdit.LocalActions[i] =_temGO.AddComponent<Ambush> (); 
					_locEdit.LocalActions[i].Startup(_locEdit); 
				}
			}
			else{
				GameObject _temGO = new GameObject(); 
				_locEdit.LocalActions[i] =_temGO.AddComponent<Ambush> ();
				_locEdit.LocalActions[i].Startup(_locEdit); 
			}
		}
		if (_locEdit.Actions [_locEdit.ChoiceList [i]] == "Scavenge") { //Scavenge ---------------------------------
			if(_locEdit.LocalActions[i] != null){
				if(!_locEdit.LocalActions[i].IsSame("Scavenge")){
					_locEdit.LocalActions[i].Cleanup(); 
					GameObject _temGO = new GameObject(); 
					_locEdit.LocalActions[i] =_temGO.AddComponent<Scavenge> ();
					_locEdit.LocalActions[i].Startup(_locEdit); 
				}
			}
			else{
				GameObject _temGO = new GameObject(); 
				_locEdit.LocalActions[i] =_temGO.AddComponent<Scavenge> ();
				_locEdit.LocalActions[i].Startup(_locEdit); 
			}
		}
	}

}
