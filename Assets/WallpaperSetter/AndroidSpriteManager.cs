using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public enum WallpaperOption
{
    HomeScreen,
    LockScreen,
    Both
}

public class AndroidSpriteManager : MonoBehaviour
{
    [Header("Sprite to Save/Set as Wallpaper")]
    [SerializeField] private Sprite selectedWallpaper;
    [SerializeField] private Image wallpaperPreview;

    [Header("UI Buttons")]
    [SerializeField] private Button downloadButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button applyWallpaperButton;

    [SerializeField] private RectTransform applyOptionRectTransform;
    [SerializeField] private float deActiveStatePositionY = -400;
    [SerializeField] private float activeStatePositionY = 0;

    [SerializeField] private bool isApplyOptionOn;

    [Header("Wallpaper Selection UI")]
    [SerializeField] private Button homeScreenButton;
    [SerializeField] private Button lockScreenButton;
    [SerializeField] private Button bothScreensButton;

    [Header("Settings")]
    public string savedImageName = "MySprite";
    public int imageQuality = 85; // JPEG quality (0-100)

    private void Awake()
    {
        //Register this
        ServiceLocator.RegisterService(this);

        // Request permissions on start
        RequestPermissions();

        AssignButtonFunctions();

        gameObject.SetActive(false);
    }

    private void RequestPermissions()
    {
        // Request write permission for Android
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageWrite))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageWrite);
        }
    }

    public void Initialize(WallpaperData data)
    {
        SetTargetSprite(data.Wallpaper);

        SetSaveName(data.SaveName);

        wallpaperPreview.sprite = selectedWallpaper;

        gameObject.SetActive(true);
    }

    public void OpenApplyOptions()
    {
        isApplyOptionOn = true;

        AnimateApplyBackgroundPosition();
    }

    private void AnimateApplyBackgroundPosition()
    {
        applyOptionRectTransform.DOAnchorPos3DY(isApplyOptionOn ? activeStatePositionY : deActiveStatePositionY, 0.25f);
    }

    private void AssignButtonFunctions()
    {
        // Setup button listeners
        if (downloadButton != null)
            downloadButton.onClick.AddListener(SaveSpriteToGallery);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseUI);

        if (applyWallpaperButton != null)
            applyWallpaperButton.onClick.AddListener(OpenApplyOptions);

        if (homeScreenButton != null)
            homeScreenButton.onClick.AddListener(SetWallpaperToHomeScreen);

        if (lockScreenButton != null)
            lockScreenButton.onClick.AddListener(SetWallpaperToLockScreen);

        if (bothScreensButton != null)
            bothScreensButton.onClick.AddListener(SetWallpaper);
    }

    #region Download Wallpaper
    public void SaveSpriteToGallery()
    {
        if (selectedWallpaper == null)
        {
            Debug.LogError("Target sprite is null!");
            ShowMessage("Error: No sprite selected");
            return;
        }

        StartCoroutine(SaveSpriteToGalleryCoroutine());
    }

    private IEnumerator SaveSpriteToGalleryCoroutine()
    {
        // Get readable texture
        Texture2D readableTexture = GetReadableTexture(selectedWallpaper.texture);

        if (readableTexture == null)
        {
            ShowMessage("Error: Could not read texture");
            yield break;
        }

        // Encode to bytes
        byte[] imageBytes = readableTexture.EncodeToJPG(imageQuality);

        // Clean up if we created a new texture
        if (readableTexture != selectedWallpaper.texture)
            DestroyImmediate(readableTexture);

        yield return StartCoroutine(SaveToGalleryAndroid(imageBytes));
    }

    private IEnumerator SaveToGalleryAndroid(byte[] imageBytes)
    {
        // Save to temporary location first
        string tempPath = Path.Combine(Application.temporaryCachePath, savedImageName + ".jpg");
        File.WriteAllBytes(tempPath, imageBytes);

        // Use Android native code to save to gallery
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaClass mediaStore = new AndroidJavaClass("android.provider.MediaStore$Images$Media"))
        {
            try
            {
                // Insert image into MediaStore
                string result = mediaStore.CallStatic<string>("insertImage",
                    currentActivity.Call<AndroidJavaObject>("getContentResolver"),
                    tempPath,
                    savedImageName,
                    "Saved from Unity App");

                if (!string.IsNullOrEmpty(result))
                {
                    ShowMessage("Image saved to Gallery!");
                }
                else
                {
                    ShowMessage("Failed to save to Gallery");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Android gallery save error: {e.Message}");
                ShowMessage("Error saving to Gallery");
            }
        }

        // Clean up temp file
        if (File.Exists(tempPath))
            File.Delete(tempPath);

        yield return null;
    }
    #endregion

    #region Set As Wallpaper
    public void SetWallpaperToHomeScreen()
    {
        if (selectedWallpaper == null)
        {
            Debug.LogError("Target sprite is null!");
            ShowMessage("Error: No sprite selected");
            return;
        }

        SetWallpaperWithOption(WallpaperOption.HomeScreen);
    }

    public void SetWallpaperToLockScreen()
    {
        if (selectedWallpaper == null)
        {
            Debug.LogError("Target sprite is null!");
            ShowMessage("Error: No sprite selected");
            return;
        }

        SetWallpaperWithOption(WallpaperOption.LockScreen);
    }

    public void SetWallpaper()
    {
        if (selectedWallpaper == null)
        {
            Debug.LogError("Target sprite is null!");
            ShowMessage("Error: No sprite selected");
            return;
        }

        SetWallpaperWithOption(WallpaperOption.Both);
    }

    public void SetWallpaperWithOption(WallpaperOption option)
    {
        StartCoroutine(SetWallpaperCoroutine(option));
    }

    private IEnumerator SetWallpaperCoroutine(WallpaperOption option)
    {
        // Get readable texture
        Texture2D readableTexture = GetReadableTexture(selectedWallpaper.texture);

        if (readableTexture == null)
        {
            ShowMessage("Error: Could not read texture");
            yield break;
        }

        // Encode to bytes
        byte[] imageBytes = readableTexture.EncodeToJPG(90); // Higher quality for wallpaper

        // Clean up if we created a new texture
        if (readableTexture != selectedWallpaper.texture)
            DestroyImmediate(readableTexture);

        if (imageBytes == null)
        {
            ShowMessage("Error: Could not encode image");
            yield break;
        }

        // Save temporary file
        string tempPath = Path.Combine(Application.temporaryCachePath, "wallpaper_temp.jpg");
        File.WriteAllBytes(tempPath, imageBytes);

        // Set as wallpaper using Android native code
        yield return StartCoroutine(SetWallpaperAndroid(tempPath, option));

        // Clean up temp file
        if (File.Exists(tempPath))
            File.Delete(tempPath);
    }

    private IEnumerator SetWallpaperAndroid(string imagePath, WallpaperOption option)
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaClass wallpaperManager = new AndroidJavaClass("android.app.WallpaperManager"))
        {
            try
            {
                AndroidJavaObject wallpaperManagerInstance = wallpaperManager.CallStatic<AndroidJavaObject>("getInstance", currentActivity);

                // Load bitmap from file
                using (AndroidJavaClass bitmapFactory = new AndroidJavaClass("android.graphics.BitmapFactory"))
                {
                    AndroidJavaObject bitmap = bitmapFactory.CallStatic<AndroidJavaObject>("decodeFile", imagePath);

                    if (bitmap != null)
                    {
                        string successMessage = "";

                        switch (option)
                        {
                            case WallpaperOption.HomeScreen:
                                // FLAG_SYSTEM = 1 (Home screen)
                                wallpaperManagerInstance.Call("setBitmap", bitmap, null, true, 1);
                                successMessage = "Home screen wallpaper set successfully!";
                                break;

                            case WallpaperOption.LockScreen:
                                // FLAG_LOCK = 2 (Lock screen)
                                wallpaperManagerInstance.Call("setBitmap", bitmap, null, true, 2);
                                successMessage = "Lock screen wallpaper set successfully!";
                                break;

                            case WallpaperOption.Both:
                                // Set both by calling setBitmap without flags (default behavior)
                                wallpaperManagerInstance.Call("setBitmap", bitmap);
                                successMessage = "Wallpaper set for both home and lock screen!";
                                break;
                        }

                        ShowMessage(successMessage);
                    }
                    else
                    {
                        ShowMessage("Failed to load image for wallpaper");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Android wallpaper error: {e.Message}");
                ShowMessage("Error setting wallpaper");
            }
        }

        yield return null;
    }
    #endregion

    private Texture2D GetReadableTexture(Texture2D source)
    {
        // If the texture is already readable and not compressed, use it directly
        if (source.isReadable && !IsCompressedFormat(source.format))
        {
            return source;
        }

        // Create a temporary RenderTexture with sRGB color space
        RenderTexture renderTexture = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.sRGB); // Use sRGB instead of Linear

        // Copy source texture to RenderTexture
        Graphics.Blit(source, renderTexture);

        // Create readable Texture2D with RGBA32 format to preserve colors and alpha
        Texture2D readableTexture = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);

        // Save current RenderTexture
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;

        // Read pixels from RenderTexture
        readableTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        readableTexture.Apply();

        // Restore previous RenderTexture
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);

        return readableTexture;
    }

    private bool IsCompressedFormat(TextureFormat format)
    {
        switch (format)
        {
            case TextureFormat.DXT1:
            case TextureFormat.DXT5:
            case TextureFormat.PVRTC_RGB2:
            case TextureFormat.PVRTC_RGBA2:
            case TextureFormat.PVRTC_RGB4:
            case TextureFormat.PVRTC_RGBA4:
            case TextureFormat.ETC_RGB4:
            case TextureFormat.ETC2_RGB:
            case TextureFormat.ETC2_RGBA8:
            case TextureFormat.ASTC_4x4:
            case TextureFormat.ASTC_5x5:
            case TextureFormat.ASTC_6x6:
            case TextureFormat.ASTC_8x8:
            case TextureFormat.ASTC_10x10:
            case TextureFormat.ASTC_12x12:
                return true;
            default:
                return false;
        }
    }

    private void ShowMessage(string message)
    {
        Debug.Log(message);
        // You can implement a UI popup here if needed
    }

    // Optional: Method to set a different sprite at runtime
    public void SetTargetSprite(Sprite newSprite)
    {
        selectedWallpaper = newSprite;
    }

    // Optional: Method to set custom save name
    public void SetSaveName(string newName)
    {
        savedImageName = newName;
    }

    private void CloseUI()
    {
        if (isApplyOptionOn)
        {
            Reset();
            return;
        }

        Reset();

        gameObject.SetActive(false);
    }

    private void Reset()
    {
        isApplyOptionOn = false;

        AnimateApplyBackgroundPosition();
    }
}