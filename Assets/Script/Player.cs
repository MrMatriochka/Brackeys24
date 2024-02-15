using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]int baseHealth;
    public static int health;
    public int damage;
    public int coins;

    private void Start()
    {
        if (WorldGeneration.playerProgression == 0)
        {
            health = baseHealth;
        }
        
    }
}
