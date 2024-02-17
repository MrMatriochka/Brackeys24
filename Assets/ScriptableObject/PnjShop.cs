using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrade")]
public class PnjShop : ScriptableObject
{
    public RoomStats.Pnj type;

    public Upgrades.Upgrade[] itemList;
}
