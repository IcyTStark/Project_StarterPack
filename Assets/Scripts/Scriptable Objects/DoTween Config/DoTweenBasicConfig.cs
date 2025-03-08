using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "FacilityUpgradeDoTweenConfig", menuName = "ScriptableObjects/DoTween/BasicParams")]
public class DoTweenBasicConfig : ScriptableObject
{
    [SerializeField] private float duration;
    [SerializeField] private Ease easeType;

    public float Duration => duration;
    public Ease EaseType => easeType;
}