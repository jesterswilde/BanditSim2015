using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine.UI; 

public class BanditsUI : ActionsUI {

	List<GameObject> _banditButtons = new List<GameObject>(); 
	public RectTransform allBanditsPanel; 
	Bandit _selectedBandit; 
	public Text banditName; 
	public Text level;
	public Text hp;
	public Text damage;
	public Text dr;
	public Text agility;
	public Text magic;
	public Text range; 
	public Text build;
	public Text cunning;
	public Text crafty; 

	public override void Reload ()
	{
		base.Reload ();
		ClearAll (); 
		for (int i = 0; i < World.TheHideout.AllBandits.Count; i++) {
			Bandit _theBandit = World.TheHideout.AllBandits[i]; 
			float _mod = (i % 2) * .5f +.25f; 
			Color _bg = new Color(_mod,_mod,_mod); 
			GameObject _thePanelGO = MakeButton(_theBandit.Name, _bg, allBanditsPanel); 
			Button _theButton= _thePanelGO.transform.GetChild (0).GetComponent<Button>(); 
			int _index = i; 
			_theButton.onClick.AddListener(() => ChoseBandit(_index)); 
			_banditButtons.Add (_thePanelGO); 
		}
		RefreshBanditInfo (); 
	}
	void ChoseBandit(int i){ //what happens when they click on a bandit name
		_selectedBandit = World.TheHideout.AllBandits[i]; 
		RefreshBanditInfo (); 
	}
	void RefreshBanditInfo(){ //load all the info about selected bandit
		if(_selectedBandit != null){
			level.text = _selectedBandit.Level.ToString(); 
			banditName.text = _selectedBandit.Name;
			hp.text = _selectedBandit.CurrentHealth.ToString() +"/"+ _selectedBandit.Health.ToString();
			damage.text = "T.B.D.";
			dr.text = _selectedBandit.DR.ToString(); 
			agility.text = _selectedBandit.Agility.ToString(); 
			magic.text = _selectedBandit.Magic.ToString(); 
			range.text = _selectedBandit.Range.ToString(); 
			build.text = _selectedBandit.Building.ToString(); 
			cunning.text = _selectedBandit.Cunning.ToString(); 
			crafty.text = _selectedBandit.Crafty.ToString(); 
		}
		else{
			banditName.text = "(Select Bandit)"; 
			level.text =  hp.text = damage.text = dr.text =agility.text = magic.text =range.text =build.text = cunning.text = crafty.text = " "; 
		}
	}
	void ClearAll(){
		Debug.Log (World.TheHideout.AllBandits.Count + " : " + _banditButtons.Count); 
		for(int i =0 ; i < _banditButtons.Count; i++) {
			Destroy(_banditButtons[i]);
		}
		_banditButtons.Clear (); 
	}
	public override void TurnOff ()
	{
		base.TurnOff ();
		_selectedBandit = null; 
	}

}
