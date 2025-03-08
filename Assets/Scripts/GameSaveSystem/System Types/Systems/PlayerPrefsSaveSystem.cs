using System.Threading.Tasks;
using UnityEngine;

using Newtonsoft.Json;
using System;

public class PlayerPrefsSaveSystem : SaveSystem
{
    public override async Task<T> LoadDataAsync<T>()
    {
        string key = typeof(T).Name;

        if (!PlayerPrefs.HasKey(key))
        {
            return await Task.FromResult((T)Activator.CreateInstance(typeof(T)));

            //return await Task.FromResult(default(T));
        }

        string encryptedString = PlayerPrefs.GetString(key);
        string decryptedString = EncryptionUtility.Decrypt(encryptedString);
        Debug.LogError($"[Save System]: {key} loaded with data: {decryptedString}");

        T saveData = JsonConvert.DeserializeObject<T>(decryptedString);
        return await Task.FromResult(saveData);
    }

    public override async Task SaveDataAsync<T>(T gameSaveData)
    {
        string key = typeof(T).Name;

        string saveData = JsonConvert.SerializeObject(gameSaveData);
        Debug.Log($"[Save System]: {key} saved with data: {saveData}");

        string encryptedString = EncryptionUtility.Encrypt(saveData);
        PlayerPrefs.SetString(key, encryptedString);
        PlayerPrefs.Save();

        await Task.CompletedTask;
    }
}