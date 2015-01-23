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
	List<float> _locDistance = new List<float> (); 
	public List<float> LocDistance { get { return _locDistance; } }

	[SerializeField]
	List<CaravanRoute> _passingRoutes = new List<CaravanRoute>(); 
	

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

	public void DrawRoads(){ //makes the visual representatino of the road
		foreach (LocNode _node in _connectedLocations) {
			if(_id > _node.ID){
				GameObject _renderGO = new GameObject(); 
				_renderGO.transform.position = transform.position; 
				_renderGO.transform.parent = this.transform; 
				_renderGO.name = "DrawRoad"; 
				LineRenderer _line = _renderGO.AddComponent<LineRenderer>(); 
				_line.SetVertexCount(2); 
				_line.SetPosition(0, _renderGO.transform.position); 
				_line.SetPosition(1, _node.transform.position); 
				_line.SetWidth(.01f,.01f); 
				_line.material = World.Map.roadMaterial; 
			}
		}
	}

	public void KnowAboutRoute(CaravanRoute _route){
		_passingRoutes.Add (_route); 
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
