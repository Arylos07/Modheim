using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//a temporary test class to display what is in a modpack
public class DataTest : MonoBehaviour
{
    public FileManager fileManager;
    public string pathToFile = string.Empty;
    public ModheimModpack modpack;

    private void OnValidate()
    {
        if (pathToFile == string.Empty) return;

        if (File.Exists(pathToFile))
        {
            modpack = fileManager.ReadMetadata(pathToFile);
        }
    }
}
