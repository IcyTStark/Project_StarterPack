using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaHandler : MonoBehaviour
{
    RectTransform Panel;
    Rect LastSafeArea = new Rect(0, 0, 0, 0);

    public static Action RefreshSafeArea;

    void Awake()
    {
        Panel = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        Refresh();
	}

    public void Refresh()
    {
        if (LastSafeArea != Screen.safeArea)
        {
            ApplySafeArea(Screen.safeArea);
        }
    }

    private void ApplySafeArea(Rect r)
    {
        LastSafeArea = r;
        Vector2 anchorMin = r.position;
        Vector2 anchorMax = r.position + r.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        Panel.anchorMin = anchorMin;
        Panel.anchorMax = anchorMax;
	}
}