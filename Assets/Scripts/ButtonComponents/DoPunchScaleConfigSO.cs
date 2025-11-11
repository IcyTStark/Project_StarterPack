using DG.Tweening;
using UnityEngine;

public class DoPunchScaleConfigSO : ScriptableObject
{
    public Vector3 punch;
    public float duration;
    public int vibrato;
    public float elasticity;

    public Ease ease;
}
