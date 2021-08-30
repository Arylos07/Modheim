using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackCreator : MonoBehaviour
{
    public FileManager fileManager;

    public InputField nameInput;
    public InputField versionInput;
    public InputField authorInput;
    public InputField creditsInput;
    public InputField descInput;

    public void Create()
    {
        if(nameInput.text != string.Empty)
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
}
