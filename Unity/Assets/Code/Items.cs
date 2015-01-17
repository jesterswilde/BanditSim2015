using UnityEngine;
using System.Collections;

public class Items : MonoBehaviour {

	[SerializeField]
	protected string _name;
	[SerializeField]
	protected string _description; 

	[SerializeField]
	protected int _value; 


	public virtual void EquipItem(){
		
	}
	public virtual void UnequipItme(){
		
	}
}
