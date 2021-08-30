using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ValheimDirectory
{
    public List<string> Files = new List<string>();
    public List<string> Folders = new List<string>();

    public ValheimDirectory()
    {
        Files = new List<string>();
        Folders = new List<string>();
    }

    public ValheimDirectory(List<string> _files, List<string> _folders)
    {
        Files = new List<string>(_files);
        Folders = new List<string>(_folders);
    }
}

[Serializable]
public class ModheimModpack
{
    public string Name; //name of modpack
    public string Description; //description
    public string Version; //version
    public string Author; //who made the modpack
    public string Credits; //any credits for mod creators
    public BitArray Data; //raw data of mods
}
