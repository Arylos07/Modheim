using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

// ensure class initializer is called whenever scripts recompile
[InitializeOnLoad]
public static class StartupScene
{
    static StartupScene()
    {
        SceneChanged();
    }

    public static void SceneChanged()
    {
        if (StartupScenePreferences.enabled)
        {
            EditorSceneManager.playModeStartScene = StartupScenePreferences.startupScene;
        }
        else
        {
            EditorSceneManager.playModeStartScene = null;
        }
    }
}

[InitializeOnLoad]
public static class StartupScenePreferences
{
    private const string key_startupScene = "startupScene";
    private const string key_enabled = "enabled";

    public static SceneAsset startupScene;
    public static bool enabled;

    static StartupScenePreferences()
    {
        if(key_startupScene != string.Empty) startupScene = AssetDatabase.LoadAssetAtPath(EditorPrefs.GetString(key_startupScene), typeof(SceneAsset)) as SceneAsset;
        enabled = EditorPrefs.GetBool(key_enabled, false);

        StartupScene.SceneChanged();
    }

#if UNITY_2019_1_OR_NEWER
    [SettingsProvider]
    public static SettingsProvider CreateStartupSettingsProvider()
    {
        // First parameter is the path in the Settings window.
        // Second parameter is the scope of this setting: it only appears in the Project Settings window.
        var provider = new SettingsProvider("Project/Play From Scene", SettingsScope.Project)
        {
            // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
            guiHandler = (searchContext) => PreferencesGUI()

            // // Populate the search keywords to enable smart search filtering and label highlighting:
            // keywords = new HashSet<string>(new[] { "Number", "Some String" })
        };

        return provider;
    }

#else

    [PreferenceItem("Play From Scene")]
#endif

    private static void PreferencesGUI()
    {
        EditorGUILayout.HelpBox("A simple editor utility for Unity so you can define a scene to start from when you hit the play button. When you exit playmode, " +
            "it will go back to whatever scene you had open.", MessageType.Info);

        GUILayout.Space(20);

        startupScene = EditorGUILayout.ObjectField("Startup Scene", startupScene, typeof(SceneAsset), false) as SceneAsset;
        enabled = EditorGUILayout.Toggle("Play from this scene", enabled);

        if (GUI.changed)
        {
            EditorPrefs.SetString(key_startupScene, AssetDatabase.GetAssetPath(startupScene));
            EditorPrefs.SetBool(key_enabled, enabled);

            StartupScene.SceneChanged();
        }
    }
}