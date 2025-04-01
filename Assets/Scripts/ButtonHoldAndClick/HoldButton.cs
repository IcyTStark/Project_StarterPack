using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Extends the standard Unity Button with hold functionality.
/// Works with Unity's native onClick event system.
/// </summary>
public class HoldButton : Button
{
    [Header("Hold Settings")]

    [SerializeField] private ButtonHoldAndClickConfig _config;

    private bool _isHoldingButton = false;
    private Coroutine _holdCoroutine;
    private bool _processNormalClick = true;

    protected override void Awake()
    {
        base.Awake();

        LoadConfigIfNeeded();
    }

    /// <summary>
    /// Attempts to load a config if one isn't already assigned
    /// </summary>
    private void LoadConfigIfNeeded()
    {
        if (_config != null)
            return;

        _config = Resources.Load<ButtonHoldAndClickConfig>("ButtonHoldAndClickConfig");
    }


    /// <summary>
    /// Override OnPointerClick to control when the click happens
    /// </summary>
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (_processNormalClick)
        {
            base.OnPointerClick(eventData);
        }
    }

    /// <summary>
    /// Called when pointer is pressed on the button
    /// </summary>
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (!_isHoldingButton && interactable)
        {
            _isHoldingButton = true;
            _processNormalClick = true;

            _holdCoroutine = StartCoroutine(HoldCoroutine());
        }
    }

    /// <summary>
    /// Called when pointer is released from the button
    /// </summary>
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        CancelHold();
    }

    /// <summary>
    /// Called when pointer exits the button area
    /// </summary>
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        CancelHold();
    }

    /// <summary>
    /// Cancels the current hold operation
    /// </summary>
    private void CancelHold()
    {
        _isHoldingButton = false;
        if (_holdCoroutine != null)
        {
            StopCoroutine(_holdCoroutine);
            _holdCoroutine = null;
        }
    }

    /// <summary>
    /// Coroutine that handles the hold behavior
    /// </summary>
    private IEnumerator HoldCoroutine()
    {
        yield return new WaitForSeconds(_config.InitialDelay);

        _processNormalClick = false;

        while (_isHoldingButton)
        {
            onClick.Invoke();

            yield return new WaitForSeconds(_config.HoldRepeatInterval);
        }
    }

    /// <summary>
    /// Cleans up any running coroutines when the object is destroyed
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
        CancelHold();
    }
}