using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(FloatReference))] 
public class FloatReferenceDrawer : PropertyDrawer
{
    //options for using local or global
    private readonly string[] popupOptions = {"Use Local", "Use Global" };

    public GUIStyle popupStyle;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (popupStyle == null)
        {
            popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
            popupStyle.imagePosition = ImagePosition.ImageOnly;
        }

        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, label);


        EditorGUI.BeginChangeCheck(); //where the editor checks if you have changed a value


        SerializedProperty useLocal = property.FindPropertyRelative("UseLocal");
        SerializedProperty localValue = property.FindPropertyRelative("localVariable");
        SerializedProperty globalValue = property.FindPropertyRelative("globalVariable");

        //calculate rect for button
        Rect buttonRect = new Rect(position);
        buttonRect.yMin += popupStyle.margin.top;
        buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
        position.xMin = buttonRect.xMax;

        //store old indent value
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0; //shift the indent

        int result = EditorGUI.Popup(buttonRect, useLocal.boolValue ? 0 : 1, popupOptions, popupStyle);

        useLocal.boolValue = result == 0;

        EditorGUI.PropertyField(position, 
            useLocal.boolValue ? localValue : globalValue, GUIContent.none);


        //apply any changes we made to the inspector to the values they represent
        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
        }
        //move the indent back
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }


}
