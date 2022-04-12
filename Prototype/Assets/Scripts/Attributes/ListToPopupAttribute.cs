using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


/*
 * This File was created to modify a string value to display a <List> as a Popup or dropdown list in the inspector. 
 * 
 * This was used to improve user experience and to allow them to customize their inventory with no programming.
 * 
 * Credit to "URocks!" on Youtube for a great Tutorial on how to make a CustomPropertyDrawer -> Link: https://www.youtube.com/watch?v=ThcSHbVh7xc
 */

public class ListToPopupAttribute : PropertyAttribute
{
    public Type myType;
    public string propertyName;
    public ListToPopupAttribute(Type _myType, string _propertyName)
    {
        myType = _myType;
        propertyName = _propertyName;
    }
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ListToPopupAttribute))]
public class ListToPopupDrawer : PropertyDrawer
{


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ListToPopupAttribute atb = attribute as ListToPopupAttribute;
        List<string> stringList = null;
        List<string> defaultList = new List<string> { "None" };

        if (atb.myType.GetField(atb.propertyName) != null)
        {
            stringList = atb.myType.GetField(atb.propertyName).GetValue(atb.myType) as List<string>;
        }

        if (stringList != null && stringList.Count != 0)
        {
            int selectedIndex = Mathf.Max(stringList.IndexOf(property.stringValue), 0);
            selectedIndex = EditorGUI.Popup(position, property.name, selectedIndex, stringList.ToArray());
            property.stringValue = stringList[selectedIndex];
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}
#endif
