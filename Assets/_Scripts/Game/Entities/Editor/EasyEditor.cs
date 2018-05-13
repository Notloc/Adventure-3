using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


/**
 * This class provides a variety of methods designed to make
 * creating custom editors faster and slightly easier
 */
public class EasyEditor : Editor
{
    //Draws a label in the same style as [Header]
    protected void Header(string header)
    {
        EditorGUILayout.LabelField(header, EditorStyles.boldLabel);
    }


}
