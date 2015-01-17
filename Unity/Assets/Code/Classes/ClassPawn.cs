using UnityEngine;
using System.Collections;

public class ClassPawn : BanditClass {

	public override void LevelUp (ref float hp, ref float dr, ref float damage, ref float agility, ref float range, ref float magic, ref float build, ref float cunning, ref float craft)
	{
		base.LevelUp (ref hp, ref dr, ref damage, ref agility, ref range, ref magic, ref build, ref cunning, ref craft);
		hp += Random.Range (2.25f, 5.75f);
		dr += Random.Range (0, 0);
		damage += Random.Range (.75f, 2f);
		agility += Random.Range (1f, 3.5f);
		range += Random.Range (0, 0); 
		magic += Random.Range (0, 0); 
		build += Random.Range (.75f, 1.5f);
		cunning += Random.Range (.75f, 1.5f);
		craft += Random.Range (.75f, 1.5f); 
	}
}
