using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnnemyStruct;

namespace EnnemyStruct
{
    [System.Serializable]
    public struct EnnemyAction
    {
        public float timing;
        public EnnemyMoves move;
    }
    public enum EnnemyMoves
    {
        Pause,
        LightAttack,
        HeavyAttack,
        ResetAction,
        Open
    }

   
}

[CreateAssetMenu(fileName = "New Sequence", menuName = "AttackSequence")]
public class AttackSequence : ScriptableObject
{
    public EnnemyAction[] sequence;
}