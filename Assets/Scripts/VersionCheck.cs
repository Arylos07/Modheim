using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class VersionCheck : MonoBehaviour
{
    public string gitReleaseURL;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(checkVersion());
    }

    IEnumerator checkVersion()
    {
        UnityWebRequest versionCheck = new UnityWebRequest(gitReleaseURL);
        yield return versionCheck.SendWebRequest();

        yield return null;

        if (versionCheck.result == UnityWebRequest.Result.Success)
        {
            //The github release URL becomes redirected to the latest release.
            //WebRequest.url changes to show the redirected URL
            //Take that and parse the latest release from the release tag
            string gitVersion = versionCheck.url.Substring("tag/v");

            if(gitVersion != Application.version)
            {
                UIConfirmation.singleton.Show("A new version of Modheim is available for install.\n\nOpen the newest release?", () =>
                {
                    Application.OpenURL(gitReleaseURL);
                });
            }
        }
    }
}
