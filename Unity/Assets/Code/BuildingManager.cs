using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour {


	public int rows; 
	public int columns;
	public float unitSize; 
	public Transform startPoint; 

	public Object MapRoomPrefab;
	public Object ArmoryPrefab; 
	public Object BarracksPrefab;
	public Object StoragePrefab; 

	List<List<BuildingUnit>> _theGrid = new List<List<BuildingUnit>>(); 

	public LayerMask buildingUnitLayer; 

	Building _selectedBuildin;
	public Building SelectedBuilding { get { return _selectedBuildin; } }
	BuildingUnit _selectedUnit; 
	bool canBuild = true; 

	void MakeGrid(){
		float xOffset = columns * unitSize * .5f * -1 +startPoint.transform.position.x; 
		float zOffset = rows * unitSize *.5f * -1 + startPoint.transform.position.z; 
		for(int r = 0; r < rows; r++){
			List<BuildingUnit> _rowList = new List<BuildingUnit>(); 
			for(int c = 0 ; c < columns; c++){
				GameObject unitGO = new GameObject(); 
				unitGO.transform.position = new Vector3(xOffset+c*unitSize,unitSize*.5f*-1,zOffset+r*unitSize); //position the box
				BoxCollider _box = unitGO.AddComponent<BoxCollider>();  //give it a box collider
				unitGO.layer = 10;  //make it buildlingUnitLayer
				BuildingUnit _bu = unitGO.AddComponent<BuildingUnit>(); 
				_bu.Startup (r,c,this); //give it the starting details; 
				unitGO.name = "BuildingUnit-"+r.ToString()+"-"+c.ToString(); //name it something unique
				unitGO.transform.localScale = new Vector3(unitSize,unitSize,unitSize); 
				unitGO.transform.parent = transform; //make it a child of this object
				_rowList.Add(_bu); 
			}
			_theGrid.Add (_rowList); 
		}
	}
	bool CanBuildHere(int _theR, int _theC){ //goes through all nearby units and checks to see if they are occpied. 
		for (int r = _theR-_selectedBuildin.Left; r <= _theR+ _selectedBuildin.Right; r++) {
			for(int c = _theC -_selectedBuildin.Down; c <= _theC +_selectedBuildin.Up;c++){
				if(_theGrid.Count > r && r >= 0){ //these are checks to make sure you aren't "Out of bounds" 
					if(_theGrid[r].Count > c && c >= 0){
						if(_theGrid[r][c].IsOccupied) return false; 
					}
					else return false; 
				}
				else return false; 
			}
		}
		bool _hasDoor = false; 
		foreach (Vector2 _doorSpot in _selectedBuildin.Doorways) { //now check to make sure doorways line up
			int r = (int)(_theR+_doorSpot.y);
			int c = (int)(_theC+_doorSpot.x);
			if(_theGrid.Count > r && r >= 0){ //these are checks to make sure you aren't "Out of bounds" 
				if(_theGrid[r].Count > c && c >= 0){
					if(_theGrid[r][c].IsDoorway){
						_hasDoor = true; 
					}
				}
			}
		}
		return _hasDoor; 
	}

	public void Click(){
		if (_selectedBuildin != null) {
			Ray _ray = World.mainCam.ScreenPointToRay (Input.mousePosition);
			RaycastHit _hit; 
			if(Physics.Raycast(_ray,out _hit, 100,buildingUnitLayer)){
				Debug.Log("Started Building"); 
				BuildingUnit _theUnit = _hit.collider.gameObject.GetComponent<BuildingUnit>(); 
				_selectedBuildin.transform.position = _theUnit.Position(); 
				if(CanBuildHere(_theUnit.Row,_theUnit.Column)){
					Debug.Log("valid location"); 
					_selectedBuildin.Build(_theGrid, _theUnit); 
					_selectedBuildin = null; 
				}
			}
		}
	}
	public void Hover(){
		if (_selectedBuildin != null) {
			Ray _ray = World.mainCam.ScreenPointToRay (Input.mousePosition);
			RaycastHit _hit; 
			if(Physics.Raycast(_ray,out _hit, 100,buildingUnitLayer)){
				BuildingUnit _theUnit = _hit.collider.gameObject.GetComponent<BuildingUnit>(); 
				_selectedBuildin.transform.position = _theUnit.Position(); 
				_selectedBuildin.IsValidLocation (CanBuildHere(_theUnit.Row,_theUnit.Column)); 
			}
		}
	}
	public void SelectBuilding(Building _building){
		_selectedBuildin = _building; 
	}
	void PlaceStartingBuildings(){
		Building[] _allBuildings = FindObjectsOfType<Building> ();
		Debug.Log (_allBuildings.Length); 
		foreach(Building _building in _allBuildings){
			_building.IsStartingBuilding(_theGrid); 
		}
	}
	void Start(){
		Debug.Log ("Stuff"); 
		MakeGrid (); 
		PlaceStartingBuildings (); 
	}
	void Update(){
		if (Input.GetKeyDown (KeyCode.Keypad1)) {
			if(_selectedBuildin != null){
				Destroy (_selectedBuildin.gameObject); 
			}
			GameObject _go =  Instantiate (MapRoomPrefab) as GameObject;
			_selectedBuildin = _go.GetComponent<Building> (); 
		}
		if (Input.GetKeyDown (KeyCode.Keypad2)) {
			if(_selectedBuildin != null){
				Destroy (_selectedBuildin.gameObject); 
			}
			GameObject _go =  Instantiate (ArmoryPrefab) as GameObject;
			_selectedBuildin = _go.GetComponent<Building> (); 
		}
		if (Input.GetKeyDown (KeyCode.Keypad3)) {
			if(_selectedBuildin != null){
				Destroy (_selectedBuildin.gameObject); 
			}
			GameObject _go =  Instantiate (BarracksPrefab) as GameObject;
			_selectedBuildin = _go.GetComponent<Building> (); 
		}
		if (Input.GetKeyDown (KeyCode.Keypad4)) {
			if(_selectedBuildin != null){
				Destroy (_selectedBuildin.gameObject); 
			}
			GameObject _go =  Instantiate (StoragePrefab) as GameObject;
			_selectedBuildin = _go.GetComponent<Building> (); 
		}
	}

}
