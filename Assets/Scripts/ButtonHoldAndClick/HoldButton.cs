using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UnityEngine.UI;

public class HoldButton : Button
{
    private bool _isHoldingButton = false;
    private Coroutine _holdCoroutine;

    public float holdDelay = 0.1f;

    public event Action onButtonClick;

    public void AssignButtonFunction(Action action)
    {
        onButtonClick = action;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (!_isHoldingButton)
        {
            _isHoldingButton = true;
            _holdCoroutine = StartCoroutine(UpgradeCoroutine());
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        _isHoldingButton = false;
        if (_holdCoroutine != null)
        {
            StopCoroutine(_holdCoroutine);
            _holdCoroutine = null;
        }
    }

    private IEnumerator UpgradeCoroutine()
    {
        while (_isHoldingButton)
        {
            if(onButtonClick != null)
            {
                onButtonClick.Invoke();
            }
            else
            {
                onClick.Invoke();
            }

            yield return new WaitForSeconds(holdDelay);
        }
    }
}