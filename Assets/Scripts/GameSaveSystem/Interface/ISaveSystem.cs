using System.Threading.Tasks;
using System.Collections.Generic;

public interface ISaveSystem
{
    public Task<T> LoadDataAsync<T>();
    public Task SaveDataAsync<T>(T gameSaveData);
}