using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class RoadClicked : MonoBehaviour {

	LocNode _startLocNode; 
	LocNode _endLocNode; 

	List<CaravanRoute> _passingRoutes = new List<CaravanRoute> (); 
	public List<CaravanRoute> PassingRoutes { get { return _passingRoutes; } }
	List<int> _arrivalTimes = new List<int>(); 

	public void Startup(LocNode _theStartLoc, LocNode _theEndLoc){
		_startLocNode = _theStartLoc;
		_endLocNode = _theEndLoc; 
	}

	public void Clicked(Vector3 _clickPos){
		_passingRoutes.Clear (); 
		CompileRouteList (_startLocNode, _endLocNode);
		CompileRouteList (_endLocNode, _startLocNode); 
	}

	public int ArrivalTime(CaravanRoute _theRoute, Vector3 _clickPos){
		LocNode _theNode = _theRoute.FirstLocation (_startLocNode, _endLocNode) ;
		return (int)(_theRoute.ArrivalTime (_theNode) + Vector3.Distance(_theNode.transform.position, _clickPos)/ _theRoute.Speed); 
	}



	void CompileRouteList(LocNode _routeLoc, LocNode _checkingLoc){
		bool _alreadyOnList = false;
		foreach (CaravanRoute _theRoute in _routeLoc.PassingRoutes) { //make sure the route passes through this road (and not just one location on it
			if(_theRoute.ContainsNode(_checkingLoc)){
				foreach(CaravanRoute _newRoute in _passingRoutes){
					if(_newRoute.ID == _theRoute.ID) _alreadyOnList = true; //make sure you aren't adding the list twice
				}
				if(!_alreadyOnList && World.TheHideout.AlreadyKnowAboutRoute(_theRoute)){
					_passingRoutes.Add(_theRoute); 
				}
			}
		}
	}
}
