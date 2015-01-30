using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine.UI; 

public class PickBanditPopupUI : MonoBehaviour {

	[SerializeField]
	RectTransform _banditListPanel; 
	[SerializeField]
	Object _banditButton; 
	[SerializeField]
	Text _daysOutGUI; 
	[SerializeField]
	Text _actionText; 

	Location _destination; 
	Actions _action; 

	List<GameObject> _allBanditButtons = new List<GameObject>(); 
	List<Bandit> _selectedBandits = new List<Bandit>(); 

	int _daysOut = 1; 

	public void ReloadList(){ //cleares out the list, then makes the buttons again
		foreach (GameObject _go in _allBanditButtons) {
			Destroy(_go); 
		}
		_allBanditButtons.Clear (); 
		for (int i = 0; i < World.HideoutLoc.LocalBandits.Count; i++) { //you can only affect bandits at the hideout
			GameObject _buttonGO = Instantiate(_banditButton) as GameObject; 
			_allBanditButtons.Add (_buttonGO); 
			_buttonGO.transform.SetParent(_banditListPanel); //bullshit to get ahold of the gui elements
			_allBanditButtons[i].transform.FindChild("BanditName").transform.FindChild("Text").GetComponent<Text>().text = World.HideoutLoc.LocalBandits[i].Name; 
			_allBanditButtons[i].transform.FindChild("CheckMark").GetComponent<Image>().enabled = false ; 
			int _index = i; 
			_allBanditButtons[i].transform.FindChild("CheckBox").GetComponent<Button>().onClick.AddListener (() => ClickOnBandit(_index)); 
		}
		SetCheckMarks (); 
		_daysOutGUI.text = _daysOut.ToString (); 
		_actionText.text = "Days "+_action.ActionName; 
	}
	void SetCheckMarks(){ //goes through and makes sure the check appears if they are sellected
		for(int i = 0; i < _allBanditButtons.Count; i ++){
			if(IsSelected(World.HideoutLoc.LocalBandits[i])){
				_allBanditButtons[i].transform.FindChild("CheckMark").GetComponent<Image>().enabled = true ; 
			}
		}
	}
	public void ReloadBanditInfo(){
		 
	}
	public void ClickOnBandit(int i){ //what happens when you click on a bandit
		Bandit _theBandit = World.HideoutLoc.LocalBandits [i]; 
		if (IsSelected (_theBandit)) { //if that bandit was already selected
			_selectedBandits.Remove(_theBandit); 
			_allBanditButtons[i].transform.FindChild("CheckMark").GetComponent<Image>().enabled = false ; 
		}
		else{
			_selectedBandits.Add (_theBandit); 
			_allBanditButtons[i].transform.FindChild("CheckMark").GetComponent<Image>().enabled = true ; 
		}
	}
	bool IsSelected(Bandit _bandit){ //returns a bool, if this bandit is already selected
		foreach (Bandit _theBandit in _selectedBandits) {
			if(_bandit.ID == _theBandit.ID)return true; 
		}
		return false; 
	}
	public void AddDay(int _dayMod){ //when they add or remove a day in the UI, this is what is ahppening
		_daysOut += _dayMod; 
		_daysOut = Mathf.Clamp (_daysOut, 1, 99); 
		_daysOutGUI.text = _daysOut.ToString (); 
	}
	public void ClickOK(){ //they are done with this menu, and the bandits are assigned their tasks
		foreach (Bandit _bandit in _selectedBandits) {
			_bandit.TravelToLocation(_destination, _daysOut); 
			_bandit.AssginTask(_action); 
		}
		ClickCancel (); 
	}
	public void ClickCancel(){ //they are done with this menu
		_selectedBandits.Clear (); 
		Destroy (this.gameObject); 
	}

	public void Startup(Location _theLoc, Actions _theAction){
		_action = _theAction; 
		_destination = _theLoc; 
	}
	void Start(){
		ReloadList (); 
	}

}
