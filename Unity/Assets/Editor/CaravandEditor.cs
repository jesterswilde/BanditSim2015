using UnityEngine;
using System.Collections;
using UnityEditor; 

[CustomEditor(typeof(Caravan))]
public class CaravandEditor : Editor {


	bool _showRewards = true;  //the bools are about showing foldouts
	Vector2 _moneyRange; 	//these are intermediary variables that allow me to have 2 input fields on one line easily
	Vector2 _foodRange; 
	Vector2 _banditRange; 
	bool _showGuard = true; 

	public override void OnInspectorGUI ()
	{
		Caravan _caraEdit = (Caravan)target; 

		_caraEdit.DisplayName = EditorGUILayout.TextField("Caravan Name", _caraEdit.DisplayName); //this first section is name and description input
		EditorGUILayout.LabelField ("Description");
		EditorGUI.indentLevel++; 
		_caraEdit.Description = EditorGUILayout.TextArea (_caraEdit.Description); 
		EditorGUI.indentLevel -= 1; 

		_showRewards = EditorGUILayout.Foldout (_showRewards, "Rewards"); //this section creates the range of rewards that each caravan will drop
		if (_showRewards) {
			_moneyRange.x = _caraEdit.MoneyMin;
			_moneyRange.y = _caraEdit.MoneyMax; 
			_moneyRange = EditorGUILayout.Vector2Field("Money Min/Max", _moneyRange); 	
			_caraEdit.MoneyMin = (int)_moneyRange.x; 
			_caraEdit.MoneyMax = (int)_moneyRange.y; 

			_foodRange.x = _caraEdit.FoodMin;
			_foodRange.y = _caraEdit.FoodMax; 
			_foodRange = EditorGUILayout.Vector2Field("Food Min/Max", _foodRange); 	
			_caraEdit.FoodMin = (int)_foodRange.x; 
			_caraEdit.FoodMax = (int)_foodRange.y; 

			_banditRange.x = _caraEdit.BanditsMin;
			_banditRange.y = _caraEdit.BanditsMax; 
			_banditRange = EditorGUILayout.Vector2Field("Bandit Min/Max", _banditRange); 	
			_caraEdit.BanditsMin = (int)_banditRange.x; 
			_caraEdit.BanditsMax = (int)_banditRange.y; 
		}
		_showGuard = EditorGUILayout.Foldout (_showGuard, "Guards"); //this section is about the guards that will be fought
		if(_showGuard){
			_caraEdit.NumGuard = EditorGUILayout.IntField ("Number of Types", _caraEdit.NumGuard);
			EditorGUI.indentLevel++; 
			_caraEdit.CheckSmallList(_caraEdit.Guards);
			_caraEdit.CheckSmallList(_caraEdit.GuardRange); 
			_caraEdit.CheckLageList(_caraEdit.Guards); 
			_caraEdit.CheckLageList(_caraEdit.GuardRange); 
			for(int i = 0; i < _caraEdit.NumGuard; i ++){
				_caraEdit.Guards[i] = EditorGUILayout.ObjectField("Guard Type",_caraEdit.Guards[i] ,typeof(UnityEngine.Object),true) as Object; 
				_caraEdit.GuardRange[i] = EditorGUILayout.Vector2Field("Min/Max", _caraEdit.GuardRange[i]); 
			}
			EditorGUI.indentLevel -=1; 
		}
		_caraEdit.TravelSpeed = EditorGUILayout.FloatField ("Speed", _caraEdit.TravelSpeed); 

	}
}
