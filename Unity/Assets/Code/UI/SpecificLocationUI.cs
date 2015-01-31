using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine.UI; 

public class SpecificLocationUI : MonoBehaviour {

	Location _loc; 
	Object _actionButtonOBJ; 
	[SerializeField]
	RectTransform _buttonParent; 

	public void Startup(Location _theLoc){
		_loc = _theLoc; 
		_actionButtonOBJ = World.PanelUI.listButton; 
		MakeActionButtons (); 
	}
	void MakeActionButtons(){
		for(int i = 0 ; i < _loc.LocalActions.Count ; i++) {
			GameObject _buttonGO = Instantiate (_actionButtonOBJ) as GameObject; 
			MakeActionButton (_loc.LocalActions[i].ActionName,i); 
			Button _button = _buttonGO.GetComponent<Button>(); 
		}
	}
	GameObject MakeActionButton(string _textName,  int _index){//generic buttonMaker
		GameObject _buttonParentGO = Instantiate (_actionButtonOBJ) as GameObject; 
		_buttonParentGO.transform.SetParent(_buttonParent); 
		_buttonParentGO.name = _textName; 
		Transform _buttonGO = _buttonParentGO.transform.GetChild (0); 
		Button _theButton = _buttonGO.GetComponent<Button> (); 
		Image _theImage = _buttonGO.GetComponent<Image> (); 
		Transform _imageGo = _buttonGO.GetChild (0); 
		_theImage.sprite = World.PanelUI.buttonBackground; 
		Text _theText = _imageGo.GetComponent<Text> (); 
		_theText.text = _textName; 
		_theButton.onClick.AddListener(() => SelectAction(_index)); 
		return _buttonParentGO;
	}
	void SelectAction(int _index){
		World.PanelUI.MakeBanditSelectPopup(_loc, _loc.LocalActions [_index]); 
	}
}
