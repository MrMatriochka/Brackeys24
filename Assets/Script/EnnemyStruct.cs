using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnnemyStruct
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
