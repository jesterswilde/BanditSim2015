using UnityEngine;
using System.Collections;

public class BD_Maproom : Building {

	public override void BuildingEffect (ref int banditCap, ref int foodCap, ref int moneyCap)
	{
		base.BuildingEffect (ref banditCap, ref foodCap, ref moneyCap);
		banditCap += 4; 
		foodCap += 200; 
		moneyCap += 400; 
	}
}
