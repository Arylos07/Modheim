using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PackCreator : MonoBehaviour
{
    public FileManager fileManager;

    public InputField nameInput;
    public InputField versionInput;
    public InputField authorInput;
    public InputField creditsInput;
    public InputField descInput;

    public Text changes;
    public Button createButton;

    public bool CanCreate
    {
        get
        {
            return nameInput.text != string.Empty &&
                changes.text.Contains("Error: no changes detected") == false;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus) DisplayChanges();
    }

    private void OnEnable()
    {
        DisplayChanges();
    }

    private void Update()
    {
        createButton.interactable = CanCreate;
    }

    public void Create()
    {
        if(CanCreate)
        {
            ModheimModpack modpack = new ModheimModpack();
            modpack.Name = nameInput.text;
            modpack.Version = versionInput.text;
            modpack.Author = authorInput.text;
            modpack.Credits = creditsInput.text;
            modpack.Description = descInput.text;

            fileManager.CreateModpack(modpack);
        }
    }

    public void DisplayChanges()
    {
        ValheimDirectory directory = fileManager.ScanFiles(true);
        changes.text = string.Empty;

        if(directory.Files.Count == 0 && directory.Folders.Count == 0)
        {
            changes.text = "<color=red>Error: no changes detected to game.\n\nPlease apply your modifications to the game and refresh to see your changes</color>";
            return;
        }

        foreach(string _folder in directory.Folders)
        {
            DirectoryInfo folder = new DirectoryInfo(_folder);
            changes.text += "<color=green>--> " + folder.Name + "</color>\n";
        }

        foreach(string _file in directory.Files)
        {
            FileInfo file = new FileInfo(_file);
            changes.text += "->  " + file.Name + "\n";
        }
    }
}
