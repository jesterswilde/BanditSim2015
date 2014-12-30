using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

	[SerializeField]
	int _cost = 0; 
	public int Cost { get { return _cost; } }

	public virtual void OnBuildEffect(){
		
	}
}
