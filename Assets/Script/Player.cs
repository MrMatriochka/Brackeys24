using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    [SerializeField] int baseHealth;
    public static int health;
    public static int maxhealth;
    [SerializeField] int baseDamage;
    public static int damage;
    //public int coins;
    [SerializeField] float baseCritMultiplier;
    public static float critMultiplier;
    [SerializeField] float baseCritRate;
    public static float critRate = 0;
    public static float paryBonus = 0;
    public static float doorTimeBonus = 0;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        if (WorldGeneration.playerProgression == 0)
        {
            Init();
        }

    }

    public void Init()
    {
        health = baseHealth;
        maxhealth = baseHealth;
        damage = baseDamage;

        critRate = baseCritRate;
        critMultiplier = baseCritMultiplier;
        paryBonus = 0;
        doorTimeBonus = 0;
    }
}

