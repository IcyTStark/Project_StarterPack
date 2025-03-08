using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProbabilityConfig<T> : ScriptableObject, IProbabilityConfig<T>
{
    [SerializeField] private List<T> items = new List<T>();
    [SerializeField] [Range(0, 100)] private List<int> weights = new List<int>();

    public IReadOnlyList<T> Items => items;
    public IReadOnlyList<int> Weights => weights;

    private void OnValidate()
    {
        ValidateConfig();
    }

    public virtual Dictionary<T, int> GetWeightDictionary()
    {
        ValidateConfig();
        return items.Zip(weights, (item, weight) => new { item, weight })
                    .ToDictionary(x => x.item, x => x.weight);
    }

    private void ValidateConfig()
    {
        if (items.Count != weights.Count)
            Debug.LogError($"[{name}] Items and weights must have the same count.");

        if (weights.Any(w => w < 0))
            Debug.LogError($"[{name}] Negative weights are not allowed.");

        if (items.Count == 0)
            Debug.LogWarning($"[{name}] No items defined.");

        if (items.Distinct().Count() != items.Count)
            Debug.LogError($"[{name}] Duplicate items found.");
    }
}