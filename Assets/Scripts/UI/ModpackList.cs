using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class ModpackList : MonoBehaviour
{
    public static ModpackList instance;

    public Transform list;
    public ModpackListElement elementPrefab;

    List<string> paths = new List<string>();
    List<ModheimModpack> packs = new List<ModheimModpack>();
    int selectedIndex = -1;

    [Header("Metadata UI")]
    public GameObject modpackInfoPanel;
    public Text nameText;
    public Text versionText;
    public Text authorText;
    public Button descriptionButton;
    public Button creditsButton;
    public Text descriptionText;
    public Text deployButtonText;

    private void OnEnable()
    {
        instance = this;
        Refresh();
    }

    void Refresh()
    {
        paths = new List<string>(Directory.GetFiles(LaunchManager.ModpacksPath));
        packs = new List<ModheimModpack>();
        foreach(string path in paths)
        {
            packs.Add(FileManager.instance.ReadMetadata(path));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (paths.Count == 0) return;

        UIUtils.BalancePrefabs(elementPrefab.gameObject, paths.Count, list);

        for (int i = 0; i < paths.Count; ++i)
        {
            ModpackListElement slot = list.GetChild(i).GetComponent<ModpackListElement>();
            ModheimModpack pack = packs[i];

            slot.PackName.text = pack.Name;
            slot.PackAuthor.text = pack.Author;
            slot.index = i;
            slot.EnabledImage.enabled = pack.Name == FileManager.instance.deployedModPack;
            slot.ElementButton.interactable = !(slot.index == selectedIndex);
        }
    }


    public void SelectPack(int index)
    {
        selectedIndex = index;
        ModheimModpack pack = packs[index];

        nameText.text = pack.Name;
        versionText.text = pack.Version;
        authorText.text = pack.Author;
        descriptionButton.interactable = false;
        creditsButton.interactable = true;
        descriptionText.text = pack.Description;

        if(FileManager.instance.deployedModPack == pack.Name)
        {
            deployButtonText.text = "Disable Modpack";
            //TODO: colours?
        }
        else
        {
            deployButtonText.text = "Enable Modpack";
        }

        modpackInfoPanel.SetActive(true); //disabled when Modheim first starts to hide placeholder values
    }

    public void ToggleMod()
    {
        ModheimModpack pack = packs[selectedIndex];

        if (FileManager.instance.deployedModPack == pack.Name)
        {
            UIConfirmation.singleton.Show("Are you sure you want to disable this modpack?", () =>
            {
                FileManager.instance.RemoveModpack(pack);
                SelectPack(selectedIndex); //redraw modpack info
                UIPopup.singleton.Show("Successfully removed " + pack.Name);
            });
        }
        else if(FileManager.instance.deployedModPack != string.Empty && FileManager.instance.deployedModPack != pack.Name)
        {
            //display warning. If confirmed, purge before continuing
            UIConfirmation.singleton.Show("You already have modpack " + FileManager.instance.deployedModPack + " enabled. To enable another modpack, the enabled modpack must be disabled " +
                "\n\nDo you want to proceed? This cannot be undone.", () =>
                {
                    FileManager.instance.RemoveModpack(packs.First(x => x.Name == FileManager.instance.deployedModPack));
                    FileManager.instance.DeployModpack(paths[selectedIndex]);
                });
        }
        else
        {
            FileManager.instance.DeployModpack(paths[selectedIndex]);
        }

        SelectPack(selectedIndex);
    }

    public void SwitchDescription(int i)
    {
        switch (i)
        {
            case 0:
                //display description
                descriptionText.text = packs[selectedIndex].Description;
                descriptionButton.interactable = false;
                creditsButton.interactable = true;
                break;
            case 1:
                //display credits
                descriptionText.text = packs[selectedIndex].Credits;
                descriptionButton.interactable = true;
                creditsButton.interactable = false;
                break;
        }
    }

    public void Delete()
    {
        UIConfirmation.singleton.Show("Are you sure you want to permanently delete this modpack? This cannot be undone.", () =>
        {
            File.Delete(paths[selectedIndex]);
            Refresh();
            modpackInfoPanel.SetActive(false);
            selectedIndex = -1;
        });
    }

    //untested
    public void OpenModPath()
    {
        FileInfo file = new FileInfo(paths[selectedIndex]);
        Application.OpenURL(file.Directory.FullName);
    }
}
