using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionNumber : MonoBehaviour
{
    public string prefix;
    public Text versionText;

    // Start is called before the first frame update
    void Start()
    {
        versionText.text = (prefix + " " + Application.version).Trim();
    }
}
