using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.IO;
using UnityEngine.SceneManagement;

public class LaunchManager : MonoBehaviour
{
    public static string dataPath;
    public static string ModpacksPath;

    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_EDITOR
        dataPath = Application.dataPath.Replace(Application.productName + "_Data", string.Empty);
        ModpacksPath = dataPath + "/Modpacks";
        if (!Directory.Exists(ModpacksPath)) Directory.CreateDirectory(ModpacksPath);
#else
        dataPath = Application.dataPath.Replace("/Assets", string.Empty);
        ModpacksPath = dataPath + "/Modpacks";
        if (!Directory.Exists(ModpacksPath)) Directory.CreateDirectory(ModpacksPath);
#endif

        Invoke(nameof(Load), 3);

    }

    public void Load()
    {
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// Generates a cryptographically secure string. Uses alphanumeric by default.
    /// </summary>
    /// <param name="length">Length of the string to generate</param>
    /// <param name="allowableChars">What characters are allowed. Example: "1234567890" for only numbers</param>
    public static string GenerateRandomString(int length, string allowableChars = null)
    {
        if (string.IsNullOrEmpty(allowableChars))
            allowableChars = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        // Generate random data
        var rnd = new byte[length];
        using (var rng = new RNGCryptoServiceProvider())
            rng.GetBytes(rnd);

        // Generate the output string
        var allowable = allowableChars.ToCharArray();
        var l = allowable.Length;
        var chars = new char[length];
        for (var i = 0; i < length; i++)
            chars[i] = allowable[rnd[i] % l];

        return new string(chars);
    }
}
