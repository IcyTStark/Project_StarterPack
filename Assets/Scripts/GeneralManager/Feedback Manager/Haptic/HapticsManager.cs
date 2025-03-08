using System;
using UnityEngine;

public class HapticsManager
{
    [SerializeField] private bool isHapticsSupported;

    public HapticsManager()
    {
        //InitializeHapticsForiOS();

        //isHapticsSupported = MMVibrationManager.HapticsSupported();

        CustomDebug.Log(this.GetType().Name, $"HapticsManager Constructor Called! Instance ID: {this.GetHashCode()}");

        CustomDebug.Log(this.GetType().Name, isHapticsSupported.ToString());
    }

    public void ToggleHaptic(bool hapticState)
    {
        //MMVibrationManager.SetHapticsActive(hapticState);
    }

    //public void PlayPreset(HapticTypes hapticTypes)
    //{
    //    if (!isHapticsSupported)
    //        return;

    //    MMVibrationManager.Haptic(hapticTypes);
    //}

    //private void InitializeHapticsForiOS()
    //{
    //    MMNViOS.iOSInitializeHaptics();
    //}
}