using System;
using UnityEngine;
using System.Collections.Generic;

using Sirenix.OdinInspector;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    [SerializeField] private Button testButton;

    private void Awake()
    {
        testButton.onClick.AddListener(() => Debug.Log("Button Clicked"));
    }
}