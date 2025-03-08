public interface IProbability<T>
{
    T GetRandomItem();
    void ValidateWeights();
}
