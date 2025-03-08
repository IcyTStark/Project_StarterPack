using System.Collections.Generic;

public interface IProbabilityConfig<T>
{
    Dictionary<T, int> GetWeightDictionary();
}