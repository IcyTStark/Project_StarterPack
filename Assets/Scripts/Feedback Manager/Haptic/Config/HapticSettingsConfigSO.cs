using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace TMS.Feedback.Haptics
{
    [CreateAssetMenu(fileName = "HapticSettings", menuName = "ScriptableObjects/Generic/Haptics/HapticSettings")]
    public class HapticSettingsConfigSO : ScriptableObject
    {
        [Header("Haptic Strength: ")]
        [PropertyRange(0f, 1f)] [MaxValue(1)] [SerializeField] private float _hapticStrength = 1.0f;
        public float HapticStrength => _hapticStrength;
    }
}