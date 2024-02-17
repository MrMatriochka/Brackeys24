using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Room", menuName = "Room")]

public class RoomStats : ScriptableObject
{
	public RoomType type;
    public int roomSpace = 3;
    [Vector2Range(0, 10, 0, 10)]
    public Vector2Int ennemyNb;
	public EnnemiesStats[] possibleEnnemies;

	public Pnj[] pnjType;
	public GameObject decor;
	public enum Pnj
	{
		None,
		Marchand,
		Shamisen,
		Apothicaire,
		Forgeron,
		Moine,
		Autel,
		Papi
	}

	public enum RoomType
    {
		Default,
		Onsen,
		Forge,
		Labo,
		Temple,
		BossRoom
	}
}
