using EnhancedSignals;
using System;
using System.Threading.Tasks;

using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    private ISaveSystem saveSystem;

    [Header("Select Save System: ")]
    [SerializeField] private SaveSystemType saveSystemType;

    [Header("Game Data: ")]
    [SerializeField] private GameSaveData saveData;
    public GameSaveData SaveData
    {
        get => saveData;
        set
        {
            saveData = value;
            Debug.LogError("Saving it");
        }
    }

    public event Action OnLoad;

    protected override void Awake()
    {
        base.Awake();

        InitializeSaveSystem();
    }

    private void Start()
    {
        Debug.LogError("[Save System] New Instance Created");

        Initialize();
    }

    public void InitializeSaveSystem()
    {
        saveSystem = CreateSaveSystem(saveSystemType);
    }

    private ISaveSystem CreateSaveSystem(SaveSystemType saveSystemType)
    {
        return saveSystemType switch
        {
            SaveSystemType.LOCAL => new PlayerPrefsSaveSystem(),
            SaveSystemType.PLAYGAMA => new PlayGamaSaveSystem(),
            //SaveSystemType.FIREBASE => new FirebaseSaveSystem(),
            //SaveSystemType.CRAZY_GAMES => new CrazyGamesSaveSystem(),
            _ => new PlayerPrefsSaveSystem(), // Default to PlayerPrefs
        };
    }

    public async void Initialize()
    {
        await LoadAsync<GameSaveData>(
            onSuccess: (data) =>
            {
                saveData = data;
                Signals.Get<OnDataLoadedSignal>().Dispatch();
            },
            onFailure: (error) =>
            {
                Debug.LogError($"Failed to load data: {error.ToString()}");
            });
    }

    public async Task LoadAsync<T>(Action<T> onSuccess = null, Action<Exception> onFailure = null)
    {
        if (saveSystem == null)
        {
            onFailure?.Invoke(new InvalidOperationException("Save system not initialized."));
            return;
        }

        try
        {
            var data = await saveSystem.LoadDataAsync<T>();
            onSuccess?.Invoke(data);
        }
        catch (Exception ex)
        {
            onFailure?.Invoke(ex);
        }
    }

    public async Task SaveAsync(Action onSuccess = null, Action<Exception> onFailure = null)
    {
        if (saveSystem == null)
        {
            onFailure?.Invoke(new InvalidOperationException("Save system not initialized."));
            return;
        }

        try
        {
            await saveSystem.SaveDataAsync(saveData);
            onSuccess?.Invoke();
        }
        catch (Exception ex)
        {
            onFailure?.Invoke(ex);
        }
    }
}
