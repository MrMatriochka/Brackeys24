using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrade")]
public class Upgrades : ScriptableObject
{
    public string displayName;
    public Sprite sprite;
    public string description;
    
    public enum Upgrade
    {
        ParcheminCrit,
        ParcheminDamage,
        ParcheminVie,
        ParcheminListen,
    }

    public Upgrade upgardeFunction;
}


