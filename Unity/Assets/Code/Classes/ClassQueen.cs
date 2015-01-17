using UnityEngine;
using System.Collections;

public class ClassQueen : BanditClass {


	//the queen is OP as shit. I increased the max range of all her stat increases by .01 to reflect this.
	public override void LevelUp (ref float hp, ref float dr, ref float damage, ref float agility, ref float range, ref float magic, ref float build, ref float cunning, ref float craft)
	{
		base.LevelUp (ref hp, ref dr, ref damage, ref agility, ref range, ref magic, ref build, ref cunning, ref craft);
		hp += Random.Range (3f, 8.01f);
		dr += Random.Range (.5f, 1.26f);
		damage += Random.Range (1f, 5.01f);
		agility += Random.Range (1f, 5.01f);
		range += Random.Range (.1f, .51f); 
		magic += Random.Range (.25f, .76f); 
		build += Random.Range (.1f, .51f);
		cunning += Random.Range (.1f, .51f);
		craft += Random.Range (.25f, 1.51f); 
	}
}
