using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public static class ProbabilitySystem
{
    private static readonly Dictionary<(Type, string), object> _strategies = new();

    public static void Initialize<T>(string key, IProbabilityConfig<T> config)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        _strategies[(typeof(T), key)] = new WeightedProbability<T>(config.GetWeightDictionary(), new System.Random());
    }

    public static T GetRandomItem<T>(string key)
    {
        if (!_strategies.TryGetValue((typeof(T), key), out object strategy))
            throw new InvalidOperationException($"ProbabilitySystem is not initialized for {typeof(T)} with key '{key}'.");

        return ((IProbability<T>)strategy).GetRandomItem();
    }

    public static bool IsInitialized<T>(string key)
    {
        return _strategies.ContainsKey((typeof(T), key));
    }

    public static void Clear()
    {
        _strategies.Clear();
    }
}