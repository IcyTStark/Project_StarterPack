using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;

[InitializeOnLoad]
public class AutoButtonComponentInitializer
{
    static AutoButtonComponentInitializer()
    {
        ObjectFactory.componentWasAdded += OnComponentAdded;
    }

    private static void OnComponentAdded(Component component)
    {
        if(component is Button button)
        {
            GameObject buttonObj = button.gameObject;

            if(buttonObj.GetComponent<ButtonOnClickSFX>() == null)
            {
                var buttonOnClickSFX = Undo.AddComponent<ButtonOnClickSFX>(buttonObj);

                SerializedObject serializedObj = new SerializedObject(buttonOnClickSFX);
                SerializedProperty audioTypeProp = serializedObj.FindProperty("audioType");

                if (audioTypeProp != null)
                {
                    audioTypeProp.enumValueIndex = (int)AudioType.UI_BUTTON_CLICK;
                    serializedObj.ApplyModifiedProperties(); // Properly records for Undo
                }
            }

            if (buttonObj.GetComponent<ButtonOnClickEffect>() == null)
            {
                var buttonOnClickEffect = Undo.AddComponent<ButtonOnClickEffect>(buttonObj);

                SerializedObject serializedObj = new SerializedObject(buttonOnClickEffect);
                SerializedProperty configTypeProp = serializedObj.FindProperty("config");

                if(configTypeProp != null)
                {
                    configTypeProp.objectReferenceValue = Resources.Load<DoPunchScaleConfigSO>("DoPunchScaleConfigSO");
                    serializedObj.ApplyModifiedProperties(); // Properly records for Undo
                }
            }
        }
    }
}