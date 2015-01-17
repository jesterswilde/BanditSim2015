using UnityEngine;
using System.Collections;

public class ClassKnight : BanditClass {

	public override void LevelUp (ref float hp, ref float dr, ref float damage, ref float agility, ref float range, ref float magic, ref float build, ref float cunning, ref float craft)
	{
		base.LevelUp (ref hp, ref dr, ref damage, ref agility, ref range, ref magic, ref build, ref cunning, ref craft);
		hp += Random.Range (3.5f, 7.75f);
		dr += Random.Range (.5f, 1.5f);
		damage += Random.Range (1f, 3f);
		agility += Random.Range (.2f, 1f);
		range += Random.Range (0, 0); 
		magic += Random.Range (0, 0); 
		build += Random.Range (.75f, 2.25f);
		cunning += Random.Range (0f, .5f);
		craft += Random.Range (0f, .5f); 
	}
}
