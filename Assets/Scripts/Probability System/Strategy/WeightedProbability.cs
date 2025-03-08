using System;
using System.Collections.Generic;
using System.Linq;

public class WeightedProbability<T> : IProbability<T>
{
    private readonly Random _random;
    private readonly List<T> _items;
    private readonly List<int> _weights;
    private readonly int _totalWeight;

    public WeightedProbability(Dictionary<T, int> weightedItems, Random random = null)
    {
        if (weightedItems == null || weightedItems.Count == 0)
            throw new ArgumentException("Weighted items dictionary cannot be null or empty.");

        _random = random ?? new Random();
        _items = weightedItems.Keys.ToList();
        _weights = weightedItems.Values.ToList();
        _totalWeight = _weights.Sum();

        ValidateWeights();
    }

    public T GetRandomItem()
    {
        int roll = _random.Next(0, _totalWeight);
        int cumulative = 0;

        for (int i = 0; i < _items.Count; i++)
        {
            cumulative += _weights[i];
            if (roll < cumulative)
                return _items[i];
        }

        return _items[^1]; // Fallback in case of unexpected rounding errors
    }

    public void ValidateWeights()
    {
        if (_totalWeight <= 0)
            throw new ArgumentException("Total weight must be greater than 0.");
    }
}