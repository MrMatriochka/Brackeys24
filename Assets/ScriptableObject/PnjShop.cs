using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New shop", menuName = "Shop")]
public class PnjShop : ScriptableObject
{
    public string pnjName;
    public Sprite sprite;
    public Upgrades[] itemList;
}
