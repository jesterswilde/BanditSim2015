using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hideout : MonoBehaviour {

	List<Bandit> _allBandits = new List<Bandit>(); 
	[SerializeField] //Money is used to buy things like new buildings. It also functions as your score. As we get further into the game it will do more
	int _money = 100; 
	public int Money { get { return _money; } set { _money = value; } }
	[SerializeField] //Food is needed to feed your peeps. More peeps, the faster food depletes. We will display this to the player in how long their food supplies will last
	int _totalFood = 100; 
	public int Food{ get { return _totalFood; } set { _totalFood = value; } }
	public int DaysOfFood { get { return CalcDaysOfFood(); } }
	bool _hasFood; 

	List<Building> _builtRooms = new List<Building> (); 
	[SerializeField]
	int _starBanditCap = 5; 
	[SerializeField]
	int _currentBanditCap =5; 

	int CalcDaysOfFood(){
		float _dailyFood = 0; 
		foreach (Bandit _theBandit in _allBandits) {
			_dailyFood += _theBandit.FoodConsumption; 
		}
		if(_dailyFood != 0){
			return _totalFood /(int)_dailyFood; 
		}
		return 9999; 
	}
	public void AddBandit(Bandit _newBandit){
		_allBandits.Add (_newBandit); 
		SetDirty (); 
	}
	public void RemoveBandit(Bandit _deadBandit){
		_allBandits.Remove (_deadBandit); 
		SetDirty ();
	}
	void SetDirty(){ //When set dirty, everything will recalculate, such as howlong food will last. 
		//gets how many days of food you have
		if (_totalFood <= 0) {
			_hasFood = false; 		
		}
		else{
			_hasFood = true; 
		}
		World.Map.UpdateMapUI ();
	}
	public void NewDay(){
		FeedBandits (); 
	}
	public void NextHour(){
		
	}
	void FeedBandits(){
		float _foodConsumed = 0 ; 
		foreach (Bandit _theBandit in _allBandits) { //tally up how much the bandits will eat
			_foodConsumed += _theBandit.FoodConsumption; 		
		}
		_totalFood -= (int)_foodConsumed; 
		if(_totalFood <= 0){ //if they eat all your food
			_hasFood = false; //you are out of food, and they will starve
			_totalFood = 0; 
		}
		else {
			_hasFood = true; //if you have food, they are fine
		}
		if (_hasFood) { //they arenot starving
			foreach(Bandit _theBandit in _allBandits){
				_theBandit.Starving = false; 
			}
		}
		else{ //they are starving
			foreach(Bandit _theBandit in _allBandits){
				_theBandit.Starving = true; 
			}
		}
		SetDirty (); 
	}
}
