﻿using UnityEngine;

public class FileBasedPrefsQuitListener : MonoBehaviour
{

    public static FileBasedPrefsQuitListener Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        FileBasedPrefs.Save();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        FileBasedPrefs.Save();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        FileBasedPrefs.Save();
    }


}
