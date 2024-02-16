using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrade")]
public class Upgrades : ScriptableObject
{
    public string name;
    public Sprite sprite;
    public string description;
}
