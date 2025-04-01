using UnityEngine;

[CreateAssetMenu(fileName = "ButtonHoldAndClickConfig", menuName = "ScriptableObjects/Generic/ButtonHoldAndClick/ButtonHoldAndClickConfig")]
public class ButtonHoldAndClickConfig : ScriptableObject
{
    [SerializeField] private float _initialDelay = 0.4f;
    [SerializeField] private float _holdRepeatInterval = 0.1f;

    public float InitialDelay => _initialDelay;
    public float HoldRepeatInterval => _holdRepeatInterval;
}