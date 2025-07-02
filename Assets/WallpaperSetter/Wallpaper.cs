using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Sirenix.OdinInspector;

public class Wallpaper : MonoBehaviour
{
    [Title("Wallpaper Data")]
    [SerializeField] private Image spriteHolder;
    [SerializeField] private Button wallpaperButton;

    [SerializeField] private WallpaperData wallpaperData;

    private AndroidSpriteManager androidSpriteManager;

    private void Awake()
    {
        wallpaperButton.onClick.AddListener(OnButtonClick);
    }

    public void Initialize(WallpaperData data)
    {
        this.wallpaperData = data;

        spriteHolder.sprite = wallpaperData.Wallpaper;
    }

    public void OnButtonClick()
    {
        androidSpriteManager ??= ServiceLocator.GetService<AndroidSpriteManager>();

        if (androidSpriteManager == null)
            return;

        androidSpriteManager.Initialize(wallpaperData);
    }
}