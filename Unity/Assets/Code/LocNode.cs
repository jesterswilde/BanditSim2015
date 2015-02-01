
using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class LocNode : MonoBehaviour {

	[SerializeField]
	int _id; 
	public int ID { get { return _id; } }
	[SerializeField]
	float _routeDistance;
	public float RouteDistance { get { return _routeDistance; } set { _routeDistance = value; } }
	bool _alreadyCalculated = false;
	public bool AlreadyCalculated { get { return _alreadyCalculated; } set { _alreadyCalculated = value; } }

	Location _loc; 
	
	List<LocNode> _connectedLocations = new List<LocNode> ();
	public List<LocNode> ConnectedNodes { get { return _connectedLocations; } }
	List<LineRenderer> _drawnRoads = new List<LineRenderer> (); 
	List<float> _locDistance = new List<float> (); 
	public List<float> LocDistance { get { return _locDistance; } }
	

	

	public void Startup(Location _theLoc){
		_loc = _theLoc; 
		GetID (); 
	}

	public void ConnectLocNodes( LocNode _theLoc){ //adds nodes to the list of connected nodes, and gets their distance
		bool _alreadyAdded = false;
		foreach (LocNode _loc in _connectedLocations) { //don't add them if they are already on the list
			if(_theLoc.ID == _loc.ID){
				_alreadyAdded = true;
			}
		}
		if(!_alreadyAdded){
			_connectedLocations.Add (_theLoc); 
			_locDistance.Add (Vector3.Distance(this.transform.position, _theLoc.transform.position)); 
			_theLoc.ConnectLocNodes(this); 
		}
	}

	public void DrawRoads(float _worldWidth){ //makes the visual representatino of the road
		foreach (LocNode _node in _connectedLocations) {
			if(_id > _node.ID){ //to only draw the road once, only the one with the highest ID# draws it.
				float _theWidth;
				if(_loc.CustomWidth) _theWidth = _loc.RoadWidth; //taking custom width
				else _theWidth = _worldWidth;  //taking global width
				GameObject _renderGO = new GameObject(); //line renderer game object
				_renderGO.transform.position = transform.position;
				_renderGO.transform.parent = this.transform; 
				_renderGO.name = "DrawRoad"; 
				_renderGO.transform.LookAt (this.transform.position); 
				LineRenderer _line = _renderGO.AddComponent<LineRenderer>();  //start adding the line renderer
				_drawnRoads.Add(_line); 
				_line.SetVertexCount(2); 
				_line.SetPosition(0, _renderGO.transform.position); 
				_line.SetPosition(1, _node.transform.position); 
				Debug.Log(_theWidth); 
				_line.SetWidth(_theWidth,_theWidth); 
				_line.material = World.Map.roadMaterial; 
				GameObject _colliderGo = new GameObject();  //collider section ------
				float _distance = Vector3.Distance (_node.transform.position, this.transform.position); 
				_colliderGo.transform.parent = transform; 
				_colliderGo.transform.position  = transform.position ; 
				_colliderGo.name = "ClickBox"; 
				BoxCollider _collider = _colliderGo.AddComponent<BoxCollider>(); 
				_collider.size = new Vector3(_theWidth, _theWidth,_distance); 
				_colliderGo.transform.LookAt(_node.transform.position); 
				_collider.center = new Vector3(0,0,_distance/2); 
				_colliderGo.layer = 12; 
			}
			else{
				_drawnRoads.Add(null);  //this is just so the lists line up properly
			}
		}
	}

	public void KnowAboutRoute(CaravanRoute _route){
		if (_loc != null) {
			_loc.AddCaravanRoute(_route); 		
		}
	}
	public void HighlightRoad(LocNode _theNode, bool _turnOn){ //used to highligh the section of the road
		int i = WhichRoad (_theNode); 
		if (i >= 0) { //if you could find the road
			if(_turnOn){
				if(_drawnRoads[i] != null){
					_drawnRoads[i].material = World.Map.selectedRoadMaterial; 
				}
			}
			else{
				if(_drawnRoads[i] != null){
					_drawnRoads[i].material = World.Map.roadMaterial; 
				}
			}
		}
	}
	int WhichRoad(LocNode _theNode){
		for (int i = 0; i < _connectedLocations.Count; i++) {
			if(_theNode.ID == _connectedLocations[i].ID){
				return i; 
			}
		}
		return -1; 
	}
	public bool IsLocation(int _theID){
		if (_loc.ID == _theID)
						return true; 
		return false; 
	}

	public void GetID(){
		_id = World.GetID (); 
	}


}
