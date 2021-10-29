using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MyLableAttribute : PropertyAttribute
{
    public string lable;
    public MyLableAttribute(string _lable)
    {
        this.lable = _lable;
    }
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(MyLableAttribute))]
public class MyLableDraw : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var target = (MyLableAttribute)attribute;
        EditorGUI.PropertyField(position, property, new GUIContent(target.lable), true);
    }
}
#endif