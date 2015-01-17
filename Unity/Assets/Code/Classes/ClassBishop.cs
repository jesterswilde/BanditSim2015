using UnityEngine;
using System.Collections;

public class ClassBishop : BanditClass {

	public override void LevelUp (ref float hp, ref float dr, ref float damage, ref float agility, ref float range, ref float magic, ref float build, ref float cunning, ref float craft)
	{
		base.LevelUp (ref hp, ref dr, ref damage, ref agility, ref range, ref magic, ref build, ref cunning, ref craft);
		hp += Random.Range (.75f, 3.25f);
		dr += Random.Range (0, 0);
		damage += Random.Range (.25f, 1f);
		agility += Random.Range (.5f, 1.5f);
		range += Random.Range (.1f, .5f); 
		magic += Random.Range (.25f, 1f); 
		build += Random.Range (0f, .5f);
		cunning += Random.Range (.75f, 2.25f);
		craft += Random.Range (.0f, .5f); 
	}
}
