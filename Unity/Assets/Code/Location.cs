using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class Location : MonoBehaviour {

	/* Locations are mainly accesed through the map. The map representation and the actual place are both counted in this component
	 * This is the parent class. Locations will be things like roads, towns, as well as the hideout. 
	 * */

	[SerializeField]
	string _displayName; //what hte player sees for this location
	public string DisplayName{get {return _displayName;}}

	protected List<Bandit> _localBandits = new List<Bandit>(); 

	public virtual void BanditArrives(Bandit _theBandit){ //just deals with bandits showing up and leaving
		_localBandits.Add (_theBandit); 
	}
	public virtual void BanditLeaves(Bandit _theBandit){
		_localBandits.Remove (_theBandit); 
	}


	//effects of being at the location
	public virtual void NewDay(){
		foreach (Bandit _bandit in _localBandits) {
			EffectOnBandit(_bandit);
			BanditGathered(_bandit); 
		}
	}
	protected void EffectOnBandit(Bandit _bandit){ //what happens to the bandit when they stay here (hurts them, loyals loyalty, they enjoy it, etc.)
		
	}
	protected void BanditGathered(Bandit _bandit){ //the benefit of the bandit being here. Eventually I want to have this be a cumulative thing. Multiple bandits
		//working together, or maybe you have a trait that helps. As opposed to now where every bandit just does their own thing. 

	}

	//combat will go here eventually 
}
