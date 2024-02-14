using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sequence", menuName = "AttackSequence")]
public class AttackSequence : ScriptableObject
{
    public EnnemyStruct.EnnemyAction[] sequence;
}
