using FIO.ModularAddressableSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

public class AddressableDependencyInitializer : IStartable
{
    private readonly AddressablesManager addressablesManager;

    public AddressableDependencyInitializer(AddressablesManager addressablesManager)
    {
        this.addressablesManager = addressablesManager;
    }

    public async void Start()
    {
        try
        {
            SmartDebug.Log($"Start Addressable Initialization", "ADDRESSABLEMANAGER");
            await addressablesManager.StartInitialization();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}