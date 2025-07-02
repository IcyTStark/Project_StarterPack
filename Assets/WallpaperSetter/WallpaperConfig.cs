using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

[System.Serializable]
public class WallpaperData
{
    [SerializeField] private Sprite wallpaper;
    [SerializeField] private string saveName;

    public Sprite Wallpaper => wallpaper;
    public string SaveName => saveName;
}

public class WallpaperConfig : ScriptableObject
{
    [SerializeField] private List<WallpaperData> wallpapers;

    public IReadOnlyList<WallpaperData> Wallpapers => wallpapers;
}