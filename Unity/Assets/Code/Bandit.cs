using UnityEngine;
using System.Collections;

public class Bandit : MonoBehaviour {

	/*The bandit is your main unit. They are simple and expendable. 
	Currently they don't have loyalty or hunger. Each 

	 */
	[SerializeField]
	int _health = 20; 
	[SerializeField]
	int _currentHealth = 20; 
	[SerializeField]
	int _level = 1; 
	int _XP = 0; 
	int _DR = 0; 
	[SerializeField]
	float _foodConsumption = 1;
	public float FoodConsumption { get { return _foodConsumption; } }
	bool _starving = false; 
	public bool Starving { get { return _starving; } set { _starving = value; } }

	Location _currentLocation; // Where the bandit currently is. This may be unnecissary as every location has a list of all bandits who are at it. 

	public void NewDay(){ //they spent the night wherevery they are at. 
		
	}
	public int DoDamage(){
		return _level + Random.Range(1,6); 
	}
	public void TakeDamage(int _damage){
		_currentHealth -= Mathf.Clamp (_damage - _DR, 0, 100000); 
	}
	void SpawnBandit(){
		Global.HideoutLoc.BanditArrives (this); 
		Global.TheHideout.AddBandit (this); 
	}
	void Start(){
		SpawnBandit (); 	
	}

}
