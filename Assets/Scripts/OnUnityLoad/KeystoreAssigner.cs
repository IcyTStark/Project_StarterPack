using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

#if UNITY_EDITOR
[InitializeOnLoad]
public class KeystoreAssigner
{
    private const string fileName = "nukeboxstudios";
    private const string password = "Techtree@123";

    static KeystoreAssigner()
    {
        string keystorePath = $"{Application.dataPath.Replace("Assets", "")}{fileName}.keystore";

        if (File.Exists(keystorePath))
        {
            PlayerSettings.Android.keystoreName = keystorePath;
            PlayerSettings.Android.keystorePass = password;
            PlayerSettings.Android.keyaliasPass = password;
        }
        else
        {
            Debug.LogWarning("Keystore file does not exist at the specified path: " + keystorePath);
        }
    }
}
#endif