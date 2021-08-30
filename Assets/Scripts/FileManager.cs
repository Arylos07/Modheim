using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Sirenix.OdinInspector;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine.UI;
using SFB;

public class FileManager : MonoBehaviour
{
    public static FileManager instance;
    //save keys
    public const string gameDirectory = "gameDirectory";
    public const string deployedFilename = ".deployed_modheim_pack";

    [Header("Directory Info")]
    public string valheimDirectory; //game directory
    [HideInInspector] public bool validDirectory = false;
    public string deployedModPack;
    public ValheimDirectory DefaultFiles; // list of default files that are shipped with the game
    public ValheimDirectory scannedDirectory; // the result of Scanfiles

    List<string> files = new List<string>();
    List<string> folders = new List<string>();

    [Header("UI")]
    public Image gameDirectoryStatus;
    public Sprite validGameDirectory;
    public Sprite invalidGameDirectory;
    public Text directoryText;

    public Text deployedText;
    public List<Button> buttonsRequiringGameDirectory = new List<Button>();

    private void Start()
    {
        instance = this;
        valheimDirectory = FileBasedPrefs.GetString(gameDirectory, string.Empty);

        if (File.Exists(valheimDirectory + "/valheim.exe"))
        {
            validDirectory = true;
            gameDirectoryStatus.sprite = validGameDirectory;

            string deployedPath = Path.Combine(valheimDirectory, deployedFilename);
            if (File.Exists(deployedPath)) deployedModPack = File.ReadAllText(deployedPath);
        }
        else
        {
            validDirectory = false;
            gameDirectoryStatus.sprite = invalidGameDirectory;
        }
    }

    private void Update()
    {
        deployedText.text = deployedModPack != string.Empty ? "Deployed modpack:\n<i>" + deployedModPack + "</i>" : string.Empty;
        directoryText.text = valheimDirectory;

        foreach(Button button in buttonsRequiringGameDirectory)
        {
            button.interactable = validDirectory;
        }
    }

    public void Run()
    {
        Application.OpenURL("steam://rungameid/892970");
    }

    public void SetGameDirectory()
    {
        string[] directory = StandaloneFileBrowser.OpenFolderPanel("Select the Valheim game folder...", valheimDirectory, false);
        if(directory.Length > 0)
        {
            valheimDirectory = directory[0];
            FileBasedPrefs.SetString(gameDirectory, valheimDirectory);

            if (File.Exists(valheimDirectory + "/valheim.exe"))
            {
                validDirectory = true;
                gameDirectoryStatus.sprite = validGameDirectory;

                string deployedPath = Path.Combine(valheimDirectory, deployedFilename);
                if (File.Exists(deployedPath)) deployedModPack = File.ReadAllText(deployedPath);
            }
            else
            {
                validDirectory = false;
                gameDirectoryStatus.sprite = invalidGameDirectory;
            }
        }
        else
        {
            validDirectory = false;
            gameDirectoryStatus.sprite = invalidGameDirectory;
        }
    }

    /// <summary>
    /// Scan game files for content, excluding default files.
    /// </summary>
    public void ScanFiles()
    {
        files = new List<string>(Directory.GetFiles(valheimDirectory));
        folders = new List<string>(Directory.GetDirectories(valheimDirectory));

        scannedDirectory = new ValheimDirectory(files, folders);

        foreach(string file in scannedDirectory.Files)
        {
            if (DefaultFiles.Files.Contains(file)) files.Remove(file);
        }

        foreach(string folder in scannedDirectory.Folders)
        {
            if (DefaultFiles.Folders.Contains(folder)) folders.Remove(folder);
        }

        scannedDirectory = new ValheimDirectory(files, folders);
    }

    /// <summary>
    /// Scan and return a ValheimDirectory of what is currently in the game directory
    /// </summary>
    /// <param name="scanForDefaultFiles">If true, the result will contain all of the default files shipped with the game</param>
    /// <returns>A ValheimDirectory object containing what is currently in the game directory.</returns>
    public ValheimDirectory ScanFiles(bool scanForDefaultFiles)
    {
        files = new List<string>(Directory.GetFiles(valheimDirectory));
        folders = new List<string>(Directory.GetDirectories(valheimDirectory));

        scannedDirectory = new ValheimDirectory(files, folders);

        if (!scanForDefaultFiles) return scannedDirectory;

        foreach (string file in scannedDirectory.Files)
        {
            if (DefaultFiles.Files.Contains(file)) files.Remove(file);
        }

        foreach (string folder in scannedDirectory.Folders)
        {
            if (DefaultFiles.Folders.Contains(folder)) folders.Remove(folder);
        }

        scannedDirectory = new ValheimDirectory(files, folders);
        return scannedDirectory;
    }

    /// <summary>
    /// Purge all non-default files from the directory, effectively reverting the game to its default state.
    /// </summary>
    public void PurgeFiles()
    {
        ValheimDirectory _directory = ScanFiles(true);

        foreach(string file in _directory.Files)
        {
            File.Delete(file);
        }

        foreach(string folder in _directory.Folders)
        {
            Directory.Delete(folder, true);
        }

        deployedModPack = string.Empty;
    }

    /// <summary>
    /// Create a Modheim Modpack containing details and data. The resulting file can be sent and unpacked by DeployModpack()
    /// </summary>
    public void CreateModpack(string modPackName)
    {
        if (modPackName == string.Empty) modPackName = UnityEngine.Random.Range(000000, 999999).ToString();
        string workingDirectory = valheimDirectory + "/" + modPackName;

        ValheimDirectory mods = ScanFiles(true);

        DirectoryCopy(valheimDirectory, workingDirectory, true, DefaultFiles);

        ZipFile.CreateFromDirectory(workingDirectory, Path.Combine(valheimDirectory, modPackName + ".zip"));

        Directory.Delete(workingDirectory, true);

        ModheimModpack rawModpack = new ModheimModpack();
        rawModpack.Name = "Test";
        rawModpack.Description = "This is a big boi test";
        byte[] bytes;
        using (FileStream file = new FileStream(Path.Combine(valheimDirectory, modPackName + ".zip"), FileMode.Open, FileAccess.Read))
        {
            bytes = new byte[file.Length];
            file.Read(bytes, 0, (int)file.Length);
        }
        rawModpack.Data = new BitArray(bytes);

        FileStream pack = new FileStream(Path.Combine(valheimDirectory, modPackName + ".modheim"), FileMode.Create);

        BinaryFormatter formatter = new BinaryFormatter();
        try
        {
            formatter.Serialize(pack, rawModpack);
        }
        catch (SerializationException e)
        {
            Console.WriteLine("Failed to serialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            pack.Close();
        }

        File.Delete(Path.Combine(valheimDirectory, modPackName + ".zip"));
    }

    /// <summary>
    /// Extract and deploy a Modpack. 
    /// </summary>
    public void DeployModpack(string modpack)
    {
        ModheimModpack pack = new ModheimModpack();

        FileStream packFile = new FileStream(modpack, FileMode.Open);
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();

            pack = (ModheimModpack)formatter.Deserialize(packFile);
        }
        catch (SerializationException e)
        {
            Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            packFile.Close();
        }

        FileStream modFile = new FileStream(Path.Combine(Application.dataPath, "modFile.zip"), FileMode.Create);
        byte[] bytes = BitArrayToByteArray(pack.Data);

        modFile.Write(bytes, 0, bytes.Length);

        modFile.Close();

        ZipFile.ExtractToDirectory(Path.Combine(Application.dataPath, "modFile.zip"), valheimDirectory);

        File.WriteAllText(Path.Combine(valheimDirectory, deployedFilename), pack.Name);

        File.Delete(Path.Combine(Application.dataPath, "modFile.zip"));

        deployedModPack = pack.Name;
    }

    /// <summary>
    /// Get Metadata of modpack for UI and version checking. Note: ModheimModpack.Data is assigned, just not used.
    /// </summary>
    /// <param name="path">Path of modpack file/</param>
    /// <returns>A ModheimModpack object containing everything in the modpack, including the raw binary data</returns>
    public ModheimModpack ReadMetadata(string path)
    {
        ModheimModpack pack = new ModheimModpack();

        FileStream packFile = new FileStream(path, FileMode.Open);
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();

            pack = (ModheimModpack)formatter.Deserialize(packFile);
        }
        catch (SerializationException e)
        {
            Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            packFile.Close();
        }

        return pack;
    }

    /// <summary>
    /// Check if a path is a syslink. Modheim can't work on syslink deployments so detect them and warn the user.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsSymbolic(string path)
    {
        FileInfo pathInfo = new FileInfo(path);
        return pathInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
    }

    private static byte[] BitArrayToByteArray(BitArray bits)
    {
        byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
        bits.CopyTo(ret, 0);
        return ret;
    }

    /// <summary>
    /// Copy one directory to another. Does not delete origin directory
    /// </summary>
    /// <param name="sourceDirName"></param>
    /// <param name="destDirName"></param>
    /// <param name="copySubDirs">Copy all subdirectories and files?</param>
    /// <param name="defaultFiles">A ValheimDirectory of what can be ignored in the copy. Leave null to copy without ignoring files.</param>
    private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, ValheimDirectory defaultFiles = null)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();

        // If the destination directory doesn't exist, create it.       
        Directory.CreateDirectory(destDirName);

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            if(defaultFiles != null)
            {
                if (defaultFiles.Files.Contains(file.FullName)) continue;
            }
            string tempPath = Path.Combine(destDirName, file.Name);
            file.CopyTo(tempPath, false);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                if(defaultFiles != null)
                {
                    if (defaultFiles.Folders.Contains(subdir.FullName)) continue;
                }
                string tempPath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
            }
        }
    }
}