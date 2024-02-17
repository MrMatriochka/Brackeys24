using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New shop", menuName = "Shop")]
public class PnjShop : ScriptableObject
{
    public RoomStats.Pnj type;

    public Upgrades[] itemList;
}
