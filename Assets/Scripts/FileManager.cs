using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine.UI;
using SimpleFileBrowser;

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
    public List<GameObject> modPanels = new List<GameObject>();

    public GameObject mainPanel;
    public ModpackList modpackList;

    [Obsolete("Deprecated; this uses a method that tanks the performance of the program if used in Update()")]
    public static bool ValheimRunning => System.Diagnostics.Process.GetProcessesByName("valheim").Length > 0;

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

        /*
        if (ValheimRunning)
        {
            foreach(GameObject panel in modPanels)
            {
                panel.SetActive(false);
            }

            mainPanel.SetActive(true);
        }
        */
    }

    public void Run()
    {
        Application.OpenURL("steam://rungameid/892970");
    }

    public void OpenModpackDirectory()
    {
        Application.OpenURL(LaunchManager.ModpacksPath);
    }

    public void SetGameDirectory()
    {
        //using this instead of Standalone File Browser.
        //SFB only works on .NET 4.0 but the Zip utilities only works on 2.0.
        FileBrowser.ShowLoadDialog(_GameDirectory, null, FileBrowser.PickMode.Folders, false, valheimDirectory, null, "Select the Valheim game folder...");
    }

    void _GameDirectory(string[] directory)
    {
        if (directory.Length > 0)
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

    public void ImportModpack()
    {
        FileBrowser.ShowLoadDialog(_importModpack, null, FileBrowser.PickMode.Files, true, LaunchManager.dataPath, ".modheim", "Select Modheim modpacks for import...", "Import");
    }

    void _importModpack(string[] directories)
    {
        if (directories.Length > 0)
        {
            foreach (string modpack in directories)
            {
                FileInfo pack = new FileInfo(modpack);
                File.Copy(modpack, Path.Combine(LaunchManager.ModpacksPath, pack.Name));
            }

            mainPanel.SetActive(false);
            modpackList.gameObject.SetActive(true);
        }
    }

    public void OpenGameDirectory()
    {
        Application.OpenURL(valheimDirectory);
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
    /// <param name="ignoreDefaultFiles">If true, the result will contain all of the default files shipped with the game</param>
    /// <returns>A ValheimDirectory object containing what is currently in the game directory.</returns>
    public ValheimDirectory ScanFiles(bool ignoreDefaultFiles)
    {
        files = new List<string>(Directory.GetFiles(valheimDirectory));
        folders = new List<string>(Directory.GetDirectories(valheimDirectory));

        scannedDirectory = new ValheimDirectory(files, folders);

        if (!ignoreDefaultFiles) return scannedDirectory;

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
    /// Purge all non-default files from the directory, effectively reverting the game to its default state. Does not warn user.
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
    /// Displays UI Confirmation before purging files.
    /// </summary>
    public void WarnPurgeFiles()
    {
        UIConfirmation.singleton.Show("Are you sure you want to purge the game folder? All non-default files and folders will be deleted.", () =>
        {
            PurgeFiles();
        });
    }

    /// <summary>
    /// Create a Modheim Modpack containing details and data. The resulting file can be sent and unpacked by DeployModpack().
    /// </summary>
    public void CreateModpack(ModheimModpack rawModpack)
    {
        string creationDate = DateTime.UtcNow.ToString("M-dd-y");

        string modPackName = rawModpack.Name + "-" + creationDate;
        string workingDirectory = valheimDirectory + "/" + modPackName;

        rawModpack.modpackDirectory = ScanFiles(true);

        int deployedFile = rawModpack.modpackDirectory.Files.FindIndex(x => x.Contains(deployedFilename));
        if(deployedFile != -1) rawModpack.modpackDirectory.Files.RemoveAt(deployedFile);

        DirectoryCopy(valheimDirectory, workingDirectory, true, DefaultFiles);

        ZipFile.CreateFromDirectory(workingDirectory, Path.Combine(valheimDirectory, modPackName + ".zip"));

        Directory.Delete(workingDirectory, true);

        byte[] bytes;
        using (FileStream file = new FileStream(Path.Combine(valheimDirectory, modPackName + ".zip"), FileMode.Open, FileAccess.Read))
        {
            bytes = new byte[file.Length];
            file.Read(bytes, 0, (int)file.Length);
            file.Close();
        }
        rawModpack.Data = new BitArray(bytes);

        FileStream pack = new FileStream(Path.Combine(LaunchManager.ModpacksPath, modPackName + ".modheim"), FileMode.Create);

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

        //mark as the deployed modpack. Modheim won't purge when done but we can assume
        // that the contents of the directory are the same as the modpack, therefore it's deployed
        File.WriteAllText(Path.Combine(valheimDirectory, deployedFilename), rawModpack.Name);
        deployedModPack = rawModpack.Name;

        File.Delete(Path.Combine(valheimDirectory, modPackName + ".zip"));

        UIPopup.singleton.Show("Modpack successfully created! You can now find it in your modpacks list.");
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

        UIPopup.singleton.Show("Successfully deployed modpack " + pack.Name);
    }

    public void RemoveModpack(ModheimModpack modpack)
    {
        foreach(string file in modpack.modpackDirectory.Files)
        {
            File.Delete(file);
        }

        foreach(string directory in modpack.modpackDirectory.Folders)
        {
            Directory.Delete(directory, true);
        }

        File.Delete(Path.Combine(valheimDirectory, deployedFilename));

        deployedModPack = string.Empty;
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