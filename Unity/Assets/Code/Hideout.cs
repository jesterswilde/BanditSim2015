using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hideout : MonoBehaviour {

	List<Bandit> _allBandits = new List<Bandit>(); 
	public List<Bandit> AllBandits { get { return _allBandits; } }

	List<Items> _allItems = new List<Items>(); //every item that you or your bandits control 
	List<Items> _storedItems = new List<Items>(); //all items that are not equipped

	List<Building> _builtBuildings = new List<Building>(); 

	[SerializeField] //Money is used to buy things like new buildings. It also functions as your score. As we get further into the game it will do more
	int _money = 100; 
	public int Money { get { return _money; } set { _money = value; } }
	[SerializeField] //Food is needed to feed your peeps. More peeps, the faster food depletes. We will display this to the player in how long their food supplies will last
	int _totalFood = 100; 
	public int Food{ get { return _totalFood; } set { _totalFood = value; } }
	public int DaysOfFood { get { return CalcDaysOfFood(); } }
	bool _hasFood; 


	[SerializeField]
	int _banditCap = 0;
	int _foodCap = 0; 
	int _moneyCap = 0; 
	[SerializeField]
	float _banditChance = 1; 

	[SerializeField]
	List<CaravanRoute> _knownRoutes = new List<CaravanRoute> (); 
	public List<CaravanRoute> KnownRoutes { get { return _knownRoutes; } }



	//ADDING BANDITS ----------------------------------------------------------------------------------------
	public bool HireBandit(Bandit _newBandit){
		if(_allBandits.Count < _banditCap){
			_allBandits.Add (_newBandit); 
			SetDirty (); 
			return true; 
		}
		return false;
	}
	public void RemoveBandit(Bandit _deadBandit){
		_allBandits.Remove (_deadBandit); 
		SetDirty ();
	}
	void NewBanditsArrive(){ //chance of getting a new bandit every day
		if (Random.Range (0f, 100f) < _banditChance) {
			World.MakeBandit(); 
		}
	}



	//ITEMS ------------------------------------------------------------------------------------------------
	public void AddItem(Items _theItem){
		_allItems.Add (_theItem); 
		_storedItems.Add (_theItem); 
	}
	public void RemoveItem(Items _theItem){
		_allItems.Remove (_theItem); 
	}



	//BUILDINGS -----------------------------------------------------------------------------------------------
	void ResetToStartingValues(){
		_banditCap = 0; 
	}
	public void RefreshBuildingStats(){
		ResetToStartingValues (); 
		foreach (Building _buildling in _builtBuildings) {
			_buildling.BuildingEffect(ref _banditCap,ref _foodCap,ref _moneyCap) ;
		}
	}
	public void AddBuilding(Building _theBuilding){
		_builtBuildings.Add(_theBuilding); 
		RefreshBuildingStats (); 
	}


	//Time --------------------------------------------------------------------------------------
	public void NewDay(){
		FeedBandits (); 
		NewBanditsArrive (); 
		List<Bandit> _tempBanditList = new List<Bandit> (); 
		foreach (Bandit _bandit in _allBandits) {
			_tempBanditList.Add(_bandit); 
		}
		foreach (Bandit _bandit in _tempBanditList) {
			_bandit.NewDay(); 
		}
		SetDirty (); 
	}	
	public void NextHour(){
		foreach (Bandit _bandit in _allBandits) {
			_bandit.NextHour(); 
		}
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
	}
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


	//BANIDTS BRINGING BACK THE GOODS ====================================================================================
	public void LearAboutRoute(CaravanRoute _theRoute){
		if (!AlreadyKnowAboutRoute(_theRoute)) {
			Debug.Log("The boss was happy with this new route"); 
			_knownRoutes.Add(_theRoute); 
		}
		else{
			Debug.Log("THe boss was displeased"); 
		}
		SetDirty (); 
	}
	public bool AlreadyKnowAboutRoute(CaravanRoute _theRoute){
		foreach (CaravanRoute _route in _knownRoutes) {
			if (_route.ID == _theRoute.ID){
				return true; 
			}
		}
		return false; 
	}
	public void AddFood(int _foodHaul){
		_totalFood += _foodHaul; 
		SetDirty (); 
	}
	void CapGoods(){
		if (_totalFood > _foodCap) {
			_totalFood = _foodCap;
		}
		if (_money > _moneyCap) {
			_money = _moneyCap; 	
		}
	}


	//BOOK KEEPING ----------------------------------------------------------------------------------------------------------
	void SetDirty(){ //When set dirty, everything will recalculate, such as howlong food will last. 
		//gets how many days of food you have
		CalcDaysOfFood (); 
		if (_totalFood <= 0) {
			_hasFood = false; 		
		}
		else{
			_hasFood = true; 
		}
		World.PanelUI.RefreshUI (); 
	}
}
