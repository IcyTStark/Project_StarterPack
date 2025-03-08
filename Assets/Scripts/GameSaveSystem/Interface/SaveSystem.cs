using System;
using System.Threading.Tasks;

public abstract class SaveSystem : ISaveSystem
{
    protected string SaveKey = "GameData";

    public virtual Task<T> LoadDataAsync<T>()
    {
        throw new NotImplementedException("LoadDataAsync must be implemented by subclasses.");
    }

    public virtual Task SaveDataAsync<T>(T gameSaveData)
    {
        throw new NotImplementedException("SaveDataAsync must be implemented by subclasses.");
    }
}
