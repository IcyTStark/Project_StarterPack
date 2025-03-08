using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine;

public class PlayGamaSaveSystem : SaveSystem
{
#if PLAYGAMA
    public override async Task<GameSaveData> LoadDataAsync()
    {
        Bridge.storage.Get(GameSaveDataKey, OnStorageGetCompleted, StorageType.LocalStorage);

        GameSaveData saveData;

        if (!PlayerPrefs.HasKey(SaveKey))
        {
            saveData = new();
            return saveData;
        }

        string encryptedString = PlayerPrefs.GetString(SaveKey);

        string decryptedString = EncryptionUtility.Decrypt(encryptedString);

        saveData = JsonConvert.DeserializeObject<GameSaveData>(decryptedString);

        return await Task.FromResult(saveData);
    }

    public override async Task SaveDataAsync(GameSaveData gameSaveData)
    {
        string saveData = JsonConvert.SerializeObject(gameSaveData);

        Bridge.storage.Set(SaveKey, saveData, OnStorageSetCompleted, StorageType.LocalStorage);

        await Task.CompletedTask;
    }

    private void OnStorageSetCompleted(bool success)
    {
        Debug.LogError($"[Playgama Storage]: success: {success}");
    }
#endif
}