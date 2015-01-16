using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; 

public class ActionsUI : MonoBehaviour {

	public GameObject mainPanel; 
	protected bool _active = false; 
	public bool Active { get { return _active; } }

	public virtual void TurnOff(){
		mainPanel.SetActive (false); 
		_active = false;
	}
	protected virtual void TurnOn(){
		mainPanel.SetActive (true); 
		Reload (); 
	}
	public virtual void Pressed(){
		_active = !_active; 
		if (_active) {
			TurnOn(); 
		}
		else {
			TurnOff(); 
		}
	}
	public virtual void Reload(){
		
	}

	protected GameObject MakeButton(string _textName, Color _bgColor, Transform _parent){//generic buttonMaker
		GameObject _buttonParentGO = Instantiate (World.PanelUI.listButton) as GameObject; 
		_buttonParentGO.transform.SetParent(_parent); 
		_buttonParentGO.name = _textName; 
		Transform _buttonGO = _buttonParentGO.transform.GetChild (0); 
		Button _theButton = _buttonGO.GetComponent<Button> (); 
		Image _theImage = _buttonGO.GetComponent<Image> (); 
		Transform _imageGo = _buttonGO.GetChild (0); 
		_theImage.sprite = World.PanelUI.buttonBackground; 
		_theImage.color = _bgColor; 
		Text _theText = _imageGo.GetComponent<Text> (); 
		_theText.text = _textName; 
		return _buttonParentGO;
	}
}
