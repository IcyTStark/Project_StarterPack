using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class ButtonContextMenu
{
    [MenuItem("CONTEXT/Button/Convert to HoldButton")]
    private static void ConvertToHoldButton(MenuCommand command)
    {
        Button normalButton = (Button)command.context;
        if (normalButton == null) return;

        GameObject buttonObj = normalButton.gameObject;
        Undo.RecordObject(buttonObj, "Convert Button to HoldButton");

        Button.ButtonClickedEvent onClickEvents = normalButton.onClick;

        Object.DestroyImmediate(normalButton);

        HoldButton customButton = buttonObj.AddComponent<HoldButton>();

        customButton.onClick = onClickEvents;

        Debug.Log($"Converted '{buttonObj.name}' to HoldButton!");
    }
    
    
    [MenuItem("CONTEXT/Button/Convert to Button")]
    private static void ConvertToButton(MenuCommand command)
    {
        HoldButton holdButton = (HoldButton)command.context;
        if (holdButton == null) return;

        GameObject buttonObj = holdButton.gameObject;
        Undo.RecordObject(buttonObj, "Convert HoldButton to Button");

        Button.ButtonClickedEvent onClickEvents = holdButton.onClick;

        Object.DestroyImmediate(holdButton);

        Button customButton = buttonObj.AddComponent<Button>();

        customButton.onClick = onClickEvents;

        Debug.Log($"Converted '{buttonObj.name}' to Button!");
    }
}