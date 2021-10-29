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
    public string valuesName;
    public string lable;
    public MyDropDownAttribute(string _valuesName, string _lable = null)
    {
        this.valuesName = _valuesName;
        this.lable = _lable;
    }

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(MyDropDownAttribute))]
public class MyDropDownDraw : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var target = (MyDropDownAttribute)attribute;
        object valuesObject = GetValues(property, target.valuesName);
        IList valuesList = (IList)valuesObject;
        string[] displayOptions = new string[valuesList.Count];
        for (int i = 0; i < displayOptions.Length; i++)
        {
            object value = valuesList[i];
            displayOptions[i] = value.ToString();
        }

        //Debug.Log(property.stringValue+"_"+(valuesObject == null).ToString()+"_"+ target.valuesName+"_"+ displayOptions[0]);

        int currIndex = System.Array.IndexOf<string>(displayOptions, property.stringValue);
        currIndex = Mathf.Clamp(currIndex, 0, displayOptions.Length);
        int seleteValue = EditorGUI.Popup(position,target.lable==null?property.displayName:target.lable, currIndex, displayOptions);
        property.stringValue = displayOptions[seleteValue];
    }
    private object GetValues(SerializedProperty property, string valuesName)
    {
        object target = PropertyUtility.GetTargetObjectWithProperty(property);

        FieldInfo valuesFieldInfo = ReflectionUtility.GetField(target, valuesName);
        if (valuesFieldInfo != null)
        {
            return valuesFieldInfo.GetValue(target);
        }

        PropertyInfo valuesPropertyInfo = ReflectionUtility.GetProperty(target, valuesName);
        if (valuesPropertyInfo != null)
        {
            return valuesPropertyInfo.GetValue(target);
        }

        MethodInfo methodValuesInfo = ReflectionUtility.GetMethod(target, valuesName);
        if (methodValuesInfo != null &&
            methodValuesInfo.ReturnType != typeof(void) &&
            methodValuesInfo.GetParameters().Length == 0)
        {
            return methodValuesInfo.Invoke(target, null);
        }

        return null;
    }
}
#endif
