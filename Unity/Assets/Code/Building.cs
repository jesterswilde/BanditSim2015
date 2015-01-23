using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class Building : MonoBehaviour {

	[SerializeField]
	protected int _cost = 0; 
	public int Cost { get { return _cost; } }

	[SerializeField]
	protected int _left; 
	public int Left{ get { return _left; } }
	[SerializeField]
	protected int _right; 
	public int Right { get { return _right; } }
	[SerializeField]
	protected int _up; 
	public int Up { get { return _up; } }
	[SerializeField]
	protected int _down; 
	public int Down { get { return _down; } }
	[SerializeField]
	Vector2 _startingLocation; 

	[SerializeField]
	List<Vector2> _doorways = new List<Vector2> (); 
	public List<Vector2> Doorways { get { return _doorways; } }
	[SerializeField]
	List<Vector2> _buildableNodes = new List<Vector2>(); 
	public List<Vector2> BuildableNodes { get { return _buildableNodes; } }

	MeshRenderer[] _meshes; 


	public void IsStartingBuilding(List<List<BuildingUnit>> _theGrid){
		transform.position = _theGrid [(int)_startingLocation.y] [(int)_startingLocation.x].Position (); 
		Build (_theGrid, _theGrid[(int)_startingLocation.y][(int)_startingLocation.x]); 
	}
	public void Build(List<List<BuildingUnit>> _theGrid, BuildingUnit _theUnit){
		Debug.Log ("Building"); 
		OccupyUnits (_theGrid, _theUnit); 
		MakeDoorways (_theGrid, _theUnit); 
		ResetColor (); 
		World.TheHideout.AddBuilding (this); 
	}
	void ResetColor(){
		foreach (MeshRenderer _meshR in _meshes) {
			_meshR.material.color = new Color(1,1,1); 		
		}
	}
	void OccupyUnits(List<List<BuildingUnit>> _theGrid, BuildingUnit _theUnit){ //go through and tell the units they are currently occupied
		for (int r = _theUnit.Row-_left; r <= _theUnit.Row+ _right; r++) { // for the rows
			if(r < _theGrid.Count && r >= 0){ //because buildings can start in odd places, make sure there aren't errors
				for(int c = _theUnit.Column - _down; c <= _theUnit.Column +_up;c++){ //for the columns
					if(c < _theGrid.Count && c >= 0)
					_theGrid[r][c].MakeBuilding (this); 
				}
			}
		}
	}
	void MakeDoorways(List<List<BuildingUnit>> _theGrid, BuildingUnit _theUnit){
		foreach (Vector2 _doorLoc in _buildableNodes) {
			int _theR = (int)(_doorLoc.y+ _theUnit.Row);
			int _theC = (int)(_doorLoc.x + _theUnit.Column); 
			if(_theR < _theGrid.Count && _theR >= 0){
				if(_theC < _theGrid[_theR].Count && _theC >=0){
					_theGrid[_theR][_theC].IsDoorway = true; 
				}
			}
		}
	}
	public virtual void BuildingEffect(ref int banditCap, ref int foodCap, ref int moneyCap){
		
	}

	public void IsValidLocation(bool _valid){
		if (_valid) {
			Valid(); 	
		}
		else{
			Invalid(); 
		}
	}
	void Valid(){
		foreach (MeshRenderer _theMesh in _meshes) {
			_theMesh.material.color = new Color(.5f,.5f,1);
		}
	}
	void Invalid(){
		foreach (MeshRenderer _theMesh in _meshes) {
			_theMesh.material.color = new Color(1,.5f,.5f);
		}
	}

	void Awake(){
		_meshes = GetComponentsInChildren<MeshRenderer> (); 
	}






}
