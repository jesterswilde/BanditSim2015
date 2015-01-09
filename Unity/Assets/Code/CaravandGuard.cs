using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class CaravandGuard : MonoBehaviour {

	Caravan _caravan;
	Bandit _targetBandit; 

	[SerializeField]
	int _HP;
	public int HP { get { return _HP; } set { _HP = value; } }
	[SerializeField]
	int _currentHP; 
	public int CurrentHP { get { return _currentHP; } set { _currentHP = value; } }
	[SerializeField]
	int _DR; 
	public int DR { get { return _DR; } set { _DR = value; } }
	[SerializeField]
	int _level; 
	public int Level { get { return _level; } set { _level = value; } }
	[SerializeField]
	int _XP; 
	public int XP { get { return _XP; } set { _XP = value; } }
	[SerializeField]
	int _minDamage;
	public int MinDamage {get {return _minDamage; } set { _minDamage = value; } }
	[SerializeField]
	int _maxDamage; 
	public int MaxDamage {get {return _maxDamage; } set { _maxDamage = value; } }
	int _id; 
	string _name; 
	bool _isAlive = true; 
	public bool IsAlive { get { return _isAlive; } }

	public void Startup(Caravan _theCaravan){
		_caravan = _theCaravan;
	}
	public int DoDamage(){
		return Random.Range(_minDamage, _maxDamage) + _level; 
	}
	public void TakeDamage(int _damageTaken){
		_currentHP -= Mathf.Clamp (_damageTaken - _DR, 0, 10000); 
		Debug.Log(_name +" took " + Mathf.Clamp (_damageTaken - _DR, 0, 100000)  + " to the face, has "+ _currentHP +" remaining");
		CheckHealth(); 
	}
	public void AcquireTarget(List<Bandit> _bandits){
		if (_bandits.Count > 0) {
			_targetBandit = _bandits[Random.Range(0,_bandits.Count)];
		}
	}
	public void Attack(List<Bandit> _bandits){
		if(_isAlive){
			if (_targetBandit == null || !_targetBandit.IsAlive) //if you don't have a target, get one
							AcquireTarget (_bandits);
			if (_targetBandit == null || !_targetBandit.IsAlive) //if you still don't have a target, it's because there are none left
							return;
			_targetBandit.TakeDamage (DoDamage()); //deal damage to said bandit
		}
	}
	void CheckHealth(){
		if (_currentHP <= 0) {
			Die(); 			
		}
	}
	void Die(){
		_isAlive = false; 
		_caravan.GuardDied (this); 
		Destroy (this.gameObject); 
	}
	void Start(){
		_id = World.GetID (); 
		_name = "MindlessDouchbag#"+ _id.ToString(); 
	}
}
