using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
#if UNITY_EDITOR
using NaughtyAttributes.Editor;
using UnityEditor;
#endif
[System.AttributeUsage(AttributeTargets.Field)]
public class MyDropDownAttribute : PropertyAttribute
{
    public string valueName;
    public string lable;
    public MyDropDownAttribute(string _valueName, string _lable = null)
    {
        valueName = _valueName;
        this.lable = _lable;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(MyDropDownAttribute))]
public class MyDropDownDraw : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var targetAttbibute = (MyDropDownAttribute)attribute;
        var targetObj = property.serializedObject.targetObject;
        Type type = targetObj.GetType();

        string[] displayOptions = null;


        var bindflags= BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        var fieldInfo = type.GetField(targetAttbibute.valueName, bindflags);
        if (fieldInfo != null)
            displayOptions = (string[])fieldInfo.GetValue(targetObj);
        else
        {
            var propertyinfo = type.GetProperty(targetAttbibute.valueName, bindflags);
            if (propertyinfo != null)
                displayOptions = (string[])propertyinfo.GetValue(targetObj);
        }
        if (displayOptions != null)
        {
            int currIndex = System.Array.IndexOf<string>(displayOptions, property.stringValue);
            currIndex = Mathf.Clamp(currIndex, 0, displayOptions.Length);
            int seleteValue = EditorGUI.Popup(position, targetAttbibute.lable == null ? property.displayName : targetAttbibute.lable, currIndex, displayOptions);
            property.stringValue = displayOptions[seleteValue];
        }
    }

    //private object GetValues(SerializedProperty property, string valuesName)
    //{
    //object target = PropertyUtility.GetTargetObjectWithProperty(property);
    //    FieldInfo valuesFieldInfo = ReflectionUtility.GetField(target, valuesName);
    //    if (valuesFieldInfo != null)
    //    {
    //        return valuesFieldInfo.GetValue(target);
    //    }

    //    PropertyInfo valuesPropertyInfo = ReflectionUtility.GetProperty(target, valuesName);
    //    if (valuesPropertyInfo != null)
    //    {
    //        return valuesPropertyInfo.GetValue(target);
    //    }

    //    MethodInfo methodValuesInfo = ReflectionUtility.GetMethod(target, valuesName);
    //    if (methodValuesInfo != null &&
    //        methodValuesInfo.ReturnType != typeof(void) &&
    //        methodValuesInfo.GetParameters().Length == 0)
    //    {
    //        return methodValuesInfo.Invoke(target, null);
    //    }

    //    return null;
    //}
}
#endif
