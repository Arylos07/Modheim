using UnityEditor;
using UnityEngine;

public class StartVisualStudio : MonoBehaviour
{
    [MenuItem("Tools/Open Visual Studio", false, 0)]
    public static void OpenVisualStudio()
    {
        //if (VSOpen) return;

        string path = Application.dataPath;
        path = path.Replace("/Assets", string.Empty);
        path += "/" + Application.productName + ".sln";
        System.Diagnostics.Process.Start(path);
    }

    //public static bool VSOpen => System.Diagnostics.Process.GetProcessesByName("devenv").Length > 0;
}
