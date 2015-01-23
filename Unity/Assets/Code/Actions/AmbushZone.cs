using UnityEngine;
using System.Collections;

public class AmbushZone : MonoBehaviour {

	public Ambush _ambush;
	public void Startup(Ambush _theAmbush){
		_ambush = _theAmbush;
	}

	void OnTriggerEnter(Collider _collider){
		if(_collider.gameObject.layer == 9){ //it's a caravan that entered the ambush zone
			Caravan _caravan = _collider.transform.parent.gameObject.GetComponent<Caravan>(); 
			_ambush.AndTheCageComesDown (_caravan); 
		}
	}
}
