using UnityEngine;
using System.Collections;

public class BuildingUnit : MonoBehaviour {

	int _row; 
	public int Row { get { return _row; } }
	int _column; 
	public int Column { get { return _column; } }
	bool _isOccupied = false; 
	Building _building; 
	BuildingManager _bManager; 
	[SerializeField]
	bool _isDoorway; 
	public bool IsDoorway { get { return _isDoorway; } set { _isDoorway = value; } }

	public bool IsOccupied {get {return _isOccupied;}}
	public void Startup(int _theRow, int _theColumn, BuildingManager _theManager){
		_row = _theRow; 
		_column = _theColumn; 
		_bManager = _theManager; 
	}

	public bool CheckArea(int _up, int _down, int _left, int _right){
		bool _obstructed = false; 
		for (int r = _row - _down; r <= _row + _up; r++) {
			for(int c = _column - _left; c <= _column + _right; c++){
				
			}
		}
		return _obstructed; 
	}

	public void MakeBuilding(Building _theBuilding){
		_building = _theBuilding; 
		_isOccupied = true; 
	}
	public Vector3 Position(){
		return new Vector3(this.transform.position.x, 0, this.transform.position.z); 
	}
}
