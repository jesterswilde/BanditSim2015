using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine.UI; 

public class RoutesUI : ActionsUI {

	List<GameObject> _buttonGOs = new List<GameObject>(); 
	public Transform CaravanRouteListPanel; 


	public override void Reload ()
	{
		base.Reload ();
		RedrawList (); 
	}
	public void RedrawList(){
		ClearList ();
		DrawList (); 
	}
	public override void CloseMenu ()
	{
		base.CloseMenu ();
		DeselectRoutes (); 
	}
	
	void DrawList(){
		for(int i = 0; i < World.TheHideout.KnownRoutes.Count; i++){
			float _mod = (i % 2) * .5f +.25f; 
			Color _bg = new Color(_mod,_mod,_mod); 
			GameObject _routeGO = MakeButton(World.TheHideout.KnownRoutes[i].name, _bg , CaravanRouteListPanel); 
			Button _theButton= _routeGO.transform.GetChild (0).GetComponent<Button>(); 
			int _index = i; 
			_theButton.onClick.AddListener(() => HighlightRoute(_index)); 
			_buttonGOs.Add (_routeGO); 
		}
	}
	void ClearList(){
		for (int i = 0; i < _buttonGOs.Count; i++) {
			Destroy (_buttonGOs[i]); 
		}
		_buttonGOs.Clear (); 
	}
	void HighlightRoute(int _index){
		DeselectRoutes (); 
		World.TheHideout.KnownRoutes [_index].HighlightRoute (true); 
	}
	void DeselectRoutes(){
		foreach (CaravanRoute _route in World.TheHideout.KnownRoutes) {
			_route.HighlightRoute (false); 
		}
	}

}
