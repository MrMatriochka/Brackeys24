using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ennemy", menuName = "Ennemy")]
public class EnnemiesStats : ScriptableObject
{
    public int health;
    public int damage;
    public AttackSequence[] sequences;

    [Header("Fight")]
    public Sprite baseStance;
    public Sprite attackStance;
    public Sprite heavyStance;

    [Header("Door")]
    public AudioClip[] sfx;
}
