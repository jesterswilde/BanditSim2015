using UnityEngine;
using System.Collections;

public class Actions : MonoBehaviour {

	[SerializeField]
	protected string _actionName;
	public string ActionName { get { return _actionName; } }
	protected Location _loc; 

	public bool IsSame(Actions _newAction){
		if (_newAction.ActionName != _actionName)
						return false; 
		return true; 
	}
	public bool IsSame(string _newAction){
		if (_newAction != _actionName)
			return false; 
		return true; 
	}
	public virtual void Startup(Location _theLoc){
		_loc = _theLoc; 
	}

	public virtual void NewDay(Bandit _bandit){
		
	}

	public virtual void NextHour(Bandit _bandit){
		
	}

	public virtual void StartGame(Location _theLoc){
		_loc = _theLoc; 
	}
	public virtual void Cleanup(){
		DestroyImmediate (this.gameObject); 
	}
}
