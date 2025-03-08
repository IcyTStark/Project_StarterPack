public abstract class Probability<T> : IProbability<T>
{
    public T GetRandomItem()
    {
        throw new System.NotImplementedException();
    }

    public void ValidateWeights()
    {
        throw new System.NotImplementedException();
    }
}