using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class Combat : MonoBehaviour {

	List<Bandit> _theBandits = new List<Bandit> (); 
	List<CaravandGuard> _theGuards = new List<CaravandGuard> (); 

	public void StartCombat (Location _ambushLoc, Caravan _caravan){
		Debug.Log ("Battle"); 
		_theBandits = _ambushLoc.LocalBandits;
		_theGuards = _caravan.ActiveGuards; 
		Debug.Log (_theBandits.Count + " : " + _theGuards.Count); 
		TheMelee (_caravan, 100); 
	}
	void TheMelee(Caravan _theCaravan, int _iterator){
		if(_iterator > 0){
			foreach (Bandit _bandit in _theBandits) //the bandits attack
							_bandit.Attack (_theGuards);
			foreach (CaravandGuard _guard in _theGuards) //the guards attack
							_guard.Attack (_theBandits); 
			if (CombatStillHappening ())//rinse repeat and lather. 
							TheMelee (_theCaravan, _iterator -1);  
			EndCombat (_theCaravan); 
		}
		else{
			Debug.Log ("COmbat Broke");
		}
	}
	void EndCombat(Caravan _theCaravan){
		if (_theBandits.Count == 0)
						_theCaravan.CaravanVictorious ();
				else
						_theCaravan.CaravanDefeated (); 

		_theBandits = new List<Bandit> ();
		_theGuards = new List<CaravandGuard> (); 
	}
	bool CombatStillHappening(){
		if (_theGuards.Count > 0 && _theBandits.Count > 0)
						return true;
		return false; 
	}
}
