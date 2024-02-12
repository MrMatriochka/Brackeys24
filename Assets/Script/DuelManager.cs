using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelManager : MonoBehaviour
{
    enum EnnemyMoves 
    {
        LightAttack,
        DoubleLightAttack,
        HeavyAttack,
        Pause
    }

    public int[] sequence;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator PlaySequence()
    {
        foreach (int i in sequence)
        {

        }
        yield return null;
    }
}
