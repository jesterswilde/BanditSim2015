using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine.UI; 

public class PanelUI : MonoBehaviour {

	public Object buttonPrefab; 
	Canvas _canvas; 

	Map _map; 
	
	public Text daysOfFood; 
	public Text muney; 

	public Sprite buttonBackground; 


	List<Button> _locBanditButtons = new List<Button>(); //all bandits at the selected location
	List<Button> _hideoutBanditButtons = new List<Button>(); // all bandits at the hideout
	List<Button> _locActionButtons = new List<Button>(); 
	List<Button> _hideoutActionButtons = new List<Button> ();

	List<Bandit> _selLocBandits = new List<Bandit>(); //all bandits selected from the current location
	List<Bandit> _selHideoutBandits = new List<Bandit>(); //all bandits selected from hideout

	public Transform leftPanel; 
	public Transform rightPanel;
	Animator _leftPanelAnim; 
	Animator _rightPanelAnim; 
	
	[SerializeField]
	List<ActionsUI> _leftPanelActions = new List<ActionsUI>(); 
	[SerializeField]
	List<ActionsUI> _rightPanelActions = new List<ActionsUI>(); 
	
	public Object listButton; 
	
	ActionsUI _leftActionUI; 
	ActionsUI _rightActionsUI; 
	



	//INFO PANEL STUFF --------------------------------------------------------------------------------------------------------------
	public void UpdateInfoPanel(){
		muney.text = "Muney: " + World.TheHideout.Money.ToString (); 
		daysOfFood.text = "Days of Food Left: " + World.TheHideout.DaysOfFood.ToString (); 
	}
	public void RefreshUI(){
		UpdateInfoPanel (); 
		if (_leftActionUI != null) {
			_leftActionUI.Reload(); 
		}
		if (_rightActionsUI != null) {
			_leftActionUI.Reload(); 
		}
	}


	// Side Panel stuff -----------------------------------------------------------------------------------------------------------------


	public void ReloadPanels(){
		if (_leftActionUI != null) {
			_leftActionUI.Reload(); 	
		}
		if (_rightActionsUI != null) {
			_rightActionsUI.Reload(); 
		}
	}

	void GetPanelComponents(){ 
		if (leftPanel != null) {
						_leftPanelAnim = leftPanel.GetComponent<Animator> (); 
				} 
		else
						Debug.Log ("Hook up th left panel"); 
		if (rightPanel != null) {
						_rightPanelAnim = rightPanel.GetComponent<Animator> ();
				} 
		else
						Debug.Log ("Hook up right panel"); 
	}
	void TogglePanel(int i){
		if (i == 0) { //left Panel
			_leftPanelAnim.SetBool ("Open", !_leftPanelAnim.GetBool("Open")); //toggles the visible state of left panel
			if(!_leftPanelAnim.GetBool("Open")){ //unload the Active components if closed
				_leftActionUI = null; 
			}
		}
		if (i == 1) { //right Panel
			_rightPanelAnim.SetBool("Open", !_rightPanelAnim.GetBool("Open")); 
			if(!_rightPanelAnim.GetBool("Open")){
				_leftActionUI = null; 
			}
		}
	}
	// List order for left panel 
	//		-0 Bandits
	public void BanditsButton(){
		_leftActionUI = _leftPanelActions[0]; 
		_leftActionUI.Pressed();
		PressedSideButton (0); 
	}
	void PressedSideButton(int i){
		if (i == 0) {
			if (!_leftActionUI.Active && _leftPanelAnim.GetBool("Open") ||
			    _leftActionUI.Active && !_leftPanelAnim.GetBool("Open") ) {
				TogglePanel(i);
			}
		}
		for(int p = 0; p < _leftPanelActions.Count; p++){
			if(p != i){ //if this is not the active button
				_leftPanelActions[p].TurnOff(); 
			}
		}
	}


	public Object locationPrefabUI;
	RectTransform _locUITrans; 
	// LOCATION STUFF -----------------------------------------------------------------------------------------
	public void SelectLocation(Location _theLoc ){
		Debug.Log ("location selected"); 
		DeslectLocation (); 
		GameObject _locUI = Instantiate (locationPrefabUI) as GameObject; 
		_locUI.transform.SetParent (_canvas.transform, false);  
		_locUITrans = _locUI.transform as RectTransform; 
		_locUI.transform.position = World.mainCam.WorldToScreenPoint (_theLoc.transform.position); 
		RedrawLocation (); 
	}
	public void RedrawLocation(){
		
	}
	public void DeslectLocation(){
		if(_locUITrans != null){
			Destroy (_locUITrans.gameObject); 
		}
	}








	//STARTUP -----------------------------------------------------------------------------------

	void Start(){
		_map = World.Map; 
		GetPanelComponents (); 
		_canvas = GetComponent<Canvas> ();
	}

	/*
	// BANDIT BUTTON STUFF -------------------------------------------------------------------------------------------------------------
	public void UpdateBanditButtons(){ // call this when you modify the bandit lists
		ClearLists (); /*
		if(_map.SelLocation != null){
			FixListLength (_locBanditButtons, _map.SelLocation.LocalBandits.Count, locationPanel);
			FixListLength (_hideoutBanditButtons, World.HideoutLoc.LocalBandits.Count, hideoutPanel);

			for (int i = 0; i < _locBanditButtons.Count; i++) { //make all the buttons for the selected location
				Button _theButton = _locBanditButtons[i];
				Text _theText = _theButton.GetComponentInChildren<Text>(); 
				_theText.text = _map.SelLocation.LocalBandits[i].Name; // give the name of hte bandit to the button
				Bandit _theBandt = _map.SelLocation.LocalBandits[i]; //capture the bandit
				_theButton.onClick.AddListener(()  => SelectLocBandit(_theBandt)); //make the on click property
			}
			for(int i = 0; i < _hideoutBanditButtons.Count; i++){
				Button _theButton = _hideoutBanditButtons[i];
				Text _theText = _theButton.GetComponentInChildren<Text>(); 
				_theText.text = World.HideoutLoc.LocalBandits[i].Name; 
				Bandit _theBandit = World.HideoutLoc.LocalBandits[i];
				_theButton.onClick.AddListener(() => SelectHideoutBandit(_theBandit)); 
			}
			UpdateLocationButtonOptions (); 
			UpdateHideoutButtonOptions (); 
	}
	void UpdateLocationButtonOptions(){/*
		for(int i = 0; i < _map.SelLocation.LocalActions.Count; i++){ //populate the list, yay!
			Button _theButton = MakeButton (locationActionPanel); 
			_locActionButtons.Add (_theButton); 
			int _index = i; 
			_theButton.onClick.AddListener(() => DoLocAction(_index)); 
			Text _theText = _theButton.GetComponentInChildren<Text>(); 
			_theText.text = _map.SelLocation.LocalActions[_index].ActionName;
	}
	void UpdateHideoutButtonOptions(){/*
		for(int i = 0; i < World.HideoutLoc.LocalActions.Count ; i++){
			Button _theButton = MakeButton(hideoutActionPanel);  
			_hideoutActionButtons.Add(_theButton); 
			int _index = i; 
			_theButton.onClick.AddListener(() => DoHideoutAction(_index)); 
			Text _theText = _theButton.GetComponentInChildren<Text>(); 
			_theText.text = World.HideoutLoc.LocalActions[_index].ActionName;
	}

	void DoLocAction(int i){ //assigns to every selected bandit at the hideout, the chosen location action
		foreach (Bandit _theBandit in _selHideoutBandits) {
			_theBandit.TravelToLocation(_map.SelLocation); //send them to the place
			_theBandit.AssginTask(_map.SelLocation.LocalActions[i]); //have them do the thing
		}
		ClearLists (); 
		UpdateBanditButtons (); 

	}
	void DoHideoutAction(int i){ //assigns to every selected bandit at hte location, the hideout action
		foreach (Bandit _theBandit in _selLocBandits) {
			_theBandit.TravelToLocation(World.HideoutLoc); //send them to the place
			_theBandit.AssginTask(World.HideoutLoc.LocalActions[i]); //have them do the thing
		}
		ClearSelectedBanditLists ();
		UpdateBanditButtons (); 
		 
	}
	public void SelectLocBandit(Bandit _theBandit){
		_selLocBandits.Add (_theBandit); 
	}
	public void SelectHideoutBandit(Bandit _theBandit){
		_selHideoutBandits.Add (_theBandit); 
	}

	void FixListLength(List<Button> _buttons, int _number, Transform _parent){ //ensures there is one button per bandit
		foreach (Button _theButton in _buttons)
						Destroy (_theButton.gameObject); 
		_buttons.Clear (); 
		for (int i = 0; i < _number; i++) {
			_buttons.Add(MakeButton(_parent)); 
		}
	}
	
	Button MakeButton(Transform _parent){
		GameObject _theButtonGO = Instantiate (buttonPrefab) as GameObject; 
		_theButtonGO.transform.SetParent (_parent); 
		return _theButtonGO.GetComponent<Button>();
		/*
		GameObject _theButtonGO = new GameObject (); 
		_theButtonGO.transform.parent = _parent; 
		Button _theButton = _theButtonGO.AddComponent<Button> (); 
		GameObject _theTextGO = new GameObject ();
		_theTextGO.transform.parent = _theButton.transform; 
		_theTextGO.AddComponent<Text> (); 
		return _theButton; 
	}
	public void ClearLists(){ //clears out all the lists and destroys all the buttons. 
		foreach (Button _theButton in _locBanditButtons) {
			Destroy(_theButton.gameObject); 
		}
		foreach (Button _theButton in _hideoutBanditButtons) {
			Destroy (_theButton.gameObject); 
		}
		_locBanditButtons.Clear (); 
		_hideoutBanditButtons.Clear (); 

		foreach (Button _theButton in _locActionButtons) { //remove the buttons that already exist
			Destroy(_theButton.gameObject); 		
		}
		_locActionButtons.Clear (); //clear the list

		foreach (Button _theButton in _hideoutActionButtons) {
			Destroy (_theButton.gameObject);
		}
		_hideoutActionButtons.Clear (); 
	}
	void ClearSelectedBanditLists(){
		_selLocBandits.Clear (); 
		_selHideoutBandits.Clear (); 
	}
	*/



	//So, Moved in with your parents today. It's doesn't feel as unfortunate as it should. You are moving forward but lots of things have fallen off the rails and
	//languished too long.  Have you found a job yet? The rejection and silence doesn't feel great over here.  Have you figured out why there are no takers?
	//What is wrong with you?  
	//Have you gone up to see Ryan and company yet? Don't let it be more than a year. That would be sad. 
}
