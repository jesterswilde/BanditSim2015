using UnityEngine;
using System.Collections;

public class BD_Barracks : Building {

	public override void BuildingEffect (ref int banditCap, ref int foodCap, ref int moneyCap)
	{
		base.BuildingEffect (ref banditCap, ref foodCap, ref moneyCap);
		banditCap += 12; 
	}
}
