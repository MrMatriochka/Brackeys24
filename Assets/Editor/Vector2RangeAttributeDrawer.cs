using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Vector2RangeAttribute))]
public class Vector2RangeAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();
        // Create a regular Vector2 field
        Vector2Int val = EditorGUI.Vector2IntField(position, label, property.vector2IntValue);
        
        // If the value changed
        if (EditorGUI.EndChangeCheck())
        {
            var rangeAttribute = (Vector2RangeAttribute)attribute;
            // Clamp the X/Y values to be within the allowed range
            val.x = Mathf.Clamp(val.x, rangeAttribute.MinX, rangeAttribute.MaxX);
            val.y = Mathf.Clamp(val.y, rangeAttribute.MinY, rangeAttribute.MaxY);
            // Update the value of the property to the clampped value
            property.vector2IntValue = val;
        }
    }
}
